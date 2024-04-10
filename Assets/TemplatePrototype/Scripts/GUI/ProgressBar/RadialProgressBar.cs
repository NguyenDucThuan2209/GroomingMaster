//#define DOTween

using UnityEngine;
using UnityEngine.UI;
using HyrphusQ.Events;
#if DOTween
using DG.Tweening;
#endif

namespace HyrphusQ.GUI
{
    [AddComponentMenu("HyrphusQ/GUI/ProgressBar/Radial")]
    public class RadialProgressBar : ProgressBar
    {
        [SerializeField]
        private Image progressImage;

        public override void SetValue(int oldValue, int value, float animationDuration)
        {
            textAdapter.DOCounter(oldValue, value, animationDuration);
            #if DOTween
            progressImage.DOFillAmount(minMaxIntValue.CalcInverseLerpValue(value), animationDuration);
            #endif
        }
        public override void SetValue(float oldValue, float value, float animationDuration)
        {
            textAdapter.DOCounter(Mathf.RoundToInt(value), Mathf.RoundToInt(value), animationDuration);
            #if DOTween
            progressImage.DOFillAmount(minMaxFloatValue.CalcInverseLerpValue(value), animationDuration);
            #endif
        }
        public override void SetValueImmediately(int value)
        {
            textAdapter.SetText(value.ToString());
            progressImage.fillAmount = minMaxIntValue.CalcInverseLerpValue(value);
        }
        public override void SetValueImmediately(float value)
        {
            textAdapter.SetText(value.ToString("0"));
            progressImage.fillAmount = minMaxFloatValue.CalcInverseLerpValue(value);
        }

        protected override void OnValueChanged(ValueDataChanged<int> data)
        {
            textAdapter.DOCounter(data.oldValue, data.newValue, minMaxIntValue.inverseLerpValue * animationDuration);
            #if DOTween
            progressImage.DOFillAmount(minMaxIntValue.inverseLerpValue, minMaxIntValue.inverseLerpValue * animationDuration);
            #endif
        }

        protected override void OnValueChanged(ValueDataChanged<float> data)
        {
            textAdapter.DOCounter((int)data.oldValue, (int)data.newValue, minMaxFloatValue.inverseLerpValue * animationDuration);
            #if DOTween
            progressImage.DOFillAmount(minMaxFloatValue.inverseLerpValue, minMaxFloatValue.inverseLerpValue * animationDuration);
            #endif
        }
    }
}