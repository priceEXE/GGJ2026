using UnityEngine;

namespace Practice
{
    /// <summary>
    /// 确保挂载此脚本的GameObject及其所有子对象在场景切换时不会被销毁
    /// </summary>
    public class PersistentPrefab : MonoBehaviour
    {
        private void Awake()
        {
            // 确保此GameObject及其所有子对象在场景加载时保持存在
            DontDestroyOnLoad(gameObject);
        }
    }
}
