using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unishade
{
    public sealed partial class Unishade
    {
        [NonSerialized] private RendererSettings rendererApi;
        [NonSerialized] private AnimationSettings animationApi;
        [NonSerialized] private DitherSettings ditherApi;
        [NonSerialized] private VertexShakeSettings vertexShakeApi;
        [NonSerialized] private ScrollTextureSettings scrollTextureApi;
        [NonSerialized] private HandDrawnSettings handDrawnApi;
        [NonSerialized] private ToneSettings toneApi;
        [NonSerialized] private ColorSettings colorApi;
        [NonSerialized] private SamplingSettings samplingApi;
        [NonSerialized] private TransitionSettings transitionApi;
        [NonSerialized] private TargetSettings targetApi;
        [NonSerialized] private BlendingSettings blendingApi;
        [NonSerialized] private GradientSettings gradientApi;
        [NonSerialized] private EdgeSettings edgeApi;
        [NonSerialized] private DetailSettings detailApi;
        [NonSerialized] private DistortionSettings distortionApi;
        [NonSerialized] private HologramSettings hologramApi;
        [NonSerialized] private GlitchSettings glitchApi;
        [NonSerialized] private InnerGlowSettings innerGlowApi;
        [NonSerialized] private OuterGlowSettings outerGlowApi;

        /// <summary>Renderer and source material settings.</summary>
        public RendererSettings Renderer => rendererApi ?? (rendererApi = new RendererSettings(this));

        /// <summary>Animation and time update settings.</summary>
        public AnimationSettings Animation => animationApi ?? (animationApi = new AnimationSettings(this));

        /// <summary>Dither effect settings.</summary>
        public DitherSettings Dither => ditherApi ?? (ditherApi = new DitherSettings(this));

        /// <summary>Vertex shake effect settings.</summary>
        public VertexShakeSettings VertexShake => vertexShakeApi ?? (vertexShakeApi = new VertexShakeSettings(this));

        /// <summary>Scrolling texture settings.</summary>
        public ScrollTextureSettings ScrollTexture => scrollTextureApi ?? (scrollTextureApi = new ScrollTextureSettings(this));

        /// <summary>Hand-drawn motion settings.</summary>
        public HandDrawnSettings HandDrawn => handDrawnApi ?? (handDrawnApi = new HandDrawnSettings(this));

        /// <summary>Tone filter settings.</summary>
        public ToneSettings Tone => toneApi ?? (toneApi = new ToneSettings(this));

        /// <summary>Color filter settings.</summary>
        public ColorSettings Color => colorApi ?? (colorApi = new ColorSettings(this));

        /// <summary>Sampling and blur settings.</summary>
        public SamplingSettings Sampling => samplingApi ?? (samplingApi = new SamplingSettings(this));

        /// <summary>Transition and dissolve settings.</summary>
        public TransitionSettings Transition => transitionApi ?? (transitionApi = new TransitionSettings(this));

        /// <summary>Target color settings.</summary>
        public TargetSettings Target => targetApi ?? (targetApi = new TargetSettings(this));

        /// <summary>Blend mode and multi-texture blending settings.</summary>
        public BlendingSettings Blending => blendingApi ?? (blendingApi = new BlendingSettings(this));

        /// <summary>Gradient settings. The Inspector calls this section Gradient as well.</summary>
        public GradientSettings Gradient => gradientApi ?? (gradientApi = new GradientSettings(this));

        /// <summary>Edge effect settings.</summary>
        public EdgeSettings Edge => edgeApi ?? (edgeApi = new EdgeSettings(this));

        /// <summary>Detail texture settings.</summary>
        public DetailSettings Detail => detailApi ?? (detailApi = new DetailSettings(this));

        /// <summary>Distortion and water ripple settings.</summary>
        public DistortionSettings Distortion => distortionApi ?? (distortionApi = new DistortionSettings(this));

        /// <summary>Hologram effect settings.</summary>
        public HologramSettings Hologram => hologramApi ?? (hologramApi = new HologramSettings(this));

        /// <summary>Glitch effect settings.</summary>
        public GlitchSettings Glitch => glitchApi ?? (glitchApi = new GlitchSettings(this));

        /// <summary>Inner glow settings.</summary>
        public InnerGlowSettings InnerGlow => innerGlowApi ?? (innerGlowApi = new InnerGlowSettings(this));

        /// <summary>Outer glow settings.</summary>
        public OuterGlowSettings OuterGlow => outerGlowApi ?? (outerGlowApi = new OuterGlowSettings(this));

        private void NotifyApiChanged(bool rebuildMaterials = false)
        {
            if (!isActiveAndEnabled) return;

            if (rebuildMaterials) RestoreMaterials();
            EnsureTextureBlendLayers();
            EnsureMaterials();
            ApplyToMaterials();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }

        public abstract class SettingsBase
        {
            protected readonly Unishade Owner;

            protected SettingsBase(Unishade owner)
            {
                Owner = owner;
            }

            protected void Changed()
            {
                Owner.NotifyApiChanged();
            }

            protected void ChangedAndRebuildMaterials()
            {
                Owner.NotifyApiChanged(true);
            }
        }

        public sealed class RendererSettings : SettingsBase
        {
            internal RendererSettings(Unishade owner) : base(owner) { }

            public RendererMode Mode
            {
                get => Owner.rendererMode;
                set { if (Owner.rendererMode == value) return; Owner.rendererMode = value; ChangedAndRebuildMaterials(); }
            }

            public Material SourceMaterial
            {
                get => Owner.sourceMaterial;
                set { if (Owner.sourceMaterial == value) return; Owner.sourceMaterial = value; ChangedAndRebuildMaterials(); }
            }

            public Color Tint
            {
                get => Owner.tint;
                set { if (Owner.tint == value) return; Owner.tint = value; Changed(); }
            }

            public bool KeepAspectRatio
            {
                get => Owner.keepAspectRatio;
                set { if (Owner.keepAspectRatio == value) return; Owner.keepAspectRatio = value; Changed(); }
            }

            public float EffectRotation
            {
                get => Owner.effectRotation;
                set { value = Mathf.Repeat(value, 360f); if (Mathf.Approximately(Owner.effectRotation, value)) return; Owner.effectRotation = value; Changed(); }
            }
        }

        public sealed class AnimationSettings : SettingsBase
        {
            internal AnimationSettings(Unishade owner) : base(owner) { }

            public UpdateMode UpdateMode
            {
                get => Owner.updateMode;
                set { if (Owner.updateMode == value) return; Owner.updateMode = value; Owner.lastAnimationClock = 0; Changed(); }
            }

            public bool Enabled
            {
                get => Owner.animationEnabled;
                set { if (Owner.animationEnabled == value) return; Owner.animationEnabled = value; Changed(); }
            }

            public float Speed
            {
                get => Owner.animationSpeed;
                set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.animationSpeed, value)) return; Owner.animationSpeed = value; Changed(); }
            }

            public float Time => Owner.animationTime;

            public void ResetTime()
            {
                Owner.animationTime = 0;
                Owner.lastAnimationClock = 0;
                Changed();
            }

            public void Advance(float deltaTime)
            {
                Owner.AdvanceAnimation(deltaTime);
            }
        }

        public sealed class DitherSettings : SettingsBase
        {
            public enum ModeType { None, Alpha }
            internal DitherSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.dither;
                set { var converted = (DitherFilter)value; if (Owner.dither == converted) return; Owner.dither = converted; Changed(); }
            }

            public float Intensity
            {
                get => Owner.ditherIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.ditherIntensity, value)) return; Owner.ditherIntensity = value; Changed(); }
            }

            public float Scale
            {
                get => Owner.ditherScale;
                set { value = Mathf.Clamp(value, 0.01f, 2f); if (Mathf.Approximately(Owner.ditherScale, value)) return; Owner.ditherScale = value; Changed(); }
            }
        }

        public sealed class VertexShakeSettings : SettingsBase
        {
            public enum ModeType { None, Shake }
            internal VertexShakeSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.vertexShake;
                set { var converted = (VertexShakeFilter)value; if (Owner.vertexShake == converted) return; Owner.vertexShake = converted; Changed(); }
            }

            public Vector3 Speed
            {
                get => Owner.vertexShakeSpeed;
                set { if (Owner.vertexShakeSpeed == value) return; Owner.vertexShakeSpeed = value; Changed(); }
            }

            public float SpeedMultiplier
            {
                get => Owner.vertexShakeSpeedMultiplier;
                set { value = Mathf.Clamp(value, 0f, 5f); if (Mathf.Approximately(Owner.vertexShakeSpeedMultiplier, value)) return; Owner.vertexShakeSpeedMultiplier = value; Changed(); }
            }

            public Vector3 MaxDisplacement
            {
                get => Owner.vertexShakeMaxDisplacement;
                set { if (Owner.vertexShakeMaxDisplacement == value) return; Owner.vertexShakeMaxDisplacement = value; Changed(); }
            }

            public float Blend
            {
                get => Owner.vertexShakeBlend;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.vertexShakeBlend, value)) return; Owner.vertexShakeBlend = value; Changed(); }
            }
        }

        public sealed class ScrollTextureSettings : SettingsBase
        {
            public enum ModeType { None, Scroll }
            internal ScrollTextureSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.scrollTexture;
                set { var converted = (ScrollTextureFilter)value; if (Owner.scrollTexture == converted) return; Owner.scrollTexture = converted; Changed(); }
            }

            public Vector2 Speed
            {
                get => Owner.scrollTextureSpeed;
                set { if (Owner.scrollTextureSpeed == value) return; Owner.scrollTextureSpeed = value; Changed(); }
            }
        }

        public sealed class HandDrawnSettings : SettingsBase
        {
            public enum ModeType { None, HandDrawn }
            internal HandDrawnSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.handDrawn;
                set { var converted = (HandDrawnFilter)value; if (Owner.handDrawn == converted) return; Owner.handDrawn = converted; Changed(); }
            }

            public float Amount
            {
                get => Owner.handDrawnAmount;
                set { value = Mathf.Clamp(value, 0f, 50f); if (Mathf.Approximately(Owner.handDrawnAmount, value)) return; Owner.handDrawnAmount = value; Changed(); }
            }

            public float Speed
            {
                get => Owner.handDrawnSpeed;
                set { value = Mathf.Clamp(value, 1f, 15f); if (Mathf.Approximately(Owner.handDrawnSpeed, value)) return; Owner.handDrawnSpeed = value; Changed(); }
            }
        }

        public sealed class ToneSettings : SettingsBase
        {
            public enum FilterType { None, Grayscale, GrayScale = Grayscale, Sepia, Negative, Retro, Posterize }
            internal ToneSettings(Unishade owner) : base(owner) { }

            public FilterType Filter
            {
                get => (FilterType)Owner.tone;
                set { var converted = (ToneFilter)value; if (Owner.tone == converted) return; Owner.tone = converted; Changed(); }
            }

            public FilterType Mode
            {
                get => Filter;
                set => Filter = value;
            }

            public float Intensity
            {
                get => Owner.toneIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.toneIntensity, value)) return; Owner.toneIntensity = value; Changed(); }
            }
        }

        public sealed class ColorSettings : SettingsBase
        {
            public enum FilterType { None, Multiply, Additive, Subtractive, Replace, MultiplyLuminance, MultiplyAdditive, HsvModifier, Contrast }
            internal ColorSettings(Unishade owner) : base(owner) { }

            public FilterType Filter
            {
                get => (FilterType)Owner.colorFilter;
                set { var converted = (ColorFilter)value; if (Owner.colorFilter == converted) return; Owner.colorFilter = converted; Changed(); }
            }

            public FilterType Mode
            {
                get => Filter;
                set => Filter = value;
            }

            public float Intensity
            {
                get => Owner.colorIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.colorIntensity, value)) return; Owner.colorIntensity = value; Changed(); }
            }

            public Color Tint
            {
                get => Owner.color;
                set { if (Owner.color == value) return; Owner.color = value; Changed(); }
            }

            public Color Value
            {
                get => Tint;
                set => Tint = value;
            }

            public bool Glow
            {
                get => Owner.colorGlow;
                set { if (Owner.colorGlow == value) return; Owner.colorGlow = value; Changed(); }
            }
        }

        public sealed class SamplingSettings : SettingsBase
        {
            public enum FilterType { None, BlurFast, BlurMedium, BlurDetail, Pixelation, RgbShift, EdgeLuminance, EdgeAlpha }
            internal SamplingSettings(Unishade owner) : base(owner) { }

            public FilterType Filter
            {
                get => (FilterType)Owner.sampling;
                set { var converted = (SamplingFilter)value; if (Owner.sampling == converted) return; Owner.sampling = converted; Changed(); }
            }

            public FilterType Mode
            {
                get => Filter;
                set => Filter = value;
            }

            public float Intensity
            {
                get => Owner.samplingIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.samplingIntensity, value)) return; Owner.samplingIntensity = value; Changed(); }
            }

            public float Width
            {
                get => Owner.samplingWidth;
                set { value = Mathf.Clamp(value, 0.5f, 10f); if (Mathf.Approximately(Owner.samplingWidth, value)) return; Owner.samplingWidth = value; Changed(); }
            }

            public float Scale
            {
                get => Owner.samplingScale;
                set { value = Mathf.Clamp(value, 0.01f, 100f); if (Mathf.Approximately(Owner.samplingScale, value)) return; Owner.samplingScale = value; Changed(); }
            }
        }

        public sealed class TransitionSettings : SettingsBase
        {
            public enum TransitionType { None, Fade, Cutoff, Dissolve, Shiny, Mask, Melt, Burn, Blaze, Pattern }
            internal TransitionSettings(Unishade owner) : base(owner) { }

            public TransitionType Mode
            {
                get => (TransitionType)Owner.transition;
                set { var converted = (TransitionFilter)value; if (Owner.transition == converted) return; Owner.transition = converted; Changed(); }
            }

            public float Amount
            {
                get => Owner.transitionRate;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.transitionRate, value)) return; Owner.transitionRate = value; Changed(); }
            }

            public float Rate
            {
                get => Amount;
                set => Amount = value;
            }

            public bool Reverse
            {
                get => Owner.transitionReverse;
                set { if (Owner.transitionReverse == value) return; Owner.transitionReverse = value; Changed(); }
            }

            public Texture Texture
            {
                get => Owner.transitionTexture;
                set { if (Owner.transitionTexture == value) return; Owner.transitionTexture = value; Changed(); }
            }

            public Vector2 Tiling
            {
                get => Owner.transitionTextureScale;
                set { if (Owner.transitionTextureScale == value) return; Owner.transitionTextureScale = value; Changed(); }
            }

            public Vector2 Offset
            {
                get => Owner.transitionTextureOffset;
                set { if (Owner.transitionTextureOffset == value) return; Owner.transitionTextureOffset = value; Changed(); }
            }

            public Vector2 Speed
            {
                get => Owner.transitionTextureSpeed;
                set { if (Owner.transitionTextureSpeed == value) return; Owner.transitionTextureSpeed = value; Changed(); }
            }

            public float Width
            {
                get => Owner.transitionWidth;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.transitionWidth, value)) return; Owner.transitionWidth = value; Changed(); }
            }

            public float Softness
            {
                get => Owner.transitionSoftness;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.transitionSoftness, value)) return; Owner.transitionSoftness = value; Changed(); }
            }

            public Vector2 Range
            {
                get => Owner.transitionRange;
                set { value.x = Mathf.Clamp01(value.x); value.y = Mathf.Clamp01(value.y); if (Owner.transitionRange == value) return; Owner.transitionRange = value; Changed(); }
            }

            public ColorFilter ColorFilter
            {
                get => Owner.transitionColorFilter;
                set { if (Owner.transitionColorFilter == value) return; Owner.transitionColorFilter = value; Changed(); }
            }

            public Color Color
            {
                get => Owner.transitionColor;
                set { if (Owner.transitionColor == value) return; Owner.transitionColor = value; Changed(); }
            }

            public bool ColorGlow
            {
                get => Owner.transitionColorGlow;
                set { if (Owner.transitionColorGlow == value) return; Owner.transitionColorGlow = value; Changed(); }
            }

            public bool PatternReverse
            {
                get => Owner.transitionPatternReverse;
                set { if (Owner.transitionPatternReverse == value) return; Owner.transitionPatternReverse = value; Changed(); }
            }

            public float AutoPlaySpeed
            {
                get => Owner.transitionAutoPlaySpeed;
                set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.transitionAutoPlaySpeed, value)) return; Owner.transitionAutoPlaySpeed = value; Changed(); }
            }

            public Gradient Gradient
            {
                get => Owner.transitionGradient;
                set { if (Owner.transitionGradient == value) return; Owner.transitionGradient = value ?? new Gradient(); Changed(); }
            }
        }

        public sealed class TargetSettings : SettingsBase
        {
            public enum TargetType { None, Hue, Luminance }
            internal TargetSettings(Unishade owner) : base(owner) { }

            public TargetType Mode
            {
                get => (TargetType)Owner.target;
                set { var converted = (TargetMode)value; if (Owner.target == converted) return; Owner.target = converted; Changed(); }
            }

            public Color Color
            {
                get => Owner.targetColor;
                set { if (Owner.targetColor == value) return; Owner.targetColor = value; Changed(); }
            }

            public float Range
            {
                get => Owner.targetRange;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.targetRange, value)) return; Owner.targetRange = value; Changed(); }
            }

            public float Softness
            {
                get => Owner.targetSoftness;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.targetSoftness, value)) return; Owner.targetSoftness = value; Changed(); }
            }
        }

        public sealed class BlendingSettings : SettingsBase
        {
            internal BlendingSettings(Unishade owner) : base(owner)
            {
                Green = new TextureLayerSettings(owner, TextureLayerSettings.TextureLayerChannel.Green);
                Blue = new TextureLayerSettings(owner, TextureLayerSettings.TextureLayerChannel.Blue);
                White = new TextureLayerSettings(owner, TextureLayerSettings.TextureLayerChannel.White);
            }

            public BlendType Type
            {
                get => Owner.blendType;
                set { if (Owner.blendType == value) return; Owner.blendType = value; Changed(); }
            }

            public BlendMode SourceBlend
            {
                get => Owner.sourceBlend;
                set { if (Owner.sourceBlend == value) return; Owner.sourceBlend = value; Changed(); }
            }

            public BlendMode DestinationBlend
            {
                get => Owner.destinationBlend;
                set { if (Owner.destinationBlend == value) return; Owner.destinationBlend = value; Changed(); }
            }

            public TextureBlendSource MaskSource
            {
                get => Owner.textureBlendSource;
                set { if (Owner.textureBlendSource == value) return; Owner.textureBlendSource = value; Changed(); }
            }

            public TextureBlendLayout MaskLayout
            {
                get => Owner.textureBlendLayout;
                set { if (Owner.textureBlendLayout == value) return; Owner.textureBlendLayout = value; Changed(); }
            }

            public Texture Mask
            {
                get => Owner.blendingMask;
                set { if (Owner.blendingMask == value) return; Owner.blendingMask = value; Changed(); }
            }

            public Vector2 MaskTiling
            {
                get => Owner.blendingMaskScale;
                set { if (Owner.blendingMaskScale == value) return; Owner.blendingMaskScale = value; Changed(); }
            }

            public Vector2 MaskOffset
            {
                get => Owner.blendingMaskOffset;
                set { if (Owner.blendingMaskOffset == value) return; Owner.blendingMaskOffset = value; Changed(); }
            }

            public TextureLayerSettings Green { get; }
            public TextureLayerSettings Blue { get; }
            public TextureLayerSettings White { get; }
        }

        public sealed class TextureLayerSettings : SettingsBase
        {
            internal enum TextureLayerChannel { Green, Blue, White }

            private readonly TextureLayerChannel channel;

            internal TextureLayerSettings(Unishade owner, TextureLayerChannel channel) : base(owner)
            {
                this.channel = channel;
            }

            private TextureBlendLayer Layer
            {
                get
                {
                    switch (channel)
                    {
                        case TextureLayerChannel.Green: return Owner.blendingLayerG;
                        case TextureLayerChannel.Blue: return Owner.blendingLayerB;
                        default: return Owner.blendingLayerWhite;
                    }
                }
            }

            public Texture Texture
            {
                get => Layer.texture;
                set { if (Layer.texture == value) return; Layer.texture = value; Changed(); }
            }

            public float Opacity
            {
                get => Layer.opacity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Layer.opacity, value)) return; Layer.opacity = value; Changed(); }
            }

            public TextureBlendMode BlendMode
            {
                get => Layer.blendMode;
                set { if (Layer.blendMode == value) return; Layer.blendMode = value; Changed(); }
            }

            public Vector2 Tiling
            {
                get => Layer.scale;
                set { if (Layer.scale == value) return; Layer.scale = value; Changed(); }
            }

            public Vector2 Offset
            {
                get => Layer.offset;
                set { if (Layer.offset == value) return; Layer.offset = value; Changed(); }
            }
        }

        public sealed class GradientSettings : SettingsBase
        {
            public enum GradientType
            {
                None = GradationMode.None,
                Horizontal = GradationMode.Horizontal,
                HorizontalGradient = GradationMode.HorizontalGradient,
                Vertical = GradationMode.Vertical,
                VerticalGradient = GradationMode.VerticalGradient,
                Radial = GradationMode.Radial,
                RadialGradient = GradationMode.RadialGradient,
                Diagonal = GradationMode.Diagonal,
                DiagonalToRightBottom = GradationMode.DiagonalToRightBottom,
                DiagonalToLeftBottom = GradationMode.DiagonalToLeftBottom,
                Angle = GradationMode.Angle,
                AngleGradient = GradationMode.AngleGradient
            }

            internal GradientSettings(Unishade owner) : base(owner) { }

            public GradientType Mode
            {
                get => (GradientType)Owner.gradation;
                set { var converted = (GradationMode)value; if (Owner.gradation == converted) return; Owner.gradation = converted; Changed(); }
            }

            public float Intensity
            {
                get => Owner.gradationIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.gradationIntensity, value)) return; Owner.gradationIntensity = value; Changed(); }
            }

            public ColorFilter ColorFilter
            {
                get => Owner.gradationColorFilter;
                set { if (Owner.gradationColorFilter == value) return; Owner.gradationColorFilter = value; Changed(); }
            }

            public Color Color1
            {
                get => Owner.gradationColor1;
                set { if (Owner.gradationColor1 == value) return; Owner.gradationColor1 = value; Changed(); }
            }

            public Color Color2
            {
                get => Owner.gradationColor2;
                set { if (Owner.gradationColor2 == value) return; Owner.gradationColor2 = value; Changed(); }
            }

            public Color Color3
            {
                get => Owner.gradationColor3;
                set { if (Owner.gradationColor3 == value) return; Owner.gradationColor3 = value; Changed(); }
            }

            public Color Color4
            {
                get => Owner.gradationColor4;
                set { if (Owner.gradationColor4 == value) return; Owner.gradationColor4 = value; Changed(); }
            }

            public Gradient Ramp
            {
                get => Owner.gradationGradient;
                set { if (Owner.gradationGradient == value) return; Owner.gradationGradient = value ?? new Gradient(); Changed(); }
            }

            public float Offset
            {
                get => Owner.gradationOffset;
                set { value = Mathf.Clamp(value, -1f, 1f); if (Mathf.Approximately(Owner.gradationOffset, value)) return; Owner.gradationOffset = value; Changed(); }
            }

            public float Scale
            {
                get => Owner.gradationScale;
                set { value = Mathf.Clamp(value, 0.01f, 10f); if (Mathf.Approximately(Owner.gradationScale, value)) return; Owner.gradationScale = value; Changed(); }
            }

            public float Rotation
            {
                get => Owner.gradationRotation;
                set { value = Mathf.Repeat(value, 360f); if (Mathf.Approximately(Owner.gradationRotation, value)) return; Owner.gradationRotation = value; Changed(); }
            }

            public bool Reverse
            {
                get => Owner.gradationReverse;
                set { if (Owner.gradationReverse == value) return; Owner.gradationReverse = value; Changed(); }
            }
        }

        public sealed class EdgeSettings : SettingsBase
        {
            public enum ModeType { None, Plain, Shiny }
            internal EdgeSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.edge;
                set { var converted = (EdgeFilter)value; if (Owner.edge == converted) return; Owner.edge = converted; Changed(); }
            }

            public float Width
            {
                get => Owner.edgeWidth;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.edgeWidth, value)) return; Owner.edgeWidth = value; Changed(); }
            }

            public ColorFilter ColorFilter
            {
                get => Owner.edgeColorFilter;
                set { if (Owner.edgeColorFilter == value) return; Owner.edgeColorFilter = value; Changed(); }
            }

            public Color Color
            {
                get => Owner.edgeColor;
                set { if (Owner.edgeColor == value) return; Owner.edgeColor = value; Changed(); }
            }

            public bool Glow
            {
                get => Owner.edgeColorGlow;
                set { if (Owner.edgeColorGlow == value) return; Owner.edgeColorGlow = value; Changed(); }
            }

            public float ShinyRate
            {
                get => Owner.edgeShinyRate;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.edgeShinyRate, value)) return; Owner.edgeShinyRate = value; Changed(); }
            }

            public float ShinyWidth
            {
                get => Owner.edgeShinyWidth;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.edgeShinyWidth, value)) return; Owner.edgeShinyWidth = value; Changed(); }
            }

            public float ShinySpeed
            {
                get => Owner.edgeShinyAutoPlaySpeed;
                set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.edgeShinyAutoPlaySpeed, value)) return; Owner.edgeShinyAutoPlaySpeed = value; Changed(); }
            }

            public PatternArea PatternArea
            {
                get => Owner.patternArea;
                set { if (Owner.patternArea == value) return; Owner.patternArea = value; Changed(); }
            }
        }

        public sealed class DetailSettings : SettingsBase
        {
            public enum ModeType { None, Masking, Multiply, Additive, Replace, MultiplyAdditive, Subtractive }
            internal DetailSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.detail;
                set { var converted = (DetailFilter)value; if (Owner.detail == converted) return; Owner.detail = converted; Changed(); }
            }

            public float Intensity
            {
                get => Owner.detailIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.detailIntensity, value)) return; Owner.detailIntensity = value; Changed(); }
            }

            public Vector2 Threshold
            {
                get => Owner.detailThreshold;
                set { value.x = Mathf.Clamp01(value.x); value.y = Mathf.Clamp01(value.y); if (Owner.detailThreshold == value) return; Owner.detailThreshold = value; Changed(); }
            }

            public Color Color
            {
                get => Owner.detailColor;
                set { if (Owner.detailColor == value) return; Owner.detailColor = value; Changed(); }
            }

            public Texture Texture
            {
                get => Owner.detailTexture;
                set { if (Owner.detailTexture == value) return; Owner.detailTexture = value; Changed(); }
            }

            public Vector2 Tiling
            {
                get => Owner.detailTextureScale;
                set { if (Owner.detailTextureScale == value) return; Owner.detailTextureScale = value; Changed(); }
            }

            public Vector2 Offset
            {
                get => Owner.detailTextureOffset;
                set { if (Owner.detailTextureOffset == value) return; Owner.detailTextureOffset = value; Changed(); }
            }

            public Vector2 Speed
            {
                get => Owner.detailTextureSpeed;
                set { if (Owner.detailTextureSpeed == value) return; Owner.detailTextureSpeed = value; Changed(); }
            }
        }

        public sealed class DistortionSettings : SettingsBase
        {
            public enum ModeType { None, Wave, Noise, Twist, Pinch, Fisheye, WaterRipple }
            internal DistortionSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.distortion;
                set { var converted = (DistortionFilter)value; if (Owner.distortion == converted) return; Owner.distortion = converted; Changed(); }
            }

            public float Intensity
            {
                get => Owner.distortionIntensity;
                set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.distortionIntensity, value)) return; Owner.distortionIntensity = value; Changed(); }
            }

            public float WaveX
            {
                get => Owner.distortionWaveX;
                set { value = Mathf.Clamp(value, 0f, 64f); if (Mathf.Approximately(Owner.distortionWaveX, value)) return; Owner.distortionWaveX = value; Changed(); }
            }

            public float WaveY
            {
                get => Owner.distortionWaveY;
                set { value = Mathf.Clamp(value, 0f, 64f); if (Mathf.Approximately(Owner.distortionWaveY, value)) return; Owner.distortionWaveY = value; Changed(); }
            }

            public float AmountX
            {
                get => Owner.distortionAmountX;
                set { value = Mathf.Clamp(value, 0f, 0.25f); if (Mathf.Approximately(Owner.distortionAmountX, value)) return; Owner.distortionAmountX = value; Changed(); }
            }

            public float AmountY
            {
                get => Owner.distortionAmountY;
                set { value = Mathf.Clamp(value, 0f, 0.25f); if (Mathf.Approximately(Owner.distortionAmountY, value)) return; Owner.distortionAmountY = value; Changed(); }
            }

            public float Speed
            {
                get => Owner.distortionSpeed;
                set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.distortionSpeed, value)) return; Owner.distortionSpeed = value; Changed(); }
            }

            public Texture Texture
            {
                get => Owner.distortionTexture;
                set { if (Owner.distortionTexture == value) return; Owner.distortionTexture = value; Changed(); }
            }

            public Vector2 Tiling
            {
                get => Owner.distortionTextureScale;
                set { if (Owner.distortionTextureScale == value) return; Owner.distortionTextureScale = value; Changed(); }
            }

            public Vector2 Offset
            {
                get => Owner.distortionTextureOffset;
                set { if (Owner.distortionTextureOffset == value) return; Owner.distortionTextureOffset = value; Changed(); }
            }

            public Vector2 RippleCenter
            {
                get => Owner.distortionRippleCenter;
                set { value.x = Mathf.Clamp01(value.x); value.y = Mathf.Clamp01(value.y); if (Owner.distortionRippleCenter == value) return; Owner.distortionRippleCenter = value; Changed(); }
            }

            public float RippleFrequency
            {
                get => Owner.distortionRippleFrequency;
                set { value = Mathf.Clamp(value, 0f, 200f); if (Mathf.Approximately(Owner.distortionRippleFrequency, value)) return; Owner.distortionRippleFrequency = value; Changed(); }
            }

            public float RippleStrength
            {
                get => Owner.distortionRippleStrength;
                set { value = Mathf.Clamp(value, 0f, 0.25f); if (Mathf.Approximately(Owner.distortionRippleStrength, value)) return; Owner.distortionRippleStrength = value; Changed(); }
            }

            public float RippleSpeed
            {
                get => Owner.distortionRippleSpeed;
                set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.distortionRippleSpeed, value)) return; Owner.distortionRippleSpeed = value; Changed(); }
            }
        }

        public sealed class HologramSettings : SettingsBase
        {
            public enum ModeType { None, Scanlines, Hologram }
            internal HologramSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.hologram;
                set { var converted = (HologramFilter)value; if (Owner.hologram == converted) return; Owner.hologram = converted; Changed(); }
            }

            public float Intensity { get => Owner.hologramIntensity; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.hologramIntensity, value)) return; Owner.hologramIntensity = value; Changed(); } }
            public float ScanlineDensity { get => Owner.hologramScanlineDensity; set { value = Mathf.Clamp(value, 1f, 256f); if (Mathf.Approximately(Owner.hologramScanlineDensity, value)) return; Owner.hologramScanlineDensity = value; Changed(); } }
            public float ScanlineStrength { get => Owner.hologramScanlineStrength; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.hologramScanlineStrength, value)) return; Owner.hologramScanlineStrength = value; Changed(); } }
            public float Noise { get => Owner.hologramNoise; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.hologramNoise, value)) return; Owner.hologramNoise = value; Changed(); } }
            public float RgbShift { get => Owner.hologramRgbShift; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.hologramRgbShift, value)) return; Owner.hologramRgbShift = value; Changed(); } }
            public float Speed { get => Owner.hologramSpeed; set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.hologramSpeed, value)) return; Owner.hologramSpeed = value; Changed(); } }
            public Color Color { get => Owner.hologramColor; set { if (Owner.hologramColor == value) return; Owner.hologramColor = value; Changed(); } }
        }

        public sealed class GlitchSettings : SettingsBase
        {
            public enum ModeType { None, Blocks, Lines, BlocksAndLines }
            internal GlitchSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.glitch;
                set { var converted = (GlitchFilter)value; if (Owner.glitch == converted) return; Owner.glitch = converted; Changed(); }
            }

            public float Intensity { get => Owner.glitchIntensity; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.glitchIntensity, value)) return; Owner.glitchIntensity = value; Changed(); } }
            public float BlockSize { get => Owner.glitchBlockSize; set { value = Mathf.Clamp(value, 0.005f, 0.5f); if (Mathf.Approximately(Owner.glitchBlockSize, value)) return; Owner.glitchBlockSize = value; Changed(); } }
            public float Speed { get => Owner.glitchSpeed; set { value = Mathf.Clamp(value, -5f, 5f); if (Mathf.Approximately(Owner.glitchSpeed, value)) return; Owner.glitchSpeed = value; Changed(); } }
        }

        public sealed class InnerGlowSettings : SettingsBase
        {
            public enum ModeType { None, Additive }
            internal InnerGlowSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.innerGlow;
                set { var converted = (InnerGlowFilter)value; if (Owner.innerGlow == converted) return; Owner.innerGlow = converted; Changed(); }
            }

            public float Intensity { get => Owner.innerGlowIntensity; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.innerGlowIntensity, value)) return; Owner.innerGlowIntensity = value; Changed(); } }
            public float Size { get => Owner.innerGlowSize; set { value = Mathf.Clamp(value, 0.25f, 8f); if (Mathf.Approximately(Owner.innerGlowSize, value)) return; Owner.innerGlowSize = value; Changed(); } }
            public Color Color { get => Owner.innerGlowColor; set { if (Owner.innerGlowColor == value) return; Owner.innerGlowColor = value; Changed(); } }
        }

        public sealed class OuterGlowSettings : SettingsBase
        {
            public enum ModeType { None, Additive }
            internal OuterGlowSettings(Unishade owner) : base(owner) { }

            public ModeType Mode
            {
                get => (ModeType)Owner.outerGlow;
                set { var converted = (OuterGlowFilter)value; if (Owner.outerGlow == converted) return; Owner.outerGlow = converted; Changed(); }
            }

            public float Intensity { get => Owner.outerGlowIntensity; set { value = Mathf.Clamp(value, 0f, 4f); if (Mathf.Approximately(Owner.outerGlowIntensity, value)) return; Owner.outerGlowIntensity = value; Changed(); } }
            public float Size { get => Owner.outerGlowSize; set { value = Mathf.Clamp(value, 0.25f, 16f); if (Mathf.Approximately(Owner.outerGlowSize, value)) return; Owner.outerGlowSize = value; Changed(); } }
            public float Softness { get => Owner.outerGlowSoftness; set { value = Mathf.Clamp01(value); if (Mathf.Approximately(Owner.outerGlowSoftness, value)) return; Owner.outerGlowSoftness = value; Changed(); } }
            public Color Color { get => Owner.outerGlowColor; set { if (Owner.outerGlowColor == value) return; Owner.outerGlowColor = value; Changed(); } }
        }

    }

    // Keeps this partial source file visible to Unity's MonoScript importer.
    internal static class UnishadeApi
    {
    }
}
