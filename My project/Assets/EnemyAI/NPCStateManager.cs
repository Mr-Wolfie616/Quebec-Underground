using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class NPCStateManager : FSM
{
    public LayerMask permittedLayers;
    public Animator animator { get; private set; }
    //public NodePath nodePath;
    public NavMeshAgent agent { get; private set; }
    public Rigidbody rb { get; private set; }

    public Collider deathCol { get; private set; }

    public Transform player;

    public float tempDisableTime = 1.5f;
    private bool agentTemporarilyDisabled;

    public float roamSpeed = 1.5f;
    public float huntSpeed = 3f;
    public float finalHuntSpeed = 5f;

    public float randomSoundInterval = 30f; // +- 25%
    private float speakInterval;
    private float speakTime = 0f;
    private float feetTime = 0f;

    public bool hasAudioTarget = false;
    public Vector3 currentAudioTarget;
    public AudioDataSO currentAudioData;
    private Vector3 newAudioTarget;

    public float footstepInterval;

    private PlayerHideScript phs;

    private void OnEnable()
    {
        AudioManager.AlertEnemyEvent += HearAudio;    
    }
    private void OnDisable()
    {
        AudioManager.AlertEnemyEvent -= HearAudio;
    }
    protected override void Start()
    {
        base.Start();

        speakTime = 0f;
        speakInterval = randomSoundInterval += Random.Range(-(randomSoundInterval / 25), (randomSoundInterval / 25));

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        deathCol = GetComponent<Collider>();

        if (player == null)
        {
            player = FindFirstObjectByType<FPCharacterController>().transform;
        }

        phs = player.gameObject.GetComponent<PlayerHideScript>();

        if (agent == null) Debug.LogWarning("No NavMeshAgent found on NPCStateManager!");
        if (rb == null) Debug.LogWarning("No Rigidbody found on NPCStateManager!");
        if (phs == null) Debug.LogWarning("No PHS found on Player!");

        states[NPCState.Idle] = new IdleState(this);
        states[NPCState.Roam] = new RoamState(this);
        states[NPCState.Hunt] = new HuntState(this);
        states[NPCState.Investigate] = new InvestigateState(this);

        TransitionToState(NPCState.Idle);
    }

    private void DoFootStep(float intvl)
    {
        feetTime += Time.deltaTime;

        if (feetTime >= intvl) {
            AudioManager.Instance.PlaySound("SFX_EnemyFootsteps", transform.position, null);
            feetTime = 0f;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (agent == null || animator == null) return;

        if (phs.isHiding && deathCol != null)
        {
            deathCol.enabled = false;
        }
        else
        {
            deathCol.enabled = true;
        }

        float normalisedVel = agent.velocity.magnitude / huntSpeed;
        normalisedVel = Mathf.Clamp01(normalisedVel);

        float interval = Mathf.Lerp(0.5f, 0.15f, normalisedVel);

        if (agent.velocity.magnitude > 0.1f)
        {
            DoFootStep(interval);
        }
        else
        {
            feetTime = 0f;
        }

        animator.SetFloat("speed", normalisedVel, 0, 1f);

        speakTime += Time.deltaTime;

        if (speakTime >= speakInterval)
        {
            speakTime = 0f;
            AudioManager.Instance.PlaySound("SFX_creature_idle", transform.position, null);
            speakInterval = randomSoundInterval += Random.Range(-(randomSoundInterval / 25), (randomSoundInterval / 25));
        }

    }

    private IEnumerator TemporarilyDisableAgent()
    {
        agentTemporarilyDisabled = true;

        //animator.SetFloat("speed", 0f);

        if (agent != null) agent.enabled = false;
        if (rb != null) rb.isKinematic = false;

        yield return new WaitForSeconds(tempDisableTime);

        if (rb != null) rb.isKinematic = true;
        if (agent != null) agent.enabled = true;

        agentTemporarilyDisabled = false;
    }

    public bool RaycastFindPlayer(float detectionDist, bool throughWalls)
    {
        Vector3 origin = transform.position + new Vector3(0,1,0);
        Vector3 aim = player.transform.position;

        Vector3 dir = aim - origin;
        float distance = dir.magnitude;
        dir.Normalize();

        if (phs != null && phs.isHiding)
        {
            Debug.DrawRay(origin, dir * detectionDist, Color.purple, 1.5f);
            Debug.Log("Enemy No Detect Hiding Player");
            return false;
        }

        if (throughWalls)
        {
            if (distance <= detectionDist)
            {
                Debug.DrawRay(origin, dir * detectionDist, Color.blue, 1.5f);
                Debug.Log("Enemy Detect Player (through walls)");
                return true;
            }
            return false;
        }

        Ray ray = new Ray(origin, dir);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionDist, permittedLayers))
        {
            if (hit.transform.TryGetComponent<FPCharacterController>(out FPCharacterController controller))
            {
                // 25% chance if crouching
                if (controller.isCrouching)
                {
                    if (Random.value > 0.25f)
                    {
                        Debug.DrawRay(origin, dir * detectionDist, Color.cyan, 1.5f);
                        Debug.Log("Enemy missed crouching player");
                        return false;
                    }
                }

                Debug.DrawRay(origin, dir * detectionDist, Color.yellow, 1.5f);
                Debug.Log("Enemy Detect Player");
                return true;
            }
        }
        return false;
    }

    public bool SampleCorrectedPosition(Vector3 pos, out NavMeshHit hit)
    {
        Vector3 floorCorrected = new Vector3(pos.x, pos.y - 0.5f, pos.z);
        return NavMesh.SamplePosition(floorCorrected, out hit, 1f, NavMesh.AllAreas);
    }

    public void HearAudio(Vector3 pos, AudioDataSO data)
    {
        float newDist = Vector3.Distance(transform.position, pos);

        if (newDist > data.alertRadius) return;

        newAudioTarget = pos;

        if (!hasAudioTarget)
        {
            currentAudioTarget = newAudioTarget;
            currentAudioData = data;
            hasAudioTarget = true;
        }
        else
        {
            float currentDist = Vector3.Distance(transform.position, currentAudioTarget);

            bool higherPriority = data.alertPriority > currentAudioData.alertPriority;
            bool closer = newDist < currentDist;

            if (higherPriority || closer)
            {
                currentAudioTarget = newAudioTarget;
                currentAudioData = data;
            }
        }

        if (!IsInState(NPCState.Investigate))
        {
            TransitionToState(NPCState.Investigate);
        }
    }
}
