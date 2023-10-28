Shader "Bro/Effects/SimpleEffectRealistic"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Main", 2D) = "white" {}
        [Toggle(_ENABLE_TINT)] _EnableTint ("Enable Tint", Float) = 1
        [HDR] _Tint ("Tint", Color) = (0.5,0.5,0.5,0.5)
        [Toggle(_ENABLE_VERTEX_COLOR)] _EnableVertexColor ("Enable Vertex Color", Float) = 1
        [Enum(Bro.Shaders.BlendType)]_BlendType ("Blend", Float) = 10
        [Enum(UnityEngine.Rendering.BlendMode)] _Blend_srcRGB ("Src RGB", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _Blend_dstRGB ("Dst RGB", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _Blend_srcA ("Src A", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _Blend_dstA ("Dst A", Float) = 1
        [Toggle(_ALPHAPREMULTIPLY_ON)] _PremultiplyAlpha ("Premultiply Alpha", Float) = 1
        [Enum(Bro.Shaders.RenderFace)]_Cull ("Render Face", Float) = 2

    }
    CustomEditor "Bro.Shaders.RealisticEffectShaderGUI"
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        
        ZTest LEqual
        ZWrite Off
        HLSLINCLUDE
        #pragma target 3.0
        ENDHLSL

        Pass
        {
            Cull[_Cull]
            Blend [_Blend_srcRGB] [_Blend_dstRGB], [_Blend_srcA] [_Blend_dstA]
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature _ENABLE_TINT
            #pragma shader_feature _ENABLE_VERTEX_COLOR
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            

            #include "Assets/Scripts/Bro/Toolbox/Client/Shaders/Core/EffectsCore.hlsl"

            sampler2D _MainTex;
            UNITY_INSTANCING_BUFFER_START(FireRealisticShader)
            UNITY_DEFINE_INSTANCED_PROP(half4, _MainTex_ST)
            #if _ENABLE_TINT
                UNITY_DEFINE_INSTANCED_PROP(half4, _Tint)
            #endif
            UNITY_INSTANCING_BUFFER_END(FireRealisticShader)

            effect_v2f vert(effect_appdata v)
            {
                effect_v2f o; 
            
                UNITY_SETUP_INSTANCE_ID(v); 
                UNITY_TRANSFER_INSTANCE_ID(v, o); 
            
                o.vertex = ObjectToClipPosition(v.vertex);
                half4 mainTex_ST = UNITY_ACCESS_INSTANCED_PROP(FireRealisticShader, _MainTex_ST);
                o.uv = v.uv.xy * mainTex_ST.xy + mainTex_ST.zw;  
                o.color = v.color; 
                return o; 
            }
            
            half4 frag(effect_v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                half4 result = tex2D(_MainTex, i.uv) 
                #ifdef _ENABLE_TINT
                * UNITY_ACCESS_INSTANCED_PROP(FireRealisticShader, _Tint)
                #endif
                #ifdef _ENABLE_VERTEX_COLOR
                * i.color;
                #endif
                ;
                #ifdef _ALPHAPREMULTIPLY_ON
                result.rgb *= result.a;
                #endif

                return result;
            }
            ENDHLSL
        }
    }
}