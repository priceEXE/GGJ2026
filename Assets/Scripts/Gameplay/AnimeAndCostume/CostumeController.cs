using System.Collections.Generic;
using UnityEngine;
using CharacterCosmetics;

namespace Gameplay.AnimeAndCostume
{
    /// <summary>
    /// 装扮类型枚举（示例）
    /// </summary>
    public enum CosmeticType
    {
        ExampleHat,        // 示例：帽子
        ExampleCape        // 示例：披风
    }

    /// <summary>
    /// 骨骼动画序列枚举
    /// </summary>
    public enum SpineAnimationType
    {
        Idle
    }

    /// <summary>
    /// 装扮颜色枚举
    /// </summary>
    public enum CharacterColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    /// <summary>
    /// 装扮控制器接口
    /// </summary>
    public interface ICostumeController
    {
        /// <summary>
        /// 显示装扮
        /// </summary>
        void ShowCosmetic(CosmeticType type);

        /// <summary>
        /// 隐藏装扮
        /// </summary>
        void HideCosmetic(CosmeticType type);

        /// <summary>
        /// 切换装扮可见性
        /// </summary>
        void ToggleCosmetic(CosmeticType type);

        /// <summary>
        /// 设置所有装扮可见性
        /// </summary>
        void SetAllCosmeticVisibility(bool visible);

        /// <summary>
        /// 获取所有装扮的可见性状态
        /// </summary>
        /// <returns>装扮类型-可见性键值对字典</returns>
        Dictionary<CosmeticType, bool> GetAllCosmeticVisibility();

        /// <summary>
        /// 角色当前颜色状态（切换版本）
        /// </summary>
        CharacterColor CurrentColor { get; set; }

        /// <summary>
        /// 播放骨骼动画
        /// </summary>
        void PlayAnimation(SpineAnimationType anim);

        /// <summary>
        /// 闪烁效果（代理Binarizer）
        /// </summary>
        /// <param name="interval">每次闪烁状态的持续时间（秒）</param>
        /// <param name="count">闪烁次数</param>
        void Flash(float interval = 0.1f, int count = 3);
    }

    /// <summary>
    /// 装扮控制器实现
    /// </summary>
    public class CostumeController : MonoBehaviour, ICostumeController
    {
        [Header("Components")]
        [SerializeField] private Binarizer binarizer;

        // 内部状态
        private Dictionary<CosmeticType, bool> _cosmeticVisibility = new Dictionary<CosmeticType, bool>();
        private CharacterColor _currentColor = CharacterColor.Red;

        private void Awake()
        {
            // 初始化所有装扮类型为不可见
            foreach (CosmeticType type in System.Enum.GetValues(typeof(CosmeticType)))
            {
                _cosmeticVisibility[type] = false;
            }
        }

        public CharacterColor CurrentColor
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                Debug.Log($"[CostumeController] CurrentColor Set: {value}");
                // TODO: 这里应实现实际的小人切换逻辑（如切换 Spine Skin 或 材质球）
            }
        }

        public void ShowCosmetic(CosmeticType type)
        {
            _cosmeticVisibility[type] = true;
            Debug.Log($"[CostumeController] ShowCosmetic: {type}");
            // TODO: 实现实际的装扮显示逻辑
        }

        public void HideCosmetic(CosmeticType type)
        {
            _cosmeticVisibility[type] = false;
            Debug.Log($"[CostumeController] HideCosmetic: {type}");
            // TODO: 实现实际的装扮隐藏逻辑
        }

        public void ToggleCosmetic(CosmeticType type)
        {
            if (_cosmeticVisibility.ContainsKey(type))
            {
                _cosmeticVisibility[type] = !_cosmeticVisibility[type];
                Debug.Log($"[CostumeController] ToggleCosmetic: {type} = {_cosmeticVisibility[type]}");
                // TODO: 实现实际的装扮切换逻辑
            }
        }

        public void SetAllCosmeticVisibility(bool visible)
        {
            List<CosmeticType> keys = new List<CosmeticType>(_cosmeticVisibility.Keys);
            foreach (var type in keys)
            {
                _cosmeticVisibility[type] = visible;
            }
            Debug.Log($"[CostumeController] SetAllCosmeticVisibility: {visible}");
            // TODO: 实现实际的批量显示/隐藏逻辑
        }

        public Dictionary<CosmeticType, bool> GetAllCosmeticVisibility()
        {
            // 返回字典的副本，避免外部修改
            return new Dictionary<CosmeticType, bool>(_cosmeticVisibility);
        }

        public void PlayAnimation(SpineAnimationType anim)
        {
            Debug.Log($"[CostumeController] PlayAnimation: {anim}");
            // TODO: 实现Spine动画播放逻辑
        }

        public void Flash(float interval = 0.1f, int count = 3)
        {
            if (binarizer == null)
            {
                Debug.LogWarning("[CostumeController] Binarizer component not found!");
                return;
            }

            // 代理调用Binarizer.Flash
            binarizer.Flash(interval, count);
            Debug.Log($"[CostumeController] Flash: interval={interval}, count={count}");
        }

        #region Context Menu Test Methods
        [ContextMenu("Test: Set Color Red")]
        private void TestSetColorRed()
        {
            CurrentColor = CharacterColor.Red;
        }

        [ContextMenu("Test: Set Color Blue")]
        private void TestSetColorBlue()
        {
            CurrentColor = CharacterColor.Blue;
        }

        [ContextMenu("Test: Print Visibility")]
        private void TestPrintVisibility()
        {
            Debug.Log($"Current Character Color: {CurrentColor}");
            var visibility = GetAllCosmeticVisibility();
            foreach (var kvp in visibility)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value}");
            }
        }
        #endregion
    }
}
