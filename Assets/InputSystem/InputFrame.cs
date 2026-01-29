using UnityEngine;

// ==========================================
// 1. 核心数据结构：按键状态 (ButtonState)
// ==========================================
/// <summary>
/// 定义一个按键的完整生命周期
/// null = 被阻挡 (Blocked)
/// true/false = 有效信号
/// </summary>
public struct ButtonState
{
    public bool? Down; // 按下瞬间
    public bool? Hold; // 持续按住
    public bool? Up;   // 松开瞬间
}

// ==========================================
// 2. 最终输入帧定义 (InputFrame)
// ==========================================
public struct InputFrame
{
    // ------------------------------------------------
    // A. 主轨道 (包含系统、键鼠、以及通用的主手柄绑定)
    // ------------------------------------------------
    
    // 坐标与方向
    public Vector2? CursorPosition; // 光标位置 (屏幕坐标)
    public Vector2? Move;           // 移动方向 (WASD / 方向键 / 手柄左摇杆)
    
    // 基础动作
    public ButtonState Space;       // 空格 (键盘空格 / 手柄A)
    public ButtonState Confirm;     // 确认 (键盘Enter/空格/鼠标左键 / 手柄A)
    public ButtonState Cancel;      // 取消 (键盘X/鼠标右键 / 手柄B)
    public ButtonState Attack;      // 攻击 (键盘J/鼠标左键 / 手柄X)
    public ButtonState Defense;     // 防御 (键盘K/鼠标右键 / 手柄Y)
    
    // 系统
    public ButtonState Pause;       // 暂停 (键盘Esc / 手柄Start)
    
    // 快捷数字键 (1-9)
    public ButtonState First;
    public ButtonState Second;
    public ButtonState Third;
    public ButtonState Fourth;
    public ButtonState Fifth;
    public ButtonState Sixth;
    public ButtonState Seventh;
    public ButtonState Eighth;
    public ButtonState Ninth;

    // ------------------------------------------------
    // B. 多人模块 (手柄 P1 - P4)
    // 纯粹的手柄采集数据
    // ------------------------------------------------
    
    // Player 1
    public Vector2? P1_Move;
    public ButtonState P1_PadX; public ButtonState P1_PadY;
    public ButtonState P1_PadA; public ButtonState P1_PadB;

    // Player 2
    public Vector2? P2_Move;
    public ButtonState P2_PadX; public ButtonState P2_PadY;
    public ButtonState P2_PadA; public ButtonState P2_PadB;

    // Player 3
    public Vector2? P3_Move;
    public ButtonState P3_PadX; public ButtonState P3_PadY;
    public ButtonState P3_PadA; public ButtonState P3_PadB;

    // Player 4
    public Vector2? P4_Move;
    public ButtonState P4_PadX; public ButtonState P4_PadY;
    public ButtonState P4_PadA; public ButtonState P4_PadB;


    // 工厂方法：创建一个初始的空帧 (所有字段默认为 null)
    public static InputFrame CreateEmpty()
    {
        return new InputFrame();
    }
}