

using UnityEngine;

namespace GGJ2026
{
    public class PlayerWallJumpState : PlayerState
    {
        public PlayerWallJumpState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log(owner.name + "wall jump");
            
            // 记录墙跳时间和方向
            owner.lastWallJumpTime = UnityEngine.Time.time;
            owner.wallJumpDirection = -owner.FacingDirection; // 墙跳方向是离开墙壁的方向
            
            // 墙跳会消耗二段跳权限（直到落地才恢复）
            owner.hasDoubleJump = false;
            
            // 使用 PlayerConfig 的墙跳参数
            float horizontalForce = PlayerConfig.WallJumpHorizontal * owner.wallJumpDirection;
            float verticalForce = PlayerConfig.WallJumpVertical;
            owner.SetVelocity(horizontalForce, verticalForce);
        }

        public override void Update()
        {
            base.Update();
            if (owner.rb.velocity.y < 0)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
                return; // 修复缺失的 return
            }
            // 墙跳锁定：跳跃后一定时间内禁止重新抓墙（防止立即吸回墙壁）
            // 既然 SlideState 已经修复了"上升时不减速"，现在我们可以放心地允许早期抓墙
            if (Time.time - owner.lastWallJumpTime > PlayerConfig.WallJumpInputLock)
            {
                if (owner.WallGrabDetected)
                {
                    owner.stateMachine.ChangeState(PlayerStates.Slide);
                }
            }

            // ========== 允许空中移动控制 ==========
            // 用户反馈希望取消"硬控"，允许在墙跳上升阶段控制方向
            // 特殊逻辑：如果在墙跳时按住朝向墙壁的方向，则视为"垂直爬墙"，取消水平弹力
            if (owner.moveValue.x != 0)
            {
                // 检测是否按住朝向墙壁的方向 (与墙跳方向相反)
                bool pressingTowardWall = Mathf.Sign(owner.moveValue.x) != Mathf.Sign(owner.wallJumpDirection);
                
                // 如果按向墙壁，且处于刚起跳阶段 (0.1s内)，则大幅衰减水平速度，实现"垂直跳"
                if (pressingTowardWall && (Time.time - owner.lastWallJumpTime < 0.1f))
                {
                     // 强行将水平速度归零或减小，配合 AirState 的后续控制
                     // 这里直接覆盖之前的 SetVelocity 的水平分量
                     owner.SetVelocity(0, owner.rb.velocity.y);
                }
                else 
                {
                    // 常规空中控制
                    float targetSpeed = owner.moveValue.x * PlayerConfig.MoveSpeed;
                    float airAcceleration = PlayerConfig.MoveSpeed * PlayerConfig.AirSpeedMultiplier * 50f;
                    float newVelocityX = Mathf.MoveTowards(owner.rb.velocity.x, targetSpeed, airAcceleration * Time.deltaTime);
                    owner.SetVelocity(newVelocityX, owner.rb.velocity.y);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}