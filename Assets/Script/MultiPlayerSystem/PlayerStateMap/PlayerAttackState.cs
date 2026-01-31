using UnityEngine;

namespace GGJ2026
{
    public class PlayerAttackState : PlayerState
    {
        
        public PlayerAttackState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            // oldVelocity = owner.GetVelocity();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}