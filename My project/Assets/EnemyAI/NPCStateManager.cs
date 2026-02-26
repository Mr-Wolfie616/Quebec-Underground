using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateManager : FSM
{
    public Animator animator { get; private set; }
    public NodePath nodePath;
    public NavMeshAgent agent { get; private set; }
    public Rigidbody rb { get; private set; }

    public float tempDisableTime = 1.5f;
    private bool agentTemporarilyDisabled;

    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (agent == null) Debug.LogWarning("No NavMeshAgent found on NPCStateManager!");
        if (rb == null) Debug.LogWarning("No Rigidbody found on NPCStateManager!");

        states["Patrol"] = new PatrolState(this);
        states["Alerted"] = new AlertedState(this);

        TransitionToState("Patrol");
    }

    private IEnumerator TemporarilyDisableAgent()
    {
        agentTemporarilyDisabled = true;

        animator.SetFloat("speed", 0f);

        if (agent != null) agent.enabled = false;
        if (rb != null) rb.isKinematic = false;

        yield return new WaitForSeconds(tempDisableTime);

        if (rb != null) rb.isKinematic = true;
        if (agent != null) agent.enabled = true;

        agentTemporarilyDisabled = false;
    }
}
