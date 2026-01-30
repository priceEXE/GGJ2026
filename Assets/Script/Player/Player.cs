using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MemoFramework;
using MemoFramework.Extension;
using M2;
using Unity.VisualScripting;

namespace GGJ2026
{
    public class Player : MonoBehaviour
    {
        private Animator anim;
        private Rigidbody2D rd;
        private Collider2D cd;

        private Vector2 lastVelocity;
        public bool isDead = false;

        [SerializeField] private float speed = 1f;
        public Transform anchor;

        private void Start()
        {
            anim = GetComponent<Animator>();
            rd = GetComponent<Rigidbody2D>();
            cd = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            MF.Blackboard.SetInt("KeyValue", 0);
        }

        private void Update()
        {
            if (isDead) return;

            if (MF.Input.InputMap.asset["Move"].ReadValue<Vector2>() * speed == Vector2.zero)
            {
                rd.velocity = Vector2.zero;
            }
            else
            {
                rd.velocity = lastVelocity = MF.Input.InputMap.asset["Move"].ReadValue<Vector2>() * speed;
            }

            anim.SetFloat("Horizontal", rd.velocity.x);
            anim.SetFloat("Vertical", rd.velocity.y);
            anim.SetFloat("LastHorizontal", lastVelocity.x);
            anim.SetFloat("LastVertical", lastVelocity.y);
            anim.SetBool("Moving", rd.velocity.magnitude > 0.1f);

            // if (Input.GetKeyDown(KeyCode.L)) SaveSystem.instance.Load();

        }

        public string Save()
        {
            return JsonUtility.ToJson(transform.position);
        }

        public void Load(string data)
        {
            transform.position = JsonUtility.FromJson<Vector3>(data);
        }

        public void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
        }

        public void FinallyDie()
        {
            MF.Event.Fire(this,OnRequireGameOver.Create());
        }
    }

    
}