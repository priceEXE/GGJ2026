using System;
using MemoFramework;
using MemoFramework.Extension;
using MemoFramework.GameState;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GGJ2026
{
    public class MainForm : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private GameObject Btns;
        [SerializeField] private Button continue_btn;
        [SerializeField] private Button return_btn;
        
        private void RequirePause(InputAction.CallbackContext ctx)
        {
            if (Btns.gameObject.activeSelf == false)
            {
                Time.timeScale = 0.0f;
                Btns.gameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                Btns.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            RegisterListener();
            PanelManager.Instance?.RegisterPanel(PanelIdConstants.MainPanel,gameObject);
        }

        private void OnDisable()
        {
            UnregisterListener();
            PanelManager.Instance?.UnregisterPanel(PanelIdConstants.MainPanel);
        }

        private void RegisterListener()
        {
            MF.Input.InputMap.asset["Return"].started += RequirePause;
            continue_btn.onClick.AddListener(ClickCountinueButton);
            return_btn.onClick.AddListener(ClickReturnButton);
        }
        
        private void UnregisterListener()
        {
            MF.Input.InputMap.asset["Return"].started -= RequirePause;
            continue_btn.onClick.RemoveAllListeners();
            return_btn.onClick.RemoveAllListeners();
        }

        private void ClickCountinueButton()
        {
            Time.timeScale = 1.0f;
            Btns.gameObject.SetActive(false);
        }

        private void ClickReturnButton()
        {
            Time.timeScale = 1.0f;
            MF.Event.Fire(this,OnRequireEnterMenu.Create());
        }
    }
}