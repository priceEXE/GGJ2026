using System.Collections.Generic;
using UnityEngine;

// ==========================================
// 输入层栈管理器
// ==========================================

/// <summary>
/// 管理所有 InputLayer 的优先级栈
/// 每帧创建空帧，并按优先级顺序让各层处理（填充或阻挡）
/// </summary>
[DefaultExecutionOrder(-100)]
public class InputLayerStack : MonoBehaviour
{
    private static InputLayerStack _instance;
    public static InputLayerStack Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[InputLayerStack]");
                _instance = go.AddComponent<InputLayerStack>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        // 确保单例唯一性
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    
    private void OnDestroy()
    {
        // 清理单例引用，避免场景关闭时的警告
        if (_instance == this)
        {
            _instance = null;
        }
    }
    
    /// <summary>
    /// 所有已注册的层
    /// </summary>
    private List<InputLayer> _layers = new List<InputLayer>();
    
    /// <summary>
    /// 当前帧的最终输入数据 (供 Gameplay/UI 读取)
    /// </summary>
    public InputFrame CurrentFrame { get; private set; }
    
    // ================================================
    // 层级管理
    // ================================================
    
    public void AddLayer(InputLayer layer)
    {
        if (!_layers.Contains(layer))
        {
            _layers.Add(layer);
            SortLayers();
            Debug.Log($"[InputLayerStack] 添加层: {layer.LayerName} (Priority: {layer.Priority})");
        }
    }
    
    public void RemoveLayer(InputLayer layer)
    {
        if (_layers.Contains(layer))
        {
            _layers.Remove(layer);
            Debug.Log($"[InputLayerStack] 移除层: {layer.LayerName}");
        }
    }
    
    /// <summary>
    /// 按优先级升序排序 (-1 -> 30)
    /// Source(-1) 先运行填充数据，Gameplay(30) 后运行进行逻辑转换
    /// </summary>
    private void SortLayers()
    {
        _layers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }
    
    // ================================================
    // 每帧处理
    // ================================================
    
    private void Update()
    {
        // 1. 创建全新的空帧 (所有 bool? 默认为 null)
        var frame = InputFrame.CreateEmpty();
        
        // 2. 按优先级遍历处理
        // 逻辑流：Source层(Priority 100) 先填充 -> 拦截层(Priority 80) 修改/阻挡 -> ...
        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers[i];
            if (layer != null && layer.IsActive)
            {
                layer.ProcessInput(ref frame);
            }
        }
        
        // 3. 保存结果供本帧使用
        CurrentFrame = frame;
    }
    
    // ================================================
    // 调试
    // ================================================
    
    public void LogCurrentLayers()
    {
        Debug.Log("[InputLayerStack] 当前活跃层级:");
        foreach (var layer in _layers)
        {
            Debug.Log($"  [{layer.Priority}] {layer.LayerName} (Active: {layer.IsActive})");
        }
    }
}