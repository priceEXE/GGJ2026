using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

namespace GGJ2026
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private float itemCreateTimer = 0;
        [SerializeField] private List<GameObject> itemCreatePoint;
        [SerializeField] private float itemCreateColdTime;
        private Dictionary<GameObject, bool> itemCreateDict;
        private Random random;
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
        public GuideTextUI guideTextUI;
        // public PlayerIM input;
        //public Dictionary<string, InputAction> playerMaps = new (){{"P1", input.P1}}
        // public PlayerInputManager playerInputManager;
        public Dictionary<string, PlayerInstance> players;
        private void Start()
        {
            itemCreateTimer = 0;
            itemCreateDict = new();
            random = new Random((int)DateTime.Now.Ticks);
        }

        private void Update()
        {
            HandleItemCreate();
        }

        private void HandleItemCreate()
        {
            itemCreateTimer -= Time.deltaTime;
            if (itemCreateTimer < 0)
            {
                var validPoint = (from x in itemCreateDict where x.Value select x.Key).ToList();
                int randomValue = random.Next(validPoint.Count);
                // 在validPoint生成物体
                itemCreateDict[validPoint[randomValue]] = false;
            }
        }
    }
    
    
}