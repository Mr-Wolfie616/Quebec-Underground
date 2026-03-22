using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class InvestigateState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float waitTime = 5f;
    private float waitTimer;

    // always check
    private float playerCheckTimer;
    private float detectionDist = 2f;
    private float checkInterval = 3f;

    private Vector3 previousTarget;

    private AudioDataSO currentAudioData;

    // check at targer

    private float targetDetDist = 5f;

    private bool reachedTarget = false;
    public InvestigateState(NPCStateManager npc)
    {
        this.npc = npc;
        player = npc.player;
    }

    public Vector3 GetAudioTarget()
    {
        return npc.currentAudioTarget;
    }

    public AudioDataSO GetAudioData()
    {
        currentAudioData = npc.currentAudioData;
        return npc.currentAudioData;
    }

    public void ClearAudioTarget()
    {
        npc.hasAudioTarget = false;
    }

    public override void Enter()
    {
        string soundID = GetAudioData().id;
        // play sound?
        Debug.Log($"NPC AI Entered Investigate State | Target {soundID} at {npc.currentAudioTarget}");

        playerCheckTimer = 0f;
        waitTimer = 0f;
        reachedTarget = false;

        npc.agent.isStopped = false;
        npc.agent.speed = npc.huntSpeed;

        npc.agent.SetDestination(GetAudioTarget());
    }

    public override void Update()
    {
        npc.agent.SetDestination(GetAudioTarget());

        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            if (npc.RaycastFindPlayer(detectionDist, false))
            {
                Debug.Log("NPC FOUND PLAYER");
                if (Random.value <= 0.75f)
                {
                    npc.TransitionToState(NPCState.Hunt);
                }
                return;
            }
        }

        if (!npc.agent.pathPending && npc.agent.remainingDistance <= npc.agent.stoppingDistance)
        {
            reachedTarget = true;
        }

        WaitAndSearch(reachedTarget);
    }

    private void WaitAndSearch(bool reachedTarget)
    {
        if (reachedTarget)
        {
            if (npc.RaycastFindPlayer(targetDetDist, false))
            {
                Debug.Log("NPC FOUND PLAYER");
                if (Random.value <= 0.9f)
                {
                    npc.TransitionToState(NPCState.Hunt);
                }
                return;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                Debug.Log("NPC finished investigating");

                if (npc.RaycastFindPlayer(targetDetDist, false))
                {
                    Debug.Log("NPC FOUND PLAYER");
                    if (Random.value <= 0.9f)
                    {
                        npc.TransitionToState(NPCState.Hunt);
                    }
                    return;
                }

                ClearAudioTarget();

                if (Random.value < 0.4f) npc.TransitionToState(NPCState.Idle);
                else npc.TransitionToState(NPCState.Roam);
            }
        }
    }

    public override void Exit()
    {
        //play sound
    }
}
