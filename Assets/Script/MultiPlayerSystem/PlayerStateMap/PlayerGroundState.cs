namespace GGJ2026
{
    public class PlayerGroundState : PlayerState
    {
        public PlayerGroundState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            owner.stateInfo.canJumpInAir = true;
            owner.stateInfo.canSlide = true;
        }

        public override void Update()
        {
            base.Update();
            if (owner.rb.velocity.y < 0 && !owner.GroundDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
            }

            if (owner.PressSpecialKey("Jump"))
            {
                owner.stateMachine.ChangeState(PlayerStates.Jump);
            }
        }
    }
}