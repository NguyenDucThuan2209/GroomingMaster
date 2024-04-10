using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Const
{
    public class Const
    {
        public class ShaderProperty
        {
            public static readonly int MainColor_ID = Shader.PropertyToID("_Color");
            public static readonly int MainTexture_ID = Shader.PropertyToID("_MainTex");
            public static readonly int PaintTexture_ID = Shader.PropertyToID("_PaintTex");
            public static readonly int PaintNormalTexture_ID = Shader.PropertyToID("_PaintNormalTex");
        }
        public class UnityTag
        {
            public static readonly string Default = "Untagged";
            public static readonly string Player = "Player";
            public static readonly string MainCamera = "MainCamera";
            public static readonly string PaintableRendererTag = "PaintableRenderer";
        }
        public class UnityLayerMask
        {
            public static readonly int Default = LayerMask.NameToLayer(nameof(Default));
            public static readonly int UI = LayerMask.NameToLayer(nameof(UI));
            public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            public static readonly int PaintableRendererLayer = LayerMask.NameToLayer("PaintableRenderer");
        }
        public class IntValue
        {
            public static readonly int Zero = 0;
            public static readonly int One = 1;
        }
        public class FloatValue
        {
            public static readonly float ZeroF = 0f;
            public static readonly float Half = 0.5f;
            public static readonly float OneF = 1f;
        }
        public class StringValue
        {

        }
    }
}