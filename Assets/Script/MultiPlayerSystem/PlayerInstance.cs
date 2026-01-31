using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public class PlayerInstance : MonoBehaviour
    {
        public string Name;
        public PlayerStateMachine stateMachine;
        public AnimationManager animaMgr;
        public Rigidbody2D rb;

        private Collider2D col;

        // private PlayerIM input;
        private PlayerInput playerInput;
        public Vector2 moveValue;

        private void Awake()
        {
            Debug.Log($"{name} Awake");
            
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            playerInput = GetComponent<PlayerInput>();
            
            playerInput.SwitchCurrentActionMap(name);
            Debug.Log(name + "switch to custom action map");
            
            animaMgr = new AnimationManager(GetComponentInChildren<Animator>());
            stateMachine = new PlayerStateMachine(this);
        }

        private void Update()
        {
            stateMachine.Update();
            moveValue = playerInput.actions["Move"].ReadValue<Vector2>();
        }
    }

    public class AnimationManager
    {
        private Animator anim;
        private HashSet<string> parameterNamesSet;

        public AnimationManager(Animator anim)
        {
            this.anim = anim;
            parameterNamesSet = Enumerable.ToHashSet(anim.parameters.Select(x => x.name));
        }

        public void SetTrigger(string key)
        {
            if (parameterNamesSet.Contains(key))
            {
                anim.SetTrigger(key);
            }
            else
            {
                Debug.LogError($"{anim}的parameters列表不包含名为{key}的Trigger字段");
            }
        }

        public void SetBool(string key, bool value)
        {
            if (parameterNamesSet.Contains(key))
            {
                anim.SetBool(key, value);
            }
            else
            {
                Debug.LogError($"{anim}的parameters列表不包含名为{key}的Bool字段");
            }
        }

        public void ReverseBool(string key)
        {
            if (parameterNamesSet.Contains(key))
            {
                // anim.SetBool(key, value);
                bool oldValue = anim.GetBool(key);
                anim.SetBool(key, !oldValue);
            }
            else
            {
                Debug.LogError($"{anim}的parameters列表不包含名为{key}的Bool字段");
            }
        }

        public void SetFloat(string key, float value = 0)
        {
            if (parameterNamesSet.Contains(key))
            {
                anim.SetFloat(key, value);
            }
            else
            {
                Debug.LogError($"{anim}的parameters列表不包含名为{key}的Float字段");
            }
        }
    }
}