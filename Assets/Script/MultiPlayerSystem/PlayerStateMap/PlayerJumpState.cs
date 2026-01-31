using GGJ2026;

namespace GGJ2026
{
    public class PlayerJumpState : PlayerAirState
    {
        public PlayerJumpState(PlayerInstance owner, string animaName) : base(owner, animaName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            owner.SetVelocity(owner.rb.velocity.x, owner.stateInfo.jumpForce);
        }

        public override void Update()
        {
            base.Update();
            if (owner.GetVelocity().y < 0)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}