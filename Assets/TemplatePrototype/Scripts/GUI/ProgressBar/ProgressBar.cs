using HyrphusQ.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        protected bool showByDefault = true;
        [SerializeField]
        protected TextAdapter textAdapter;

        protected float animationDuration;
        protected RangeVariableReference<int> minMaxIntValue;
        protected RangeVariableReference<float> minMaxFloatValue;
        protected RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                    m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        protected virtual void OnEnable()
        {
            textAdapter.OnEnable();
            gameObject.SetActive(showByDefault);
        }
        protected virtual void OnDestroy()
        {
            if (minMaxIntValue != null && !minMaxIntValue.Equals(default(RangeVariableReference<int>)))
                minMaxIntValue.onValueChanged -= OnValueChanged;
        }

        public virtual void Init(RangeVariableReference<int> minMaxIntValue, float animationDuration = 0f)
        {
            this.animationDuration = animationDuration;
            this.minMaxIntValue = minMaxIntValue;
            this.minMaxIntValue.onValueChanged += OnValueChanged;
            SetValueImmediately(minMaxIntValue.value);
        }
        public virtual void Init(RangeVariableReference<float> minMaxFloatValue, float animationDuration = 0f)
        {
            this.animationDuration = animationDuration;
            this.minMaxFloatValue = minMaxFloatValue;
            this.minMaxFloatValue.onValueChanged += OnValueChanged;
            SetValueImmediately(minMaxFloatValue.value);
        }
        public abstract void SetValueImmediately(int value);
        public abstract void SetValueImmediately(float value);
        public abstract void SetValue(int oldValue, int value, float animationDuration);
        public abstract void SetValue(float oldValue, float value, float animationDuration);

        protected abstract void OnValueChanged(ValueDataChanged<int> data);
        protected abstract void OnValueChanged(ValueDataChanged<float> data);

        protected IEnumerator TextCounterCR(int oldValue, int newValue, float animationDuration)
        {
            float t = 0.0f;
            float duration = animationDuration;
            while (t <= duration)
            {
                var value = (int)Mathf.Lerp(oldValue, newValue, Mathf.Clamp01(t / duration));
                SetValueImmediately(value);
                yield return null;
                t += Time.deltaTime;
            }
        }
        protected IEnumerator TextCounterCR(float oldValue, float newValue, float animationDuration)
        {
            float t = 0.0f;
            float duration = animationDuration;
            while (t <= duration)
            {
                var value = Mathf.Lerp(oldValue, newValue, Mathf.Clamp01(t / duration));
                SetValueImmediately(value);
                yield return null;
                t += Time.deltaTime;
            }
        }
    }
}