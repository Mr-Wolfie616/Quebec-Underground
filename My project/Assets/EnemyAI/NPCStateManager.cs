using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateManager : FSM
{
    public Animator animator { get; private set; }
    //public NodePath nodePath;
    public NavMeshAgent agent { get; private set; }
    public Rigidbody rb { get; private set; }

    public Transform player;

    public float tempDisableTime = 1.5f;
    private bool agentTemporarilyDisabled;

    public float roamSpeed = 1.5f;
    public float huntSpeed = 3f;
    public float finalHuntSpeed = 5f;

    public bool hasAudioTarget = false;
    public Vector3 currentAudioTarget;
    public AudioDataSO currentAudioData;
    private Vector3 newAudioTarget;

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

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            player = FindFirstObjectByType<FPCharacterController>().transform;
        }

        if (agent == null) Debug.LogWarning("No NavMeshAgent found on NPCStateManager!");
        if (rb == null) Debug.LogWarning("No Rigidbody found on NPCStateManager!");

        states[NPCState.Idle] = new IdleState(this);
        states[NPCState.Roam] = new RoamState(this);
        states[NPCState.Hunt] = new HuntState(this);
        states[NPCState.Investigate] = new InvestigateState(this);

        TransitionToState(NPCState.Idle);
    }

    protected override void Update()
    {
        base.Update();

        if (agent == null || animator == null) return;

        float speed = agent.velocity.magnitude;
        float targetAnimSpeed = GetAnimationSpeed(speed);

        animator.SetFloat("speed", targetAnimSpeed, 0.1f, Time.deltaTime);
    }

    private float GetAnimationSpeed(float speed)
    {
        if (speed >= huntSpeed) return 2f;
        if (speed >= roamSpeed * 0.5f) return 1f;
        return 0f;
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
        Vector3 origin = transform.position;
        Vector3 aim = player.transform.position;

        Vector3 dir = aim - origin;
        float distance = dir.magnitude;
        dir.Normalize();

        var hide = player.GetComponentInParent<PlayerHideScript>();

        if (hide != null && hide.isHiding)
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

            Debug.DrawRay(origin, dir * detectionDist, Color.black, 1.5f);
            return false;
        }

        Ray ray = new Ray(origin, dir);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionDist))
        {
            if (hit.transform.GetComponent<FPCharacterController>())
            {
                Debug.DrawRay(origin, dir * detectionDist, Color.yellow, 1.5f);
                Debug.Log("Enemy Detect Player");
                return true;
            }
            else
            {
                Debug.DrawRay(origin, dir * detectionDist, Color.blue, 1.5f);
            }
        }
        else
        {
            Debug.DrawRay(origin, dir * detectionDist, Color.black, 1.5f);
        }
        return false;
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
