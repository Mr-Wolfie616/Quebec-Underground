using System.Collections;
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

        TransitionToState(NPCState.Idle);
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

    public bool RaycastFindPlayer(float detectionDist)
    {
        Vector3 origin = transform.position;
        Vector3 aim = player.transform.position;

        Vector3 dir = aim - origin;
        dir.Normalize();

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
}
