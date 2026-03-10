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

    public RoamState(NPCStateManager npc)
    {
        this.npc = npc;
        player = npc.player;
    }

    public override void Enter()
    {
        Debug.Log($"NPC AI Entered Roam State");
        playerCheckTimer = 0f;

        if (npc.agent != null)
        {
            npc.agent.isStopped = false;
            npc.agent.speed = npc.roamSpeed;
        }
    }

    public override void Update()
    {
        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            if (npc.RaycastFindPlayer(detectionDist))
            {
                if (Random.value > huntChance) return; // chance it doesnt hunt
                //npc.TransitionToState(NPCState.Hunt);
            }
        }

        if (!npc.agent.pathPending && npc.agent.remainingDistance < 0.5f)
        {
            ChooseNewRoamPoint();
        }
    }

    private void ChooseNewRoamPoint()
    {
        Vector3 forward = npc.transform.forward * Random.Range(2f, roamRadius);
        Vector3 sideways = npc.transform.right * Random.Range(-3f, 3f);

        Vector3 randomDir = forward + sideways;

        Vector3 targetPos = npc.transform.position + randomDir;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            npc.agent.SetDestination(hit.position);
        }
    }
}
