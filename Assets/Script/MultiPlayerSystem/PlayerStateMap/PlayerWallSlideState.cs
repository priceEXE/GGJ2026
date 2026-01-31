using GGJ2026;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public class PlayerWallSlideState : PlayerState
    {
        public PlayerWallSlideState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            
            // 粘墙时失去二段跳权限
            owner.hasDoubleJump = false;
            
            StateTimer = owner.stateInfo.slideDuration;
        }

        public override void Update()
        {
            base.Update();

            if (StateTimer < 0.0f)
            {
                Debug.LogWarning("Slide Time out");
                owner.stateInfo.canSlide = false;
                owner.stateMachine.ChangeState(PlayerStates.Fall);
            }

            HandleSlide();
            if (owner.PressJumpKey())
            {
                owner.stateMachine.ChangeState(PlayerStates.WallJump);
                return; // 重要：立即返回，防止后续逻辑覆盖状态
            }

            // 如果不再主动抓墙 (WallGrabDetected 为 false)，则退出滑行状态
            if (!owner.WallGrabDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
                return;
            }

            // 如果玩家松开朝向墙壁的按键，立即脱离墙壁
            bool pressingTowardWall = owner.moveValue.x * owner.FacingDirection > 0;
            if (!pressingTowardWall)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
            }

            if (owner.GroundDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Idle);
                owner.Flip();
            }
        }

        public override void Exit()
        {
            base.Exit();
            // 记录离开墙壁的时间（用于墙跳缓冲）
            owner.lastWallSlideTime = Time.time;
        }

        private void HandleSlide()
        {
            if (owner.moveValue.y < 0)
            {
                owner.SetVelocity(0, owner.rb.velocity.y);
            }
            if (owner.moveValue.y < 0)
            {
                owner.SetVelocity(0, owner.rb.velocity.y);
            }
            else
            {
                // 如果是上升阶段 (velocity.y > 0)，不应用摩擦力，允许保留墙跳冲量
                if (owner.rb.velocity.y > 0)
                {
                    owner.SetVelocity(owner.moveValue.x, owner.rb.velocity.y);
                }
                else
                {
                    // 下落阶段才应用滑墙摩擦
                    owner.SetVelocity(owner.moveValue.x, owner.rb.velocity.y * owner.stateInfo.slideSpeedMulti);
                }
            }
        }
    }
}