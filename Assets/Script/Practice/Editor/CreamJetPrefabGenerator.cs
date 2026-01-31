using UnityEngine;
using UnityEditor;
using System.IO;

namespace Practice.Editor
{
    public class CreamJetPrefabGenerator
    {
        private const string SAVE_PATH = "Assets/Res/Prefab/Particles/";

        [MenuItem("Practice/Generate Cream Jet Prefab")]
        public static void GenerateCreamJetOnly()
        {
            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
            }

            CreateCreamJet();
            
            AssetDatabase.Refresh();
            Debug.Log("Cream Jet Particle Prefab Generated in " + SAVE_PATH);
        }

        private static GameObject CreateBaseParticleSystem(string name)
        {
            GameObject go = new GameObject(name);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();

            Sprite circle = TextureGenerator.GetOrGenerateCircleSprite();
            
            string matPath = SAVE_PATH + "SolidCircle.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Particles/Standard Unlit"));
                mat.mainTexture = circle.texture;
                mat.SetFloat("_Mode", 2); 
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
                AssetDatabase.CreateAsset(mat, matPath);
            }
            psr.material = mat;

            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            main.playOnAwake = true;
            main.loop = false;
            
            return go;
        }

        private static void SavePrefab(GameObject go, string name)
        {
            string path = SAVE_PATH + name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"Created {name} at {path}");
        }

        private static void CreateCreamJet()
        {
            string name = "CreamJet";
            GameObject go = CreateBaseParticleSystem(name);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();

            // Main: 模拟液体喷溅
            var main = ps.main;
            main.duration = 1.0f;
            // 较低的随机性：速度和生命比较统一，形成“流体感”
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(8.0f, 10.0f); 
            main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
            // 【关键】受重力影响，产生下坠感，不轻飘飘
            main.gravityModifier = 1.2f; 
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            // 初始颜色作为基色
            main.startColor = new Color(1.0f, 0.95f, 0.85f); // 默认奶油色

            // Emission: 单次爆发
            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 30) });

            // Shape: Cone
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f; 
            shape.radius = 0.1f; 
            shape.radiusThickness = 1f;

            // Size Over Lifetime: 液体喷出后略微变大
            var sizeOL = ps.sizeOverLifetime;
            sizeOL.enabled = true;
            sizeOL.size = new ParticleSystem.MinMaxCurve(1.0f, AnimationCurve.Linear(0, 1, 1, 1.5f));

            // Color Over Lifetime: 保持不透明感，仅在末尾淡出
            var colOL = ps.colorOverLifetime;
            colOL.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(Color.white, 0.0f), 
                    new GradientColorKey(Color.white, 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1.0f, 0.0f), 
                    new GradientAlphaKey(1.0f, 0.7f), // 保持 70% 的生命周期不透明
                    new GradientAlphaKey(0.0f, 1.0f) 
                }
            );
            colOL.color = grad;

            // Noise: 极低的随机扰动，保持液体的整体性
            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.2f; 
            noise.frequency = 0.3f;
            noise.scrollSpeed = 0.2f;
            noise.damping = true;

            // Limit Velocity: 更低的衰减，让喷溅冲力更远
            var limitVel = ps.limitVelocityOverLifetime;
            limitVel.enabled = true;
            limitVel.limit = 1.0f; 
            limitVel.dampen = 0.1f; // 仅 10% 阻尼，保持惯性

            // Collision: 无碰撞
            var collision = ps.collision;
            collision.enabled = false;

            SavePrefab(go, name);
        }
    }
}
