using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class InvestigateState : State
{
    private NPCStateManager npc;
    private Transform player;

    private float waitTime = 3f;
    private float waitTimer;

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
        npc.currentAudioData = null;
        npc.currentAudioTarget = Vector3.zero;
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

        if (Random.value < 0.1f)
        {
            npc.TransitionToState(NPCState.Roam);
            return;
        }

        string soundID = currentAudioData.id;

        AudioManager.Instance.PlaySound("SFX_creature_alert", npc.transform.position, null);

        Debug.Log($"NPC AI Entered Investigate State | Target {soundID} at {npc.currentAudioTarget}");

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
            if (Vector3.Distance(hit.position, previousTarget) > 0.25f)
            {
                previousTarget = hit.position;
                npc.agent.SetDestination(hit.position);
            }
        }

        bool reachedTarget = !npc.agent.pathPending && npc.agent.pathStatus == NavMeshPathStatus.PathComplete && npc.agent.remainingDistance <= npc.agent.stoppingDistance;

        WaitAndSearch(reachedTarget);
    }

    private void WaitAndSearch(bool reachedTarget)
    {
        if (!reachedTarget) return;
        Debug.Log($"NPC is investigating. Reached target: {reachedTarget}. Wait timer: {waitTimer}");
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitTime)
        {
            Debug.Log("NPC finished investigating");

            float chance;

            if (npc.phs.isHiding)
            {
                chance = 0.05f;
            }
            else if (npc.playerScript.isCrouching)
            {
                chance = 0.20f;
            }
            else
            {
                chance = 0.65f;
            }

            Debug.Log(chance + "| Crouching: " + npc.playerScript.isCrouching.ToString() + " | Hiding: " + npc.phs.isHiding.ToString());

            if (TryHuntPlayer(targetDetDist, chance)) return;

            ClearAudioTarget();

            if (Random.value < 0.25f) npc.TransitionToState(NPCState.Idle);
            else npc.TransitionToState(NPCState.Roam);

            waitTimer = 0f;
            return;
        }
    }

    private bool TryHuntPlayer(float dist, float chance)
    {
        if (!npc.RaycastFindPlayer(dist, false)) return false;

        Debug.Log("NPC FOUND PLAYER");

        if (Random.value <= chance)
        {
            npc.TransitionToState(NPCState.Hunt);
            return true;
        }

        return false;
    }

    public override void Exit()
    {
        waitTimer = 0f;
    }
}
