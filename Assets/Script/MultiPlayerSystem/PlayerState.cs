using UnityEngine;

namespace GGJ2026
{
    public class PlayerState : IPlayerState
    {
        public PlayerInstance owner;
        public string animaName;
        public virtual void Enter()
        {
            // throw new System.NotImplementedException();
            Debug.Log($"{owner.name} Enter State: {animaName}");
            owner.animaMgr.SetBool(animaName, true);
        }

        public virtual void Update()
        {
            // throw new System.NotImplementedException();
        }

        public virtual void Exit()
        {
            owner.animaMgr.SetBool(animaName, false);
        }
    }
}