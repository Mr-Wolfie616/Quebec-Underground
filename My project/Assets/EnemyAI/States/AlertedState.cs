using UnityEngine;
using UnityEngine.AI;

public class AlertedState : State
{
    private NPCStateManager npc;
    private NavMeshAgent agent;
    private float playerCheckRadius = 3f;
    private float timeOutTime = 10f;

    public AlertedState(NPCStateManager npc)
    {
        this.npc = npc;
        agent = npc.agent;
    }

    public override void Enter()
    {
        Debug.Log("Entering Alerted State");

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.updatePosition = true;
        agent.SetDestination(npc.propPos);
    }

    public override void Update()
    {
        if (!agent.enabled) return;

        timeOutTime -= Time.deltaTime;
        if (timeOutTime <= 0)
        {
            Debug.Log("AlertedState: Timed out");
            npc.TransitionToState("Patrol");
        }

        float speed = Mathf.Clamp01(agent.velocity.magnitude);
        npc.animator.SetFloat("speed", speed);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Alerted State");
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
    }
}
