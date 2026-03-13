using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HuntState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float playerCheckTimer;
    private float detectionDist = 12f;

    private float checkTime = 0f;
    private float checkInterval = 1f;
    private float loseTime = 6f;

    private Vector3 lastKnownPlayerPos;

    private bool lostPlayer = false;

    public HuntState(NPCStateManager npc)
    {
        this.npc = npc;
        player = npc.player;
    }

    public override void Enter()
    {
        Debug.Log($"NPC AI Entered Hunt State");
        playerCheckTimer = 0f;
        lastKnownPlayerPos = player.position;

        npc.agent.isStopped = false;
        npc.agent.speed = npc.huntSpeed;
        npc.agent.SetDestination(lastKnownPlayerPos);

        // audio cue
    }

    public override void Update()
    {
        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            if (!npc.RaycastFindPlayer(detectionDist, true))
            {
                Debug.Log("NPC AI cant find player");
                lostPlayer = true;
            }
            else
            {
                lostPlayer = false;
                checkTime = 0f;
                Debug.Log("NPC AI NOT Lost player");

                lastKnownPlayerPos = player.position;
                npc.agent.SetDestination(lastKnownPlayerPos);
            }
        }

        if (lostPlayer)
        {
            checkTime += Time.deltaTime;
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
