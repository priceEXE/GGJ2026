using UnityEngine;

// ==========================================
// 输入层基类
// ==========================================
/// <summary>
/// 输入层：处理、阻挡、透传输入
/// </summary>
public abstract class InputLayer : MonoBehaviour
{
    /// <summary>
    /// 层级优先级 (0-100+，越大越先执行写入)
    /// </summary>
    public abstract int Priority { get; }
    
    public virtual string LayerName => GetType().Name;
    
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// 处理输入帧
    /// </summary>
    public abstract void ProcessInput(ref InputFrame frame);
    
    // ================================================
    // 生命周期管理 (自动注册)
    // ================================================
    
    protected virtual void OnEnable()
    {
        if (InputLayerStack.Instance != null)
        {
            InputLayerStack.Instance.AddLayer(this);
        }
    }
    
    protected virtual void OnDisable()
    {
        if (InputLayerStack.Instance != null)
        {
            InputLayerStack.Instance.RemoveLayer(this);
        }
    }
    
    // ================================================
    // 阻挡工具方法 (适配 ButtonState)
    // ================================================
    
    /// <summary>
    /// 重载1：阻挡模拟量 (Vector2?)
    /// 直接设为 null
    /// </summary>
    protected void Block(ref Vector2? vec)
    {
        vec = null;
    }

    /// <summary>
    /// 重载2：阻挡按键全状态 (默认行为)
    /// 将 Down, Hold, Up 全部设为 null
    /// </summary>
    protected void Block(ref ButtonState btn)
    {
        btn.Down = null;
        btn.Hold = null;
        btn.Up = null;
    }
     
    /// <summary>
    /// 重载3：极致精细控制 - 直接阻挡任意一个 bool? 字段
    /// 用法：Block(ref frame.Confirm.Down);
    /// </summary>
    protected void Block(ref bool? state)
    {
        state = null;
    }

}