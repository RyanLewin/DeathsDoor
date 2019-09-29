using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Tool
{
    [Range(0, 10)]
    /// <summary> Lower accuracyError is better; </summary>
    public float accuracyError = 1;
    public Projectile projectile;
    [SerializeField]
    ProjectileType projectileType = ProjectileType.Single;
    [SerializeField]
    int shotsPerBurst = 3;


    [SerializeField]
    Transform firePoint;

    public void Fire ()
    {
        switch (projectileType)
        {
            case (ProjectileType.Single):
                SingleFire();
                break;
            case (ProjectileType.Auto):
                StartCoroutine(AutoFire());
                break;
            case (ProjectileType.Burst):
                BurstFire();
                break;
        }
    }

    void SingleFire ()
    {
        Shot();
    }

    IEnumerator AutoFire ()
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            Shot();
            yield return new WaitForSeconds(0.2f);
        }
    }

    void BurstFire ()
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            Shot(true);
        }
    }

    void Shot (bool burst = false)
    {
        float z = firePoint.rotation.z;
        z += Random.Range(-accuracyError, accuracyError);
        Projectile p = Instantiate(projectile, firePoint.position, firePoint.rotation);
        p.damage = burst ? damage / shotsPerBurst : damage;
        p.range = range;
        p.GetComponent<Rigidbody2D>().AddForce(p.transform.forward * p.force);
    }
}

public enum ProjectileType { Single, Auto, Burst }
