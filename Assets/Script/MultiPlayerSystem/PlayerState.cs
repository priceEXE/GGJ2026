using UnityEngine;

namespace GGJ2026
{
    public class PlayerState : IPlayerState
    {
        public PlayerInstance owner;
        public string animaName;
        protected float StateTimer;
        public virtual void Enter()
        {
            // throw new System.NotImplementedException();
            Debug.Log($"{owner.name} Enter State: {animaName}");
            owner.animaMgr.SetBool(animaName, true);
        }

        public virtual void Update()
        {
            // throw new System.NotImplementedException();
            owner.animaMgr.SetFloat("yVelocity", owner.rb.velocity.y);
            if (owner.PressDashKey() && CanDash())
            {
                owner.stateMachine.ChangeState(PlayerStates.Dash);
            }
            StateTimer -= Time.deltaTime;
        }

        public virtual void Exit()
        {
            owner.animaMgr.SetBool(animaName, false);
        }
        
        private bool CanDash()
        {
            if (owner.WallDetected)
            {
                return false;
            }
            if (owner.stateMachine.currentStateType == PlayerStates.Dash)
            {
                return false;
            }

            return true;
        }
    }
}