using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
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
        public IActor actor;
        private Collider2D col;

        // private PlayerIM input;
        private PlayerInput playerInput;
        public PlayerIM playerIM;
        public Vector2 moveValue;
        
        // State Change
        public PlayerStateInfo stateInfo; // TODO: 将被 PlayerConfig 替代

        // 时间记录
        public float lastDashTime;
        public float lastJumpTime; // 记录最后一次跳跃的时间
        public float lastWallJumpTime; // 记录最后一次墙跳的时间
        public float lastWallSlideTime; // 记录最后一次离开墙壁的时间
        public int wallJumpDirection; // 墙跳的方向（1 或 -1）
        
        // 二段跳
        public bool hasDoubleJump = true; // 是否拥有二段跳权限
        
        // 冲刺
        public float dashStartTime; // 冲刺开始时间
        public Vector2 dashDirection; // 冲刺方向
        
        private bool isFacingRight;
        [SerializeField] private LayerMask groundLayer; //墙和地面的Layer
        public int FacingDirection { get; private set; } = 1;
        public bool GroundDetected { get; private set; }
        public bool WallDetected { get; private set; }
        public Vector2 fireDirection => Vector2.right * FacingDirection;

        private void Awake()
        {
            Debug.Log($"{name} Awake");
            
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            actor = GetComponent<IActor>();
            playerInput = GetComponent<PlayerInput>();
            
            playerInput.SwitchCurrentActionMap(name);
            Debug.Log(name + "switch to custom action map");
            
            animaMgr = new AnimationManager(GetComponentInChildren<Animator>());
            stateInfo = GetComponent<PlayerStateInfo>();
            
            // 检查必需组件
            if (stateInfo == null)
            {
                Debug.LogError($"[{name}] CRITICAL: PlayerStateInfo component is missing! Please attach it to the GameObject. StateMachine will not be initialized.");
                return; // 不初始化状态机，避免崩溃
            }
            
            // 尝试初始化状态机，捕获可能的异常
            try
            {
                stateMachine = new PlayerStateMachine(this);
                Debug.Log($"[{name}] StateMachine initialized successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"[{name}] CRITICAL: Failed to initialize StateMachine! Exception: {e.GetType().Name}\nMessage: {e.Message}\nStackTrace:\n{e.StackTrace}");
                stateMachine = null;
            }
        }

        private void Start()
        {
            isFacingRight = true;
            hasDoubleJump = true; // 初始拥有二段跳
            
            // 应用 PlayerConfig 的物理参数
            rb.gravityScale = PlayerConfig.GravityScale;
            // Unity 不直接支持 maxSpeed，需要在 FixedUpdate 中手动限制
        }

        private void Update()
        {
            if (stateMachine == null)
            {
                Debug.LogError($"[{name}] stateMachine is null! Check if PlayerStateInfo component is attached.");
                return;
            }
            
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
            return playerInput.actions["Dash"].WasPressedThisFrame();
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