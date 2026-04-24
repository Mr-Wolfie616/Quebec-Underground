using UnityEngine;
using UnityEngine.AI;

public class RoamState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float playerCheckTimer;

    private float checkInterval = 2f;
    private float detectionDist = 2f;

    private float roamRadius = 6f;

    public float huntChance = 0.9f; // 90%
    public float biasChance; // chance it tends to player

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
        if (npc.agent == null) return;

        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            if (npc.RaycastFindPlayer(detectionDist, false))
            {
                if (Random.value <= huntChance)
                {
                    npc.TransitionToState(NPCState.Hunt);
                    return;
                }
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
                //Debug.Log("NPC stuck");
                ChooseNewRoamPoint();
            }

            lastPosition = npc.transform.position;
            stuckTimer = 0f;
        }
    }

    private void ChooseNewRoamPoint()
    {
        int maxAttempts = 5;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 origin = npc.transform.position;
            Vector3 direction;

            float distToPlayer = Vector3.Distance(origin, player.position);
            float biasChance = Mathf.Pow(Mathf.InverseLerp(2f, 30f, distToPlayer), 2); 

            Debug.Log(biasChance.ToString());

            bool goTowardPlayer = player != null && Random.value < biasChance;

            Debug.Log(goTowardPlayer);

            if (goTowardPlayer)
            {
                Vector3 toPlayer = (player.position - origin).normalized;

                Vector2 randomOffset = Random.insideUnitCircle * 2f;
                Vector3 offset = new Vector3(randomOffset.x, 0, randomOffset.y);

                direction = (toPlayer * Random.Range(2f, roamRadius)) + offset;
            }
            else
            {
                Vector2 circle = Random.insideUnitCircle * roamRadius;
                direction = new Vector3(circle.x, 0, circle.y);
            }

            Vector3 targetPos = origin + direction;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 4f, NavMesh.AllAreas))
            {
                npc.agent.SetDestination(hit.position);
                return;
            }
        }

        NavMesh.SamplePosition(npc.transform.position, out NavMeshHit fallbackHit, 4f, NavMesh.AllAreas);
        npc.agent.SetDestination(fallbackHit.position);
    }
}
