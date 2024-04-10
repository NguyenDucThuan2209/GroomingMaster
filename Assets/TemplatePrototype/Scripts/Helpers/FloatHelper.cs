using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class FloatHelper
    {
        public static float Ceil(float value, int digits)
        {
            var m = Mathf.Pow(10, digits);
            value *= m;
            value = Mathf.Ceil(value);
            return value / m;
        }
        public static float Floor(float value, int digits)
        {
            var m = Mathf.Pow(10, digits);
            value *= m;
            value = Mathf.Floor(value);
            return value / m;
        }
    }
}
