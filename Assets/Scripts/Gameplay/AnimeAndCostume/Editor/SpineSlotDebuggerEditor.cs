using UnityEngine;
using UnityEditor;
using Gameplay.AnimeAndCostume;

namespace Gameplay.AnimeAndCostume.Editor
{
    [CustomEditor(typeof(SpineSlotDebugger))]
    public class SpineSlotDebuggerEditor : UnityEditor.Editor
    {
        private SpineSlotDebugger debugger;

        private void OnEnable()
        {
            debugger = (SpineSlotDebugger)target;
        }

        public override void OnInspectorGUI()
        {
            // 绘制默认属性（引用、控制方式等）
            DrawDefaultInspector();

            GUILayout.Space(10);

            // 刷新插槽列表按钮
            if (GUILayout.Button("刷新插槽列表", GUILayout.Height(30)))
            {
                debugger.RefreshSlotList();
            }

            // 如果没有插槽数据，提示用户先刷新
            if (debugger.slots == null || debugger.slots.Count == 0)
            {
                EditorGUILayout.HelpBox("请先点击「刷新插槽列表」按钮来加载 Spine 插槽信息", MessageType.Info);
                return;
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("插槽可见性控制", EditorStyles.boldLabel);

            // 全部显示/隐藏按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("显示全部"))
            {
                debugger.ShowAll();
            }
            if (GUILayout.Button("隐藏全部"))
            {
                debugger.HideAll();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 绘制所有插槽的复选框
            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < debugger.slots.Count; i++)
            {
                var slot = debugger.slots[i];
                
                EditorGUILayout.BeginHorizontal();
                
                // 复选框
                bool newVisibility = EditorGUILayout.Toggle(slot.isVisible, GUILayout.Width(20));
                
                // 插槽名称
                EditorGUILayout.LabelField($"[{i}] {slot.slotName}", GUILayout.ExpandWidth(true));
                
                // 如果有原始附件，显示附件名称
                if (!string.IsNullOrEmpty(slot.originalAttachmentName))
                {
                    EditorGUILayout.LabelField($"({slot.originalAttachmentName})", EditorStyles.miniLabel, GUILayout.Width(150));
                }

                EditorGUILayout.EndHorizontal();

                // 如果可见性发生变化
                if (newVisibility != slot.isVisible)
                {
                    slot.isVisible = newVisibility;
                    debugger.ApplyVisibility();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            GUILayout.Space(10);

            // 底部信息
            EditorGUILayout.HelpBox(
                $"共 {debugger.slots.Count} 个插槽\n" +
                $"可见性控制方式: {(debugger.useAlphaControl ? "透明度 (推荐)" : "移除附件")}\n\n" +
                "勾选 = 显示，取消勾选 = 隐藏",
                MessageType.Info
            );
        }
    }
}
