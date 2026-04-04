using UnityEngine;
using UnityEngine.AI;

public class RoamState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float playerCheckTimer;

    private float checkInterval = 2f;
    private float detectionDist = 4f;

    private float roamRadius = 6f;

    public float huntChance = 0.9f; // 90%

    private Vector3 lastPosition;
    private float stuckTimer;

    private float stuckCheckInterval = 1f;
    private float moveThreshold = 0.2f;

    public RoamState(NPCStateManager npc)
    {
        this.npc = npc;
        player = npc.player;
    }

    public override void Enter()
    {
        Debug.Log($"NPC AI Entered Roam State");

        AudioManager.Instance.PlaySound("SFX_creature_roam", npc.transform.position, null);

        ChooseNewRoamPoint();
        playerCheckTimer = 0f;

        if (npc.agent != null)
        {
            npc.agent.isStopped = false;
            npc.agent.speed = npc.roamSpeed;
        }

        lastPosition = npc.transform.position;
        stuckTimer = 0f;
    }

    public override void Update()
    {
        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            if (npc.RaycastFindPlayer(detectionDist, false))
            {
                if (Random.value > huntChance) return; // chance it doesnt hunt
                npc.TransitionToState(NPCState.Hunt);
            }
        }

        if (!npc.agent.pathPending && npc.agent.hasPath && npc.agent.remainingDistance < 0.5f)
        {
            ChooseNewRoamPoint();
        }

        stuckTimer += Time.deltaTime;

        if (stuckTimer >= stuckCheckInterval)
        {
            float movedDistance = Vector3.Distance(npc.transform.position, lastPosition);

            if (movedDistance < moveThreshold && npc.agent.hasPath && npc.agent.remainingDistance > npc.agent.stoppingDistance)
            {
                Debug.Log("NPC stuck");
                ChooseNewRoamPoint();
            }

            lastPosition = npc.transform.position;
            stuckTimer = 0f;
        }
    }

    private void ChooseNewRoamPoint()
    {
        Vector3 forward = npc.transform.forward * Random.Range(2f, roamRadius);
        Vector3 sideways = npc.transform.right * Random.Range(-2f, 2f);

        Vector3 randomDir = forward + sideways;
        Vector3 targetPos = npc.transform.position + randomDir;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            npc.agent.SetDestination(hit.position);
            return;
        }

        Vector3 back = -npc.transform.forward * Random.Range(2f, roamRadius);
        sideways = npc.transform.right * Random.Range(-2f, 2f);

        randomDir = back + sideways;
        targetPos = npc.transform.position + randomDir;

        if (NavMesh.SamplePosition(targetPos, out hit, 4f, NavMesh.AllAreas))
        {
            npc.agent.SetDestination(hit.position);
        }
    }
}
