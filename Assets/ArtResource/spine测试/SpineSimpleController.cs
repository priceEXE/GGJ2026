using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 注意：此脚本依赖 Spine Unity Runtime。
// 如果报错 "The type or namespace name 'Spine' could not be found"，
// 请确保已导入 Spine 插件 (https://esotericsoftware.com/spine-unity-download)
namespace Spine.Unity 
{
    public class SpineSimpleController : MonoBehaviour
    {
        public SkeletonAnimation skeletonAnimation;
        [Header("Settings")]
        public bool loop = true;
        public float timeScale = 1f;

        private void Start()
        {
            if (skeletonAnimation == null)
                skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        private void OnGUI()
        {
            if (skeletonAnimation == null)
            {
                GUILayout.Label("No SkeletonAnimation found!");
                return;
            }

            if (skeletonAnimation.Skeleton == null || skeletonAnimation.Skeleton.Data == null)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 200, Screen.height - 20));
            GUILayout.BeginVertical("box");
            GUILayout.Label($"Animations ({skeletonAnimation.Skeleton.Data.Animations.Count})");

            // 动态遍历所有动画并创建按钮
            foreach (var anim in skeletonAnimation.Skeleton.Data.Animations)
            {
                if (GUILayout.Button(anim.Name))
                {
                    Debug.Log($"Playing: {anim.Name}");
                    skeletonAnimation.timeScale = timeScale;
                    skeletonAnimation.AnimationState.SetAnimation(0, anim.Name, loop);
                }
            }

            GUILayout.Space(10);
            loop = GUILayout.Toggle(loop, "Loop");
            GUILayout.Label($"TimeScale: {timeScale:F1}");
            timeScale = GUILayout.HorizontalSlider(timeScale, 0, 2);

            if (GUILayout.Button("Stop"))
            {
                skeletonAnimation.AnimationState.ClearTracks();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
