using UnityEngine;

namespace GGJ2026
{
    public class PlayerAirState : PlayerState
    {
        public PlayerAirState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            // ========== 空中移动控制（带速度衰减） ==========
            HandleAirMovement();

            // ========== 跳跃输入检测 ==========
            if (owner.PressJumpKey())
            {
                // 检查墙跳缓冲（离开墙壁 0.15 秒内优先触发墙跳）
                float timeSinceWallSlide = Time.time - owner.lastWallSlideTime;
                bool inWallJumpBuffer = timeSinceWallSlide < 0.15f;
                
                if (inWallJumpBuffer)
                {
                    // 墙跳缓冲期内：触发墙跳（优先于二段跳）
                    owner.stateMachine.ChangeState(PlayerStates.WallJump);
                    return;
                }
                else if (owner.hasDoubleJump)
                {
                    // 不在墙跳缓冲期：触发二段跳
                    owner.hasDoubleJump = false;
                    owner.stateMachine.ChangeState(PlayerStates.Jump);
                    return;
                }
            }

            // ========== 墙壁滑行检测 ==========
            bool pressingTowardWall = owner.moveValue.x * owner.FacingDirection > 0;
            float timeSinceJump = Time.time - owner.lastJumpTime;
            bool jumpGracePeriodOver = timeSinceJump > 0.1f;
            
            // 只在明显下降时才能粘墙（避免平台边缘被抓住）
            bool isFalling = owner.rb.velocity.y < -0.5f;
            
            if (owner.WallDetected && owner.stateInfo.canSlide && isFalling && pressingTowardWall && jumpGracePeriodOver)
            {
                owner.stateMachine.ChangeState(PlayerStates.Slide);
                return;
            }

            base.Update();
        }

        /// <summary>
        /// 处理空中移动（保留惯性，AirSpeedMultiplier 作为加速度倍率）
        /// </summary>
        private void HandleAirMovement()
        {
            // 墙跳后0.2秒内，忽略反方向的输入
            float timeSinceWallJump = Time.time - owner.lastWallJumpTime;
            bool inWallJumpLockPeriod = timeSinceWallJump < 0.2f;

            if (inWallJumpLockPeriod && owner.moveValue.x != 0)
            {
                // 如果输入方向与墙跳方向相反，忽略输入
                bool inputOppositeToWallJump = Mathf.Sign(owner.moveValue.x) != Mathf.Sign(owner.wallJumpDirection);
                if (inputOppositeToWallJump)
                    return; // 忽略反方向输入
            }

            // 空中移动：保留惯性，但加速度较慢
            if (owner.moveValue.x != 0)
            {
                // 目标速度
                float targetSpeed = owner.moveValue.x * PlayerConfig.MoveSpeed;
                
                // 空中加速度（相对于地面的倍率）
                float airAcceleration = PlayerConfig.MoveSpeed * PlayerConfig.AirSpeedMultiplier * 50f; // 50 是调节参数
                
                // 平滑过渡到目标速度（保留惯性）
                float newVelocityX = Mathf.MoveTowards(owner.rb.velocity.x, targetSpeed, airAcceleration * Time.deltaTime);
                owner.SetVelocity(newVelocityX, owner.rb.velocity.y);
            }
            // 如果没有输入，保持当前速度（惯性）
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}