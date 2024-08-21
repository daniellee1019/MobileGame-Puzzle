Shader "HSM/Toon"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        _BaseSaturation("BaseSaturation", Range(0, 5)) = 1
        _BaseBright("BaseBright", Range(-1, 1)) = 0

        _BumpScale("Scale", Range(0, 3)) = 1
        _BumpMap("Normal Map", 2D) = "bump" {}

        _Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.5
        _ClippingMaskMap("ClippingMask", 2D) = "white" {}
        [Enum(OFF,0,ON,1)] _InverseClippingMask("Inverse Clipping Mask", int) = 0
        
        _AOMap("AOMap", 2D) = "white" {}
        _ShadowMap("ShadowMap", 2D) = "white" {}
        _ShadowColor("ShadowColor", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowStep("ShadowStep", Range(-1, 1)) = 0
        _ShadowSmooth("ShadowSmooth", Range(0.0001, 1)) = 0.0001
        _SSAOStep("SSAOStep", Range(-1, 1)) = 0
        _SSAOSmooth("SSAOSmooth", Range(0.0001, 1)) = 0.0001

        [Toggle] _MatCap("MatCap", int) = 0
        _MatCapTextureMap("MatCapTextureMap", 2D) = "white" {}
        _MatCapColor("MatCapColor", Color) = (1,1,1,1)
        _MatCapBlur("MatCapBlur", Range(0, 10)) = 0
        [Enum(Additive,0,Multiply,1)] _MatCapBlendMode("MatCapBlendMode", int) = 1
        _MatCapIntensity("MatCapIntensity", Range(0, 1)) = 0

        [Toggle] _Specular("Specular", int) = 0
        _SpecularMap("SpecularMap", 2D) = "white" {}
        _SpecularColor("SpecularColor", Color) = (1,1,1,1)
        _SpecularIntensity("SpecularIntensity", Range(0, 20)) = 0
        _SpecularStep("SpecularStep", Range(-1, 1)) = 0
        _SpecularSmooth("SpecularSmooth", Range(0.0001, 1)) = 0.0001
        [Enum(OFF,0,ON,1)] _UseMetallicSpecular("UseMetallicSpecular", int) = 0

        [Toggle] _RimLight("RimLight", int) = 0
        _RimLightColor("RimLightColor", Color) = (1,1,1,1)
        _RimLightIntensity("RimLightIntensity", Range(0, 20)) = 0
        _RimLightStep("RimLightStep", Range(-1, 1)) = 0
        _RimLightSmooth("RimLightSmooth", Range(0.0001, 1)) = 0.0001
        [Enum(OFF,0,ON,1)] _RimLightWithLambert("RimLightWithLambert", int) = 0
        [Enum(OFF,0,ON,1)] _UseMetallicRimLight("UseMetallicRimLight", int) = 0

        [Toggle] _Emission("Emission", int) = 0
        _EmissionMap("EmissionMap", 2D) = "white" {}
        _EmissionColor("EmissionColor", Color) = (1,1,1,1)
        _EmissionIntensity("EmissionIntensity", Range(0, 20)) = 0
        [Enum(OFF,0,ON,1)] _EmissionFlashing("EmissionFlashing", int) = 0
        _EmissionFlashingInterval("EmissionFlashingInterval", Range(0, 10)) = 0
        _EmissionFlashingMin("EmissionFlashingMin", Range(0, 1)) = 0

        _DissolveMap("DissolveMap", 2D) = "white" {}
        _DissolveFade("DissolveFade", Range(0, 1)) = 0
        _DissolveStep("DissolveStep", Range(0, 1)) = 0
        _DissolveSmooth("DissolveSmooth", Range(0.0001, 1)) = 0.01
        _DissolveColor("DissolveColor", Color) = (1,1,1,1)
        _DissolveIntensity("DissolveIntensity", Range(0, 20)) = 1

        _DitherFade("DitherFade", Range(0, 1)) = 0

         [Enum(OFF,0,ON,1)] _OutLine("OutLine", int) = 1
        _OutLineWidth("OutLineWidth", Range(0, 1)) = 0.03
        _OutLineColor("OutLineColor", Color) = (0,0,0,1)

        _ZWriteMode("ZWrite Mode", int) = 1
        [Enum(OFF,0,FRONT,1,BACK,2)] _CullMode("Cull Mode", int) = 2
        [Enum(Alpha,0,Premultiply,1,Additive,2,Multiply,3)] _BlendMode("_BlendMode", int) = 0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector][Toggle] _AlphaClip("__clip", int) = 0
        
        _StencilComp("Stencil Comparison", Float) = 8
        _StencilNo("Stencil No", Float) = 1
        _StencilOpPass("Stencil Operation Pass", Float) = 0
        _StencilOpFail("Stencil Operation Fail", Float) = 0

        [HideInInspector][Enum(Off,0,On,1)] _AutoQueue("AutoQueue", int) = 1
        [HideInInspector] _RenderQueueValue("Render QueueValue", int) = 2000
        [HideInInspector][Enum(OPAQUE,0,TRANSPARENT,1)] _RenderQueueMode("RenderQueueMode", int) = 0
        [HideInInspector][Enum(Off,0,StencilMask,1,StencilOut,2)] _StencilMode("Stencil Mode", int) = 0
        [HideInInspector][Toggle] _GPUInstancing("GPUInstancing", int) = 0
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite[_ZWriteMode]
            Cull[_CullMode]
            Blend[_SrcBlend][_DstBlend]
            Stencil {

                Ref[_StencilNo]

                Comp[_StencilComp]
                Pass[_StencilOpPass]
                Fail[_StencilOpFail]
            }

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // Material Keywords
            #pragma shader_feature _ALPHACLIP_ON
            #pragma shader_feature _MATCAP_ON
            #pragma shader_feature _SPECULAR_ON
            #pragma shader_feature _RIMLIGHT_ON
            #pragma shader_feature _EMISSION_ON

            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex vert
            #pragma fragment frag

            #include "HSMToonInputData.hlsl"
            #include "HSMToonForward.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull[_CullMode]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ALPHACLIP_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "HSMToonInputData.hlsl"
            #include "HSMToonShadowCasterPass.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_CullMode]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature _ALPHACLIP_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "HSMToonInputData.hlsl"
            #include "HSMToonDepthOnlyPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_CullMode]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ALPHACLIP_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "HSMToonInputData.hlsl"
            #include "HSMToonDepthNormalsPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "OutLine"
            Tags { "LightMode" = "OutLine"
            "RenderType" = "Opaque"
            "Queue" = "Geometry + 50"}

            Blend One Zero
            Cull Front
            ZWrite On
            Stencil
            {
                Ref[_StencilNo]
                Comp[_StencilComp]
                Pass[_StencilOpPass]
                Fail[_StencilOpFail]
            }

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _DITHER_FADE

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ALPHACLIP_ON

            //V.2.0.4
            #pragma multi_compile _IS_OUTLINE_CLIPPING_NO _IS_OUTLINE_CLIPPING_YES
            #pragma multi_compile _OUTLINE_NML _OUTLINE_POS

            #include "HSMToonInputData.hlsl"            
            #include "HSMToonOutLine.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "HSMToonGUIEditor"
}
