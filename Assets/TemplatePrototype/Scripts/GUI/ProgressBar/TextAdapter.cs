//#define DOTween

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if DOTween
using DG.Tweening;
#endif

namespace HyrphusQ.GUI
{
    [Serializable]
    public class TextAdapter
    {
        public enum TextType
        {
            None,
            BuildIn,
            TextMeshPro
        }

        public TextType textType = TextType.None;
        [SerializeField]
        private Text textBuiltIn;
        [SerializeField]
        private TMP_Text textMeshPro;

        private Func<string> getTextMethod;
        private Action<string> setTextMethod;
        private Action<string, float, bool> doTextMethod;
        private Action<int, int, float> doCounterIntMethod;

        public void OnEnable()
        {
            switch (textType)
            {
                case TextType.BuildIn:
                    getTextMethod = () => textBuiltIn.text;
                    setTextMethod = text => textBuiltIn.text = text;
                    break;
                case TextType.TextMeshPro:
                    getTextMethod = () => textMeshPro.text;
                    setTextMethod = text => textMeshPro.text = text;
                    doTextMethod = (to, duration, richTextEnable) => {
                        #if DOTween
                        textMeshPro.DOText(to, duration, richTextEnable);
                        #endif
                    };
                    doCounterIntMethod = (fromValue, endValue, duration) => {
                        #if DOTween
                        textMeshPro.DOCounter(fromValue, endValue, duration);
                        #endif
                    };
                    break;
                default:
                    break;
            }
        }

        public T GetAdapteeText<T>() where T : MaskableGraphic
        {
            if (typeof(T).Equals(typeof(Text)))
                return textBuiltIn as T;
            else if (typeof(T).Equals(typeof(TMP_Text)))
                return textMeshPro as T;
            Debug.LogError($"Type mismatch exception {typeof(T)}");
            return null;
        }
        public string GetText()
        {
            return getTextMethod?.Invoke() ?? null;
        }
        public void SetText(string text)
        {
            setTextMethod?.Invoke(text);
        }
        public void DOText(string to, float duration, bool richTextEnable = true)
        {
            doTextMethod?.Invoke(to, duration, richTextEnable);
        }
        public void DOCounter(int fromValue, int endValue, float duration)
        {
            doCounterIntMethod?.Invoke(fromValue, endValue, duration);
        }
    }
}