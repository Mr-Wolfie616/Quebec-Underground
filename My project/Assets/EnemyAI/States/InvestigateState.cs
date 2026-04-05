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
        return npc.currentAudioData;
    }

    public void ClearAudioTarget()
    {
        npc.hasAudioTarget = false;
    }

    public override void Enter()
    {
        currentAudioData = GetAudioData();

        if (currentAudioData == null)
        {
            Debug.LogWarning("No audio data for investigate state");
            npc.TransitionToState(NPCState.Roam);
            return;
        }

        string soundID = currentAudioData.id;

        AudioManager.Instance.PlaySound("SFX_creature_alert", npc.transform.position, null);

        Debug.Log($"NPC AI Entered Investigate State | Target {soundID} at {npc.currentAudioTarget}");

        playerCheckTimer = 0f;
        waitTimer = 0f;

        npc.agent.isStopped = false;
        npc.agent.speed = npc.huntSpeed;

        previousTarget = GetAudioTarget();
        if (npc.SampleCorrectedPosition(previousTarget, out NavMeshHit hit))
        {
            npc.agent.SetDestination(hit.position);
        }
    }

    public override void Update()
    {
        Vector3 newTarget = GetAudioTarget();

        if (npc.SampleCorrectedPosition(newTarget, out NavMeshHit hit))
        {
            if (hit.position != previousTarget)
            {
                previousTarget = hit.position;
                npc.agent.SetDestination(hit.position);
            }
        }

        playerCheckTimer += Time.deltaTime;

        if (playerCheckTimer >= checkInterval)
        {
            playerCheckTimer = 0f;

            if (TryHuntPlayer(detectionDist, 0.6f)) return;
        }

        bool reachedTarget = !npc.agent.pathPending && npc.agent.pathStatus == NavMeshPathStatus.PathComplete && npc.agent.remainingDistance <= npc.agent.stoppingDistance;

        WaitAndSearch(reachedTarget);
    }

    private void WaitAndSearch(bool reachedTarget)
    {
        if (reachedTarget)
        {
            if (TryHuntPlayer(targetDetDist, 0.9f)) return;

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                Debug.Log("NPC finished investigating");

                if (TryHuntPlayer(targetDetDist, 0.9f)) return;

                ClearAudioTarget();

                if (Random.value < 0.4f) npc.TransitionToState(NPCState.Idle);
                else npc.TransitionToState(NPCState.Roam);
                return;
            }
        }
    }

    private bool TryHuntPlayer(float dist, float chance)
    {
        if (!npc.RaycastFindPlayer(dist, false)) return false;

        Debug.Log("NPC FOUND PLAYER");

        if (Random.value <= chance)
        {
            npc.TransitionToState(NPCState.Hunt);
        }

        return true;
    }

    public override void Exit()
    {
        waitTimer = 0f;
        playerCheckTimer = 0f;
    }
}
