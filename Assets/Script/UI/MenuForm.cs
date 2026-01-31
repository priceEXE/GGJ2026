using System;
using MemoFramework.Extension;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGJ2026
{
    public class MenuForm : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private Button EnterGame_Btn;
        [SerializeField] private Button TestGame_Btn;
        [SerializeField] private Button ExitGame_Btn;
        

        private void OnEnable()
        {
            RegisterListener();
#if UNITY_EDITOR
            TestGame_Btn.gameObject.SetActive(true);
#else
            TestGame_Btn.gameObject.SetActive(false);
#endif
            PanelManager.Instance?.RegisterPanel(PanelIdConstants.MenuPanel,gameObject);
        }
        private void OnDisable()
        {
            UnregisterListener();
            PanelManager.Instance?.UnregisterPanel(PanelIdConstants.MenuPanel);
        }

        private void RegisterListener()
        {
            EnterGame_Btn.onClick.AddListener(ClickEnter);
            ExitGame_Btn.onClick.AddListener(ClickExit);
            TestGame_Btn.onClick.AddListener(ClickTest);
        }
        
        private void UnregisterListener()
        {
            EnterGame_Btn.onClick.RemoveListener(ClickEnter);
            ExitGame_Btn.onClick.RemoveListener(ClickExit);
            TestGame_Btn.onClick.RemoveListener(ClickTest);
        }
        
        private void ClickEnter()
        {
            MF.Event.Fire(this, OnRequireEnterGame.Create());
        }

        private void ClickTest()
        {
            MF.Event.Fire(this, OnRequireEnterTest.Create());
        }

        private void ClickExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        
    }
}
