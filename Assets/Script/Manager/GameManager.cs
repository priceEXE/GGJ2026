using System;
using UnityEngine;

namespace TapTap2025
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