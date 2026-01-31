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
            
            // 落地时恢复二段跳权限
            owner.hasDoubleJump = true;
            
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

            if (owner.PressJumpKey())
            {
                owner.stateMachine.ChangeState(PlayerStates.Jump);
            }
        }
    }
}