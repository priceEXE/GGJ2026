using System;
using System.Collections.Generic;
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
        Slide,
        WallJump,
        Fall,
        GetHit
    }
    
    public class PlayerStateMachine
    {
        public PlayerInstance owner;
        public Dictionary<PlayerStates, IPlayerState> states;
        public IPlayerState currentState;
        public PlayerStates currentStateType;

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
            states.Add(PlayerStates.Jump, new PlayerJumpState(this.owner, "JumpFall"));
            states.Add(PlayerStates.Dash, new PlayerDashState(this.owner, "Dash"));
            states.Add(PlayerStates.Fall, new PlayerFallState(this.owner, "JumpFall"));
            states.Add(PlayerStates.WallJump, new PlayerWallJumpState(this.owner, "JumpFall"));
            states.Add(PlayerStates.Slide, new PlayerWallSlideState(this.owner, "Slide"));
            
            ChangeState(PlayerStates.Idle);
            currentStateType = PlayerStates.Idle;
        }

        public void Update()
        {
            currentState.Update();
        }

        public void ChangeState(PlayerStates newState)
        {
            currentState?.Exit();
            currentStateType = newState;
            currentState = states[newState];
            currentState.Enter();
        }
    }
}