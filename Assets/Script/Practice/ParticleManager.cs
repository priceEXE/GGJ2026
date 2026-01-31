using UnityEngine;

namespace Practice
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }

        [Header("Particle Prefabs")]
        [Tooltip("Smoke ring diffusion (gravity, collision)")]
        public GameObject dustRingPrefab;
        
        [Tooltip("Smoke jet (gravity, collision)")]
        public GameObject dustJetPrefab;
        
        [Tooltip("Cream jet (no gravity, no collision)")]
        public GameObject creamJetPrefab;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 烟尘圈形弥散（有重力有碰撞）
        /// </summary>
        /// <param name="position">产生位置</param>
        /// <param name="normal">法向量（决定喷射平面/方向）</param>
        /// <param name="size">粒子大小</param>
        /// <param name="count">粒子数量</param>
        /// <param name="force">喷射力度</param>
        public void PlayDustRing(Vector3 position, Vector2 normal, float size, int count, float force)
        {
            if (dustRingPrefab == null) return;
            
            // 实例化
            GameObject go = Instantiate(dustRingPrefab, position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps == null) return;

            // 1. 设置方向 (Shape Rotation)
            // 在横版 2D 游戏中，地面烟尘圈是一个平贴在地面的圆。
            // 我们需要让粒子系统的发射中心面（Circle Shape 的平面）法线对齐地面法线。
            // 粒子系统的 Circle Shape 默认法线是本地 Z 轴。
            // 将本地 Z 轴对齐法向，则圆环面垂直于法向且垂直于屏幕。
            go.transform.forward = normal;

            // 2. 粒子大小 (在预制体基础上进行缩放)
            var main = ps.main;
            main.startSizeMultiplier = size;
            main.startSpeedMultiplier = force;

            // 3. 粒子数量
            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, count) });

            ps.Play();
        }

        /// <summary>
        /// 烟尘喷射（写死灰土色，有重力有碰撞）
        /// </summary>
        /// <param name="position">产生位置</param>
        /// <param name="direction">XY喷射方向向量</param>
        /// <param name="spread">扩散角度 (Cone Angle)</param>
        /// <param name="sizeMultiplier">大小倍率</param>
        /// <param name="count">粒子数量</param>
        /// <param name="force">爆发力度</param>
        public void PlayDustJet(Vector3 position, Vector2 direction, float spread = 30f, float sizeMultiplier = 1f, int count = 25, float force = 8.0f)
        {
            if (dustJetPrefab == null) return;

            GameObject go = Instantiate(dustJetPrefab, position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps == null) return;

            // 1. 设置方向 (发射轴 Z 对齐 XY 方向)
            if (direction != Vector2.zero)
            {
                go.transform.forward = direction;
            }

            // 2. 扩散度 (Shape Angle)
            var shape = ps.shape;
            shape.angle = spread;

            // 3. 参数覆盖
            var main = ps.main;
            main.startSizeMultiplier = sizeMultiplier;
            main.startSpeedMultiplier = force;

            // 3. 数量
            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, count) });

            ps.Play();
        }
        /// <summary>
        /// 奶油喷射（受重力，无碰撞，高膨胀）
        /// </summary>
        /// <param name="position">产生位置</param>
        /// <param name="direction">XY喷射方向向量</param>
        /// <param name="color">自定义颜色</param>
        /// <param name="spread">扩散度</param>
        /// <param name="sizeMultiplier">大小倍率</param>
        /// <param name="count">数量</param>
        /// <param name="force">爆发力度</param>
        public void PlayCreamJet(Vector3 position, Vector2 direction, Color color, float spread = 15f, float sizeMultiplier = 1f, int count = 30, float force = 10.0f)
        {
            if (creamJetPrefab == null) return;

            GameObject go = Instantiate(creamJetPrefab, position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps == null) return;

            // 1. 设置方向
            if (direction != Vector2.zero)
            {
                go.transform.forward = direction;
            }

            // 2. 颜色与扩散度
            var main = ps.main;
            main.startColor = color;
            var shape = ps.shape;
            shape.angle = spread;

            // 3. 参数覆盖
            main.startSizeMultiplier = sizeMultiplier;
            main.startSpeedMultiplier = force;

            // 4. 数量
            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, count) });

            ps.Play();
        }
    }
}
