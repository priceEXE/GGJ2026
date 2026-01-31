using UnityEngine;

namespace GGJ2026.PlayerStateMap
{
    public class PlayerIdleState : PlayerState
    {

        public PlayerIdleState(PlayerInstance owner, string animaName)
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
            // base.Update();
            if (owner.moveValue.x != 0)
            {
                owner.stateMachine.ChangeState(PlayerStates.Move);
                // Debug.Log($"{owner.name} 's XAxis Value is {owner.moveValue.x}");
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}