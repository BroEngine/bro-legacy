
#ifndef SHADER_CORE_INCLUDED
#define SHADER_CORE_INCLUDED

// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

inline float4 ObjectToClipPosition(float4 pos)
{
    return mul(UNITY_MATRIX_MVP, float4(pos.xyz, 1.0) );
}

#endif