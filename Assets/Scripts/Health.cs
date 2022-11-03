using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : BulletHit
{
    public int StartingHealth = 100;

    [SyncVar]
    public int CurrentHealth = 100;

    /// <summary>
    /// returns true if you are handling destroying the object yourself
    /// </summary>
    public Func<bool> OnDeath;
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = StartingHealth;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth<= 0)
        {
            if (!(OnDeath?.Invoke() ?? false))
                Destroy(this.gameObject);
        }
    }
	public override void OnBulletHit(Bullet bullet)
	{
        if (isServer)
            TakeDamage(bullet.DamageAmount);
		base.OnBulletHit(bullet);
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
