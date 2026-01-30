using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace TapTap2025
{
    public class GuideObject : MonoBehaviour
    {
        public string showText;
        public void ShowGuideText()
        {
            GameManager.instance?.guideTextUI.Show(showText);
        }
        
        public void OnDisable()
        {
            GameManager.instance?.guideTextUI.HideText();
        }
    }
}