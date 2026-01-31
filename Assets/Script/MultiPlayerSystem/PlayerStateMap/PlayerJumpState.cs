using UnityEngine;

namespace GGJ2026
{
    public class PlayerJumpState : PlayerAirState
    {
        private bool isDoubleJump; // 是否为二段跳

        public PlayerJumpState(PlayerInstance owner, string animaName) : base(owner, animaName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            // 记录跳跃时间
            owner.lastJumpTime = Time.time;
            
            // 判断是一段跳还是二段跳
            isDoubleJump = !owner.hasDoubleJump;
            
            // 应用不同的跳跃力度
            float jumpForce = isDoubleJump ? PlayerConfig.DoubleJumpForce : PlayerConfig.JumpForce;
            owner.SetVelocity(owner.rb.velocity.x, jumpForce);
            
            Debug.Log($"[{owner.name}] Jump: {(isDoubleJump ? "Double" : "First")} jump, force={jumpForce}");
        }

        public override void Update()
        {
            base.Update();
            
            // 开始下降时切换到 Fall 状态
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