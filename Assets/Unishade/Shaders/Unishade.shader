Shader "Unishade/Renderer Effect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _UnishadeUVRect ("UV Rect", Vector) = (0,0,1,1)

        [HideInInspector] _SrcBlend ("Source Blend", Float) = 1
        [HideInInspector] _DstBlend ("Destination Blend", Float) = 10
        [HideInInspector] _TextureBlendSource ("Texture Blend Mask Source", Float) = 1
        [HideInInspector] _TextureBlendLayout ("Texture Blend Layout", Float) = 0
        [HideInInspector] _BlendingMask ("Texture Blending Mask", 2D) = "white" {}
        [HideInInspector] _BlendingMask_ST ("Texture Blending Mask ST", Vector) = (1,1,0,0)
        [HideInInspector] _BlendingTextureG ("Blending Texture G", 2D) = "white" {}
        [HideInInspector] _BlendingTextureG_ST ("Blending Texture G ST", Vector) = (1,1,0,0)
        [HideInInspector] _BlendingTextureG_Opacity ("Blending Texture G Opacity", Float) = 0
        [HideInInspector] _BlendingTextureG_Mode ("Blending Texture G Mode", Float) = 0
        [HideInInspector] _BlendingTextureB ("Blending Texture B", 2D) = "white" {}
        [HideInInspector] _BlendingTextureB_ST ("Blending Texture B ST", Vector) = (1,1,0,0)
        [HideInInspector] _BlendingTextureB_Opacity ("Blending Texture B Opacity", Float) = 0
        [HideInInspector] _BlendingTextureB_Mode ("Blending Texture B Mode", Float) = 0
        [HideInInspector] _BlendingTextureWhite ("Blending Texture White", 2D) = "white" {}
        [HideInInspector] _BlendingTextureWhite_ST ("Blending Texture White ST", Vector) = (1,1,0,0)
        [HideInInspector] _BlendingTextureWhite_Opacity ("Blending Texture White Opacity", Float) = 0
        [HideInInspector] _BlendingTextureWhite_Mode ("Blending Texture White Mode", Float) = 0
        [HideInInspector] _ToneIntensity ("Tone Intensity", Float) = 1
        [HideInInspector] _ColorFilter ("Color Filter", Float) = 0
        [HideInInspector] _ColorValue ("Color Value", Color) = (1,1,1,1)
        [HideInInspector] _ColorIntensity ("Color Intensity", Float) = 1
        [HideInInspector] _ColorGlow ("Color Glow", Float) = 0
        [HideInInspector] _SamplingIntensity ("Sampling Intensity", Float) = 0.5
        [HideInInspector] _SamplingWidth ("Sampling Width", Float) = 1
        [HideInInspector] _SamplingScale ("Sampling Scale", Float) = 1
        [HideInInspector] _TransitionTex ("Transition Texture", 2D) = "white" {}
        [HideInInspector] _TransitionTex_ST ("Transition Texture ST", Vector) = (1,1,0,0)
        [HideInInspector] _TransitionTex_Speed ("Transition Texture Speed", Vector) = (0,0,0,0)
        [HideInInspector] _TransitionRate ("Transition Rate", Float) = 0.5
        [HideInInspector] _TransitionReverse ("Transition Reverse", Float) = 0
        [HideInInspector] _TransitionColorFilter ("Transition Color Filter", Float) = 6
        [HideInInspector] _TransitionColor ("Transition Color", Color) = (0,0.5,1,1)
        [HideInInspector] _TransitionColorGlow ("Transition Color Glow", Float) = 0
        [HideInInspector] _TransitionSoftness ("Transition Softness", Float) = 0.2
        [HideInInspector] _TransitionRange ("Transition Range", Vector) = (0,1,0,0)
        [HideInInspector] _TransitionWidth ("Transition Width", Float) = 0.2
        [HideInInspector] _TransitionPatternReverse ("Transition Pattern Reverse", Float) = 0
        [HideInInspector] _TransitionAutoPlaySpeed ("Transition Auto Play Speed", Float) = 0
        [HideInInspector] _TransitionGradientTex ("Transition Gradient", 2D) = "white" {}
        [HideInInspector] _TargetColor ("Target Color", Color) = (1,1,1,1)
        [HideInInspector] _TargetRange ("Target Range", Float) = 0.1
        [HideInInspector] _TargetSoftness ("Target Softness", Float) = 0.5
        [HideInInspector] _ShadowBlurIntensity ("Shadow Blur", Float) = 1
        [HideInInspector] _ShadowColorFilter ("Shadow Color Filter", Float) = 4
        [HideInInspector] _ShadowColor ("Shadow Color", Color) = (1,1,1,1)
        [HideInInspector] _ShadowColorGlow ("Shadow Color Glow", Float) = 0
        [HideInInspector] _EdgeWidth ("Edge Width", Float) = 0.5
        [HideInInspector] _EdgeShinyRate ("Edge Shiny Rate", Float) = 0.5
        [HideInInspector] _EdgeShinyAutoPlaySpeed ("Edge Shiny Speed", Float) = 1
        [HideInInspector] _EdgeShinyWidth ("Edge Shiny Width", Float) = 0.5
        [HideInInspector] _EdgeColorFilter ("Edge Color Filter", Float) = 4
        [HideInInspector] _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        [HideInInspector] _EdgeColorGlow ("Edge Color Glow", Float) = 0
        [HideInInspector] _PatternArea ("Pattern Area", Float) = 1
        [HideInInspector] _DetailIntensity ("Detail Intensity", Float) = 1
        [HideInInspector] _DetailThreshold ("Detail Threshold", Vector) = (0,1,0,0)
        [HideInInspector] _DetailColor ("Detail Color", Color) = (1,1,1,1)
        [HideInInspector] _DetailTex ("Detail Texture", 2D) = "white" {}
        [HideInInspector] _DetailTex_ST ("Detail Texture ST", Vector) = (1,1,0,0)
        [HideInInspector] _DetailTex_Speed ("Detail Texture Speed", Vector) = (0,0,0,0)
        [HideInInspector] _GradationIntensity ("Gradation Intensity", Float) = 1
        [HideInInspector] _GradationColorFilter ("Gradation Color Filter", Float) = 1
        [HideInInspector] _GradationColor1 ("Gradation Color 1", Color) = (1,1,1,1)
        [HideInInspector] _GradationColor2 ("Gradation Color 2", Color) = (1,1,1,1)
        [HideInInspector] _GradationColor3 ("Gradation Color 3", Color) = (1,1,1,1)
        [HideInInspector] _GradationColor4 ("Gradation Color 4", Color) = (1,1,1,1)
        [HideInInspector] _GradationTex ("Gradation Texture", 2D) = "white" {}
        [HideInInspector] _GradationTex_ST ("Gradation Texture ST", Vector) = (1,1,0,0)
        [HideInInspector] _GradationRadial ("Gradation Radial", Float) = 0
        [HideInInspector] _UnishadeTime ("Unishade Time", Float) = 0
        [HideInInspector] _DitherIntensity ("Dither Intensity", Float) = 1
        [HideInInspector] _DitherScale ("Dither Scale", Float) = 1
        [HideInInspector] _VertexShakeSpeed ("Vertex Shake Speed", Vector) = (41,49,45,0)
        [HideInInspector] _VertexShakeSpeedMultiplier ("Vertex Shake Speed Multiplier", Float) = 1
        [HideInInspector] _VertexShakeDisplacement ("Vertex Shake Displacement", Vector) = (0.1,0.1,0.1,0)
        [HideInInspector] _VertexShakeBlend ("Vertex Shake Blend", Float) = 1
        [HideInInspector] _ScrollTextureSpeed ("Scroll Texture Speed", Vector) = (1,1,0,0)
        [HideInInspector] _HandDrawnAmount ("Hand Drawn Amount", Float) = 10
        [HideInInspector] _HandDrawnSpeed ("Hand Drawn Speed", Float) = 5
        [HideInInspector] _DistortionIntensity ("Distortion Intensity", Float) = 0.5
        [HideInInspector] _DistortionWave ("Distortion Wave", Vector) = (12,8,0,0)
        [HideInInspector] _DistortionAmount ("Distortion Amount", Vector) = (0.03,0.03,0,0)
        [HideInInspector] _DistortionSpeed ("Distortion Speed", Float) = 1
        [HideInInspector] _DistortionTex ("Distortion Texture", 2D) = "gray" {}
        [HideInInspector] _DistortionTex_ST ("Distortion Texture ST", Vector) = (1,1,0,0)
        [HideInInspector] _DistortionRippleCenter ("Distortion Ripple Center", Vector) = (0.5,0.5,0,0)
        [HideInInspector] _DistortionRippleFrequency ("Distortion Ripple Frequency", Float) = 32
        [HideInInspector] _DistortionRippleStrength ("Distortion Ripple Strength", Float) = 0.03
        [HideInInspector] _DistortionRippleSpeed ("Distortion Ripple Speed", Float) = 1
        [HideInInspector] _HologramIntensity ("Hologram Intensity", Float) = 0.75
        [HideInInspector] _HologramScanlineDensity ("Hologram Scanline Density", Float) = 48
        [HideInInspector] _HologramScanlineStrength ("Hologram Scanline Strength", Float) = 0.35
        [HideInInspector] _HologramNoise ("Hologram Noise", Float) = 0.2
        [HideInInspector] _HologramRgbShift ("Hologram RGB Shift", Float) = 0.15
        [HideInInspector] _HologramSpeed ("Hologram Speed", Float) = 1
        [HideInInspector] _HologramColor ("Hologram Color", Color) = (0.1,0.8,1,1)
        [HideInInspector] _HologramMode ("Hologram Mode", Float) = 0
        [HideInInspector] _GlitchIntensity ("Glitch Intensity", Float) = 0.5
        [HideInInspector] _GlitchBlockSize ("Glitch Block Size", Float) = 0.08
        [HideInInspector] _GlitchSpeed ("Glitch Speed", Float) = 1
        [HideInInspector] _GlitchMode ("Glitch Mode", Float) = 0
        [HideInInspector] _InnerGlowIntensity ("Inner Glow Intensity", Float) = 0.75
        [HideInInspector] _InnerGlowSize ("Inner Glow Size", Float) = 1.5
        [HideInInspector] _InnerGlowColor ("Inner Glow Color", Color) = (1,1,1,1)
        [HideInInspector] _OuterGlowIntensity ("Outer Glow Intensity", Float) = 1
        [HideInInspector] _OuterGlowSize ("Outer Glow Size", Float) = 3
        [HideInInspector] _OuterGlowSoftness ("Outer Glow Softness", Float) = 0.75
        [HideInInspector] _OuterGlowColor ("Outer Glow Color", Color) = (0.1,0.8,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        ZTest LEqual
        Blend [_SrcBlend] [_DstBlend]

        Pass
        {
            Name "UnishadeRendererEffect"
            Tags { "LightMode"="SRPDefaultUnlit" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #pragma shader_feature_local_fragment _ TONE_GRAYSCALE TONE_SEPIA TONE_NEGATIVE TONE_RETRO TONE_POSTERIZE
            #pragma shader_feature_local_fragment _ COLOR_FILTER
            #pragma shader_feature_local_fragment _ SAMPLING_BLUR_FAST SAMPLING_BLUR_MEDIUM SAMPLING_BLUR_DETAIL SAMPLING_PIXELATION SAMPLING_RGB_SHIFT SAMPLING_EDGE_LUMINANCE SAMPLING_EDGE_ALPHA
            #pragma shader_feature_local_fragment _ TRANSITION_FADE TRANSITION_CUTOFF TRANSITION_DISSOLVE TRANSITION_SHINY TRANSITION_MASK TRANSITION_MELT TRANSITION_BURN TRANSITION_PATTERN TRANSITION_BLAZE
            #pragma shader_feature_local_fragment _ EDGE_PLAIN EDGE_SHINY
            #pragma shader_feature_local_fragment _ DETAIL_MASKING DETAIL_MULTIPLY DETAIL_ADDITIVE DETAIL_SUBTRACTIVE DETAIL_REPLACE DETAIL_MULTIPLY_ADDITIVE
            #pragma shader_feature_local_fragment _ TARGET_HUE TARGET_LUMINANCE
            #pragma shader_feature_local_fragment _ GRADATION_GRADIENT GRADATION_COLOR2 GRADATION_COLOR4
            #pragma shader_feature_local_fragment _ DISTORTION_WAVE DISTORTION_NOISE DISTORTION_TWIST DISTORTION_PINCH DISTORTION_FISHEYE DISTORTION_WATER_RIPPLE
            #pragma shader_feature_local_fragment _ DITHER_EFFECT
            #pragma shader_feature_local _ VERTEX_SHAKE_EFFECT
            #pragma shader_feature_local_fragment _ SCROLL_TEXTURE_EFFECT
            #pragma shader_feature_local_fragment _ HAND_DRAWN_EFFECT
            #pragma shader_feature_local_fragment _ HOLOGRAM_EFFECT
            #pragma shader_feature_local_fragment _ GLITCH_EFFECT
            #pragma shader_feature_local_fragment _ INNER_GLOW_EFFECT
            #pragma shader_feature_local_fragment _ OUTER_GLOW_EFFECT
            #pragma shader_feature_local_fragment _ TEXTURE_BLEND

            #include "UnityCG.cginc"

            #define UNISHADE_LOCAL_UV 1
            #define UIEFFECT_EDITOR 1

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 uvMask : TEXCOORD1;
                float4 localPosition : TEXCOORD2;
                fixed4 vertexColor : TEXCOORD3;
                float4 screenPosition : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _RendererColor;
            fixed4 _TextureSampleAdd;
            float4 _UnishadeUVRect;
            float _UnishadeTime;
            float4 _VertexShakeSpeed;
            float _VertexShakeSpeedMultiplier;
            float4 _VertexShakeDisplacement;
            float _VertexShakeBlend;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float3 vertexPosition = v.vertex.xyz;
                #if defined(VERTEX_SHAKE_EFFECT)
                {
                    const float3 speedOffset = float3(1.0, 1.13, 1.07) * _VertexShakeSpeedMultiplier;
                    const float3 displacement = sin(_UnishadeTime * _VertexShakeSpeed.xyz * speedOffset)
                        * _VertexShakeDisplacement.xyz * saturate(_VertexShakeBlend);
                    vertexPosition += displacement;
                }
                #endif
                const float4 shakenVertex = float4(vertexPosition, v.vertex.w);
                o.vertex = UnityObjectToClipPos(shakenVertex);
                o.texcoord = v.texcoord;
                o.uvMask = _UnishadeUVRect;
                o.localPosition = shakenVertex;
                o.vertexColor = v.color;
                o.color = v.color * _Color * _RendererColor;
                o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 uieffect_frag(v2f i, float2 uv)
            {
                half4 color = tex2D(_MainTex, uv) + _TextureSampleAdd;
                color.rgb *= i.color.rgb;
                color.rgb *= color.a;
                return color;
            }

            #define UIEFFECT_FRAG_STRUCT v2f
            #include "Unishade.cgin"

            half4 frag(v2f i) : SV_Target
            {
                half4 color = uieffect(i.texcoord, i.uvMask, i.localPosition, i);
                return color * i.color.a;
            }
            ENDCG
        }
    }
}
