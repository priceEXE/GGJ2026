namespace GGJ2026
{
    public class PlayerGetHitState : PlayerAirState
    {
        public PlayerGetHitState(PlayerInstance owner, string animaName) : base(owner, animaName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            owner.SetVelocity(owner.stateInfo.getHitDirection * owner.stateInfo.hitDirection.x, owner.stateInfo.hitDirection.y);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}