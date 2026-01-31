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
            owner.animeController?.PlayAnimation(Gameplay.AnimeAndCostume.SpineAnimations.run, true);
        }

        public override void Update()
        {
            // 使用 PlayerConfig 的移动速度
            owner.SetVelocity(PlayerConfig.MoveSpeed * owner.moveValue.x, owner.rb.velocity.y);
            
            if (Mathf.Abs(owner.moveValue.x) < 0.01f)
            {
                owner.stateMachine.ChangeState(PlayerStates.Idle);
                return;
            }
            base.Update();
        }

        public override void Exit()
        {
            owner.rb.velocity = new Vector2(0, owner.rb.velocity.y);
            base.Exit();
        }
        
    }
}