using System;
using UnityEngine;

namespace GGJ2026
{
    [RequireComponent(typeof(Collider2D))]
    public class KillBox : MonoBehaviour
    {
        private Collider2D col;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                //Player Die
                
            }
        }
    }
}