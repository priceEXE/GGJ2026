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
            }

            if (!owner.WallDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
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
            else
            {
                owner.SetVelocity(owner.moveValue.x, owner.rb.velocity.y * owner.stateInfo.slideSpeedMulti);
            }
        }
    }
}