using HyrphusQ.Events;
using System.Collections;
using UnityEngine;

namespace HyrphusQ.GUI
{
    [AddComponentMenu("HyrphusQ/GUI/ProgressBar/Text")]
    public class TextProgressBar : ProgressBar
    {
        public override void SetValue(int oldValue, int value, float animationDuration)
        {
            StartCoroutine(TextCounterCR(oldValue, value, animationDuration));
        }
        public override void SetValue(float oldValue, float value, float animationDuration)
        {
            StartCoroutine(TextCounterCR(oldValue, value, animationDuration));
        }
        public override void SetValueImmediately(int value)
        {
            textAdapter.SetText($"{value}/{minMaxIntValue.maxValue}");
        }
        public override void SetValueImmediately(float value)
        {
            textAdapter.SetText($"{value.ToString("0.00")}/{minMaxIntValue.maxValue.ToString("0.00")}");
        }

        protected override void OnValueChanged(ValueDataChanged<int> data)
        {
            float duration = minMaxIntValue.inverseLerpValue * animationDuration;
            SetValue(data.oldValue, data.newValue, duration);
        }
        protected override void OnValueChanged(ValueDataChanged<float> data)
        {
            float duration = minMaxFloatValue.inverseLerpValue * animationDuration;
            SetValue(data.oldValue, data.newValue, duration);
        }
    }
}