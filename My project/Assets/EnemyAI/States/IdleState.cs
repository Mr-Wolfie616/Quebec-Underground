using UnityEngine;

public class IdleState : State
{
    private NPCStateManager npc;

    private float idleTimer;
    private float playerCheckTimer;

    private Transform player;

    private float idleDur = 20f;
    private float checkInterval = 3f;
    private float detectionDist = 2.5f;

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

        //npc.agent.isStopped = true;
    }

    public override void Update()
    {
        idleTimer += Time.deltaTime;
        playerCheckTimer += Time.deltaTime;

        RaycastFindPlayer();

        if (idleTimer >= idleDur)
        {
            npc.TransitionToState(NPCState.Idle);
        }
    }

    private void RaycastFindPlayer()
    {
        if (playerCheckTimer < checkInterval) return;

        playerCheckTimer = 0f;

        Vector3 dir = player.transform.position - npc.transform.position;
        dir.Normalize();

        Ray ray = new Ray(npc.transform.position, dir * detectionDist);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionDist))
        {
            if (hit.transform.GetComponent<FPCharacterController>())
            {
                Debug.DrawRay(npc.transform.position, dir * detectionDist, Color.green, 1.5f);
                Debug.Log("Enemy Detect Player");
            }
            else
            {
                Debug.DrawRay(npc.transform.position, dir * detectionDist, Color.blue, 1.5f);
            }
        }
        else
        {
            Debug.DrawRay(npc.transform.position, dir * detectionDist, Color.orangeRed, 1.5f);
        }
    }
}
