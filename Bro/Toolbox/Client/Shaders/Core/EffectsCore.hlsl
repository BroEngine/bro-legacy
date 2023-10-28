
#ifndef EFFECTS_CORE_INCLUDED
#define EFFECTS_CORE_INCLUDED


#include "./ShaderCore.hlsl"

// #define APPDATA struct appdata   \
// {  \
//     float4 vertex : POSITION; \
//     half2 uv : TEXCOORD0; \
//     half4 color : COLOR; \
//     \
//     UNITY_VERTEX_INPUT_INSTANCE_ID \
// };

struct effect_appdata   
{  
    float4 vertex : POSITION; 
    half2 uv : TEXCOORD0; 
    half4 color : COLOR; 
    
    UNITY_VERTEX_INPUT_INSTANCE_ID 
};

struct effect_v2f 
{ 
    half2 uv : TEXCOORD0; 
    float4 vertex : SV_POSITION; 
    half4 color : TEXCOORD1; 
    
    UNITY_VERTEX_INPUT_INSTANCE_ID 
};

inline effect_v2f effect_vert (effect_appdata v, half4 mainTex_ST) 
{ 
    effect_v2f o; 

    UNITY_SETUP_INSTANCE_ID(v); 
    UNITY_TRANSFER_INSTANCE_ID(v, o); 

    o.vertex = ObjectToClipPosition(v.vertex); 
    o.uv = v.uv.xy * mainTex_ST.xy + mainTex_ST.zw;  
    o.color = v.color; 
    return o; 
}

inline half4 effect_frag (effect_v2f i, sampler2D mainTex
    #ifdef _ENABLE_TINT
    , half4 tint
    #endif
    ) 
{
    UNITY_SETUP_INSTANCE_ID(i);
    return tex2D(mainTex, i.uv) 
    #ifdef _ENABLE_TINT
    * tint
    #endif
    #ifdef _ENABLE_VERTEX_COLOR
    * i.color;
    #endif
    ;
}

// #define V2F struct effect_v2f 
// { \
//     half2 uv : TEXCOORD0; \
//     float4 vertex : SV_POSITION; \
//     half4 color : TEXCOORD1; \
//     \
//     UNITY_VERTEX_INPUT_INSTANCE_ID \
// };

// #define VERT_FUNC v2f vert (stdAppdata v) \
// { \
//     v2f o; \
// \
//     UNITY_SETUP_INSTANCE_ID(v); \
//     UNITY_TRANSFER_INSTANCE_ID(v, o); \
// \
//     o.vertex = ObjectToClipPosition(v.vertex); \
//     o.uv = TRANSFORM_TEX(v.uv, _MainTex); \
//     o.color = v.color; \
//     return o; \
// }

#endif