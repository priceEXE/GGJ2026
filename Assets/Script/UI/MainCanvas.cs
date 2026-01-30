using UnityEngine;

namespace TapTap2025
{
    public class MainCanvas : MonoBehaviour
    {
        private void Awake()
        {
            PanelManager.Instance?.RegisterMainCanvas(this);
        }

        private void OnDestroy()
        {
            PanelManager.Instance?.UnregisterMainCanvas(this);
        }
    }
}