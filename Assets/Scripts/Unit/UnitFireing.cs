using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class UnitFireing : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileFirePoint = null;
    [SerializeField] private float fireingRange = 6f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;
    private Quaternion targetRotation;
    private Quaternion projectileRotation;
    private Targetable target;

    [ServerCallback]
    private void Update()
    {
        target = targeter.GetTarget();

        if (target == null) { return; }

        if(!CanFireAtTarget()) { return; }

        targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Time.time > (1/fireRate) + lastFireTime)
        {
            projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileFirePoint.position);

            GameObject projectileInstance = Instantiate(projectilePrefab, projectileFirePoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        return (target.transform.position - transform.position).sqrMagnitude <= fireingRange * fireingRange;
    }

}
