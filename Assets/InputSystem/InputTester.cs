using UnityEngine;

/// <summary>
/// 输入测试脚本
/// 可以选择输入源（全局/P1-P4），根据输入移动并显示按键特效
/// 所有元素自动创建，无需手动设置
/// </summary>
public class InputTester : MonoBehaviour
{
    public enum InputSource
    {
        Global,  // 全局控制（主轨道）
        P1,
        P2,
        P3,
        P4
    }

    [Header("设置")]
    public InputSource inputSource = InputSource.Global;
    public float moveSpeed = 5f;
    public float deadzone = 0.1f;

    [Header("特效参数")]
    public float bubbleLifetime = 1f;
    public float bubbleRiseSpeed = 2f;

    // XYAB 按键颜色 (参考Xbox手柄)
    private Color colorX = new Color(0.2f, 0.5f, 1f); // 蓝色
    private Color colorY = new Color(1f, 0.9f, 0.2f); // 黄色
    private Color colorA = new Color(0.3f, 0.9f, 0.3f); // 绿色
    private Color colorB = new Color(1f, 0.3f, 0.3f); // 红色

    private TextMesh labelText;

    void Start()
    {
        // 创建文本标签
        CreateLabel();
    }

    void Update()
    {
        // 获取当前帧的输入
        InputFrame frame = InputLayerStack.Instance.CurrentFrame;

        // 处理移动
        Vector2? move = GetMoveInput(frame);
        
        // 添加调试日志
        if (move.HasValue && move.Value.magnitude > 0.01f)
        {
            Debug.Log($"[{inputSource}对象] 读取到 Move={move.Value}");
        }
        
        if (move.HasValue && move.Value.magnitude > deadzone)
        {
            Vector3 movement = new Vector3(move.Value.x, move.Value.y, 0) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }

        // 处理按键特效
        ProcessButtonEffects(frame);
    }

    Vector2? GetMoveInput(InputFrame frame)
    {
        switch (inputSource)
        {
            case InputSource.Global:
                return frame.Move;
            case InputSource.P1:
                return frame.P1_Move;
            case InputSource.P2:
                return frame.P2_Move;
            case InputSource.P3:
                return frame.P3_Move;
            case InputSource.P4:
                return frame.P4_Move;
            default:
                return null;
        }
    }

    void ProcessButtonEffects(InputFrame frame)
    {
        // 根据输入源检测按键
        if (inputSource == InputSource.Global)
        {
            // 全局控制下，XYAB对应不同的动作键
            if (frame.Attack.Down == true) SpawnBubble(colorX);
            if (frame.Defense.Down == true) SpawnBubble(colorY);
            if (frame.Confirm.Down == true) SpawnBubble(colorA);
            if (frame.Cancel.Down == true) SpawnBubble(colorB);
        }
        else
        {
            // P1-P4 模式
            ButtonState? padX = null, padY = null, padA = null, padB = null;
            
            switch (inputSource)
            {
                case InputSource.P1:
                    padX = frame.P1_PadX; padY = frame.P1_PadY;
                    padA = frame.P1_PadA; padB = frame.P1_PadB;
                    break;
                case InputSource.P2:
                    padX = frame.P2_PadX; padY = frame.P2_PadY;
                    padA = frame.P2_PadA; padB = frame.P2_PadB;
                    break;
                case InputSource.P3:
                    padX = frame.P3_PadX; padY = frame.P3_PadY;
                    padA = frame.P3_PadA; padB = frame.P3_PadB;
                    break;
                case InputSource.P4:
                    padX = frame.P4_PadX; padY = frame.P4_PadY;
                    padA = frame.P4_PadA; padB = frame.P4_PadB;
                    break;
            }

            if (padX.HasValue && padX.Value.Down == true) SpawnBubble(colorX);
            if (padY.HasValue && padY.Value.Down == true) SpawnBubble(colorY);
            if (padA.HasValue && padA.Value.Down == true) SpawnBubble(colorA);
            if (padB.HasValue && padB.Value.Down == true) SpawnBubble(colorB);
        }
    }

    void SpawnBubble(Color color)
    {
        // 自动创建简单圆球（2D 场景使用 Sprite）
        GameObject bubble = new GameObject("Bubble");
        bubble.transform.position = transform.position;
        
        // 添加 SpriteRenderer（2D圆形）
        SpriteRenderer sr = bubble.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = color;
        bubble.transform.localScale = Vector3.one * 0.3f;

        // 添加上升和销毁组件
        BubbleEffect effect = bubble.AddComponent<BubbleEffect>();
        effect.lifetime = bubbleLifetime;
        effect.riseSpeed = bubbleRiseSpeed;
    }

    Sprite CreateCircleSprite()
    {
        // 创建一个简单的圆形纹理
        int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = distance < radius ? 1f : 0f;
                colors[y * size + x] = new Color(1, 1, 1, alpha);
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void CreateLabel()
    {
        // 在对象上方创建文本标签
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = new Vector3(0, 1, 0);

        labelText = labelObj.AddComponent<TextMesh>();
        labelText.text = inputSource.ToString();
        labelText.characterSize = 0.1f;
        labelText.anchor = TextAnchor.MiddleCenter;
        labelText.alignment = TextAlignment.Center;
        labelText.color = Color.white;
    }

    void OnValidate()
    {
        // 当Inspector中修改时更新标签
        if (labelText != null)
        {
            labelText.text = inputSource.ToString();
        }
    }
}

/// <summary>
/// 气泡特效辅助组件
/// </summary>
public class BubbleEffect : MonoBehaviour
{
    public float lifetime = 1f;
    public float riseSpeed = 2f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // 上升
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // 渐变透明
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            float alpha = 1f - (timer / lifetime);
            
            // 支持 2D SpriteRenderer
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
            
            // 支持 3D Renderer
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }
    }
}
