using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Gameplay.AnimeAndCostume;

namespace GGJ2026
{
    public class PlayerInstance : MonoBehaviour
    {
        public string Name;
        public PlayerStateMachine stateMachine;
        public AnimationManager animaMgr;
        public CharacterAnimeController animeController;
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
        public float lastGroundedTime; // 记录最后一次在地面的时间
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
            animeController = GetComponentInChildren<CharacterAnimeController>();
            if (animeController == null) Debug.LogError($"{name} failed to find CharacterAnimeController!");
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
        
        public bool WallGrabDetected { get; private set; } // 主动蹬墙检测

        private void HandleCollisionDetection()
        {
            UpdateGroundCheck();
            UpdatePassiveWallCheck();
            UpdateActiveWallGrabCheck();
        }

        private void UpdateGroundCheck()
        {
            GroundDetected = Physics2D.Raycast(transform.position, Vector2.down, stateInfo.groundCheckDistance, groundLayer);
        }

        private void UpdatePassiveWallCheck()
        {
            // ============ 墙壁检测预计算 ============
            float bodyHeight = col.bounds.size.y;
            Vector2 wallCheckOrigin = (Vector2)transform.position + Vector2.up * (bodyHeight * 0.5f);

            // 2. 被动墙壁检测 (WallDetected) - 用于冲刺撞墙等被动物理逻辑
            // 依旧基于朝向 (FacingDirection)
            WallDetected = Physics2D.Raycast(wallCheckOrigin, Vector2.right * FacingDirection, stateInfo.wallCheckDistance, groundLayer);
        }

        private void UpdateActiveWallGrabCheck()
        {
            // 3. 主动蹬墙检测 (WallGrabDetected) - 用于爬墙/蹬墙跳
            WallGrabDetected = false;
            
            // 无输入则不检测
            if (Mathf.Abs(moveValue.x) <= 0.1f) return;

            float inputDir = Mathf.Sign(moveValue.x);
            float scanDistance = 0.1f;

            // 设置过滤条件：只检测 Ground 层
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(groundLayer);
            filter.useLayerMask = true;

            RaycastHit2D[] results = new RaycastHit2D[1];
            
            // 执行投射
            int hitCount = col.Cast(Vector2.right * inputDir, filter, results, scanDistance);

            if (hitCount > 0)
            {
                RaycastHit2D hit = results[0];
                Vector2 normal = hit.normal;

                // 1. 法线过滤：必须是侧面墙壁 (Abs(x) > Abs(y))
                bool isSideWall = Mathf.Abs(normal.x) > Mathf.Abs(normal.y);
                
                // 2. 平台过滤：(移除)
                // 用户反馈需要抓取单向 EdgeCollider 墙壁。
                // 依赖严格的法线判定来过滤地面。
                
                if (isSideWall)
                {
                    WallGrabDetected = true;
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (col == null) return;
            
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,-stateInfo.groundCheckDistance,0));
            
            // 显示墙壁检测射线（从身体中部）
            float bodyHeight = col.bounds.size.y;
            Vector3 wallCheckOrigin = transform.position + Vector3.up * (bodyHeight * 0.5f);
            
            // 被动检测 (红)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheckOrigin, wallCheckOrigin + new Vector3(stateInfo.wallCheckDistance, 0) * FacingDirection);
            
            // 主动检测 (绿) - 仅当有输入时显示
            if (Application.isPlaying && Mathf.Abs(moveValue.x) > 0.1f)
            {
                Gizmos.color = Color.green;
                float scanDistance = 0.1f;
                float inputDir = Mathf.Sign(moveValue.x);
                
                // 可视化 Cast 效果
                // 画出目标位置的碰撞体 bounds
                Vector3 center = col.bounds.center;
                Vector3 targetCenter = center + (Vector3)(Vector2.right * inputDir * scanDistance);
                
                Gizmos.DrawWireCube(targetCenter, col.bounds.size);
                Gizmos.DrawLine(center, targetCenter);
            }
            Gizmos.color = Color.white;
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