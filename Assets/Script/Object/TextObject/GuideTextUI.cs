using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class GuideTextUI : MonoBehaviour
    {
        public TextMeshProUGUI Ttext;
        private RectTransform recttrans;
        private bool isShowing;
        

        public void OnEnable()
        {
            recttrans = GetComponent<RectTransform>();
            isShowing = false;
        }

        public void SetText(string text)
        {
            Ttext.SetText(text);
            SetSize();
        }

        public void SetSize()
        {
            string str = Ttext.text;
            // Debug.Log(str.Length);
            recttrans.sizeDelta = new Vector2(recttrans.sizeDelta.x, (str.Length / 10+1) * 16);
        }

        public void Show(string text)
        {
            this.gameObject.SetActive(true);
            SetText(text);
            StartCoroutine(ShowText());
        }
        
        IEnumerator ShowText()
        {
            if (isShowing) yield break;
            recttrans.DOLocalMoveY(-590 + recttrans.sizeDelta.y + 50, 0.5f).onComplete+= () => { isShowing = true; };
            yield return new WaitForSeconds(1.0f);
            HideText();
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
        
        public void HideText()
        {
            if (isShowing) recttrans.DOLocalMoveY(-590.0f, 0.5f).onComplete += () => {Hide();isShowing = false; };
        }
    }
}