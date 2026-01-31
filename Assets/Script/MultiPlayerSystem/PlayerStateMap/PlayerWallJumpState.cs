

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
            owner.SetVelocity(owner.stateInfo.wallJumpDirection.x * -owner.FacingDirection, owner.stateInfo.wallJumpDirection.y);
        }

        public override void Update()
        {
            base.Update();
            if (owner.rb.velocity.y < 0)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
            }
            if (owner.WallDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Slide);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}