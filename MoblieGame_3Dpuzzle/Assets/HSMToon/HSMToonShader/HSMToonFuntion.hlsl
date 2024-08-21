#ifndef HSM_TOON_INCLUDED
#define HSM_TOON_INCLUDED

Light GetMainLight(Varyings src)
{
    float4 shadowCoord = 0;
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    shadowCoord = src.shadowcoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    shadowCoord = TransformWorldToShadowCoord(src.positionWS);
#else
    shadowCoord = float4(0, 0, 0, 0);
#endif

    float4 shadowMask = 0;
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    shadowMask = SAMPLE_SHADOWMASK(src.lightmapUV);
#elif !defined (LIGHTMAP_ON)
    shadowMask = unity_ProbesOcclusion;
#else
    shadowMask = half4(1, 1, 1, 1);
#endif

    return GetMainLight(shadowCoord, src.positionWS, shadowMask);
}

half3 Saturation(half3 src, half saturation)
{
    float luma = dot(src, float3(0.2126729, 0.7151522, 0.0721750));
    return luma.xxx + saturation.xxx * (src - luma.xxx);
}

half3 Bright(half3 src, half bright)
{
    return saturate(src + bright);
}

half3 SampleNoramlWS(half4 bumpMap, half bumpScale, half4 tangentWS, half3 normalWS)
{
    half3 normalTS = UnpackNormalScale(bumpMap, bumpScale);
    float sgn = tangentWS.w;
    float3 bitangent = sgn * cross(normalWS.xyz, tangentWS.xyz);
    return normalize(TransformTangentToWorld(normalTS, half3x3(tangentWS.xyz, bitangent.xyz, normalWS.xyz)));
}

float2 RotateUV(float2 uv, float radian, float2 piv, float time)
{
    float rotateUV_ang = radian;
    float rotateUV_cos = cos(time * rotateUV_ang);
    float rotateUV_sin = sin(time * rotateUV_ang);
    return (mul(uv - piv, float2x2(rotateUV_cos, -rotateUV_sin, rotateUV_sin, rotateUV_cos)) + piv);
}

half3 MatCap(half mirrorFlag, half3 normalWS, half3 outColor, half2 uv)
{
    float sign_Mirror = mirrorFlag;

    float3 viewNormal = mul(UNITY_MATRIX_V, normalWS);
    float2 viewNormalAsMatCapUV = viewNormal.rg * 0.5 + 0.5;

    float2 rot_MatCapUV_var =
        RotateUV((0.0 + ((viewNormalAsMatCapUV) * (1.0 - 0.0)) / ((1.0) - (0.0))),
            0,
            float2(0.5, 0.5),
            1.0);

    if (sign_Mirror < 0) {
        rot_MatCapUV_var.x = 1 - rot_MatCapUV_var.x;
    }
    else {
        rot_MatCapUV_var = rot_MatCapUV_var;
    }

    half3 outMatCapColor = outColor.rgb;

    switch (_MatCapBlendMode)
    {
    case 0:
        outMatCapColor += SAMPLE_TEXTURE2D_LOD(_MatCapTextureMap, sampler_MatCapTextureMap, half2(rot_MatCapUV_var.x, rot_MatCapUV_var.y), _MatCapBlur).rgb * _MatCapColor.rgb;
        break;
    case 1:
        outMatCapColor *= SAMPLE_TEXTURE2D_LOD(_MatCapTextureMap, sampler_MatCapTextureMap, half2(rot_MatCapUV_var.x, rot_MatCapUV_var.y), _MatCapBlur).rgb * _MatCapColor.rgb;
        break;
    }

    outMatCapColor = lerp(outColor.rgb, outMatCapColor, saturate(_MatCapIntensity));

    return outMatCapColor;
}

#endif