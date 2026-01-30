using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapTap2025
{
    [Serializable]
    public class PanelConfig
    {
        public int PanelId;
        public GameObject PanelPrefab;
    }

    public class PanelManager : MonoBehaviour
    {
        [SerializeField] List<PanelConfig> panelConfigs;
        public static PanelManager Instance { get; private set; }

        private Dictionary<int, GameObject> _panelPrefabs = new();
        private MainCanvas _mainCanvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
                LoadPanelConfig();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private Dictionary<int, GameObject> _shownPanels = new Dictionary<int, GameObject>();

        public void RegisterPanel(int panelId, GameObject panel)
        {
            _shownPanels.Add(panelId, panel);
        }

        public void UnregisterPanel(int panelId)
        {
            if (_shownPanels.ContainsKey(panelId))
            {
                _shownPanels.Remove(panelId);
            }
            else
            {
                Debug.LogError($"Panel with ID {panelId} is not registered.");
            }
        }

        public void CheckShow(int panelId, bool top = false, bool allowMulti = false)
        {
            if (!_panelPrefabs.ContainsKey(panelId))
            {
                Debug.LogError("Panel with ID " + panelId + " does not exist in the configuration.");
                return;
            }

            if (_shownPanels.ContainsKey(panelId) && !allowMulti)
            {
                Debug.LogError($"Panel with ID {panelId} is already shown.");
                return;
            }

            GameObject panelPrefab = _panelPrefabs[panelId];
            GameObject panelInstance = Instantiate(panelPrefab, _mainCanvas.transform);
            panelInstance.SetActive(true);
            if (top)
            {
                panelInstance.transform.SetAsLastSibling();
            }
        }

        public void Close(int panelId)
        {
            if (_shownPanels.TryGetValue(panelId, out GameObject panel))
            {
                Destroy(panel);
            }
            else
            {
                Debug.LogError($"Panel with ID {panelId} is not currently shown.");
            }
        }

        public void RegisterMainCanvas(MainCanvas canvas)
        {
            if (_mainCanvas)
            {
                Debug.LogError($"Panel {canvas.gameObject.name} is already present.");
            }

            _mainCanvas = canvas;
        }

        public void UnregisterMainCanvas(MainCanvas canvas)
        {
            if (canvas == _mainCanvas)
            {
                _mainCanvas = null;
            }
        }

        private void LoadPanelConfig()
        {
            if (panelConfigs != null)
            {
                foreach (PanelConfig panelConfig in panelConfigs)
                {
                    if (!_panelPrefabs.ContainsKey(panelConfig.PanelId))
                    {
                        _panelPrefabs.Add(panelConfig.PanelId, panelConfig.PanelPrefab);
                    }
                    else
                    {
                        Debug.LogError($"Panel with ID {panelConfig.PanelId} is already present.");
                    }
                }
            }
        }
    }
}