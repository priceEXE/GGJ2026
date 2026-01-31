using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterCosmetics
{
    /// <summary>
    /// PSD读取器 - 直接读取PSD文件，自动提取图层作为子SpriteRenderer
    /// 支持通过图层名称控制可见性，并设置基准排序层级
    /// </summary>
    public class PSDReader : MonoBehaviour
    {
        [Header("PSD文件配置")]
        [Tooltip("拖入PSD/PSB文件（TextureType必须设置为Sprite）")]
        public Object psdFile;

        [Header("排序层级设置")]
        [Tooltip("基准排序层级，最底层图层使用此值，往上递增")]
        public int baseLayerOrder = 0;

        [Header("运行时状态")]
        [Tooltip("是否在Start时自动初始化图层")]
        public bool initializeOnStart = true;

        // 图层名称到SpriteRenderer的映射
        private Dictionary<string, SpriteRenderer> layerRenderers = new Dictionary<string, SpriteRenderer>();

        // 缓存的图层数据
        private Sprite[] cachedLayers;

        void Start()
        {
            if (initializeOnStart)
            {
                InitializeLayers();
            }
        }

        /// <summary>
        /// 初始化所有图层，创建子对象和SpriteRenderer
        /// </summary>
        public void InitializeLayers()
        {
            // 清理现有子对象
            ClearChildren();

            if (psdFile == null)
            {
                Debug.LogWarning($"[PSDReader] {gameObject.name}: 没有设置PSD文件！");
                return;
            }

            // 1. 尝试寻找 PSD 内部生成的 Prefab (这包含空间坐标信息)
            GameObject prefab = ExtractPrefabFromPSD(psdFile);
            
            if (prefab != null)
            {
                // 如果找到了预制体，直接实例化它，保留空间结构
                SetupFromPrefab(prefab);
            }
            else
            {
                // 2. 如果没找到（比如导入设置不是 Rig/Prefab），回退到手动平等创建（坐标会丢失）
                Debug.LogWarning($"[PSDReader] {gameObject.name}: 未在PSD中找到预制体结构，将回退到手动提取Sprite。建议在导入设置中开启 'Character Rig'。");
                cachedLayers = ExtractSpritesFromPSD(psdFile);
                if (cachedLayers != null && cachedLayers.Length > 0)
                {
                    CreateLayerRenderers(cachedLayers);
                }
            }

            Debug.Log($"[PSDReader] {gameObject.name}: 成功初始化 {layerRenderers.Count} 个图层");
        }

        private GameObject ExtractPrefabFromPSD(Object psd)
        {
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(psd);
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var asset in allAssets)
            {
                if (asset is GameObject go) return go;
            }
#endif
            return null;
        }

        private void SetupFromPrefab(GameObject prefab)
        {
            GameObject inst = Instantiate(prefab, transform);
            inst.name = "PSD_Structure";
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
            inst.transform.localScale = Vector3.one;

            // 查找所有子渲染器
            SpriteRenderer[] renderers = inst.GetComponentsInChildren<SpriteRenderer>(true);
            
            // 按名称或层级深度排序，以便分配 sortingOrder
            List<SpriteRenderer> sortedRenderers = new List<SpriteRenderer>(renderers);
            // 简单处理：按层级顺序自下而上分配
            
            for (int i = 0; i < sortedRenderers.Count; i++)
            {
                var renderer = sortedRenderers[i];
                // 默认使用反转逻辑：让层级列表顶部的对象具有更高的排序值（显示在更前面）
                renderer.sortingOrder = baseLayerOrder + (sortedRenderers.Count - 1 - i);
                layerRenderers[renderer.gameObject.name] = renderer;
            }
        }

        /// <summary>
        /// 从PSD文件中提取所有Sprite图层
        /// </summary>
        private Sprite[] ExtractSpritesFromPSD(Object psd)
        {
#if UNITY_EDITOR
            // 编辑器模式：使用AssetDatabase读取子资源
            string assetPath = AssetDatabase.GetAssetPath(psd);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError($"[PSDReader] 无法获取PSD文件路径");
                return null;
            }

            // 加载所有子资源（Sprite）
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            List<Sprite> sprites = new List<Sprite>();

            foreach (Object asset in allAssets)
            {
                if (asset is Sprite sprite)
                {
                    sprites.Add(sprite);
                }
            }

            // 按名称排序并反转，确保逻辑一致
            sprites.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            sprites.Reverse();

            return sprites.ToArray();
#else
            // 运行时模式：从预先缓存的数据加载
            Debug.LogWarning($"[PSDReader] 运行时无法动态读取PSD文件，请在编辑器中先初始化或使用Build时序列化的数据");
            return cachedLayers;
#endif
        }

        /// <summary>
        /// 为Sprite数组创建子对象和SpriteRenderer
        /// </summary>
        private void CreateLayerRenderers(Sprite[] sprites)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i];
                if (sprite == null) continue;

                // 创建子对象
                GameObject layerObj = new GameObject(sprite.name);
                layerObj.transform.SetParent(transform, false);
                layerObj.transform.localPosition = Vector3.zero;
                layerObj.transform.localRotation = Quaternion.identity;
                layerObj.transform.localScale = Vector3.one;

                // 添加SpriteRenderer
                SpriteRenderer renderer = layerObj.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = baseLayerOrder + i;

                // 缓存到字典
                layerRenderers[sprite.name] = renderer;
            }
        }

        /// <summary>
        /// 清理所有子对象
        /// </summary>
        private void ClearChildren()
        {
            // 在编辑器中使用DestroyImmediate，运行时使用Destroy
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                while (transform.childCount > 0)
                {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
            else
            #endif
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }

            layerRenderers.Clear();
        }

        /// <summary>
        /// 设置指定图层的可见性
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible">是否可见</param>
        public void SetLayerVisible(string layerName, bool visible)
        {
            if (layerRenderers.TryGetValue(layerName, out SpriteRenderer renderer))
            {
                renderer.enabled = visible;
            }
            else
            {
                Debug.LogWarning($"[PSDReader] {gameObject.name}: 找不到图层 '{layerName}'. 可用图层: {string.Join(", ", layerRenderers.Keys)}");
            }
        }

        /// <summary>
        /// 显示指定图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ShowLayer(string layerName)
        {
            SetLayerVisible(layerName, true);
        }

        /// <summary>
        /// 隐藏指定图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void HideLayer(string layerName)
        {
            SetLayerVisible(layerName, false);
        }

        /// <summary>
        /// 切换指定图层的可见性
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ToggleLayer(string layerName)
        {
            if (layerRenderers.TryGetValue(layerName, out SpriteRenderer renderer))
            {
                renderer.enabled = !renderer.enabled;
            }
            else
            {
                Debug.LogWarning($"[PSDReader] {gameObject.name}: 找不到图层 '{layerName}'. 可用图层: {string.Join(", ", layerRenderers.Keys)}");
            }
        }

        /// <summary>
        /// 获取所有图层名称
        /// </summary>
        /// <returns>图层名称列表</returns>
        public List<string> GetLayerNames()
        {
            return new List<string>(layerRenderers.Keys);
        }

        /// <summary>
        /// 重新初始化所有图层（当在Inspector中修改psdFile或baseLayerOrder后调用）
        /// </summary>
        [ContextMenu("重新初始化图层")]
        public void ReinitializeLayers()
        {
            InitializeLayers();
        }

#if UNITY_EDITOR
        // 在Inspector中修改值时自动重新初始化
        private void OnValidate()
        {
            if (psdFile != null && (Application.isPlaying || transform.childCount > 0))
            {
                // 延迟调用避免编辑器错误
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        InitializeLayers();
                    }
                };
            }
        }
#endif
    }
}

