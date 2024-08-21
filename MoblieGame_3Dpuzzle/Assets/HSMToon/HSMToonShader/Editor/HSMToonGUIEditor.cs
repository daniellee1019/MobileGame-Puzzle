using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;

public class HSMToonGUIEditor : ShaderGUI
{
    private MaterialProperty cullModeProp;
    private MaterialProperty autoQueueProp;
    private MaterialProperty renderQueueValueProp;
    private MaterialProperty renderQueueModeProp;
    private MaterialProperty blendModeProp;

    private MaterialProperty stencilModeProp;
    private MaterialProperty stencilNoProp;

    private MaterialProperty alphaClipProp;
    private MaterialProperty alphaClipMapProp;
    private MaterialProperty cutOffProp;
    private MaterialProperty inverseClippingMaskProp;

    private MaterialProperty gpuInstancingProp;

    private MaterialProperty baseMapProp;
    private MaterialProperty baseColorProp;
    private MaterialProperty baseSaturationProp;
    private MaterialProperty baseBrightProp;
    private MaterialProperty basebumpMapProp;
    private MaterialProperty basebumpScaleProp;

    private MaterialProperty aoMapProp;
    private MaterialProperty shadowMapProp;
    private MaterialProperty shadowColorProp;
    private MaterialProperty shadowStepProp;
    private MaterialProperty shadowSmoothProp;
    private MaterialProperty ssaoStepProp;
    private MaterialProperty ssaoSmoothProp;

    private MaterialProperty matCapProp;
    private MaterialProperty matCapTextureMapProp;
    private MaterialProperty matCapColorProp;
    private MaterialProperty matCapBlurProp;
    private MaterialProperty matCapBlendModeProp;
    private MaterialProperty matCapIntensityProp;

    private MaterialProperty specularProp;
    private MaterialProperty specularMapProp;
    private MaterialProperty specularColorProp;
    private MaterialProperty specularIntensityProp;
    private MaterialProperty specularStepProp;
    private MaterialProperty specularSmoothProp;
    private MaterialProperty useMetallicSpecularProp;

    private MaterialProperty rimLightProp;
    private MaterialProperty rimLightColorProp;
    private MaterialProperty rimLightIntensityProp;
    private MaterialProperty rimLightStepProp;
    private MaterialProperty rimLightSmoothProp;
    private MaterialProperty rimLightWithLambertProp;
    private MaterialProperty useMetallicRimLightProp;

    private MaterialProperty emissionProp;
    private MaterialProperty emissionMapProp;
    private MaterialProperty emissionColorProp;
    private MaterialProperty emissionIntensityProp;
    private MaterialProperty emissionFlashingProp;
    private MaterialProperty emissionFlashingIntervalProp;
    private MaterialProperty emissionFlashingMinProp;

    private MaterialProperty dissolveMapProp;
    private MaterialProperty dissolveFadeProp;
    private MaterialProperty dissolveStepProp;
    private MaterialProperty dissolveSmoothProp;
    private MaterialProperty dissolveColorProp;
    private MaterialProperty dissolveIntensityProp;

    private MaterialProperty ditherFadeProp;

    private MaterialProperty outLineProp;
    private MaterialProperty outLineWidthProp;
    private MaterialProperty outLineColorProp;

    private static bool isRenderSetting = true;
    private static bool isBaseSetting = true;
    private static bool isShadeSetting = true;
    private static bool isMatCapSetting = true;
    private static bool isSpecularSetting = true;
    private static bool isRimLightSetting = true;
    private static bool isEmissionSetting = true;
    private static bool isDissolveSetting = true;
    private static bool isDitherSetting = true;
    private static bool isOutLineSetting = true;
    private bool onDissolve = false;
    private int originCullmode = -1;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        //base.OnGUI(materialEditor, properties);
        Material material = materialEditor.target as Material;

        PropertiesInitialize(properties);

        isRenderSetting = EditorGUILayout.Foldout(isRenderSetting, "Render Setting");
        if(isRenderSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            GUILayout.Label("Render Options", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(cullModeProp, new GUIContent("CullMode"));
            materialEditor.ShaderProperty(autoQueueProp, new GUIContent("AutoQueue"));
            RenderModeSetting(materialEditor, material);
            if (material.GetInt("_RenderQueueMode") == 0 && material.renderQueue < 3000)
            {
                materialEditor.ShaderProperty(alphaClipProp, new GUIContent("AlphaClip"));
                EditorGUI.indentLevel++;
                AlphaClipingSetting(materialEditor, material);
                EditorGUI.indentLevel--;
            }
            else if (material.GetInt("_RenderQueueMode") == 1 || material.renderQueue >= 3000)
                material.SetInt("_AlphaClip", 0);
            EditorGUILayout.Space();

            GUILayout.Label("Stencil", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(stencilModeProp, new GUIContent("StencilMode"));
            StencilSetting(materialEditor, material);
            EditorGUILayout.Space();

            GUILayout.Label("GPU Instancing", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(gpuInstancingProp, new GUIContent("GPU Instancing"));
            if (material.GetInt("_GPUInstancing") == 1)
                material.enableInstancing = true;
            else
                material.enableInstancing = false;
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isBaseSetting = EditorGUILayout.Foldout(isBaseSetting, "Base Setting");
        if(isBaseSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            GUILayout.Label("Base Texture", EditorStyles.boldLabel);
            materialEditor.TexturePropertySingleLine(new GUIContent("TextureMap"), baseMapProp, baseColorProp);
            materialEditor.ShaderProperty(baseSaturationProp, new GUIContent("Saturation"));
            materialEditor.ShaderProperty(baseBrightProp, new GUIContent("Bright"));
            EditorGUILayout.Space();
            GUILayout.Label("Base Normal", EditorStyles.boldLabel);
            materialEditor.TexturePropertySingleLine(new GUIContent("NormalMap"), basebumpMapProp, basebumpScaleProp);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isShadeSetting = EditorGUILayout.Foldout(isShadeSetting, "Shade Setting");
        if(isShadeSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            GUILayout.Label("AO", EditorStyles.boldLabel);
            materialEditor.TexturePropertySingleLine(new GUIContent("AO Map"), aoMapProp);
            EditorGUILayout.Space();
            GUILayout.Label("Shadow", EditorStyles.boldLabel);
            materialEditor.TexturePropertySingleLine(new GUIContent("ShadowMap"), shadowMapProp, shadowColorProp);
            materialEditor.ShaderProperty(shadowStepProp, new GUIContent("Step"));
            materialEditor.ShaderProperty(shadowSmoothProp, new GUIContent("Smooth"));
            EditorGUILayout.Space();
            GUILayout.Label("SSAO", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(ssaoStepProp, new GUIContent("Step"));
            materialEditor.ShaderProperty(ssaoSmoothProp, new GUIContent("Smooth"));

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isMatCapSetting = EditorGUILayout.Foldout(isMatCapSetting, new GUIContent("MatCap Setting"));
        if (isMatCapSetting)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical("box");
            materialEditor.ShaderProperty(matCapProp, new GUIContent("Use MatCap"));
            switch(material.GetInt("_MatCap"))
            {
                case 0:
                    break;
                case 1:
                    materialEditor.TexturePropertySingleLine(new GUIContent("MatCapTexture"), matCapTextureMapProp, matCapColorProp);
                    materialEditor.ShaderProperty(matCapIntensityProp, new GUIContent("Intensity"));
                    materialEditor.ShaderProperty(matCapBlurProp, new GUIContent("Blur"));
                    materialEditor.ShaderProperty(matCapBlendModeProp, new GUIContent("BlendMode"));
                    break;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isSpecularSetting = EditorGUILayout.Foldout(isSpecularSetting, "Specular Setting");
        if (isSpecularSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(specularProp, new GUIContent("Use Specular"));
            switch(material.GetInt("_Specular"))
            {
                case 0:
                    break;
                case 1:
                    materialEditor.TexturePropertySingleLine(new GUIContent("Specular Map"), specularMapProp, specularColorProp);
                    materialEditor.ShaderProperty(specularIntensityProp, new GUIContent("Intensity"));
                    materialEditor.ShaderProperty(specularStepProp, new GUIContent("Step"));
                    materialEditor.ShaderProperty(specularSmoothProp, new GUIContent("Smooth"));
                    materialEditor.ShaderProperty(useMetallicSpecularProp, new GUIContent("Use Metallic"));
                    break;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isRimLightSetting = EditorGUILayout.Foldout(isRimLightSetting, "RimLight Setting");
        if(isRimLightSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(rimLightProp, new GUIContent("Use RimLight"));
            switch(material.GetInt("_RimLight"))
            {
                case 0:
                    break;
                case 1:
                    materialEditor.ShaderProperty(rimLightColorProp, new GUIContent("Color"));
                    materialEditor.ShaderProperty(rimLightIntensityProp, new GUIContent("Intensity"));
                    materialEditor.ShaderProperty(rimLightStepProp, new GUIContent("Step"));
                    materialEditor.ShaderProperty(rimLightSmoothProp, new GUIContent("Smooth"));
                    materialEditor.ShaderProperty(rimLightWithLambertProp, new GUIContent("With Lambert"));
                    materialEditor.ShaderProperty(useMetallicRimLightProp, new GUIContent("Use Metallic"));
                    break;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isEmissionSetting = EditorGUILayout.Foldout(isEmissionSetting, "Emission Setting");
        if(isEmissionSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(emissionProp, new GUIContent(" Use Emission"));
            switch(material.GetInt("_Emission"))
            {
                case 0:
                    break;
                case 1:
                    materialEditor.TexturePropertySingleLine(new GUIContent("EmissionMap"), emissionMapProp, emissionColorProp);
                    materialEditor.ShaderProperty(emissionIntensityProp, new GUIContent("Intensity"));
                    materialEditor.ShaderProperty(emissionFlashingProp, new GUIContent("Use Flashing"));
                    switch(material.GetInt("_EmissionFlashing"))
                    {
                        case 0:
                            break;
                        case 1:
                            materialEditor.ShaderProperty(emissionFlashingIntervalProp, new GUIContent("FlashingInterval"));
                            materialEditor.ShaderProperty(emissionFlashingMinProp, new GUIContent("FlashingMin"));
                            break;
                    }
                    break;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isDissolveSetting = EditorGUILayout.Foldout(isDissolveSetting, "Dissolve Setting");
        if(isDissolveSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            materialEditor.TexturePropertySingleLine(new GUIContent("DissolveMap"), dissolveMapProp, dissolveColorProp);
            materialEditor.ShaderProperty(dissolveFadeProp, new GUIContent("Fade"));
            materialEditor.ShaderProperty(dissolveStepProp, new GUIContent("Step"));
            materialEditor.ShaderProperty(dissolveSmoothProp, new GUIContent("Smooth"));
            materialEditor.ShaderProperty(dissolveIntensityProp, new GUIContent("Intensity"));

            if (material.GetFloat("_DissolveFade") == 0)
            {
                if (originCullmode != material.GetInt("_CullMode") && onDissolve == false)
                    originCullmode = material.GetInt("_CullMode");
                else
                {
                    material.SetInt("_CullMode", originCullmode);
                    onDissolve = false;
                }
            }
            else if (material.GetFloat("_DissolveFade") > 0)
            {
                material.SetInt("_CullMode", 0);
                onDissolve = true;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isDitherSetting = EditorGUILayout.Foldout(isDitherSetting, "Dither Setting");
        if(isDitherSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(ditherFadeProp, new GUIContent("Fade"));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        isOutLineSetting = EditorGUILayout.Foldout(isOutLineSetting, "OutLine Setting");
        if(isOutLineSetting)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            materialEditor.ShaderProperty(outLineProp, new GUIContent("Use OutLine"));
            switch(material.GetInt("_OutLine"))
            {
                case 0:
                    material.SetShaderPassEnabled("OutLine", false);
                    break;
                case 1:
                    material.SetShaderPassEnabled("OutLine", true);
                    materialEditor.ShaderProperty(outLineColorProp, new GUIContent("Color"));
                    materialEditor.ShaderProperty(outLineWidthProp, new GUIContent("Width"));
                    break;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }

    private void PropertiesInitialize(MaterialProperty[] properties)
    {
        cullModeProp = FindProperty("_CullMode", properties);
        autoQueueProp = FindProperty("_AutoQueue", properties);
        renderQueueValueProp = FindProperty("_RenderQueueValue", properties);
        renderQueueModeProp = FindProperty("_RenderQueueMode", properties);
        blendModeProp = FindProperty("_BlendMode", properties);

        stencilModeProp = FindProperty("_StencilMode", properties);
        stencilNoProp = FindProperty("_StencilNo", properties);

        alphaClipProp = FindProperty("_AlphaClip", properties);
        alphaClipMapProp = FindProperty("_ClippingMaskMap", properties);
        inverseClippingMaskProp = FindProperty("_InverseClippingMask", properties);
        cutOffProp = FindProperty("_Cutoff", properties);

        gpuInstancingProp = FindProperty("_GPUInstancing", properties);

        baseMapProp = FindProperty("_BaseMap", properties);
        baseColorProp = FindProperty("_BaseColor", properties);
        baseSaturationProp = FindProperty("_BaseSaturation", properties);
        baseBrightProp = FindProperty("_BaseBright", properties);
        basebumpMapProp = FindProperty("_BumpMap", properties);
        basebumpScaleProp = FindProperty("_BumpScale", properties);

        aoMapProp = FindProperty("_AOMap", properties);
        shadowMapProp = FindProperty("_ShadowMap", properties);
        shadowColorProp = FindProperty("_ShadowColor", properties);
        shadowStepProp = FindProperty("_ShadowStep", properties);
        shadowSmoothProp = FindProperty("_ShadowSmooth", properties);
        ssaoStepProp = FindProperty("_SSAOStep", properties);
        ssaoSmoothProp = FindProperty("_SSAOSmooth", properties);

        matCapProp = FindProperty("_MatCap", properties);
        matCapTextureMapProp = FindProperty("_MatCapTextureMap", properties);
        matCapColorProp = FindProperty("_MatCapColor", properties);
        matCapBlurProp = FindProperty("_MatCapBlur", properties);
        matCapBlendModeProp = FindProperty("_MatCapBlendMode", properties);
        matCapIntensityProp = FindProperty("_MatCapIntensity", properties);

        specularProp = FindProperty("_Specular", properties);
        specularMapProp = FindProperty("_SpecularMap", properties);
        specularColorProp = FindProperty("_SpecularColor", properties);
        specularIntensityProp = FindProperty("_SpecularIntensity", properties);
        specularStepProp = FindProperty("_SpecularStep", properties);
        specularSmoothProp = FindProperty("_SpecularSmooth", properties);
        useMetallicSpecularProp = FindProperty("_UseMetallicSpecular", properties);

        rimLightProp = FindProperty("_RimLight", properties);
        rimLightColorProp = FindProperty("_RimLightColor", properties);
        rimLightIntensityProp = FindProperty("_RimLightIntensity", properties);
        rimLightStepProp = FindProperty("_RimLightStep", properties);
        rimLightSmoothProp = FindProperty("_RimLightSmooth", properties);
        rimLightWithLambertProp = FindProperty("_RimLightWithLambert", properties);
        useMetallicRimLightProp = FindProperty("_UseMetallicRimLight", properties);

        emissionProp = FindProperty("_Emission", properties);
        emissionMapProp = FindProperty("_EmissionMap", properties);
        emissionColorProp = FindProperty("_EmissionColor", properties);
        emissionIntensityProp = FindProperty("_EmissionIntensity", properties);
        emissionFlashingProp = FindProperty("_EmissionFlashing", properties);
        emissionFlashingIntervalProp = FindProperty("_EmissionFlashingInterval", properties);
        emissionFlashingMinProp = FindProperty("_EmissionFlashingMin", properties);

        dissolveMapProp = FindProperty("_DissolveMap", properties);
        dissolveFadeProp = FindProperty("_DissolveFade", properties);
        dissolveStepProp = FindProperty("_DissolveStep", properties);
        dissolveSmoothProp = FindProperty("_DissolveSmooth", properties);
        dissolveColorProp = FindProperty("_DissolveColor", properties);
        dissolveIntensityProp = FindProperty("_DissolveIntensity", properties);

        ditherFadeProp = FindProperty("_DitherFade", properties);

        outLineProp = FindProperty("_OutLine", properties);
        outLineWidthProp = FindProperty("_OutLineWidth", properties);
        outLineColorProp = FindProperty("_OutLineColor", properties);
    }

    private void RenderModeSetting(MaterialEditor materialEditor, Material material)
    {
        if (material.GetInt("_AutoQueue") == 0)
        {
            materialEditor.ShaderProperty(renderQueueValueProp, new GUIContent("Render Queue"));
            material.renderQueue = material.GetInt("_RenderQueueValue");
            if(material.renderQueue >= 3000)
            {
                BlendSetting(materialEditor, material);
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_ZWriteMode", 0);
                material.SetShaderPassEnabled("ShadowCaster", false);
            }
            else
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWriteMode", 1);
                material.SetShaderPassEnabled("ShadowCaster", true);
                material.SetOverrideTag("RenderType", "Opaque");
            }
        }
        else
        {
            materialEditor.ShaderProperty(renderQueueModeProp, new GUIContent("Render Mode"));
            switch(material.GetInt("_RenderQueueMode"))
            {
                case 0:
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWriteMode", 1);
                    material.SetShaderPassEnabled("ShadowCaster", true);
                    material.SetOverrideTag("RenderType", "Opaque");
                    break;

                case 1:
                    BlendSetting(materialEditor, material);
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_ZWriteMode", 0);
                    material.renderQueue = (int)RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
            }
        }
    }

    private void BlendSetting(MaterialEditor materialEditor, Material material)
    {
        materialEditor.ShaderProperty(blendModeProp, new GUIContent("Blend Mode"));

        switch (material.GetInt("_BlendMode"))
        {
            case 0: //Alpha
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;

            case 1: //Premultiply
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;

            case 2: // Additive
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;

            case 3: //Multiply
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                break;
        }
    }
    private void StencilSetting(MaterialEditor materialEditor, Material material)
    {
        switch (material.GetInt("_StencilMode"))
        {
            case 0: //Off
                material.SetFloat("_StencilComp", 0);
                material.SetFloat("_StencilNo", 1);
                material.SetFloat("_StencilOpPass", 0);
                material.SetFloat("_StencilOpFail", 0);
                return;
            case 1: //StencilOut
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                materialEditor.ShaderProperty(stencilNoProp, new GUIContent("StencilNo"));
                material.SetFloat("_StencilComp", 6);
                material.SetFloat("_StencilOpPass", 0);
                material.SetFloat("_StencilOpFail", 0);
                break;
            case 2: //StrncilMask
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest - 1;
                materialEditor.ShaderProperty(stencilNoProp, new GUIContent("StencilNo"));
                material.SetFloat("_StencilComp", 8);
                material.SetFloat("_StencilOpPass", 2);
                material.SetFloat("_StencilOpFail", 2);
                break;
        }
    }

    private void AlphaClipingSetting(MaterialEditor materialEditor, Material material)
    {
        switch(material.GetInt("_AlphaClip"))
        {
            case 0:
                material.SetInt("_InverseClippingMask", 0);
                material.DisableKeyword("_INVERSECLIPPINGMASK_ON");
                break;
            case 1:
                materialEditor.TexturePropertySingleLine(new GUIContent("ClippingMaskMap"), alphaClipMapProp);
                materialEditor.ShaderProperty(inverseClippingMaskProp, new GUIContent("InverseClippingMask"));
                materialEditor.ShaderProperty(cutOffProp, new GUIContent("Cutoff"));
                material.renderQueue = (int)RenderQueue.AlphaTest;
                material.SetOverrideTag("RenderType", "TransparentCutout");

                material.SetTexture("_MainTex", baseMapProp.textureValue);
                var baseMapTiling = baseMapProp.textureScaleAndOffset;
                material.SetTextureScale("_MainTex", new Vector2(baseMapTiling.x, baseMapTiling.y));
                material.SetTextureOffset("_MainTex", new Vector2(baseMapTiling.z, baseMapTiling.w));
                break;
        }
    }
}
