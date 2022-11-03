using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{

    public float destroyAfter = 5;
    public Rigidbody rigidBody;
    public float force = 500;
    public GameObject Player { get; set; }

    public int DamageAmount = 20;
    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var bulletTarget = hit.GetComponentInChildren<BulletHit>();
        if (bulletTarget != null)
        {
            bulletTarget?.OnBulletHit(this);
        }
        else
            Destroy(gameObject);
    }


    public override void OnStartServer ()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start ()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf ()
    {
        NetworkServer.Destroy(gameObject);
    }

	// ServerCallback because we don't want a warning if OnTriggerEnter is
	// called on the client
	[ServerCallback]
	void OnTriggerEnter(Collider co)
	{
		var hit = co.gameObject;
		var bulletTarget = hit.GetComponentInChildren<BulletHit>();
        if (bulletTarget != null)
        {
            bulletTarget?.OnBulletHit(this);
        }
        else
        {
            NetworkServer.Destroy(gameObject);
        }
	}
}
