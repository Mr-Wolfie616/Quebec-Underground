using UnityEngine;

public class IdleState : State
{
    private NPCStateManager npc;

    private float idleTimer;
    private float playerCheckTimer;

    private Transform player;

    private float idleDur = 10f;
    private float checkInterval = 3f;
    private float detectionDist = 2.5f;

    private float huntChance = 0.7f; // 70 percent chance to hunting

    public IdleState(NPCStateManager npc)
    {
        this.npc = npc;
        player = npc.player;
    }

    public override void Enter()
    {
        Debug.Log($"NPC AI Entered Idle State");
        idleTimer = 0f;
        playerCheckTimer = 0f;

        if (npc.agent != null)
        {
            npc.agent.isStopped = true;
            npc.agent.speed = npc.roamSpeed;
        }
    }

    public override void Update()
    {
        idleTimer += Time.deltaTime;
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

        if (idleTimer >= idleDur)
        {
            npc.TransitionToState(NPCState.Roam);
        }
    }
}
