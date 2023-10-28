#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;

namespace Bro.Shaders
{
    public class RealisticEffectShaderGUI : ShaderGUI
    {
        static void RemoveProperty(string propertyName, ref MaterialProperty[] properties)
        {
            if (properties.Length == 0)
            {
                return;
            }

            var newProperties = new MaterialProperty[properties.Length - 1];
            int newIndex = 0;
            foreach (var property in properties)
            {
                if (property == null || property.name != propertyName)
                {
                    newProperties[newIndex++] = property;
                }
            }

            properties = newProperties;
        }

        static MaterialProperty GetProperty(string propertyName, MaterialProperty[] propertyList)
        {
            return propertyList.FirstOrDefault(p => p != null && p.name == propertyName);
        }

        private struct Blend
        {
            public BlendMode SrcRGB;
            public BlendMode DstRGB;
            public BlendMode SrcA;
            public BlendMode DstA;
        }


        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (GetProperty("_EnableTint", properties).floatValue < 1f)
            {
                RemoveProperty("_Tint", ref properties);
            }

            SetupBlending(ref properties);

            base.OnGUI(materialEditor, properties);
        }

        private static void SetupBlending(ref MaterialProperty[] properties)
        {
            var blendType = (BlendType) GetProperty("_BlendType", properties).floatValue;
            bool useBlendPreset = blendType != BlendType.Custom;
            if (useBlendPreset)
            {
                Blend blend = new Blend();

                switch (blendType)
                {
                    case BlendType.Fire:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.SrcAlpha,
                            DstRGB = BlendMode.One,
                            SrcA = BlendMode.One,
                            DstA = BlendMode.One,
                        };
                        break;
                    case BlendType.Smoke:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.SrcAlpha,
                            DstRGB = BlendMode.OneMinusSrcAlpha,
                            SrcA = BlendMode.One,
                            DstA = BlendMode.OneMinusSrcAlpha,
                        };
                        break;

                    case BlendType.Tracer:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.One,
                            DstRGB = BlendMode.OneMinusSrcAlpha,
                            SrcA = BlendMode.One,
                            DstA = BlendMode.OneMinusSrcAlpha
                        };
                        break;


                    case BlendType.Custom:
                        break;

                    case BlendType.Transparency:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.SrcAlpha,
                            DstRGB = BlendMode.OneMinusSrcAlpha,
                            SrcA = BlendMode.SrcAlpha,
                            DstA = BlendMode.OneMinusSrcAlpha
                        };
                        break;
                    case BlendType.TransparencyPremultiplied:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.One,
                            DstRGB = BlendMode.OneMinusSrcAlpha,
                            SrcA = BlendMode.One,
                            DstA = BlendMode.OneMinusSrcAlpha
                        };
                        break;
                    case BlendType.Additive:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.One,
                            DstRGB = BlendMode.One,
                            SrcA = BlendMode.One,
                            DstA = BlendMode.One
                        };
                        break;
                    case BlendType.AdditiveSoft:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.OneMinusDstColor,
                            DstRGB = BlendMode.One,
                            SrcA = BlendMode.OneMinusDstColor,
                            DstA = BlendMode.One
                        };
                        break;
                    case BlendType.Multiplicative:
                        blend = new Blend
                        {
                            SrcRGB = BlendMode.DstColor,
                            DstRGB = BlendMode.Zero,
                            SrcA = BlendMode.DstColor,
                            DstA = BlendMode.Zero
                        };
                        break;
                    default:
                        UnityEngine.Debug.LogError("unknown blend type = " + blendType);
                        break;
                }

                FindProperty("_Blend_srcRGB", properties).floatValue = (float) blend.SrcRGB;
                FindProperty("_Blend_dstRGB", properties).floatValue = (float) blend.DstRGB;
                FindProperty("_Blend_srcA", properties).floatValue = (float) blend.SrcA;
                FindProperty("_Blend_dstA", properties).floatValue = (float) blend.DstA;

                RemoveProperty("_Blend_srcRGB", ref properties);
                RemoveProperty("_Blend_dstRGB", ref properties);
                RemoveProperty("_Blend_srcA", ref properties);
                RemoveProperty("_Blend_dstA", ref properties);
            }

            
        }

        // void SetKeywordEnabled (string keyword, bool enabled) {
        //     if (enabled) {
        //         foreach (Material m in materials) {
        //             m.EnableKeyword(keyword);
        //         }
        //     }
        //     else {
        //         foreach (Material m in materials) {
        //             m.DisableKeyword(keyword);
        //         }
        //     }
        // }
    }
}

#endif