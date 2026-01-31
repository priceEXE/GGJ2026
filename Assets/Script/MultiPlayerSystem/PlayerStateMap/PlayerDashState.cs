using UnityEngine;

namespace GGJ2026
{
    public class PlayerDashState : PlayerState
    {
        private float oldGravityScale;
        private float dashDirection;
        public PlayerDashState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            // Debug.Log($"{name} Press Special Key: {keyName}");
            StateTimer = owner.stateInfo.dashDuration;
            oldGravityScale = owner.rb.gravityScale;
            owner.rb.gravityScale = 0;
            dashDirection = owner.FacingDirection;
            owner.lastDashTime = Time.time;
        }

        public override void Update()
        {
            base.Update();
            CancelDashing();
            owner.SetVelocity(owner.stateInfo.dashSpeed * dashDirection, 0f);
            if (StateTimer < 0.0f)
            {
                if (owner.GroundDetected)
                    owner.stateMachine.ChangeState(PlayerStates.Idle);
                else
                    owner.stateMachine.ChangeState(PlayerStates.Fall);
            }
        }

        public override void Exit()
        {
            base.Exit();
            owner.SetVelocity(0f, 0f);
            owner.rb.gravityScale = oldGravityScale;
        }
        
        private void CancelDashing()
        {
            if (owner.WallDetected)
            {
                if (owner.GroundDetected)
                    owner.stateMachine.ChangeState(PlayerStates.Idle);
                else
                    owner.stateMachine.ChangeState(PlayerStates.Fall);
            }
        }
        
    }
}