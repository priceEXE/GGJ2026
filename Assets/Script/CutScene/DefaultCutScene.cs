using System;
using DG.Tweening;
using MemoFramework;
using UnityEngine;

namespace TapTap2025
{
    public class DefaultCutScene : CutsceneAgent
    {
        public override Transform CutsceneView => transform;
        [SerializeField] private CanvasGroup CG;
        public override void EnterCutscene(float duration, Action onEnd)
        {
            CG.alpha = 0;
            CG.DOFade(1,duration).OnComplete(()=> onEnd?.Invoke());
        }

        public override void FadeCutscene(float duration, Action onEnd)
        {
            CG.alpha = 1;
            CG.DOFade(0, duration).OnComplete(()=> onEnd?.Invoke());
        }
    }
}