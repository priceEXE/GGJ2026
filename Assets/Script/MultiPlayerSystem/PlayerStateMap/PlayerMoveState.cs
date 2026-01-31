using UnityEngine;

namespace GGJ2026
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(PlayerInstance owner, string animaName)
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
            owner.rb.velocity = Vector2.right * owner.moveValue;
        }

        public override void Exit()
        {
            owner.rb.velocity = new Vector2(0, owner.rb.velocity.y);
            base.Exit();
        }
        
    }
}