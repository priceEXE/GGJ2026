using UnityEngine;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using CharacterCosmetics;

namespace Gameplay.AnimeAndCostume
{
    /// <summary>
    /// 角色动画与插槽控制器接口
    /// </summary>
    public interface ICharacterAnimeController
    {
        /// <summary>
        /// 显示指定插槽
        /// </summary>
        void ShowSlot(SpineSlots slot);

        /// <summary>
        /// 隐藏指定插槽
        /// </summary>
        void HideSlot(SpineSlots slot);

        /// <summary>
        /// 切换插槽可见性
        /// </summary>
        void ToggleSlot(SpineSlots slot);

        /// <summary>
        /// 获取所有插槽的可见性状态（用于缓存）
        /// </summary>
        Dictionary<SpineSlots, bool> GetAllSlotVisibility();

        /// <summary>
        /// 批量设置插槽可见性（用于还原状态）
        /// </summary>
        void SetAllSlotVisibility(Dictionary<SpineSlots, bool> visibility);

        /// <summary>
        /// 播放骨骼动画
        /// </summary>
        void PlayAnimation(SpineAnimations anim, bool loop = true);

        /// <summary>
        /// 闪烁效果（代理Binarizer）
        /// </summary>
        void Flash(float interval = 0.1f, int count = 3);
    }

    /// <summary>
    /// 角色动画与插槽控制器
    /// 集成 Spine 骨骼动画控制和 Binarizer 闪烁效果
    /// </summary>
    public class CharacterAnimeController : MonoBehaviour, ICharacterAnimeController
    {
        [Header("Spine 引用")]
        [Tooltip("Spine 骨骼动画组件")]
        public SkeletonAnimation skeletonAnimation;

        [Header("组件")]
        [SerializeField] private Binarizer binarizer;

        // 缓存原始附件名称，用于恢复
        private Dictionary<SpineSlots, string> originalAttachments = new Dictionary<SpineSlots, string>();

        private void Awake()
        {
            if (skeletonAnimation == null)
            {
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            }

            CacheOriginalAttachments();
        }

        /// <summary>
        /// 缓存所有插槽的原始附件名称
        /// </summary>
        private void CacheOriginalAttachments()
        {
            if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
                return;

            var skeleton = skeletonAnimation.Skeleton;
            foreach (SpineSlots slotEnum in System.Enum.GetValues(typeof(SpineSlots)))
            {
                if (slotEnum == SpineSlots.None) continue;

                string slotName = slotEnum.ToString();
                var slot = skeleton.FindSlot(slotName);
                if (slot != null && slot.Attachment != null)
                {
                    originalAttachments[slotEnum] = slot.Attachment.Name;
                }
            }
        }

        /// <summary>
        /// 显示指定插槽（恢复其附件）
        /// </summary>
        public void ShowSlot(SpineSlots slot)
        {
            if (slot == SpineSlots.None || skeletonAnimation == null) return;

            string slotName = slot.ToString();
            var skeleton = skeletonAnimation.Skeleton;
            var slotObj = skeleton.FindSlot(slotName);

            if (slotObj != null)
            {
                slotObj.A = 1f;

                // 尝试恢复原始附件
                if (originalAttachments.ContainsKey(slot) && slotObj.Attachment == null)
                {
                    skeleton.SetAttachment(slotName, originalAttachments[slot]);
                }
            }
        }

        /// <summary>
        /// 隐藏指定插槽（使用透明度）
        /// </summary>
        public void HideSlot(SpineSlots slot)
        {
            if (slot == SpineSlots.None || skeletonAnimation == null) return;

            string slotName = slot.ToString();
            var skeleton = skeletonAnimation.Skeleton;
            var slotObj = skeleton.FindSlot(slotName);

            if (slotObj != null)
            {
                slotObj.A = 0f;
            }
        }

        /// <summary>
        /// 切换插槽可见性
        /// </summary>
        public void ToggleSlot(SpineSlots slot)
        {
            if (slot == SpineSlots.None || skeletonAnimation == null) return;

            string slotName = slot.ToString();
            var slotObj = skeletonAnimation.Skeleton.FindSlot(slotName);

            if (slotObj != null)
            {
                slotObj.A = slotObj.A > 0.5f ? 0f : 1f;
            }
        }

        /// <summary>
        /// 获取所有插槽的可见性状态（用于缓存）
        /// </summary>
        public Dictionary<SpineSlots, bool> GetAllSlotVisibility()
        {
            var result = new Dictionary<SpineSlots, bool>();

            if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
                return result;

            var skeleton = skeletonAnimation.Skeleton;

            foreach (SpineSlots slotEnum in System.Enum.GetValues(typeof(SpineSlots)))
            {
                if (slotEnum == SpineSlots.None) continue;

                string slotName = slotEnum.ToString();
                var slot = skeleton.FindSlot(slotName);

                if (slot != null)
                {
                    // 通过透明度判断是否可见
                    result[slotEnum] = slot.A > 0.5f;
                }
            }

            return result;
        }

        /// <summary>
        /// 批量设置插槽可见性（用于还原缓存的状态）
        /// </summary>
        public void SetAllSlotVisibility(Dictionary<SpineSlots, bool> visibility)
        {
            if (visibility == null || skeletonAnimation == null) return;

            foreach (var kvp in visibility)
            {
                if (kvp.Value)
                {
                    ShowSlot(kvp.Key);
                }
                else
                {
                    HideSlot(kvp.Key);
                }
            }
        }

        /// <summary>
        /// 播放指定动画
        /// </summary>
        public void PlayAnimation(SpineAnimations anim, bool loop = true)
        {
            if (anim == SpineAnimations.None || skeletonAnimation == null) return;

            string animName = anim.ToString();
            skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
            Debug.Log($"[CharacterAnimeController] Playing: {animName} (loop={loop})");
        }

        /// <summary>
        /// 闪烁效果（代理 Binarizer）
        /// </summary>
        public void Flash(float interval = 0.1f, int count = 3)
        {
            if (binarizer == null)
            {
                Debug.LogWarning("[CharacterAnimeController] Binarizer component not found!");
                return;
            }

            binarizer.Flash(interval, count);
        }

        #region Context Menu 测试方法
        [ContextMenu("测试: 显示全部插槽")]
        private void TestShowAllSlots()
        {
            foreach (SpineSlots slot in System.Enum.GetValues(typeof(SpineSlots)))
            {
                if (slot != SpineSlots.None)
                {
                    ShowSlot(slot);
                }
            }
        }

        [ContextMenu("测试: 隐藏全部插槽")]
        private void TestHideAllSlots()
        {
            foreach (SpineSlots slot in System.Enum.GetValues(typeof(SpineSlots)))
            {
                if (slot != SpineSlots.None)
                {
                    HideSlot(slot);
                }
            }
        }

        [ContextMenu("测试: 缓存并还原插槽状态")]
        private void TestCacheAndRestore()
        {
            // 获取当前状态
            var cachedState = GetAllSlotVisibility();
            Debug.Log($"已缓存 {cachedState.Count} 个插槽状态");

            // 隐藏全部
            TestHideAllSlots();

            // 延迟 2 秒后还原
            Invoke(nameof(RestoreCachedState), 2f);
        }

        private Dictionary<SpineSlots, bool> _cachedState;
        private void RestoreCachedState()
        {
            if (_cachedState != null)
            {
                SetAllSlotVisibility(_cachedState);
                Debug.Log("已还原插槽状态");
            }
        }

        [ContextMenu("测试: 播放第一个动画")]
        private void TestPlayFirstAnimation()
        {
            var animations = (SpineAnimations[])System.Enum.GetValues(typeof(SpineAnimations));
            if (animations.Length > 1) // 跳过 None
            {
                PlayAnimation(animations[1]);
            }
        }
        #endregion
    }
}
