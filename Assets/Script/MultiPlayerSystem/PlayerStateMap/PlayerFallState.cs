namespace GGJ2026
{
    public class PlayerFallState : PlayerAirState
    {
        public PlayerFallState(PlayerInstance owner, string animaName) : base(owner, animaName)
        {
        }

        public override void Update()
        {
            base.Update();
            if (owner.GroundDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Idle);
            }
        }
    }
}