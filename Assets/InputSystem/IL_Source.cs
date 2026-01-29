using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 硬件输入源：将 键盘、鼠标、以及最多4个手柄 的原始输入映射到 InputFrame 中。
/// 包含动态绑定功能，可在运行时分配设备到 P1-P4 槽位。
/// </summary>
public class IL_Source : InputLayer
{
    public override int Priority => 100;
    public override string LayerName => "Hardware Source";

    [Header("调试模式")]
    public bool enableDebugBinding = false; // 是否启用绑定调试功能
    
    [Header("UI 颜色设置")]
    public Color unboundColor = Color.gray;
    public Color boundColor = Color.green;

    // 自动创建的UI引用（无需手动设置）
    private GameObject bindingPanel;
    private Image[] playerSlots = new Image[4];
    
    // 绑定状态 (0=未绑定, 1=键鼠, 2-5=手柄1-4)
    private int[] bindings = new int[4] { 0, 0, 0, 0 };
    private bool isBindingMode = false;

    // 用于虚拟按键检测的状态记忆
    private bool _lastConfirm, _lastCancel, _lastAttack, _lastDefense;
    private bool _lastSpace, _lastPause;

    void Start()
    {
        // 如果启用了调试绑定，自动创建UI
        if (enableDebugBinding)
        {
            CreateBindingUI();
        }
        
        // 从 PlayerPrefs 加载绑定
        LoadBindings();
        UpdateBindingUI();
    }

    void CreateBindingUI()
    {
        Debug.Log("[绑定UI] 开始创建...");
        
        // 1. 创建 Canvas
        GameObject canvasObj = new GameObject("BindingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // 确保在最上层
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("[绑定UI] Canvas 创建完成");

        // 2. 创建 Panel
        bindingPanel = new GameObject("BindingPanel");
        bindingPanel.transform.SetParent(canvasObj.transform, false);
        
        Image panelBg = bindingPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f); // 更明显的背景

        RectTransform panelRect = bindingPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(500, 150);
        panelRect.anchoredPosition = Vector2.zero;
        
        Debug.Log("[绑定UI] Panel 创建完成");

        // 3. 创建 4 个方块
        for (int i = 0; i < 4; i++)
        {
            GameObject slotObj = new GameObject($"Slot{i + 1}");
            slotObj.transform.SetParent(bindingPanel.transform, false);
            
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = unboundColor;
            
            RectTransform slotRect = slotObj.GetComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(100, 100);
            
            // 横向排列
            float xPos = -200 + i * 120;
            slotRect.anchoredPosition = new Vector2(xPos, 0);
            
            playerSlots[i] = slotImage;
            
            Debug.Log($"[绑定UI] Slot{i + 1} 创建完成，位置: {xPos}");
        }

        // 初始隐藏
        bindingPanel.SetActive(false);
        
        Debug.Log("[绑定UI] ✓ 所有UI元素创建完成并隐藏");
    }

    public override void ProcessInput(ref InputFrame frame)
    {
        // 调试模式：检测绑定按键
        if (enableDebugBinding && Input.GetKeyDown(KeyCode.N))
        {
            ToggleBindingMode();
        }

        // 绑定模式：检测输入并绑定
        if (isBindingMode)
        {
            DetectBindingInput();
            return; // 绑定模式下不处理正常输入
        }

        // --------------------------------------------------------------------
        // 1. 系统与光标
        // --------------------------------------------------------------------
        frame.CursorPosition = Input.mousePosition;
        
        bool pausePressed = Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.JoystickButton7);
        frame.Pause = ProcessVirtualKey(pausePressed, ref _lastPause);

        // --------------------------------------------------------------------
        // 2. 主轨道 (键盘、鼠标、以及主手柄) - 根据绑定动态分配
        // --------------------------------------------------------------------
        
        // 检查哪个设备被绑定到了"主轨道"
        // 如果没有显式绑定，默认键鼠输入到主轨道
        bool hasKeyboardBindingToPlayer = System.Array.IndexOf(bindings, 1) >= 0;
        
        if (!hasKeyboardBindingToPlayer)
        {
            // 键鼠没有绑定到任何玩家，输入到主轨道
            ProcessMainTrack(ref frame);
        }
        else
        {
            // 键鼠已绑定到某个玩家槽位，主轨道留空
            frame.Move = null;
        }

        // --------------------------------------------------------------------
        // 3. 多人模块 (P1-P4) - 根据绑定动态分配
        // --------------------------------------------------------------------
        for (int slot = 0; slot < 4; slot++)
        {
            int deviceId = bindings[slot];
            
            if (deviceId == 0)
            {
                // 未绑定，跳过
                continue;
            }
            
            string playerName = $"P{slot + 1}";
            
            if (deviceId == 1) // 键鼠
            {
                Debug.Log($"[输入处理] {playerName} ← 键鼠");
                ProcessKeyboardToPlayer(slot, ref frame);
            }
            else if (deviceId >= 2 && deviceId <= 5) // 手柄1-4
            {
                int joyIndex = deviceId - 1; // 2->1, 3->2, 4->3, 5->4
                Debug.Log($"[输入处理] {playerName} ← 手柄{joyIndex}");
                ProcessGamepadToPlayer(slot, joyIndex, ref frame);
            }
        }
    }

    void ProcessMainTrack(ref InputFrame frame)
    {
        // 移动
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        frame.Move = new Vector2(moveX, moveY);

        // 空格
        bool spacePressed = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);
        frame.Space = ProcessVirtualKey(spacePressed, ref _lastSpace);

        // 确认
        bool confirmPressed = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space) || 
                              Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.JoystickButton0);
        frame.Confirm = ProcessVirtualKey(confirmPressed, ref _lastConfirm);

        // 取消
        bool cancelPressed = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Mouse1) || 
                             Input.GetKey(KeyCode.JoystickButton1);
        frame.Cancel = ProcessVirtualKey(cancelPressed, ref _lastCancel);

        // 攻击
        bool attackPressed = Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Mouse0) || 
                             Input.GetKey(KeyCode.JoystickButton2);
        frame.Attack = ProcessVirtualKey(attackPressed, ref _lastAttack);

        // 防御
        bool defensePressed = Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Mouse1) || 
                              Input.GetKey(KeyCode.JoystickButton3);
        frame.Defense = ProcessVirtualKey(defensePressed, ref _lastDefense);

        // 1-9 数字键
        frame.First   = GetRawButtonState(KeyCode.Alpha1);
        frame.Second  = GetRawButtonState(KeyCode.Alpha2);
        frame.Third   = GetRawButtonState(KeyCode.Alpha3);
        frame.Fourth  = GetRawButtonState(KeyCode.Alpha4);
        frame.Fifth   = GetRawButtonState(KeyCode.Alpha5);
        frame.Sixth   = GetRawButtonState(KeyCode.Alpha6);
        frame.Seventh = GetRawButtonState(KeyCode.Alpha7);
        frame.Eighth  = GetRawButtonState(KeyCode.Alpha8);
        frame.Ninth   = GetRawButtonState(KeyCode.Alpha9);
    }

    void ProcessKeyboardToPlayer(int slot, ref InputFrame frame)
    {
        // 键鼠输入映射到指定玩家槽位
        // 必须使用 GetKey 而不是 GetAxis，否则会混入连接的第一个手柄信号
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveY += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveY -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX += 1f;

        Vector2 move = new Vector2(moveX, moveY);

        // 键鼠没有真正的 XYAB，这里映射到主轨道的动作键
        ButtonState padX = GetRawButtonState(KeyCode.J); // Attack
        ButtonState padY = GetRawButtonState(KeyCode.K); // Defense
        ButtonState padA = GetRawButtonState(KeyCode.Space); // Space/Confirm
        ButtonState padB = GetRawButtonState(KeyCode.X); // Cancel

        SetPlayerInput(slot, ref frame, move, padX, padY, padA, padB);
    }

    void ProcessGamepadToPlayer(int slot, int joyIndex, ref InputFrame frame)
    {
        // 手柄输入映射到指定玩家槽位
        Vector2 move = Vector2.zero;
        
        // Unity 的 Joystick 轴命名规则：
        // 第N个手柄的左摇杆X轴 = $"Joy{N} X"
        // 第N个手柄的左摇杆Y轴 = $"Joy{N} Y"
        try 
        {
            string xAxis = $"Joy{joyIndex} X";
            string yAxis = $"Joy{joyIndex} Y";
            
            float h = Input.GetAxisRaw(xAxis);
            float v = Input.GetAxisRaw(yAxis);
            move = new Vector2(h, v);
            
            Debug.Log($"[手柄{joyIndex}] 读取摇杆: X轴({xAxis})={h}, Y轴({yAxis})={v}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[手柄{joyIndex}] 读取摇杆失败: {e.Message}");
        }

        int btnOffset = (joyIndex - 1) * 20;
        ButtonState padX = GetRawButtonState((KeyCode)((int)KeyCode.Joystick1Button2 + btnOffset));
        ButtonState padY = GetRawButtonState((KeyCode)((int)KeyCode.Joystick1Button3 + btnOffset));
        ButtonState padA = GetRawButtonState((KeyCode)((int)KeyCode.Joystick1Button0 + btnOffset));
        ButtonState padB = GetRawButtonState((KeyCode)((int)KeyCode.Joystick1Button1 + btnOffset));

        SetPlayerInput(slot, ref frame, move, padX, padY, padA, padB);
    }

    void SetPlayerInput(int slot, ref InputFrame frame, Vector2 move, 
                        ButtonState padX, ButtonState padY, ButtonState padA, ButtonState padB)
    {
        string slotName = $"P{slot + 1}";
        Debug.Log($"[写入数据] {slotName} Move={move}, PadA.Down={padA.Down}");
        
        switch (slot)
        {
            case 0:
                frame.P1_Move = move;
                frame.P1_PadX = padX; frame.P1_PadY = padY;
                frame.P1_PadA = padA; frame.P1_PadB = padB;
                break;
            case 1:
                frame.P2_Move = move;
                frame.P2_PadX = padX; frame.P2_PadY = padY;
                frame.P2_PadA = padA; frame.P2_PadB = padB;
                break;
            case 2:
                frame.P3_Move = move;
                frame.P3_PadX = padX; frame.P3_PadY = padY;
                frame.P3_PadA = padA; frame.P3_PadB = padB;
                break;
            case 3:
                frame.P4_Move = move;
                frame.P4_PadX = padX; frame.P4_PadY = padY;
                frame.P4_PadA = padA; frame.P4_PadB = padB;
                break;
        }
    }

    // ========================================================================
    // 绑定系统
    // ========================================================================

    void ToggleBindingMode()
    {
        isBindingMode = !isBindingMode;
        
        if (bindingPanel != null)
            bindingPanel.SetActive(isBindingMode);
        
        if (isBindingMode)
        {
            Debug.Log("[绑定模式] 开启。请按任意键/手柄按键进行绑定。");
        }
        else
        {
            Debug.Log("[绑定模式] 保存并关闭。");
            SaveBindings();
        }
    }

    void DetectBindingInput()
    {
        // 检测键鼠输入（排除N键）
        if (Input.anyKeyDown)
        {
            // 检查是否是N键
            if (Input.GetKeyDown(KeyCode.N))
                return;
            
            // 任意其他键盘按键都视为键鼠输入
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && (int)keyCode < 330) // 只检测键盘和鼠标
                {
                    Debug.Log($"[绑定] 检测到键鼠输入: {keyCode}");
                    TryBind(1);
                    return;
                }
            }
        }

        // 检测手柄输入
        for (int joy = 1; joy <= 4; joy++)
        {
            for (int btn = 0; btn < 20; btn++)
            {
                KeyCode key = (KeyCode)((int)KeyCode.Joystick1Button0 + (joy - 1) * 20 + btn);
                if (Input.GetKeyDown(key))
                {
                    Debug.Log($"[绑定] 检测到手柄{joy}输入: {key}");
                    TryBind(joy + 1);
                    return;
                }
            }
        }
    }

    void TryBind(int deviceId)
    {
        int existingSlot = System.Array.IndexOf(bindings, deviceId);
        
        if (existingSlot >= 0)
        {
            bindings[existingSlot] = 0;
            Debug.Log($"[绑定] 设备 {GetDeviceName(deviceId)} 从 P{existingSlot + 1} 移除。");
        }
        else
        {
            int emptySlot = System.Array.IndexOf(bindings, 0);
            if (emptySlot >= 0)
            {
                bindings[emptySlot] = deviceId;
                Debug.Log($"[绑定] 设备 {GetDeviceName(deviceId)} 绑定到 P{emptySlot + 1}。");
            }
            else
            {
                Debug.LogWarning("[绑定] 所有槽位已满！");
            }
        }
        
        UpdateBindingUI();
    }

    void UpdateBindingUI()
    {
        if (playerSlots == null) return;
        
        for (int i = 0; i < 4; i++)
        {
            if (playerSlots[i] != null)
            {
                playerSlots[i].color = bindings[i] > 0 ? boundColor : unboundColor;
            }
        }
    }

    void SaveBindings()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.SetInt($"PlayerSlot_{i}", bindings[i]);
        }
        PlayerPrefs.Save();
        Debug.Log("[绑定] 已保存。");
    }

    void LoadBindings()
    {
        for (int i = 0; i < 4; i++)
        {
            bindings[i] = PlayerPrefs.GetInt($"PlayerSlot_{i}", 0);
        }
        
        Debug.Log("=== [绑定配置] 已加载 ===");
        for (int i = 0; i < 4; i++)
        {
            string deviceName = GetDeviceName(bindings[i]);
            Debug.Log($"  P{i + 1} → {deviceName} (ID:{bindings[i]})");
        }
        Debug.Log("========================");
    }

    string GetDeviceName(int deviceId)
    {
        if (deviceId == 1) return "键鼠";
        if (deviceId >= 2 && deviceId <= 5) return $"手柄{deviceId - 1}";
        return "未知";
    }

    // ========================================================================
    // 工具方法
    // ========================================================================

    private ButtonState GetRawButtonState(KeyCode key)
    {
        return new ButtonState
        {
            Down = Input.GetKeyDown(key),
            Hold = Input.GetKey(key),
            Up = Input.GetKeyUp(key)
        };
    }

    private ButtonState ProcessVirtualKey(bool isPressedNow, ref bool lastState)
    {
        bool down = false, hold = false, up = false;
        if (isPressedNow)
        {
            hold = true;
            if (!lastState) down = true;
        }
        else
        {
            if (lastState) up = true;
        }
        lastState = isPressedNow;
        return new ButtonState { Down = down, Hold = hold, Up = up };
    }
}