#ifndef HSM_TOON_SHADOW_CASTER_PASS_INCLUDED
#define HSM_TOON_SHADOW_CASTER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

float3 _LightDirection;

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;
};

float4 GetShadowPositionHClip(Attributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif

    return positionCS;
}

Varyings ShadowPassVertex(Attributes input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.positionCS = GetShadowPositionHClip(input);
    return output;
}

half4 ShadowPassFragment(Varyings input) : SV_TARGET
{
    if (_DissolveFade > 0)
    {
        half4 dissolveMap = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, input.uv);
        half dissolveFade = (dissolveMap.r + dissolveMap.g + dissolveMap.b) / 3;
        clip(dissolveFade - _DissolveFade);
    }

#ifdef _ALPHACLIP_ON
    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    half4 alphaClipMap = SAMPLE_TEXTURE2D(_ClippingMaskMap, sampler_ClippingMaskMap, input.uv);
    half alphaClip = (alphaClipMap.r + alphaClipMap.g + alphaClipMap.b) / 3;
    alphaClip *= baseMap.a;
    switch (_InverseClippingMask)
    {
    case 1:
        alphaClip = 1 - alphaClip;
        break;
    }
    clip(alphaClip - _Cutoff);
#endif
    return 0;
}

#endif
