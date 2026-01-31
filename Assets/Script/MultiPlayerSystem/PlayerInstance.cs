using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        public PlayerIM playerIM;
        public Vector2 moveValue;
        
        // State Change
        public PlayerStateInfo stateInfo;

        public float lastDashTime;
        
        private bool isFacingRight;
        [SerializeField] private LayerMask groundLayer; //墙和地面的Layer
        public int FacingDirection { get; private set; } = 1;
        public bool GroundDetected { get; private set; }
        public bool WallDetected { get; private set; }

        private void Awake()
        {
            Debug.Log($"{name} Awake");
            
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            playerInput = GetComponent<PlayerInput>();
            
            playerInput.SwitchCurrentActionMap(name);
            Debug.Log(name + "switch to custom action map");
            
            animaMgr = new AnimationManager(GetComponentInChildren<Animator>());
            stateInfo = GetComponent<PlayerStateInfo>();
            stateMachine = new PlayerStateMachine(this);
        }

        private void Start()
        {
            isFacingRight = true;
            lastDashTime = -stateInfo.dashColdDown;
        }

        private void Update()
        {
            stateMachine.Update();
            moveValue = playerInput.actions["Move"].ReadValue<Vector2>();
            HandleCollisionDetection();
        }

        public void SetVelocity(float x, float y)
        {
            this.rb.velocity = new Vector2(x, y);
            FlipHandler(x);
        }

        public void SetVelocity(Vector2 vector)
        {
            this.rb.velocity = vector;
            FlipHandler(vector.x);
        }
        
        private void FlipHandler(float xVelocity)
        {
            if ((xVelocity > 0 && !isFacingRight) || (xVelocity < 0 && isFacingRight))
            {
                Flip();
            }
        }

        public void Flip()
        {
            transform.Rotate(0,180,0);
            isFacingRight = !isFacingRight;
            FacingDirection *= -1;
        }
        
        private void HandleCollisionDetection()
        {
            GroundDetected = Physics2D.Raycast(transform.position, Vector2.down, stateInfo.groundCheckDistance, groundLayer);
            WallDetected = Physics2D.Raycast(transform.position, Vector2.right * FacingDirection, stateInfo.wallCheckDistance, groundLayer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,-stateInfo.groundCheckDistance,0));
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(stateInfo.wallCheckDistance, 0) * FacingDirection);// (_isFacingRight ? 1 : -1)
        }

        public Vector2 GetVelocity()
        {
            return this.rb.velocity;
        }

        public bool PressJumpKey()
        {
            return playerInput.actions["Jump"].WasPressedThisFrame();
            // playerIM.P1.Jump.WasPressedThisFrame()
        }

        public bool PressDashKey()
        {
            return playerInput.actions["Jump"].WasPressedThisFrame();
        }

        public bool PressSpecialKey(string keyName)
        {
            return playerInput.actions[keyName].WasPressedThisFrame();
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