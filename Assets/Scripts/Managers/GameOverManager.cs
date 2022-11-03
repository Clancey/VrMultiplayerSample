using UnityEngine;
using System.Linq;

namespace CompleteProject
{
    public class GameOverManager : MonoBehaviour
    {
        public Health playerHealth;       // Reference to the player's health.


        Animator anim;                          // Reference to the animator component.


        void Awake ()
        {
            // Set up the reference.
            anim = GetComponent <Animator> ();
        }


        void Update ()
        {
            var gos = GameObject.FindGameObjectsWithTag("Player");
            if ((gos?.Length ?? 0) <= 0)
                return;
            var allDead = gos.Select(x => x.GetComponentInChildren<Health>() ?? x.GetComponent<Health>()).All(x=> x.CurrentHealth <= 0) ;
            // If the player has run out of health...
            if (allDead)
            {
                // ... tell the animator the game is over.
                anim.SetTrigger ("GameOver");
            }
        }
    }
}