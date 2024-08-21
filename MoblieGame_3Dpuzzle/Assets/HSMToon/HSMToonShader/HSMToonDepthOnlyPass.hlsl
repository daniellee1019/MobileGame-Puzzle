#ifndef HSM_TOON_DEPTH_ONLY_PASS_INCLUDED
#define HSM_TOON_DEPTH_ONLY_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 position     : POSITION;
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings DepthOnlyVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.positionCS = TransformObjectToHClip(input.position.xyz);
    return output;
}

half4 DepthOnlyFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

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
