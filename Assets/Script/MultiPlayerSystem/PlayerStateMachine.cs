using System;
using System.Collections.Generic;
using GGJ2026.PlayerStateMap;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GGJ2026
{
    public enum PlayerStates
    {
        Idle,
        Move,
        Jump,
        Dash,
        Slid,
        Fall
    }
    
    public class PlayerStateMachine
    {
        public PlayerInstance owner;
        public Dictionary<PlayerStates, IPlayerState> states;
        public IPlayerState currentState;

        public PlayerStateMachine(PlayerInstance owner)
        {
            this.owner = owner;
            states = new ();
            Init();
        }

        private void Init()
        {
            states.Add(PlayerStates.Idle, new PlayerIdleState(this.owner, "Idle"));
            states.Add(PlayerStates.Move, new PlayerMoveState(this.owner, "Move"));
            
            ChangeState(PlayerStates.Idle);
        }

        public void Update()
        {
            currentState.Update();
        }

        public void ChangeState(PlayerStates newState)
        {
            currentState?.Exit();
            currentState = states[newState];
            currentState.Enter();
        }
    }
}