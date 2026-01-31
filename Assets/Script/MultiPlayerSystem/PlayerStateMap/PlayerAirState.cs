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
            if (owner.moveValue.x != 0)
            {
                owner.SetVelocity(owner.moveValue.x * owner.stateInfo.moveSpeed, owner.rb.velocity.y);
            }

            if (owner.WallDetected && owner.stateInfo.canSlide)
            {
                owner.stateMachine.ChangeState(PlayerStates.Slide);
            }

            if (owner.PressSpecialKey("Jump") && owner.stateInfo.canJumpInAir)
            {
                owner.stateInfo.canJumpInAir = false;
                owner.stateMachine.ChangeState(PlayerStates.Jump);
            }
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}