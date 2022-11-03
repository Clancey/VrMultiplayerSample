using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHit : NetworkBehaviour
{
   public virtual void OnBulletHit(Bullet bullet)
	{

		Destroy(bullet.gameObject);
	}
}
