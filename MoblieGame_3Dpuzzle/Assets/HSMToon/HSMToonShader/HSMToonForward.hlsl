#ifndef HSM_TOON_FORWARD
#define HSM_TOON_FORWARD

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float4 color        : COLOR;
    float2 texcoord     : TEXCOORD0;
    float2 lightmapUV   : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv                       : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
    float fogcoord : TEXCOORD2;
    float4 shadowcoord              : TEXCOORD3;
    float2 uv2                      : TEXCOORD4;
    float3 positionWS               : TEXCOORD5;
    float4 positionCS               : SV_POSITION;
    float3 normalWS                 : NORMAL;
    float4 tangentWS                : TEXCOORD6;    // xyz: tangent, w: sign
    float mirrorFlag                : TEXCOORD7;
    float4 screenPosition           : TEXCOORD8;
    float4 color                    : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

#include "HSMToonFuntion.hlsl"

Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.normalWS = normalInput.normalWS;
    output.positionCS = vertexInput.positionCS;
    output.positionWS = vertexInput.positionWS;

    real sign = input.tangentOS.w * GetOddNegativeScale();
    output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
    float3 crossFwd = cross(UNITY_MATRIX_V[0].xyz, UNITY_MATRIX_V[1].xyz);
    output.mirrorFlag = dot(crossFwd, UNITY_MATRIX_V[2].xyz) < 0 ? 1 : -1;
    output.screenPosition = ComputeScreenPos(output.positionCS);

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    output.fogcoord = ComputeFogFactor(output.positionCS.z);
    
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowcoord = GetShadowCoord(vertexInput);
#endif

    return output;
}

half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    baseMap.rgb = Saturation(baseMap.rgb, _BaseSaturation);
    baseMap.rgb = Bright(baseMap.rgb, _BaseBright);
    half3 outColor = baseMap.rgb * _BaseColor.rgb;
#ifdef _MATCAP_ON
    outColor.rgb = MatCap(input.mirrorFlag, input.normalWS, outColor.rgb, input.uv);
#endif

    half outAlpha = baseMap.a * _BaseColor.a;

    input.normalWS = SampleNoramlWS(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv), _BumpScale, input.tangentWS, input.normalWS);

    Light mainLight = GetMainLight(input);
    outColor *= mainLight.color;

    half nDotML = dot(input.normalWS, mainLight.direction) * mainLight.distanceAttenuation;
    half4 aoMap = SAMPLE_TEXTURE2D(_AOMap, sampler_AOMap, input.uv);
    half aoMask = (aoMap.r + aoMap.g + aoMap.b) / 3;
    half lambert = smoothstep(_ShadowStep, _ShadowStep + _ShadowSmooth, nDotML)
        * smoothstep(_ShadowStep, _ShadowStep + _ShadowSmooth, mainLight.shadowAttenuation)
        * smoothstep(_ShadowStep, _ShadowStep + _ShadowSmooth, aoMask);
    half4 shadowMap = SAMPLE_TEXTURE2D(_ShadowMap, sampler_ShadowMap, input.uv);
    outColor = lerp(outColor * shadowMap.rgb * _ShadowColor.rgb, outColor, lambert);//nDotML * mainLight.shadowAttenuation;

    half3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, input.normalWS);
#if defined(_SCREEN_SPACE_OCCLUSION)
    float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(normalizedScreenSpaceUV);
    half ao = smoothstep(_SSAOStep, _SSAOStep + _SSAOSmooth, aoFactor.directAmbientOcclusion);
    outColor = lerp(outColor * shadowMap.rgb * _ShadowColor.rgb, outColor, ao);
    bakedGI *= aoFactor.indirectAmbientOcclusion;
#endif
    MixRealtimeAndBakedGI(mainLight, input.normalWS, bakedGI);

    half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.positionWS);
    half3 nReflectionML = normalize(reflect(mainLight.direction, input.normalWS));
    half vDotref = saturate(dot(-viewDirection, nReflectionML)) * mainLight.distanceAttenuation;
    half nDotv = dot(input.normalWS, viewDirection);

#ifdef _SPECULAR_ON
    half4 specularMap = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, input.uv);
    half specularMask = (specularMap.r + specularMap.g + specularMap.b) / 3;
    half specularvDotref = smoothstep(_SpecularStep, _SpecularStep + _SpecularSmooth, vDotref);
    specularvDotref *= _SpecularIntensity;
    half3 specularColor = _SpecularColor.rgb * specularvDotref;
    switch (_UseMetallicSpecular)
    {
    case 0:
        outColor += specularColor * specularMask * mainLight.color;
        break;
    case 1:
        outColor += specularColor * specularMask * mainLight.color * baseMap.rgb * _BaseColor.rgb;
        break;
    }
#endif

#ifdef _RIMLIGHT_ON
    half rimLightnDotv = (1 - nDotv) * mainLight.distanceAttenuation;
    switch (_RimLightWithLambert)
    {
    case 1:
        rimLightnDotv *= nDotML;
        break;
    }
    rimLightnDotv = smoothstep(_RimLightStep, _RimLightStep + _RimLightSmooth, saturate(rimLightnDotv));
    rimLightnDotv *= _RimLightIntensity;
    half3 rimLightColor = _RimLightColor.rgb * rimLightnDotv;
    switch (_UseMetallicRimLight)
    {
    case 0:
        outColor += rimLightColor * mainLight.color;
        break;
    case 1:
        outColor += rimLightColor * mainLight.color * baseMap.rgb * _BaseColor.rgb;
        break;
    }
#endif

#ifdef _EMISSION_ON
        half4 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv);
        half3 emissionColor = emissionMap.rgb * _EmissionColor.rgb;
        switch(_EmissionFlashing)
        {
        case 1:
            half flashing = sin(_Time.y * _EmissionFlashingInterval);
            flashing = flashing * 0.5 + 0.5;
            flashing = saturate(flashing + _EmissionFlashingMin);
            emissionColor *= flashing;
            break;
        }
        emissionColor *= _EmissionIntensity;
        outColor += emissionColor;
#endif

#ifdef _ADDITIONAL_LIGHTS

    uint ipixelLightCount = GetAdditionalLightsCount();
    half3 addLightColor = half3(0, 0, 0);

    for (uint lightIndex = 0u; lightIndex < ipixelLightCount; ++lightIndex)
    {
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
        half4 shadowMask = SAMPLE_SHADOWMASK(src.lightmapUV);
#elif !defined (LIGHTMAP_ON)
        half4 shadowMask = unity_ProbesOcclusion;
#else
        half4 shadowMask = half4(1, 1, 1, 1);
#endif
        Light light = GetAdditionalLight(lightIndex, input.positionWS, shadowMask);

        half3 lightColor = light.color * light.distanceAttenuation;
        half nDotL = dot(input.normalWS, light.direction);
        half addLightLambert = smoothstep(_ShadowStep, _ShadowStep + _ShadowSmooth, nDotL)
            * smoothstep(_ShadowStep, _ShadowStep + _ShadowSmooth, light.shadowAttenuation)
            * smoothstep(_ShadowStep, _ShadowStep + _ShadowSmooth, aoMask);
        half3 addLightBaseColor = baseMap.rgb * _BaseColor.rgb * lightColor;
        half3 outAddLightColor = addLightBaseColor * addLightLambert;

#if defined(_SCREEN_SPACE_OCCLUSION)
        half ao = smoothstep(_SSAOStep, _SSAOStep + _SSAOSmooth, aoFactor.directAmbientOcclusion);
        outAddLightColor = lerp(outAddLightColor * shadowMap.rgb * _ShadowColor.rgb, outAddLightColor, ao);
#endif
#ifdef _MATCAP_ON
        outAddLightColor.rgb = MatCap(input.mirrorFlag, input.normalWS, addLightBaseColor.rgb, input.uv) * lightColor;
#endif

#ifdef _SPECULAR_ON
        half3 addnReflectionL = normalize(reflect(light.direction, input.normalWS));
        half addvDotref = saturate(dot(-viewDirection, addnReflectionL));
        half addSpecularvDotref = smoothstep(_SpecularStep, _SpecularStep + _SpecularSmooth, addvDotref);
        addSpecularvDotref *= _SpecularIntensity;
        half3 addSpecularColor = _SpecularColor.rgb * addSpecularvDotref;
        switch (_UseMetallicSpecular)
        {
        case 0:
            outAddLightColor += addSpecularColor * specularMask * lightColor;
            break;
        case 1:
            outAddLightColor += addSpecularColor * specularMask * lightColor * baseMap.rgb * _BaseColor.rgb;
            break;
        }
#endif
#ifdef _RIMLIGHT_ON
        half addRimLightnDotv = (1 - nDotv);
        switch (_RimLightWithLambert)
        {
        case 1:
            addRimLightnDotv *= nDotL;
            break;
        }
        addRimLightnDotv = smoothstep(_RimLightStep, _RimLightStep + _RimLightSmooth, saturate(addRimLightnDotv));
        addRimLightnDotv *= _RimLightIntensity;
        half3 rimLightColor = _RimLightColor.rgb * addRimLightnDotv;

        switch (_UseMetallicRimLight)
        {
        case 0:
            outAddLightColor += rimLightColor * lightColor;
            break;
        case 1:
            outAddLightColor += rimLightColor * lightColor * baseMap.rgb * _BaseColor.rgb;
            break;
        }
#endif
        addLightColor += outAddLightColor;
    }

    outColor.rgb += addLightColor;
#endif
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
        half3 dissolveColor = lerp(_DissolveColor.rgb, 0, smoothstep(_DissolveStep + _DissolveFade, _DissolveStep + _DissolveFade + _DissolveSmooth, dissolveFade));
        dissolveColor *= _DissolveIntensity;
        outColor += dissolveColor;
    }

#ifdef _ALPHACLIP_ON
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
    return half4(outColor, outAlpha);
}

#endif