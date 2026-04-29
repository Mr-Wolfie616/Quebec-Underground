using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RoamState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float playerCheckTimer;

    private float checkInterval = 2f;
    private float detectionDist = 4f;

    private float roamRadius = 12f;

    public float huntChance = 0.8f; // 90%
    public float biasChance; // chance it tends to player

    private Vector3 lastPosition;
    private float stuckTimer;

    private float stuckCheckInterval = 5f;
    private float moveThreshold = 0.2f;

    private bool headingAway = false;
    private float headingAwayTimer = 5f;
    private float haTime;

    private Vector3 lastKnownPlayerPos;
    private Vector3 playerVelocity;

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
        haTime = 0f;

        if (npc.agent != null)
        {
            npc.agent.isStopped = false;
            npc.agent.speed = npc.roamSpeed;
        }

        lastPosition = npc.transform.position;
        stuckTimer = 0f;

        lastKnownPlayerPos = player.position;
        playerVelocity = Vector3.zero;
    }

    public override void Update()
    {
        if (npc.agent == null) return;

        playerCheckTimer += Time.deltaTime;

        if (headingAway)
        {
            haTime -= Time.deltaTime;

            if (haTime <= 0f)
            {
                Debug.Log("NPC stopped heading away from player");
                headingAway = false;
            }
        }

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

        if (!npc.agent.pathPending && npc.agent.hasPath && npc.agent.remainingDistance < 0.25f)
        {
            ChooseNewRoamPoint();
        }

        stuckTimer += Time.deltaTime;

        if (stuckTimer >= stuckCheckInterval)
        {
            float movedDistance = Vector3.Distance(npc.transform.position, lastPosition);

            if (movedDistance < moveThreshold && npc.agent.hasPath)
            {
                Debug.Log("NPC stuck");
                ChooseNewRoamPoint();
            }

            Debug.Log($"NPC moved {movedDistance} units in the last {stuckCheckInterval} seconds");

            lastPosition = npc.transform.position;
            stuckTimer = 0f;
        }
    }

    private void BeginHeadingAwayTimer()
    {
        Debug.Log("NPC is heading away from player");
        haTime = headingAwayTimer;
        headingAway = true;
    }

    private void ChooseNewRoamPoint()
    {
        int maxAttempts = 5;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 origin = npc.transform.position;
            Vector3 direction;

            float distToPlayer = Vector3.Distance(origin, player.position);
            float biasChance = Mathf.Pow(Mathf.InverseLerp(10f, 40f, distToPlayer), 2); 

            //Debug.Log(biasChance.ToString());

            bool goTowardPlayer = player != null && Random.value < biasChance;
            bool goAway = player != null && npc.phs.isHiding && npc.RaycastFindPlayer(3f, true) && Random.value < 0.75f;

            if (goAway)
            {
                BeginHeadingAwayTimer();
            }

            Vector2 circle = Random.insideUnitCircle * roamRadius;
            Vector3 randomDir = new Vector3(circle.x, 0, circle.y);
            if (headingAway)
            {
                Vector3 awayFromPlayer = (origin - player.position).normalized;
                float weight = 1.2f;

                direction = (randomDir + awayFromPlayer * weight).normalized * Random.Range(roamRadius * 0.5f, roamRadius);
            }
            else
            {
                playerVelocity = (player.position - lastKnownPlayerPos) / Time.deltaTime;
                lastKnownPlayerPos = player.position;

                Vector3 predictedPos = player.position + playerVelocity * (biasChance * 1.5f);

                Vector3 interceptDir = (predictedPos - origin).normalized;

                direction = Vector3.Lerp(randomDir, interceptDir * roamRadius, biasChance).normalized * Random.Range(roamRadius * 0.5f, roamRadius);
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
