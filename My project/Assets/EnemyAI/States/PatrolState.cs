using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class PatrolState : State
{
    protected NPCStateManager npc;
    private NodePath path;
    private Transform nextNode;
    private int currentNodeIndex = 0;
    private int finalNodeIndex;
    private NavMeshAgent agent;

    public PatrolState(NPCStateManager npc)
    {
        this.npc = npc;
        path = npc.nodePath;
        agent = npc.agent;
    }

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");

        if (path == null || path.nodes.Length == 0)
        {
            Debug.LogWarning("No path nodes assigned!");
            return;
        }

        finalNodeIndex = path.nodes.Length - 1;
        currentNodeIndex = 0;

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.updatePosition = true;

        agent.SetDestination(path.nodes[currentNodeIndex].position);
    }

    public override void Update()
    {
        if (!agent.enabled) return;

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentNodeIndex++;
            if (currentNodeIndex > finalNodeIndex)
            {
                currentNodeIndex = 0;
            }

            agent.SetDestination(path.nodes[currentNodeIndex].position);
        }

        float walkSpeed = agent.velocity.magnitude;
        walkSpeed = Mathf.Clamp01(walkSpeed);

        npc.animator.SetFloat("speed", walkSpeed);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Patrol State");
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
    }
}