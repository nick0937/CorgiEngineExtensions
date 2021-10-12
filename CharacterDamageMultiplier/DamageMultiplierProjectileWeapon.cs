﻿using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace CharacterDamageMultiplier
{
    public class DamageMultiplierProjectileWeapon : ProjectileWeapon
    {
	    [MMInspectorGroup("Damage", true, 66)]
	    public float DamageCaused = 10;
	    [MMReadOnly] private float _damageMultiplier = 1;

        public override void ApplyDamageMultiplier(float multiplier) { _damageMultiplier = multiplier; }

        public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles,
            bool triggerObjectActivation = true)
        {
            /// we get the next object in the pool and make sure it's not null
			GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

			// mandatory checks
			if (nextGameObject==null)	{ return null; }
			if (nextGameObject.GetComponent<MMPoolableObject>()==null)
			{
				throw new Exception(gameObject.name+" is trying to spawn objects that don't have a PoolableObject component.");		
			}	
			// we position the object
			nextGameObject.transform.position = spawnPosition;
			// we set its direction

			Projectile projectile = nextGameObject.GetComponent<Projectile>();
			if (projectile != null)
			{
				projectile.SetDamage((int)(DamageCaused * _damageMultiplier));
				projectile.SetWeapon(this);
                if (Owner != null)
                {
                    projectile.SetOwner(Owner.gameObject);
                }				
			}
			// we activate the object
			nextGameObject.gameObject.SetActive(true);


			if (projectile != null)
			{
                if (RandomSpread)
                {
                    _randomSpreadDirection.x = UnityEngine.Random.Range(-Spread.x, Spread.x);
                    _randomSpreadDirection.y = UnityEngine.Random.Range(-Spread.y, Spread.y);
                    _randomSpreadDirection.z = UnityEngine.Random.Range(-Spread.z, Spread.z);
                }
                else
                {
                    if (totalProjectiles > 1)
                    {
                        _randomSpreadDirection.x = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.x, Spread.x);
                        _randomSpreadDirection.y = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.y, Spread.y);
                        _randomSpreadDirection.z = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.z, Spread.z);
                    }
                    else
                    {
                        _randomSpreadDirection = Vector3.zero;
                    }
                }               

                Quaternion spread = Quaternion.Euler(_randomSpreadDirection);
                projectile.SetDirection(spread * transform.right * (Flipped ? -1 : 1), transform.rotation, Owner.IsFacingRight);
                if (RotateWeaponOnSpread)
                {
                    this.transform.rotation = this.transform.rotation * spread;
                }
			}

			if (triggerObjectActivation)
			{
				if (nextGameObject.GetComponent<MMPoolableObject>()!=null)
				{
					nextGameObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
				}
			}

			return (nextGameObject);
        }
    }
}
