using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : NetworkBehaviour, IVRHandInteractable
{

    public GameObject BulletPrefab;
    public Transform BarrelPoint;
    float fireRate = .3f;        // The time between each shot.
    public int BulletSpeed = 500;
    public bool IsAutomatic = true;
    ParticleSystem gunParticles;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.


    float timer;                                    // A timer to determine when to fire.

    // Use this for initialization
    void Awake()
    {

        gunParticles = this.GetComponentInChildren<ParticleSystem>();
        gunAudio = this.GetComponentInChildren<AudioSource>();
        gunLight = this.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;
        if (shouldFire && (timer >= fireRate && Time.timeScale != 0))
        {
            // ... shoot the gun.
            Shoot();
        }
        
        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= fireRate * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }


    bool shouldFire;
    public void Fire()
    {
        if (IsAutomatic)
        {
            shouldFire = true;
        }
        Shoot();
    }
    public void StopFiring()
    {
        shouldFire = false;
    }
    void Shoot()
    {
        timer = 0f;
        if (isClient)
            CmdFire();
        else
            fireIsServer();
    }

    void fireIsServer()
    {
        var bullet = (GameObject)Instantiate(
           BulletPrefab,
           BarrelPoint.position,
           BarrelPoint.rotation);

        playShootEffects();
        // Destroy the bullet after 4 seconds
        Destroy(bullet, 4.0f);
    }
    [Command]
    void CmdFire(NetworkConnectionToClient sender = null)
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            BulletPrefab,
            BarrelPoint.position,
            BarrelPoint.rotation);

        // Add velocity to the bullet
        //bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * BulletSpeed);
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Player = sender.identity.gameObject;

        NetworkServer.Spawn(bullet);
        RpcShootEffects();
        // Destroy the bullet after 4 seconds
        Destroy(bullet, 4.0f);
    }
    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLight.enabled = false;
    }


    [ClientRpc]
    void RpcShootEffects()
    {
        if (isLocalPlayer)
            return;
        playShootEffects();
    }

    void playShootEffects()
    {
        // Play the gun shot audioclip.
        gunAudio.Play();

        // Enable the lights.
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();
    }

    void IVRHandInteractable.TriggerPressed () => Fire();

    void IVRHandInteractable.TriggerReleased () => StopFiring();
}
