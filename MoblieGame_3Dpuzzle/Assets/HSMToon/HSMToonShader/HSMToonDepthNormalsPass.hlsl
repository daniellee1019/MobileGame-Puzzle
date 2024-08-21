#ifndef HSM_TOON_DEPTH_ONLY_PASS_INCLUDED
#define HSM_TOON_DEPTH_ONLY_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS     : POSITION;
    float4 tangentOS      : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float3 normal       : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD1;
    float3 normalWS                 : TEXCOORD2;
    float4 screenPosition : TEXCOORD3;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings DepthNormalsVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.uv         = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
    output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
    output.screenPosition = ComputeScreenPos(output.positionCS);

    return output;
}

float4 DepthNormalsFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    if (_DitherFade > 0)
    {
        float2 screenUV = input.screenPosition.xy / input.screenPosition.w;
        float2 ditherPos = _ScreenParams.xy * screenUV;
        DitherFade(ditherPos, 1 - _DitherFade);
    }

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
    return float4(PackNormalOctRectEncode(TransformWorldToViewDir(input.normalWS, true)), 0.0, 0.0);
}
#endif
