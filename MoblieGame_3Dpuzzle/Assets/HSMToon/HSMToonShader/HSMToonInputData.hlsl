#ifndef HSM_TOON_INPUT_DATA
#define HSM_TOON_INPUT_DATA

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)

half _Cutoff;
half _AlphaClip;
half4 _ClippingMaskMap_ST;
half _InverseClippingMask;

half4 _BaseMap_ST;
half4 _BaseColor;
half _BaseSaturation;
half _BaseBright;

half4 _BumpMap_ST;
half _BumpScale;

half4 _AOMap_ST;
half4 _ShadowMap_ST;
half4 _ShadowColor;
half _ShadowStep;
half _ShadowSmooth;
half _SSAOStep;
half _SSAOSmooth;

half4 _SpecularMap_ST;
half4 _SpecularColor;
half _SpecularIntensity;
half _SpecularStep;
half _SpecularSmooth;
half _UseMetallicSpecular;

half4 _RimLightColor;
half _RimLightIntensity;
half _RimLightStep;
half _RimLightSmooth;
half _RimLightWithLambert;
half _UseMetallicRimLight;

half4 _EmissionMap_ST;
half4 _EmissionColor;
half _EmissionIntensity;
half _EmissionFlashing;
half _EmissionFlashingInterval;
half _EmissionFlashingMin;

half4 _DissolveMap_ST;
half _DissolveFade;
half _DissolveStep;
half _DissolveSmooth;
half4 _DissolveColor;
half _DissolveIntensity;

half _DitherFade;

half _OutLineWidth;
half4 _OutLineColor;

half4 _MatCapTextureMap_ST;
half4 _MatCapColor;
half _MatCapBlur;
half _MatCapBlendMode;
half _MatCapIntensity;

CBUFFER_END

TEXTURE2D(_ClippingMaskMap);	SAMPLER(sampler_ClippingMaskMap);
TEXTURE2D(_ShadowMap);			SAMPLER(sampler_ShadowMap);
TEXTURE2D(_DissolveMap);		SAMPLER(sampler_DissolveMap);
TEXTURE2D(_MatCapTextureMap);	SAMPLER(sampler_MatCapTextureMap);
TEXTURE2D(_SpecularMap);	    SAMPLER(sampler_SpecularMap);
TEXTURE2D(_AOMap);	            SAMPLER(sampler_AOMap);

void DitherFade_float(float2 vpos, float fade, out float ditherClip)
{
    static const uint	masks[17 + 1] = {
        0xFFFF, 0xFEFF, 0xFEFB, 0xFAFB, 0xFAFA, 0xDAFA, 0xDA7A, 0x5A7A,
        0x5A5A, 0x525A, 0x5258, 0x5058, 0x5050, 0x4050, 0x4010, 0x0010, 0x0000, 0x0000
    };

    uint2	ipos = floor(frac(vpos / 4) * 4);
    uint	bit = ipos.x * 4 + ipos.y;
    uint	index = floor(fade * 17);
    uint	mask = masks[index];
    ditherClip = -(float)((mask >> bit) & 1);
}

void DitherFade(float2 vpos, float fade)
{
    float	ditherClip;
    DitherFade_float(vpos, fade, ditherClip);
    clip(0.5 + ditherClip);
}

#endif