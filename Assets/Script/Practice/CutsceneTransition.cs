using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Practice
{
    /// <summary>
    /// 三色条纹滑动转场效果
    /// </summary>
    public class CutsceneTransition : MonoBehaviour
    {
        public static CutsceneTransition Instance { get; private set; }

        [Header("Transition Parameters")]
        [Tooltip("进入动画时长")]
        public float enterDuration = 0.6f;
        
        [Tooltip("保持遮盖时长")]
        public float holdDuration = 0.3f;
        
        [Tooltip("退出动画时长")]
        public float exitDuration = 0.6f;
        
        [Tooltip("色片之间的延迟（秒）")]
        public float staggerDelay = 0.1f;
        
        [Tooltip("滑入角度（0°=垂直，90°=水平）")]
        [Range(0f, 90f)]
        public float angle = 15f;

        [Header("Colors")]
        public Color color1 = new Color(0.2f, 0.3f, 0.8f); // 蓝色
        public Color color2 = new Color(0.8f, 0.3f, 0.5f); // 粉色
        public Color color3 = new Color(0.9f, 0.7f, 0.2f); // 黄色

        [Header("Debug")]
        public bool enableDebug = false;
        public KeyCode debugKey = KeyCode.Y;

        [Header("References (Auto-generated)")]
        private Canvas transitionCanvas;
        private Image[] panels = new Image[3];
        private RectTransform[] panelRects = new RectTransform[3];

        private bool isTransitioning = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCanvas();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (enableDebug && Input.GetKeyDown(debugKey))
            {
                Play();
            }
        }

        /// <summary>
        /// 初始化 Canvas 和色片
        /// </summary>
        private void InitializeCanvas()
        {
            // 创建 Canvas
            GameObject canvasObj = new GameObject("TransitionCanvas");
            canvasObj.transform.SetParent(transform);
            transitionCanvas = canvasObj.AddComponent<Canvas>();
            transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            transitionCanvas.sortingOrder = 9999; // 最高显示层级

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();

            // 创建三个色片
            Color[] colors = { color1, color2, color3 };
            for (int i = 0; i < 3; i++)
            {
                GameObject panelObj = new GameObject($"Panel_{i + 1}");
                panelObj.transform.SetParent(canvasObj.transform, false);

                Image img = panelObj.AddComponent<Image>();
                img.color = colors[i];
                panels[i] = img;

                RectTransform rect = panelObj.GetComponent<RectTransform>();
                panelRects[i] = rect;

                // 设置足够大以遮盖屏幕（考虑旋转后的对角线）
                rect.sizeDelta = new Vector2(3000, 3000);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                // 初始位置：屏幕左侧外
                rect.anchoredPosition = new Vector2(-2000, 0);
            }

            // 初始隐藏
            canvasObj.SetActive(false);
        }

        /// <summary>
        /// 播放转场效果（无参数版本）
        /// </summary>
        public void Play()
        {
            Play(() =>
            {
                if (enableDebug)
                {
                    Debug.Log("[CutsceneTransition] Midpoint reached.");
                }
            });
        }

        /// <summary>
        /// 播放转场效果
        /// </summary>
        /// <param name="onMidpoint">在画面完全遮盖时的回调（通常用于切换场景）</param>
        public void Play(Action onMidpoint)
        {
            if (isTransitioning) return;
            StartCoroutine(TransitionSequence(onMidpoint));
        }

        /// <summary>
        /// 转场序列
        /// </summary>
        private IEnumerator TransitionSequence(Action onMidpoint)
        {
            isTransitioning = true;

            // 更新角度
            UpdatePanelRotations();

            // 【关键】重置所有色片到初始位置（在激活 Canvas 之前！）
            // 修正：不再使用 Screen.width (像素)，而是使用基于 ReferenceResolution 的固定安全距离
            // Canvas宽1920，半宽960；色片3000，旋转后最大半径约2121
            // 安全距离 = 960 + 2121 ≈ 3081 -> 取 4000 绝对安全
            float offScreenOffset = 4000f; 
            
            Vector2 initialPos = new Vector2(-offScreenOffset, 0); 
            foreach (var rect in panelRects)
            {
                rect.anchoredPosition = initialPos;
            }

            // 等待一帧，确保位置设置生效
            yield return null;

            // 现在才激活 Canvas，此时所有色片已经在左侧外了
            transitionCanvas.gameObject.SetActive(true);

            // ===== 进入阶段 =====
            yield return StartCoroutine(AnimatePanelsIn(offScreenOffset));

            // ===== 保持阶段 =====
            yield return new WaitForSeconds(holdDuration);

            // 触发中点回调（场景切换）
            onMidpoint?.Invoke();

            // ===== 退出阶段 =====
            yield return StartCoroutine(AnimatePanelsOut(offScreenOffset));

            transitionCanvas.gameObject.SetActive(false);
            isTransitioning = false;
        }

        /// <summary>
        /// 更新色片的旋转角度
        /// </summary>
        private void UpdatePanelRotations()
        {
            foreach (var rect in panelRects)
            {
                rect.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        /// <summary>
        /// 色片滑入动画
        /// </summary>
        private IEnumerator AnimatePanelsIn(float offset)
        {
            Vector2 startPos = new Vector2(-offset, 0);
            Vector2 endPos = new Vector2(0, 0);

            // 依次启动每个色片的滑入动画
            for (int i = 0; i < 3; i++)
            {
                int index = i; // 闭包捕获
                StartCoroutine(SmoothMove(panelRects[index], startPos, endPos, enterDuration, EaseInOutQuad));
                yield return new WaitForSeconds(staggerDelay);
            }

            // 等待最后一个色片完成
            yield return new WaitForSeconds(enterDuration);
        }

        /// <summary>
        /// 色片滑出动画（反向顺序，形成对称效果）
        /// </summary>
        private IEnumerator AnimatePanelsOut(float offset)
        {
            Vector2 startPos = new Vector2(0, 0);
            Vector2 endPos = new Vector2(offset, 0);

            // 倒序启动色片的滑出动画（3 → 2 → 1）
            for (int i = 2; i >= 0; i--)
            {
                int index = i;
                StartCoroutine(SmoothMove(panelRects[index], startPos, endPos, exitDuration, EaseInOutQuad));
                yield return new WaitForSeconds(staggerDelay);
            }

            // 等待最后一个色片完成
            yield return new WaitForSeconds(exitDuration);
        }

        /// <summary>
        /// 平滑移动动画
        /// </summary>
        private IEnumerator SmoothMove(RectTransform rect, Vector2 from, Vector2 to, float duration, Func<float, float> easeFunc)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = easeFunc(t);
                rect.anchoredPosition = Vector2.Lerp(from, to, easedT);
                yield return null;
            }
            rect.anchoredPosition = to;
        }

        /// <summary>
        /// 缓动函数：Ease In Out Quad
        /// </summary>
        private float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }
    }
}
