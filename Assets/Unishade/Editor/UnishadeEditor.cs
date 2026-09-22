using UnityEditor;
using UnityEngine;

namespace Unishade.Editor
{
    [CustomEditor(typeof(Unishade), true)]
    public sealed class UnishadeEditor : UnityEditor.Editor
    {
        private SerializedProperty rendererMode;
        private SerializedProperty sourceMaterial;
        private SerializedProperty tint;
        private SerializedProperty keepAspectRatio;
        private SerializedProperty effectRotation;
        private SerializedProperty updateMode;
        private SerializedProperty animationEnabled;
        private SerializedProperty animationSpeed;

        private void OnEnable()
        {
            rendererMode = Find("rendererMode");
            sourceMaterial = Find("sourceMaterial");
            tint = Find("tint");
            keepAspectRatio = Find("keepAspectRatio");
            effectRotation = Find("effectRotation");
            updateMode = Find("updateMode");
            animationEnabled = Find("animationEnabled");
            animationSpeed = Find("animationSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "Unishade owns an isolated material for this renderer. Effects can be combined in one component and work with SpriteRenderer and MeshRenderer.",
                MessageType.Info);

            DrawBox(() =>
            {
                Property(rendererMode);
                Property(sourceMaterial);
                Property(tint);
                Property(keepAspectRatio);
                Property(effectRotation);
            });

            DrawBox(() =>
            {
                Property(updateMode);
                Property(animationEnabled);
                if (animationEnabled.boolValue) Property(animationSpeed);
            });

            DrawDither();
            DrawVertexShake();
            DrawScrollTexture();
            DrawHandDrawn();
            DrawEnumBox("tone", "Filter", "toneIntensity");
            DrawEnumBox("colorFilter", "Filter", "colorIntensity", "color", "colorGlow");
            DrawEnumBox("sampling", "Filter", "samplingIntensity", "samplingWidth", "samplingScale");
            DrawTransition();
            DrawTarget();
            DrawBlending();
            DrawGradation();
            DrawEdge();
            DrawDetail();
            DrawDistortion();
            DrawHologram();
            DrawGlitch();
            DrawInnerGlow();
            DrawOuterGlow();

            serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty Find(string propertyName) => serializedObject.FindProperty(propertyName);

        private void DrawDither()
        {
            DrawBox(() =>
            {
                var mode = Find("dither");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("ditherIntensity", "Intensity");
                Property("ditherScale", "Scale");
            });
        }

        private void DrawVertexShake()
        {
            DrawBox(() =>
            {
                var mode = Find("vertexShake");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("vertexShakeSpeed", "Speed");
                Property("vertexShakeSpeedMultiplier", "Speed Multiplier");
                Property("vertexShakeMaxDisplacement", "Max Displacement");
                Property("vertexShakeBlend", "Blend");
            });
        }

        private void DrawScrollTexture()
        {
            DrawBox(() =>
            {
                var mode = Find("scrollTexture");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("scrollTextureSpeed", "Scroll Speed");
            });
        }

        private void DrawHandDrawn()
        {
            DrawBox(() =>
            {
                var mode = Find("handDrawn");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("handDrawnAmount", "Amount");
                Property("handDrawnSpeed", "Speed");
            });
        }

        private void DrawTransition()
        {
            DrawBox(() =>
            {
                var mode = Find("transition");
                Property(mode, "Filter");
                if (mode.enumValueIndex == 0) return;
                Property("transitionRate");
                Property("transitionReverse");
                Property("transitionTexture");
                Property("transitionTextureScale");
                Property("transitionTextureOffset");
                Property("transitionTextureSpeed");
                Property("transitionWidth");
                Property("transitionSoftness");
                Property("transitionRange");
                Property("transitionColorFilter");
                Property("transitionColor");
                Property("transitionColorGlow");
                Property("transitionPatternReverse");
                Property("transitionAutoPlaySpeed");
                Property("transitionGradient");
            });
        }

        private void DrawTarget()
        {
            DrawBox(() =>
            {
                var mode = Find("target");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("targetColor");
                Property("targetRange");
                Property("targetSoftness");
            });
        }

        private void DrawBlending()
        {
            DrawBox(() =>
            {
                var mode = Find("blendType");
                Property(mode, "Blend Type");
                if (mode.enumValueIndex == 0)
                {
                    Property("sourceBlend", "Source Blend");
                    Property("destinationBlend", "Destination Blend");
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Texture Blending", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Main Texture берётся из SpriteRenderer или MeshRenderer. Ниже добавляются текстурные слои поверх неё.",
                    MessageType.None);

                var layout = Find("textureBlendLayout");
                Property("textureBlendSource", "Mask Source");
                Property(layout, "Blend Layout");
                if (Find("textureBlendSource").enumValueIndex == 1)
                {
                    Property("blendingMask", "Mask Texture");
                    Property("blendingMaskScale", "Mask Tiling");
                    Property("blendingMaskOffset", "Mask Offset");
                }

                if (layout.enumValueIndex == 0)
                {
                    DrawTextureBlendLayer("Layer 1 (Green Mask)", Find("blendingLayerG"));
                    DrawTextureBlendLayer("Layer 2 (Blue Mask)", Find("blendingLayerB"));
                }
                else
                {
                    DrawTextureBlendLayer("Layer 1 (Black & White Mask)", Find("blendingLayerWhite"));
                }
            });
        }

        private void DrawTextureBlendLayer(string title, SerializedProperty layer)
        {
            if (layer == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            Property(layer.FindPropertyRelative("texture"), "Texture");
            Property(layer.FindPropertyRelative("opacity"), "Opacity");
            Property(layer.FindPropertyRelative("blendMode"), "Blend Mode");
            Property(layer.FindPropertyRelative("scale"), "Tiling");
            Property(layer.FindPropertyRelative("offset"), "Offset");
            EditorGUILayout.EndVertical();
        }

        private void DrawGradation()
        {
            DrawBox(() =>
            {
                var mode = Find("gradation");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("gradationIntensity", "Gradient Intensity");
                Property("gradationColorFilter", "Gradient Color Filter");
                Property("gradationColor1", "Gradient Color 1");
                Property("gradationColor2", "Gradient Color 2");
                Property("gradationColor3", "Gradient Color 3");
                Property("gradationColor4", "Gradient Color 4");
                Property("gradationGradient", "Gradient Texture");
                Property("gradationOffset", "Gradient Offset");
                Property("gradationScale", "Gradient Scale");
                Property("gradationRotation", "Gradient Rotation");
                Property("gradationReverse", "Reverse Gradient");
            });
        }

        private void DrawEdge()
        {
            DrawBox(() =>
            {
                var mode = Find("edge");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("edgeWidth");
                Property("edgeColorFilter");
                Property("edgeColor");
                Property("edgeColorGlow");
                if (mode.enumValueIndex == 2)
                {
                    Property("edgeShinyRate");
                    Property("edgeShinyWidth");
                    Property("edgeShinyAutoPlaySpeed");
                }
                Property("patternArea");
            });
        }

        private void DrawDetail()
        {
            DrawBox(() =>
            {
                var mode = Find("detail");
                Property(mode, "Filter");
                if (mode.enumValueIndex == 0) return;
                Property("detailIntensity");
                Property("detailThreshold");
                Property("detailColor");
                Property("detailTexture");
                Property("detailTextureScale");
                Property("detailTextureOffset");
                Property("detailTextureSpeed");
            });
        }

        private void DrawDistortion()
        {
            DrawBox(() =>
            {
                var mode = Find("distortion");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("distortionIntensity");
                if (mode.enumValueIndex == 1)
                {
                    Property("distortionWaveX");
                    Property("distortionWaveY");
                    Property("distortionAmountX");
                    Property("distortionAmountY");
                }
                else if (mode.enumValueIndex == 2)
                {
                    Property("distortionAmountX");
                    Property("distortionAmountY");
                    Property("distortionTexture");
                    Property("distortionTextureScale");
                    Property("distortionTextureOffset");
                }
                else if (mode.enumValueIndex == 6)
                {
                    Property("distortionRippleCenter", "Ripple Center");
                    Property("distortionRippleFrequency", "Ripple Frequency");
                    Property("distortionRippleStrength", "Ripple Strength");
                    Property("distortionRippleSpeed", "Ripple Speed");
                }
                else
                {
                    Property("distortionAmountX");
                }
                if (mode.enumValueIndex != 6) Property("distortionSpeed");
            });
        }

        private void DrawHologram()
        {
            DrawBox(() =>
            {
                var mode = Find("hologram");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("hologramIntensity");
                Property("hologramScanlineDensity");
                Property("hologramScanlineStrength");
                Property("hologramNoise");
                Property("hologramRgbShift");
                Property("hologramSpeed");
                Property("hologramColor");
            });
        }

        private void DrawGlitch()
        {
            DrawBox(() =>
            {
                var mode = Find("glitch");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("glitchIntensity");
                Property("glitchBlockSize");
                Property("glitchSpeed");
            });
        }

        private void DrawInnerGlow()
        {
            DrawBox(() =>
            {
                var mode = Find("innerGlow");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("innerGlowIntensity");
                Property("innerGlowSize");
                Property("innerGlowColor");
            });
        }

        private void DrawOuterGlow()
        {
            DrawBox(() =>
            {
                var mode = Find("outerGlow");
                Property(mode, "Mode");
                if (mode.enumValueIndex == 0) return;
                Property("outerGlowIntensity", "Intensity");
                Property("outerGlowSize", "Size");
                Property("outerGlowSoftness", "Softness");
                Property("outerGlowColor", "Color");
            });
        }

        private void DrawEnumBox(string enumName, string selectorLabel, params string[] dependentProperties)
        {
            DrawBox(() =>
            {
                var mode = Find(enumName);
                Property(mode, selectorLabel);
                if (mode.enumValueIndex == 0) return;
                for (var i = 0; i < dependentProperties.Length; i++) Property(dependentProperties[i]);
            });
        }

        private void DrawBox(System.Action content)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            content();
            EditorGUILayout.EndVertical();
        }

        private void Property(string propertyName)
        {
            Property(Find(propertyName));
        }

        private void Property(string propertyName, string label)
        {
            Property(Find(propertyName), label);
        }

        private void Property(SerializedProperty property, string label)
        {
            if (property != null) EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        private static void Property(SerializedProperty property)
        {
            if (property != null) EditorGUILayout.PropertyField(property);
        }
    }
}
