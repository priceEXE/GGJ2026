using UnityEngine;

namespace GGJ2026.PlayerStateMap
{
    public class PlayerIdleState : PlayerGroundState
    {

        public PlayerIdleState(PlayerInstance owner, string animaName) : base(owner, animaName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            owner.SetVelocity(0,owner.rb.velocity.y);
        }

        public override void Update()
        {
            if (owner.moveValue.x != 0)
            {
                owner.stateMachine.ChangeState(PlayerStates.Move);
                // Debug.Log($"{owner.name} 's XAxis Value is {owner.moveValue.x}");
            }
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}