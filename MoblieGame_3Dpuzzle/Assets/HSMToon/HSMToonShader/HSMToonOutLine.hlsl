#ifndef HSM_TOON_OUTLINE
#define HSM_TOON_OUTLINE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct VertexInput
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    float3 normal : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
    float4 positionCS : POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float4 screenPosition : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutput OutlineVertex(VertexInput input)
{
    VertexOutput output = (VertexOutput)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.positionWS = TransformObjectToWorld(input.vertex.xyz);
    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.positionCS = TransformObjectToHClip(input.vertex.xyz + input.normal * _OutLineWidth);
    output.screenPosition = ComputeScreenPos(output.positionCS);

    return output;
}

half4 OutlineFragment(VertexOutput input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
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

    half3 outColor = _OutLineColor.rgb;
    return half4(outColor, 1);
}
#endif