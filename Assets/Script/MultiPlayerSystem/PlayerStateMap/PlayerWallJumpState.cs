

using UnityEngine;

namespace GGJ2026
{
    public class PlayerWallJumpState : PlayerState
    {
        public PlayerWallJumpState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log(owner.name + "wall jump");
            
            // 记录墙跳时间和方向
            owner.lastWallJumpTime = UnityEngine.Time.time;
            owner.wallJumpDirection = -owner.FacingDirection; // 墙跳方向是离开墙壁的方向
            
            // 墙跳会消耗二段跳权限（直到落地才恢复）
            owner.hasDoubleJump = false;
            
            // 使用 PlayerConfig 的墙跳参数
            float horizontalForce = PlayerConfig.WallJumpHorizontal * owner.wallJumpDirection;
            float verticalForce = PlayerConfig.WallJumpVertical;
            owner.SetVelocity(horizontalForce, verticalForce);
        }

        public override void Update()
        {
            base.Update();
            if (owner.rb.velocity.y < 0)
            {
                owner.stateMachine.ChangeState(PlayerStates.Fall);
            }
            if (owner.WallDetected)
            {
                owner.stateMachine.ChangeState(PlayerStates.Slide);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}