using UnityEngine;

namespace Practice
{
    /// <summary>
    /// 粒子发射管理器接口
    /// 提供高度封装的简易调用接口，屏蔽底层参数。
    /// </summary>
    public interface IParticleManager
    {
        /// <summary>
        /// [简易接口] 产生地表烟尘圈（落地、跳跃反馈）
        /// <para>生成位置和法向必须调整。</para>
        /// <para>烟圈只应该调整粒子数量（基础适中值：5），其他默认。</para>
        /// </summary>
        void SpawnDustRing(Vector3 position, Vector2 normal, int count = 5);

        /// <summary>
        /// [简易接口] 产生奶油喷射（受重力，无碰撞）
        /// <para>生成位置和法向（方向）必须调整。</para>
        /// <para>奶油喷射只调整力度（基础适中值：30），其他默认。</para>
        /// </summary>
        void SpawnCreamJet(Vector3 position, Vector2 direction, float force = 30f);

        /// <summary>
        /// [简易接口] 产生灰土烟雾喷射（有重力有碰撞）
        /// <para>生成位置和法向（方向）必须调整。无需调整其他参数。</para>
        /// </summary>
        void SpawnDustJet(Vector3 position, Vector2 direction);
    }

    public class ParticleManager : MonoBehaviour, IParticleManager
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

        #region IParticleManager Implementation (简易接口实现)

        public void SpawnDustRing(Vector3 position, Vector2 normal, int count = 5)
        {
            SpawnDustRingInternal(position, normal, 0.5f, count, 3f);
        }

        public void SpawnCreamJet(Vector3 position, Vector2 direction, float force = 30f)
        {
            SpawnCreamJetInternal(position, direction, new Color(1.0f, 0.95f, 0.85f), 10f, 0.2f, 10, force);
        }

        public void SpawnDustJet(Vector3 position, Vector2 direction)
        {
            SpawnDustJetInternal(position, direction, 15f, 0.5f, 15, 20f);
        }

        #endregion

        /// <summary>
        /// 烟尘圈形弥散（有重力有碰撞）
        /// </summary>
        public void SpawnDustRingInternal(Vector3 position, Vector2 normal, float size, int count, float force)
        {
            if (dustRingPrefab == null) return;
            
            GameObject go = Instantiate(dustRingPrefab, position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps == null) return;

            go.transform.forward = normal;

            var main = ps.main;
            main.startSizeMultiplier = size;
            main.startSpeedMultiplier = force;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, count) });

            ps.Play();
        }

        /// <summary>
        /// 烟尘喷射（写死灰土色，有重力有碰撞）
        /// </summary>
        public void SpawnDustJetInternal(Vector3 position, Vector2 direction, float spread = 15f, float sizeMultiplier = 0.5f, int count = 15, float force = 20.0f)
        {
            if (dustJetPrefab == null) return;

            GameObject go = Instantiate(dustJetPrefab, position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps == null) return;

            if (direction != Vector2.zero)
            {
                go.transform.forward = direction;
            }

            var shape = ps.shape;
            shape.angle = spread;

            var main = ps.main;
            main.startSizeMultiplier = sizeMultiplier;
            main.startSpeedMultiplier = force;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, count) });

            ps.Play();
        }

        /// <summary>
        /// 奶油喷射（受重力，无碰撞，高膨胀）
        /// </summary>
        public void SpawnCreamJetInternal(Vector3 position, Vector2 direction, Color color, float spread = 10f, float sizeMultiplier = 0.2f, int count = 10, float force = 30.0f)
        {
            if (creamJetPrefab == null) return;

            GameObject go = Instantiate(creamJetPrefab, position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps == null) return;

            if (direction != Vector2.zero)
            {
                go.transform.forward = direction;
            }

            var main = ps.main;
            main.startColor = color;
            var shape = ps.shape;
            shape.angle = spread;

            main.startSizeMultiplier = sizeMultiplier;
            main.startSpeedMultiplier = force;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, count) });

            ps.Play();
        }
    }
}
