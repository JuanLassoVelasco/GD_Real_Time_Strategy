using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent navAgent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chasingRange = 10f;

    private Targetable currentTarget;

    #region Server

    [ServerCallback]
    private void Update()
    {
        currentTarget = targeter.GetTarget();

        if (currentTarget != null)
        {
            if ((currentTarget.transform.position - transform.position).sqrMagnitude > chasingRange * chasingRange)
            {
                navAgent.SetDestination(currentTarget.transform.position);
            }
            else if (navAgent.hasPath)
            {
                navAgent.ResetPath();
            }

            return;
        }

        if (!navAgent.hasPath) { return; }

        if (navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            navAgent.ResetPath();
        }
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        navAgent.SetDestination(hit.position);
    }

    #endregion
}
