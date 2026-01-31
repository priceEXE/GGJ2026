using UnityEngine;

namespace GGJ2026
{
    public class PlayerMoveState : PlayerGroundState
    {
        public PlayerMoveState(PlayerInstance owner, string animaName) : base(owner, animaName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            // owner.rb.velocity = Vector2.right * owner.moveValue;
            owner.SetVelocity(owner.stateInfo.moveSpeed * owner.moveValue.x,owner.rb.velocity.y);
            base.Update();
        }

        public override void Exit()
        {
            owner.rb.velocity = new Vector2(0, owner.rb.velocity.y);
            base.Exit();
        }
        
    }
}