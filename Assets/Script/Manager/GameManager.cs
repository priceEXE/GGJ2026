using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                }
                return _instance;
            }
        }
        private Player _player;
        public GuideTextUI guideTextUI;
        // public PlayerIM input;
        //public Dictionary<string, InputAction> playerMaps = new (){{"P1", input.P1}}
        // public PlayerInputManager playerInputManager;
        public Player player
        {
            get
            {
                if (_player == null)
                {
                    _player = FindObjectOfType<Player>();
                }
                return _player;
            }
        }
    }
}