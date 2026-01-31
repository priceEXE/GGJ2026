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

            if (owner.GroundDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Idle);
                owner.Flip();
            }
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