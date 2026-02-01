using UnityEngine;
using Gameplay.AnimeAndCostume;

namespace Gameplay.AnimeAndCostume
{
    public class SlotVisibilityTester : MonoBehaviour
    {
        public CharacterAnimeController controller;

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterAnimeController>();
            }
        }

        private void Update()
        {
            // 1. 测试 face1 (默认: 打开)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("[Tester] Toggling face1 (Default: ON)");
                controller.ToggleSlot(SpineSlots.face1);
            }

            // 2. 测试 cake (默认: 打开)
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("[Tester] Toggling cake (Default: ON)");
                controller.ToggleSlot(SpineSlots.cake);
            }

            // 3. 测试 cakeFace (默认: 关闭)
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("[Tester] Toggling cakeFace (Default: OFF)");
                controller.ToggleSlot(SpineSlots.cakeFace);
            }

             // 4. 测试 body (永远打开，不应消失)
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("[Tester] Attempting to Toggle body (Permanent, Should stay ON)");
                controller.ToggleSlot(SpineSlots.body);
            }

            // 5. 测试 Cannon (非Spine槽位，使用SpriteRenderer)
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("[Tester] Toggling Cannon (Non-Spine Slot, Default: OFF)");
                controller.ToggleSlot(SpineSlots.Cannon);
            }

            // 6. 测试 Gun (非Spine槽位，使用SpriteRenderer)
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Debug.Log("[Tester] Toggling Gun (Non-Spine Slot, Default: OFF)");
                controller.ToggleSlot(SpineSlots.Gun);
            }
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 250));
            GUILayout.Label("Slot Visibility Tester");
            GUILayout.Label("Press 1: Toggle face1 (Default ON)");
            GUILayout.Label("Press 2: Toggle cake (Default ON)");
            GUILayout.Label("Press 3: Toggle cakeFace (Default OFF)");
            GUILayout.Label("Press 4: Toggle body (Permanent ON)");
            GUILayout.Label("Press 5: Toggle Cannon (Non-Spine, Default OFF)");
            GUILayout.Label("Press 6: Toggle Gun (Non-Spine, Default OFF)");
            GUILayout.EndArea();
        }
    }
}
