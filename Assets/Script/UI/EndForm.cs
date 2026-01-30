using MemoFramework.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace TapTap2025
{
    public class EndForm : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button closeButton;
        
        private void OnEnable()
        {
            RegisterListener();
            PanelManager.Instance?.RegisterPanel(PanelIdConstants.EndPanel,gameObject);
        }
        private void OnDisable()
        {
            UnregisterListener();
            PanelManager.Instance?.UnregisterPanel(PanelIdConstants.MenuPanel);
        }

        private void RegisterListener()
        {
            restartButton.onClick.AddListener(ClickReStart);
            closeButton.onClick.AddListener(ClickQuit);
        }

        private void UnregisterListener()
        {
            restartButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }
        
        private void ClickReStart()
        {
            MF.Event.Fire(this, OnRequireEnterGame.Create());
        }

        private void ClickQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}