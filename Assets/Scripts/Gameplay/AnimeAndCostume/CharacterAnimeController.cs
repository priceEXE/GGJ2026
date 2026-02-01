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
        void ShowSlot(SpineSlots slot);
        void HideSlot(SpineSlots slot);
        void ToggleSlot(SpineSlots slot);
        Dictionary<SpineSlots, bool> GetAllSlotVisibility();
        void SetAllSlotVisibility(Dictionary<SpineSlots, bool> visibility);
        void PlayAnimation(SpineAnimations anim, bool loop = true);
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

        [Header("非Spine槽位 (使用 SpriteRenderer)")]
        [Tooltip("Cannon 槽位的 SpriteRenderer")]
        [SerializeField] private SpriteRenderer cannonRenderer;
        
        [Tooltip("Gun 槽位的 SpriteRenderer")]
        [SerializeField] private SpriteRenderer gunRenderer;

        // 目标可见性状态 (SpineSlots -> ShouldBeVisible)
        private Dictionary<SpineSlots, bool> _targetVisibility = new Dictionary<SpineSlots, bool>();
        
        // 延迟缓存的附件名称（仅在首次隐藏时记录）
        private Dictionary<SpineSlots, string> _cachedAttachments = new Dictionary<SpineSlots, string>();
        
        // 非Spine槽位列表（使用SpriteRenderer）
        private static readonly HashSet<SpineSlots> NonSpineSlots = new HashSet<SpineSlots>
        {
            SpineSlots.Cannon,
            SpineSlots.Gun
        };

        // 永远打开的插槽（忽略 Hide/Toggle 指令）
        private static readonly HashSet<SpineSlots> PermanentSlots = new HashSet<SpineSlots>
        {
            SpineSlots.body,
            SpineSlots.lefthand,
            SpineSlots.righthand,
            SpineSlots.leftleg,
            SpineSlots.rightleg,
            SpineSlots.sound
        };

        // 默认打开但可切换的插槽
        private static readonly HashSet<SpineSlots> DefaultOpenSwitchableSlots = new HashSet<SpineSlots>
        {
            SpineSlots.face1,
            SpineSlots.cake
        };

        private void Awake()
        {
            if (skeletonAnimation == null)
            {
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            }
        }

        private void Start()
        {
            InitializeSlotVisibility();
        }

        private void LateUpdate()
        {
            // 每帧强制应用可见性状态，防止动画覆盖
            ApplyVisibilityState();
        }

        /// <summary>
        /// 初始化插槽可见性
        /// </summary>
        private void InitializeSlotVisibility()
        {
            _targetVisibility.Clear();

            foreach (SpineSlots slotEnum in System.Enum.GetValues(typeof(SpineSlots)))
            {
                if (slotEnum == SpineSlots.None) continue;

                // 永久槽位和默认打开槽位设为可见，其他设为不可见
                _targetVisibility[slotEnum] = 
                    PermanentSlots.Contains(slotEnum) || 
                    DefaultOpenSwitchableSlots.Contains(slotEnum);
            }
            
            // 立即应用一次
            ApplyVisibilityState();
        }

        /// <summary>
        /// 应用可见性状态到所有槽位
        /// </summary>
        private void ApplyVisibilityState()
        {
            foreach (var kvp in _targetVisibility)
            {
                var slotEnum = kvp.Key;
                bool shouldBeVisible = kvp.Value;
                
                // 特殊处理：非Spine槽位（使用SpriteRenderer）
                if (NonSpineSlots.Contains(slotEnum))
                {
                    ApplyNonSpineSlotVisibility(slotEnum, shouldBeVisible);
                    continue;
                }
                
                // 常规处理：Spine槽位
                if (skeletonAnimation == null || skeletonAnimation.Skeleton == null) continue;
                
                string slotName = SpineNames.GetSlotName(slotEnum);
                var slot = skeletonAnimation.Skeleton.FindSlot(slotName);

                if (slot == null) continue;

                if (shouldBeVisible)
                {
                    // 显示：设置透明度为1，如果需要则恢复附件
                    slot.A = 1f;
                    
                    if (slot.Attachment == null && _cachedAttachments.TryGetValue(slotEnum, out string attachmentName))
                    {
                        var attachment = skeletonAnimation.Skeleton.GetAttachment(slotName, attachmentName);
                        if (attachment != null)
                        {
                            slot.Attachment = attachment;
                        }
                    }
                }
                else
                {
                    // 隐藏：首次隐藏时缓存附件，然后设置透明度为0
                    if (!_cachedAttachments.ContainsKey(slotEnum) && slot.Attachment != null)
                    {
                        _cachedAttachments[slotEnum] = slot.Attachment.Name;
                        #if UNITY_EDITOR
                        Debug.Log($"[CharacterAnimeController] 缓存附件: {slotName} -> {slot.Attachment.Name}");
                        #endif
                    }
                    
                    slot.A = 0f;
                }
            }
        }

        /// <summary>
        /// 应用非Spine槽位的可见性（使用SpriteRenderer）
        /// </summary>
        private void ApplyNonSpineSlotVisibility(SpineSlots slot, bool visible)
        {
            SpriteRenderer renderer = slot switch
            {
                SpineSlots.Cannon => cannonRenderer,
                SpineSlots.Gun => gunRenderer,
                _ => null
            };

            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }

        /// <summary>
        /// 显示指定插槽
        /// </summary>
        public void ShowSlot(SpineSlots slot)
        {
            if (slot == SpineSlots.None || skeletonAnimation == null) return;

            _targetVisibility[slot] = true;
            ApplyVisibilityState();
        }

        /// <summary>
        /// 隐藏指定插槽
        /// </summary>
        public void HideSlot(SpineSlots slot)
        {
            if (slot == SpineSlots.None) return;
            
            // 永久槽位不允许隐藏
            if (PermanentSlots.Contains(slot)) return;

            _targetVisibility[slot] = false;
            ApplyVisibilityState();
        }

        /// <summary>
        /// 切换插槽可见性
        /// </summary>
        public void ToggleSlot(SpineSlots slot)
        {
            if (slot == SpineSlots.None) return;

            // 永久槽位不允许切换
            if (PermanentSlots.Contains(slot)) return;

            if (_targetVisibility.ContainsKey(slot))
            {
                _targetVisibility[slot] = !_targetVisibility[slot];
                ApplyVisibilityState();
            }
        }

        /// <summary>
        /// 获取所有插槽的可见性状态
        /// </summary>
        public Dictionary<SpineSlots, bool> GetAllSlotVisibility()
        {
            // 直接返回目标可见性状态的副本
            return new Dictionary<SpineSlots, bool>(_targetVisibility);
        }

        /// <summary>
        /// 批量设置插槽可见性
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
            if (skeletonAnimation == null) return;

            if (anim == SpineAnimations.None)
            {
                skeletonAnimation.AnimationState.ClearTracks();
                return;
            }

            string animName = SpineNames.GetAnimationName(anim);
            skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
            
            #if UNITY_EDITOR
            Debug.Log($"[CharacterAnimeController] Playing: {animName} (loop={loop})");
            #endif
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
