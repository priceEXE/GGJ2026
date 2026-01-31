using UnityEngine;
using Spine;
using Spine.Unity;
using System.Collections.Generic;

namespace Gameplay.AnimeAndCostume
{
    /// <summary>
    /// Spine 插槽可见性调试器
    /// 用于测试和调试各个插槽的显示和隐藏
    /// </summary>
    public class SpineSlotDebugger : MonoBehaviour
    {
        [Header("Spine 引用")]
        [Tooltip("要调试的 Spine 骨骼对象")]
        public SkeletonAnimation skeletonAnimation;

        [Header("可见性控制方式")]
        [Tooltip("使用透明度控制（推荐）还是移除附件")]
        public bool useAlphaControl = true;

        [System.Serializable]
        public class SlotVisibility
        {
            public string slotName;
            public bool isVisible = true;
            [HideInInspector] public string originalAttachmentName;
        }

        public List<SlotVisibility> slots = new List<SlotVisibility>();

        private void Start()
        {
            RefreshSlotList();
        }

        /// <summary>
        /// 刷新插槽列表（从 Skeleton 重新读取）
        /// </summary>
        [ContextMenu("刷新插槽列表")]
        public void RefreshSlotList()
        {
            if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
            {
                Debug.LogWarning($"[SpineSlotDebugger] {gameObject.name}: 缺失 SkeletonAnimation 引用!");
                return;
            }

            slots.Clear();
            var skeleton = skeletonAnimation.Skeleton;

            foreach (var slot in skeleton.Slots)
            {
                var slotVis = new SlotVisibility
                {
                    slotName = slot.Data.Name,
                    isVisible = slot.Attachment != null,
                    originalAttachmentName = slot.Attachment?.Name ?? ""
                };
                slots.Add(slotVis);
            }

            Debug.Log($"[SpineSlotDebugger] {gameObject.name}: 已刷新 {slots.Count} 个插槽");
        }

        /// <summary>
        /// 应用所有插槽的可见性设置
        /// </summary>
        [ContextMenu("应用可见性")]
        public void ApplyVisibility()
        {
            if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
            {
                Debug.LogWarning($"[SpineSlotDebugger] {gameObject.name}: 缺失 SkeletonAnimation 引用!");
                return;
            }

            var skeleton = skeletonAnimation.Skeleton;

            foreach (var slotVis in slots)
            {
                var slot = skeleton.FindSlot(slotVis.slotName);
                if (slot == null) continue;

                if (useAlphaControl)
                {
                    // 使用透明度控制
                    slot.A = slotVis.isVisible ? 1f : 0f;
                }
                else
                {
                    // 使用附件移除/恢复
                    if (slotVis.isVisible)
                    {
                        // 恢复原始附件
                        if (!string.IsNullOrEmpty(slotVis.originalAttachmentName))
                        {
                            skeleton.SetAttachment(slotVis.slotName, slotVis.originalAttachmentName);
                        }
                    }
                    else
                    {
                        // 移除附件
                        skeleton.SetAttachment(slotVis.slotName, null);
                    }
                }
            }

            // 强制刷新显示
            skeletonAnimation.LateUpdate();
        }

        /// <summary>
        /// 显示所有插槽
        /// </summary>
        [ContextMenu("显示全部")]
        public void ShowAll()
        {
            foreach (var slotVis in slots)
            {
                slotVis.isVisible = true;
            }
            ApplyVisibility();
        }

        /// <summary>
        /// 隐藏所有插槽
        /// </summary>
        [ContextMenu("隐藏全部")]
        public void HideAll()
        {
            foreach (var slotVis in slots)
            {
                slotVis.isVisible = false;
            }
            ApplyVisibility();
        }

        /// <summary>
        /// 切换指定插槽的可见性
        /// </summary>
        public void ToggleSlot(string slotName)
        {
            var slotVis = slots.Find(s => s.slotName == slotName);
            if (slotVis != null)
            {
                slotVis.isVisible = !slotVis.isVisible;
                ApplyVisibility();
            }
        }

        /// <summary>
        /// 设置指定插槽的可见性
        /// </summary>
        public void SetSlotVisibility(string slotName, bool visible)
        {
            var slotVis = slots.Find(s => s.slotName == slotName);
            if (slotVis != null)
            {
                slotVis.isVisible = visible;
                ApplyVisibility();
            }
        }
    }
}
