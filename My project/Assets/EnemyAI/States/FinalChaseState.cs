using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FinalChaseState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float playerCheckTimer;
    private float detectionDist = 10f;

    private float checkTime = 0f;
    private float checkInterval = 3f;
    private float loseTime = 6f;

    private Vector3 lastKnownPlayerPos;

    public FinalChaseState(NPCStateManager npc)
    {
        this.npc = npc;
        player = npc.player;
    }

    public override void Enter()
    {
        Debug.Log($"NPC AI Entered Hunt State");

        playerCheckTimer = 0f;
        checkTime = 0f;
        
        lastKnownPlayerPos = player.position;

        npc.agent.isStopped = false;
        npc.agent.speed = npc.finalHuntSpeed;
        npc.agent.SetDestination(lastKnownPlayerPos);

        AudioManager.Instance.PlaySound("SFX_creature_alert", npc.transform.position, null);
    }

    public override void Update()
    {
        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            bool canSeePlayer = npc.RaycastFindPlayer(detectionDist, true);

            if (canSeePlayer)
            {
                Debug.Log("NPC Hunt Sees Player");
                checkTime = 0f;
                if (Vector3.Distance(lastKnownPlayerPos, player.position) > 1f)
                {
                    lastKnownPlayerPos = player.position;
                    npc.agent.SetDestination(lastKnownPlayerPos);
                }
            }

            else
            {
                Debug.Log("NPC Hunt CANT See Player");
                checkTime += Time.deltaTime;
            }

            // Fully lost player
            if (checkTime >= loseTime)
            {
                Debug.Log($"NPC AI FULLY LOST PLAYER FOR {loseTime} SECONDS");
                npc.TransitionToState(NPCState.Roam);
            }
        }
    }

    public override void Exit()
    {
        //play sound
    }
}
