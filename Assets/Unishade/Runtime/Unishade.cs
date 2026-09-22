using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unishade
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("Unishade")]
    public sealed partial class Unishade : MonoBehaviour
    {
        public enum RendererMode { Auto, SpriteRenderer, MeshRenderer }
        public enum ToneFilter { None, Grayscale, Sepia, Negative, Retro, Posterize }
        public enum ColorFilter { None, Multiply, Additive, Subtractive, Replace, MultiplyLuminance, MultiplyAdditive, HsvModifier, Contrast }
        public enum SamplingFilter { None, BlurFast, BlurMedium, BlurDetail, Pixelation, RgbShift, EdgeLuminance, EdgeAlpha }
        public enum TransitionFilter { None, Fade, Cutoff, Dissolve, Shiny, Mask, Melt, Burn, Blaze, Pattern }
        public enum TargetMode { None, Hue, Luminance }
        public enum GradationMode
        {
            None = 0, Horizontal = 1, HorizontalGradient = 2, Vertical = 3,
            VerticalGradient = 4, Radial = 5, RadialGradient = 12,
            Diagonal = 11, DiagonalToRightBottom = 7, DiagonalToLeftBottom = 8,
            Angle = 9, AngleGradient = 10
        }
        public enum DetailFilter { None, Masking, Multiply, Additive, Subtractive = 6, Replace = 4, MultiplyAdditive = 5 }
        public enum EdgeFilter { None, Plain, Shiny }
        public enum PatternArea { All, Inner, Edge }
        public enum BlendType { Custom, AlphaBlend, Multiply, Additive, SoftAdditive, MultiplyAdditive }
        public enum UpdateMode { Always, PlayMode, Manual }
        public enum DistortionFilter { None, Wave, Noise, Twist, Pinch, Fisheye, WaterRipple }
        public enum DitherFilter { None, Alpha }
        public enum VertexShakeFilter { None, Shake }
        public enum ScrollTextureFilter { None, Scroll }
        public enum HandDrawnFilter { None, HandDrawn }
        public enum HologramFilter { None, Scanlines, Hologram }
        public enum GlitchFilter { None, Blocks, Lines, BlocksAndLines }
        public enum InnerGlowFilter { None, Additive }
        public enum OuterGlowFilter { None, Additive }
        public enum TextureBlendSource { VertexColor, Texture }
        public enum TextureBlendLayout { RGB, BlackAndWhite }
        public enum TextureBlendMode { Normal, Multiply, Additive, Subtractive, Screen, Overlay }
        public enum EffectPreset { Clear, Hologram, Glitch, Distortion, InnerGlow }

        [Serializable]
        private sealed class TextureBlendLayer
        {
            [SerializeField] internal Texture texture;
            [Range(0, 1)] [SerializeField] internal float opacity = 1;
            [SerializeField] internal TextureBlendMode blendMode = TextureBlendMode.Normal;
            [SerializeField] internal Vector2 scale = Vector2.one;
            [SerializeField] internal Vector2 offset;
        }

        private const string ShaderName = "Unishade/Renderer Effect";

        [Header("Renderer")]
        [Tooltip("Automatically detects SpriteRenderer or MeshRenderer on this GameObject.")]
        [SerializeField] private RendererMode rendererMode = RendererMode.Auto;
        [Tooltip("Optional material used as the texture source. If empty, the current renderer material is used.")]
        [SerializeField] private Material sourceMaterial;
        [SerializeField] private UnityEngine.Color tint = UnityEngine.Color.white;
        [Tooltip("Keeps effect coordinates proportional when the object is not square.")]
        [SerializeField] private bool keepAspectRatio = true;
        [Range(0, 360)] [SerializeField] private float effectRotation;

        [Header("Animation")]
        [Tooltip("Always animates in the Editor and Play Mode, only in Play Mode, or only when advanced manually.")]
        [SerializeField] private UpdateMode updateMode = UpdateMode.Always;
        [SerializeField] private bool animationEnabled = true;
        [Range(-5, 5)] [SerializeField] private float animationSpeed = 1;

        [Header("Dither")]
        [SerializeField] private DitherFilter dither = DitherFilter.None;
        [Range(0, 1)] [SerializeField] private float ditherIntensity = 1;
        [Range(0.01f, 2)] [SerializeField] private float ditherScale = 1;

        [Header("Vertex Shake")]
        [SerializeField] private VertexShakeFilter vertexShake = VertexShakeFilter.None;
        [SerializeField] private Vector3 vertexShakeSpeed = new Vector3(41, 49, 45);
        [Range(0, 5)] [SerializeField] private float vertexShakeSpeedMultiplier = 1;
        [SerializeField] private Vector3 vertexShakeMaxDisplacement = new Vector3(0.1f, 0.1f, 0.1f);
        [Range(0, 1)] [SerializeField] private float vertexShakeBlend = 1;

        [Header("Scroll Texture")]
        [SerializeField] private ScrollTextureFilter scrollTexture = ScrollTextureFilter.None;
        [SerializeField] private Vector2 scrollTextureSpeed = Vector2.one;

        [Header("Hand Drawn")]
        [SerializeField] private HandDrawnFilter handDrawn = HandDrawnFilter.None;
        [Range(0, 50)] [SerializeField] private float handDrawnAmount = 10;
        [Range(1, 15)] [SerializeField] private float handDrawnSpeed = 5;

        [Header("Tone")]
        [SerializeField] private ToneFilter tone = ToneFilter.None;
        [Range(0, 1)] [SerializeField] private float toneIntensity = 1;

        [Header("Color")]
        [SerializeField] private ColorFilter colorFilter = ColorFilter.None;
        [Range(0, 1)] [SerializeField] private float colorIntensity = 1;
        [SerializeField] private UnityEngine.Color color = UnityEngine.Color.white;
        [SerializeField] private bool colorGlow;

        [Header("Sampling")]
        [SerializeField] private SamplingFilter sampling = SamplingFilter.None;
        [Range(0, 1)] [SerializeField] private float samplingIntensity = 0.5f;
        [Range(0.5f, 10)] [SerializeField] private float samplingWidth = 1;
        [Range(0.01f, 100)] [SerializeField] private float samplingScale = 1;

        [Header("Transition")]
        [SerializeField] private TransitionFilter transition = TransitionFilter.None;
        [Range(0, 1)] [SerializeField] private float transitionRate = 0.5f;
        [SerializeField] private bool transitionReverse;
        [SerializeField] private Texture transitionTexture;
        [SerializeField] private Vector2 transitionTextureScale = Vector2.one;
        [SerializeField] private Vector2 transitionTextureOffset;
        [SerializeField] private Vector2 transitionTextureSpeed;
        [Range(0, 1)] [SerializeField] private float transitionWidth = 0.2f;
        [Range(0, 1)] [SerializeField] private float transitionSoftness = 0.2f;
        [SerializeField] private Vector2 transitionRange = new Vector2(0, 1);
        [SerializeField] private ColorFilter transitionColorFilter = ColorFilter.MultiplyAdditive;
        [SerializeField] private UnityEngine.Color transitionColor = new UnityEngine.Color(0, 0.5f, 1, 1);
        [SerializeField] private bool transitionColorGlow;
        [SerializeField] private bool transitionPatternReverse;
        [Range(-5, 5)] [SerializeField] private float transitionAutoPlaySpeed;
        [SerializeField] private Gradient transitionGradient = new Gradient();

        [Header("Target")]
        [SerializeField] private TargetMode target = TargetMode.None;
        [SerializeField] private UnityEngine.Color targetColor = UnityEngine.Color.white;
        [Range(0, 1)] [SerializeField] private float targetRange = 0.1f;
        [Range(0, 1)] [SerializeField] private float targetSoftness = 0.5f;

        [Header("Blending")]
        [SerializeField] private BlendType blendType = BlendType.AlphaBlend;
        [SerializeField] private BlendMode sourceBlend = BlendMode.One;
        [SerializeField] private BlendMode destinationBlend = BlendMode.OneMinusSrcAlpha;

        [Header("Texture Blending")]
        [SerializeField] private TextureBlendSource textureBlendSource = TextureBlendSource.Texture;
        [SerializeField] private TextureBlendLayout textureBlendLayout = TextureBlendLayout.RGB;
        [SerializeField] private Texture blendingMask;
        [SerializeField] private Vector2 blendingMaskScale = Vector2.one;
        [SerializeField] private Vector2 blendingMaskOffset;
        [SerializeField] private TextureBlendLayer blendingLayerG = new TextureBlendLayer();
        [SerializeField] private TextureBlendLayer blendingLayerB = new TextureBlendLayer();
        [SerializeField] private TextureBlendLayer blendingLayerWhite = new TextureBlendLayer();

        [Header("Gradient")]
        [SerializeField] private GradationMode gradation = GradationMode.None;
        [Range(0, 1)] [SerializeField] private float gradationIntensity = 1;
        [SerializeField] private ColorFilter gradationColorFilter = ColorFilter.Multiply;
        [SerializeField] private UnityEngine.Color gradationColor1 = UnityEngine.Color.white;
        [SerializeField] private UnityEngine.Color gradationColor2 = UnityEngine.Color.white;
        [SerializeField] private UnityEngine.Color gradationColor3 = UnityEngine.Color.white;
        [SerializeField] private UnityEngine.Color gradationColor4 = UnityEngine.Color.white;
        [SerializeField] private Gradient gradationGradient = new Gradient();
        [Range(-1, 1)] [SerializeField] private float gradationOffset;
        [Range(0.01f, 10)] [SerializeField] private float gradationScale = 1;
        [Range(0, 360)] [SerializeField] private float gradationRotation;
        [SerializeField] private bool gradationReverse;

        [Header("Edge")]
        [SerializeField] private EdgeFilter edge = EdgeFilter.None;
        [Range(0, 1)] [SerializeField] private float edgeWidth = 0.5f;
        [SerializeField] private ColorFilter edgeColorFilter = ColorFilter.Replace;
        [SerializeField] private UnityEngine.Color edgeColor = UnityEngine.Color.white;
        [SerializeField] private bool edgeColorGlow;
        [HideInInspector] [SerializeField] private bool edgeShiny;
        [Range(0, 1)] [SerializeField] private float edgeShinyRate = 0.5f;
        [Range(0, 1)] [SerializeField] private float edgeShinyWidth = 0.5f;
        [Range(-5, 5)] [SerializeField] private float edgeShinyAutoPlaySpeed = 1;
        [SerializeField] private PatternArea patternArea = PatternArea.Inner;

        [Header("Detail")]
        [SerializeField] private DetailFilter detail = DetailFilter.None;
        [Range(0, 1)] [SerializeField] private float detailIntensity = 1;
        [SerializeField] private Vector2 detailThreshold = new Vector2(0, 1);
        [SerializeField] private UnityEngine.Color detailColor = UnityEngine.Color.white;
        [SerializeField] private Texture detailTexture;
        [SerializeField] private Vector2 detailTextureScale = Vector2.one;
        [SerializeField] private Vector2 detailTextureOffset;
        [SerializeField] private Vector2 detailTextureSpeed;

        [Header("Distortion")]
        [SerializeField] private DistortionFilter distortion = DistortionFilter.None;
        [Range(0, 1)] [SerializeField] private float distortionIntensity = 0.5f;
        [Range(0, 64)] [SerializeField] private float distortionWaveX = 12;
        [Range(0, 64)] [SerializeField] private float distortionWaveY = 8;
        [Range(0, 0.25f)] [SerializeField] private float distortionAmountX = 0.03f;
        [Range(0, 0.25f)] [SerializeField] private float distortionAmountY = 0.03f;
        [Range(-5, 5)] [SerializeField] private float distortionSpeed = 1;
        [SerializeField] private Texture distortionTexture;
        [SerializeField] private Vector2 distortionTextureScale = Vector2.one;
        [SerializeField] private Vector2 distortionTextureOffset;
        [SerializeField] private Vector2 distortionRippleCenter = new Vector2(0.5f, 0.5f);
        [Range(0, 200)] [SerializeField] private float distortionRippleFrequency = 32;
        [Range(0, 0.25f)] [SerializeField] private float distortionRippleStrength = 0.03f;
        [Range(-5, 5)] [SerializeField] private float distortionRippleSpeed = 1;

        [Header("Hologram")]
        [SerializeField] private HologramFilter hologram = HologramFilter.None;
        [Range(0, 1)] [SerializeField] private float hologramIntensity = 0.75f;
        [Range(1, 256)] [SerializeField] private float hologramScanlineDensity = 48;
        [Range(0, 1)] [SerializeField] private float hologramScanlineStrength = 0.35f;
        [Range(0, 1)] [SerializeField] private float hologramNoise = 0.2f;
        [Range(0, 1)] [SerializeField] private float hologramRgbShift = 0.15f;
        [Range(-5, 5)] [SerializeField] private float hologramSpeed = 1;
        [SerializeField] private UnityEngine.Color hologramColor = new UnityEngine.Color(0.1f, 0.8f, 1f, 1f);

        [Header("Glitch")]
        [SerializeField] private GlitchFilter glitch = GlitchFilter.None;
        [Range(0, 1)] [SerializeField] private float glitchIntensity = 0.5f;
        [Range(0.005f, 0.5f)] [SerializeField] private float glitchBlockSize = 0.08f;
        [Range(-5, 5)] [SerializeField] private float glitchSpeed = 1;

        [Header("Inner Glow")]
        [SerializeField] private InnerGlowFilter innerGlow = InnerGlowFilter.None;
        [Range(0, 1)] [SerializeField] private float innerGlowIntensity = 0.75f;
        [Range(0.25f, 8)] [SerializeField] private float innerGlowSize = 1.5f;
        [SerializeField] private UnityEngine.Color innerGlowColor = UnityEngine.Color.white;

        [Header("Outer Glow")]
        [SerializeField] private OuterGlowFilter outerGlow = OuterGlowFilter.None;
        [Range(0, 4)] [SerializeField] private float outerGlowIntensity = 1;
        [Range(0.25f, 16)] [SerializeField] private float outerGlowSize = 3;
        [Range(0, 1)] [SerializeField] private float outerGlowSoftness = 0.75f;
        [ColorUsage(true, true)] [SerializeField] private UnityEngine.Color outerGlowColor = new UnityEngine.Color(0.1f, 0.8f, 1f, 1f);

        // Kept only to migrate scenes created before the mode dropdowns were introduced.
        [HideInInspector] [SerializeField] private bool hologramEnabled;
        [HideInInspector] [SerializeField] private bool glitchEnabled;
        [HideInInspector] [SerializeField] private bool innerGlowEnabled;
        // Kept only to migrate scenes that used the old Edge Enabled + Edge Shiny checkboxes.
        [HideInInspector] [SerializeField] private bool edgeEnabled;

        private Renderer targetRenderer;
        private Material[] originalMaterials;
        private Material[] effectMaterials;
        private Texture2D gradationRamp;
        private Texture2D transitionRamp;
        private MeshFilter meshFilter;
        private float animationTime;
        private double lastAnimationClock;

        private static readonly string[] ToneKeywords = { "", "TONE_GRAYSCALE", "TONE_SEPIA", "TONE_NEGATIVE", "TONE_RETRO", "TONE_POSTERIZE" };
        private static readonly string[] SamplingKeywords = { "", "SAMPLING_BLUR_FAST", "SAMPLING_BLUR_MEDIUM", "SAMPLING_BLUR_DETAIL", "SAMPLING_PIXELATION", "SAMPLING_RGB_SHIFT", "SAMPLING_EDGE_LUMINANCE", "SAMPLING_EDGE_ALPHA" };
        private static readonly string[] TransitionKeywords = { "", "TRANSITION_FADE", "TRANSITION_CUTOFF", "TRANSITION_DISSOLVE", "TRANSITION_SHINY", "TRANSITION_MASK", "TRANSITION_MELT", "TRANSITION_BURN", "TRANSITION_BLAZE", "TRANSITION_PATTERN" };
        private static readonly string[] EdgeKeywords = { "", "EDGE_PLAIN", "EDGE_SHINY" };
        private static readonly string[] DetailKeywords = { "", "DETAIL_MASKING", "DETAIL_MULTIPLY", "DETAIL_ADDITIVE", "DETAIL_REPLACE", "DETAIL_MULTIPLY_ADDITIVE", "DETAIL_SUBTRACTIVE" };
        private static readonly string[] TargetKeywords = { "", "TARGET_HUE", "TARGET_LUMINANCE" };
        private static readonly string[] DistortionKeywords = { "", "DISTORTION_WAVE", "DISTORTION_NOISE", "DISTORTION_TWIST", "DISTORTION_PINCH", "DISTORTION_FISHEYE", "DISTORTION_WATER_RIPPLE" };

        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int RendererColor = Shader.PropertyToID("_RendererColor");
        private static readonly int Tint = Shader.PropertyToID("_Color");
        private static readonly int UvRect = Shader.PropertyToID("_UnishadeUVRect");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int TextureBlendSourceId = Shader.PropertyToID("_TextureBlendSource");
        private static readonly int TextureBlendLayoutId = Shader.PropertyToID("_TextureBlendLayout");
        private static readonly int BlendingMask = Shader.PropertyToID("_BlendingMask");
        private static readonly int BlendingMaskST = Shader.PropertyToID("_BlendingMask_ST");
        private static readonly int BlendingTextureG = Shader.PropertyToID("_BlendingTextureG");
        private static readonly int BlendingTextureGST = Shader.PropertyToID("_BlendingTextureG_ST");
        private static readonly int BlendingTextureGOpacity = Shader.PropertyToID("_BlendingTextureG_Opacity");
        private static readonly int BlendingTextureGMode = Shader.PropertyToID("_BlendingTextureG_Mode");
        private static readonly int BlendingTextureB = Shader.PropertyToID("_BlendingTextureB");
        private static readonly int BlendingTextureBST = Shader.PropertyToID("_BlendingTextureB_ST");
        private static readonly int BlendingTextureBOpacity = Shader.PropertyToID("_BlendingTextureB_Opacity");
        private static readonly int BlendingTextureBMode = Shader.PropertyToID("_BlendingTextureB_Mode");
        private static readonly int BlendingTextureWhite = Shader.PropertyToID("_BlendingTextureWhite");
        private static readonly int BlendingTextureWhiteST = Shader.PropertyToID("_BlendingTextureWhite_ST");
        private static readonly int BlendingTextureWhiteOpacity = Shader.PropertyToID("_BlendingTextureWhite_Opacity");
        private static readonly int BlendingTextureWhiteMode = Shader.PropertyToID("_BlendingTextureWhite_Mode");
        private static readonly int ToneIntensity = Shader.PropertyToID("_ToneIntensity");
        private static readonly int ColorFilterId = Shader.PropertyToID("_ColorFilter");
        private static readonly int ColorValue = Shader.PropertyToID("_ColorValue");
        private static readonly int ColorIntensity = Shader.PropertyToID("_ColorIntensity");
        private static readonly int ColorGlow = Shader.PropertyToID("_ColorGlow");
        private static readonly int SamplingIntensity = Shader.PropertyToID("_SamplingIntensity");
        private static readonly int SamplingWidth = Shader.PropertyToID("_SamplingWidth");
        private static readonly int SamplingScale = Shader.PropertyToID("_SamplingScale");
        private static readonly int TransitionTex = Shader.PropertyToID("_TransitionTex");
        private static readonly int TransitionTexST = Shader.PropertyToID("_TransitionTex_ST");
        private static readonly int TransitionTexSpeed = Shader.PropertyToID("_TransitionTex_Speed");
        private static readonly int TransitionRate = Shader.PropertyToID("_TransitionRate");
        private static readonly int TransitionReverse = Shader.PropertyToID("_TransitionReverse");
        private static readonly int TransitionWidth = Shader.PropertyToID("_TransitionWidth");
        private static readonly int TransitionSoftness = Shader.PropertyToID("_TransitionSoftness");
        private static readonly int TransitionRange = Shader.PropertyToID("_TransitionRange");
        private static readonly int TransitionColorFilter = Shader.PropertyToID("_TransitionColorFilter");
        private static readonly int TransitionColor = Shader.PropertyToID("_TransitionColor");
        private static readonly int TransitionColorGlow = Shader.PropertyToID("_TransitionColorGlow");
        private static readonly int TransitionPatternReverse = Shader.PropertyToID("_TransitionPatternReverse");
        private static readonly int TransitionAutoPlaySpeed = Shader.PropertyToID("_TransitionAutoPlaySpeed");
        private static readonly int TransitionGradientTex = Shader.PropertyToID("_TransitionGradientTex");
        private static readonly int TargetColor = Shader.PropertyToID("_TargetColor");
        private static readonly int TargetRange = Shader.PropertyToID("_TargetRange");
        private static readonly int TargetSoftness = Shader.PropertyToID("_TargetSoftness");
        private static readonly int EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        private static readonly int EdgeShinyRate = Shader.PropertyToID("_EdgeShinyRate");
        private static readonly int EdgeShinySpeed = Shader.PropertyToID("_EdgeShinyAutoPlaySpeed");
        private static readonly int EdgeShinyWidth = Shader.PropertyToID("_EdgeShinyWidth");
        private static readonly int EdgeColorFilter = Shader.PropertyToID("_EdgeColorFilter");
        private static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
        private static readonly int EdgeColorGlow = Shader.PropertyToID("_EdgeColorGlow");
        private static readonly int PatternAreaId = Shader.PropertyToID("_PatternArea");
        private static readonly int DetailIntensity = Shader.PropertyToID("_DetailIntensity");
        private static readonly int DetailThreshold = Shader.PropertyToID("_DetailThreshold");
        private static readonly int DetailColor = Shader.PropertyToID("_DetailColor");
        private static readonly int DetailTex = Shader.PropertyToID("_DetailTex");
        private static readonly int DetailTexST = Shader.PropertyToID("_DetailTex_ST");
        private static readonly int DetailTexSpeed = Shader.PropertyToID("_DetailTex_Speed");
        private static readonly int GradationIntensity = Shader.PropertyToID("_GradationIntensity");
        private static readonly int GradationColorFilter = Shader.PropertyToID("_GradationColorFilter");
        private static readonly int GradationColor1 = Shader.PropertyToID("_GradationColor1");
        private static readonly int GradationColor2 = Shader.PropertyToID("_GradationColor2");
        private static readonly int GradationColor3 = Shader.PropertyToID("_GradationColor3");
        private static readonly int GradationColor4 = Shader.PropertyToID("_GradationColor4");
        private static readonly int GradationTex = Shader.PropertyToID("_GradationTex");
        private static readonly int GradationTexST = Shader.PropertyToID("_GradationTex_ST");
        private static readonly int GradationRadial = Shader.PropertyToID("_GradationRadial");
        private static readonly int RootViewMatrix = Shader.PropertyToID("_RootViewMatrix");
        private static readonly int GradViewMatrix = Shader.PropertyToID("_GradViewMatrix");
        private static readonly int CanvasToWorldMatrix = Shader.PropertyToID("_CanvasToWorldMatrix");
        private static readonly int UnishadeTime = Shader.PropertyToID("_UnishadeTime");
        private static readonly int DitherIntensity = Shader.PropertyToID("_DitherIntensity");
        private static readonly int DitherScale = Shader.PropertyToID("_DitherScale");
        private static readonly int VertexShakeSpeed = Shader.PropertyToID("_VertexShakeSpeed");
        private static readonly int VertexShakeSpeedMultiplier = Shader.PropertyToID("_VertexShakeSpeedMultiplier");
        private static readonly int VertexShakeDisplacement = Shader.PropertyToID("_VertexShakeDisplacement");
        private static readonly int VertexShakeBlend = Shader.PropertyToID("_VertexShakeBlend");
        private static readonly int ScrollTextureSpeed = Shader.PropertyToID("_ScrollTextureSpeed");
        private static readonly int HandDrawnAmount = Shader.PropertyToID("_HandDrawnAmount");
        private static readonly int HandDrawnSpeed = Shader.PropertyToID("_HandDrawnSpeed");
        private static readonly int DistortionIntensity = Shader.PropertyToID("_DistortionIntensity");
        private static readonly int DistortionWave = Shader.PropertyToID("_DistortionWave");
        private static readonly int DistortionAmount = Shader.PropertyToID("_DistortionAmount");
        private static readonly int DistortionSpeed = Shader.PropertyToID("_DistortionSpeed");
        private static readonly int DistortionTex = Shader.PropertyToID("_DistortionTex");
        private static readonly int DistortionTexST = Shader.PropertyToID("_DistortionTex_ST");
        private static readonly int DistortionRippleCenter = Shader.PropertyToID("_DistortionRippleCenter");
        private static readonly int DistortionRippleFrequency = Shader.PropertyToID("_DistortionRippleFrequency");
        private static readonly int DistortionRippleStrength = Shader.PropertyToID("_DistortionRippleStrength");
        private static readonly int DistortionRippleSpeed = Shader.PropertyToID("_DistortionRippleSpeed");
        private static readonly int HologramIntensity = Shader.PropertyToID("_HologramIntensity");
        private static readonly int HologramScanlineDensity = Shader.PropertyToID("_HologramScanlineDensity");
        private static readonly int HologramScanlineStrength = Shader.PropertyToID("_HologramScanlineStrength");
        private static readonly int HologramNoise = Shader.PropertyToID("_HologramNoise");
        private static readonly int HologramRgbShift = Shader.PropertyToID("_HologramRgbShift");
        private static readonly int HologramSpeed = Shader.PropertyToID("_HologramSpeed");
        private static readonly int HologramColor = Shader.PropertyToID("_HologramColor");
        private static readonly int HologramMode = Shader.PropertyToID("_HologramMode");
        private static readonly int GlitchIntensity = Shader.PropertyToID("_GlitchIntensity");
        private static readonly int GlitchBlockSize = Shader.PropertyToID("_GlitchBlockSize");
        private static readonly int GlitchSpeed = Shader.PropertyToID("_GlitchSpeed");
        private static readonly int GlitchMode = Shader.PropertyToID("_GlitchMode");
        private static readonly int InnerGlowIntensity = Shader.PropertyToID("_InnerGlowIntensity");
        private static readonly int InnerGlowSize = Shader.PropertyToID("_InnerGlowSize");
        private static readonly int InnerGlowColor = Shader.PropertyToID("_InnerGlowColor");
        private static readonly int OuterGlowIntensity = Shader.PropertyToID("_OuterGlowIntensity");
        private static readonly int OuterGlowSize = Shader.PropertyToID("_OuterGlowSize");
        private static readonly int OuterGlowSoftness = Shader.PropertyToID("_OuterGlowSoftness");
        private static readonly int OuterGlowColor = Shader.PropertyToID("_OuterGlowColor");

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
#endif
            MigrateLegacyModes();
            EnsureTextureBlendLayers();
            EnsureMaterials();
            ApplyToMaterials();
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
            RestoreMaterials();
            DestroyRuntimeObjects();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
            RestoreMaterials();
            DestroyRuntimeObjects();
        }

        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            MigrateLegacyModes();
            EnsureTextureBlendLayers();
            EnsureMaterials();
            ApplyToMaterials();
        }

        private void Update()
        {
            if (!isActiveAndEnabled) return;
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            EnsureMaterials();

            var now = (double)Time.realtimeSinceStartup;
            if (lastAnimationClock <= 0) lastAnimationClock = now;
            var deltaTime = Mathf.Clamp((float)(now - lastAnimationClock), 0, 0.1f);
            lastAnimationClock = now;
            if (animationEnabled && updateMode != UpdateMode.Manual &&
                (updateMode == UpdateMode.Always || Application.isPlaying))
                animationTime += deltaTime * animationSpeed;

            ApplyToMaterials();
        }

#if UNITY_EDITOR
        private void EditorUpdate()
        {
            if (Application.isPlaying || !isActiveAndEnabled || !animationEnabled || updateMode != UpdateMode.Always)
                return;

            var now = EditorApplication.timeSinceStartup;
            if (lastAnimationClock <= 0) lastAnimationClock = now;
            var deltaTime = Mathf.Clamp((float)(now - lastAnimationClock), 0, 0.1f);
            lastAnimationClock = now;
            if (deltaTime <= 0) return;

            animationTime += deltaTime * animationSpeed;
            EnsureMaterials();
            ApplyToMaterials();
            SceneView.RepaintAll();
        }
#endif

        /// <summary>Advances animated effects when Update Mode is Manual.</summary>
        public void AdvanceAnimation(float deltaTime)
        {
            animationTime += Mathf.Max(0, deltaTime) * animationSpeed;
            EnsureMaterials();
            ApplyToMaterials();
#if UNITY_EDITOR
            if (!Application.isPlaying) SceneView.RepaintAll();
#endif
        }

        /// <summary>Applies a small starting preset without replacing the current material or renderer setup.</summary>
        public void ApplyPreset(EffectPreset preset)
        {
            switch (preset)
            {
                case EffectPreset.Clear:
                    distortion = DistortionFilter.None;
                    hologram = HologramFilter.None;
                    glitch = GlitchFilter.None;
                    innerGlow = InnerGlowFilter.None;
                    break;
                case EffectPreset.Hologram:
                    distortion = DistortionFilter.None;
                    hologram = HologramFilter.Hologram;
                    glitch = GlitchFilter.None;
                    innerGlow = InnerGlowFilter.None;
                    hologramIntensity = 0.8f;
                    hologramScanlineDensity = 48;
                    hologramScanlineStrength = 0.35f;
                    hologramNoise = 0.25f;
                    hologramRgbShift = 0.18f;
                    hologramSpeed = 1;
                    hologramColor = new UnityEngine.Color(0.1f, 0.8f, 1f, 1f);
                    break;
                case EffectPreset.Glitch:
                    distortion = DistortionFilter.None;
                    hologram = HologramFilter.None;
                    glitch = GlitchFilter.BlocksAndLines;
                    innerGlow = InnerGlowFilter.None;
                    glitchIntensity = 0.65f;
                    glitchBlockSize = 0.08f;
                    glitchSpeed = 1.5f;
                    break;
                case EffectPreset.Distortion:
                    distortion = DistortionFilter.Wave;
                    hologram = HologramFilter.None;
                    glitch = GlitchFilter.None;
                    innerGlow = InnerGlowFilter.None;
                    distortionIntensity = 0.5f;
                    distortionWaveX = 12;
                    distortionWaveY = 8;
                    distortionAmountX = 0.03f;
                    distortionAmountY = 0.03f;
                    distortionSpeed = 1;
                    break;
                case EffectPreset.InnerGlow:
                    distortion = DistortionFilter.None;
                    hologram = HologramFilter.None;
                    glitch = GlitchFilter.None;
                    innerGlow = InnerGlowFilter.Additive;
                    innerGlowIntensity = 0.8f;
                    innerGlowSize = 1.5f;
                    innerGlowColor = new UnityEngine.Color(0.2f, 0.8f, 1f, 1f);
                    break;
            }

            ApplyToMaterials();
        }

        private void EnsureMaterials()
        {
            var renderer = ResolveRenderer();
            if (!renderer) return;

            var shared = renderer.sharedMaterials;
            if (shared == null || shared.Length == 0) return;
            if (effectMaterials != null && targetRenderer == renderer && MaterialsAreAssigned(renderer)) return;

            RestoreMaterials();
            targetRenderer = renderer;
            originalMaterials = (Material[])shared.Clone();
            effectMaterials = new Material[shared.Length];

            var effectShader = Shader.Find(ShaderName);
            if (!effectShader)
            {
                Debug.LogError($"{ShaderName} was not found. Let Unity finish importing the Unishade shaders.", this);
                return;
            }

            for (var i = 0; i < shared.Length; i++)
            {
                var source = sourceMaterial ? sourceMaterial : shared[i];
                var material = new Material(effectShader)
                {
                    name = "Unishade Renderer Effect",
                    hideFlags = HideFlags.HideAndDontSave
                };

                if (source)
                {
                    var texture = sourceMaterial
                        ? (source.HasProperty("_BaseMap") ? source.GetTexture("_BaseMap") : source.mainTexture)
                        : renderer is SpriteRenderer spriteRenderer && spriteRenderer.sprite
                            ? spriteRenderer.sprite.texture
                            : (source.HasProperty("_BaseMap") ? source.GetTexture("_BaseMap") : source.mainTexture);
                    if (texture) material.SetTexture(MainTex, texture);
                    if (source.HasProperty("_BaseColor")) tint = source.GetColor("_BaseColor");
                    else if (source.HasProperty("_Color")) tint = source.GetColor("_Color");
                }

                effectMaterials[i] = material;
            }

            renderer.sharedMaterials = effectMaterials;
        }

        private Renderer ResolveRenderer()
        {
            if (rendererMode == RendererMode.SpriteRenderer || rendererMode == RendererMode.Auto)
            {
                var sprite = GetComponent<SpriteRenderer>();
                if (sprite && (rendererMode == RendererMode.SpriteRenderer || !GetComponent<MeshRenderer>())) return sprite;
            }

            if (rendererMode == RendererMode.MeshRenderer || rendererMode == RendererMode.Auto)
            {
                var mesh = GetComponent<MeshRenderer>();
                if (mesh)
                {
                    meshFilter = GetComponent<MeshFilter>();
                    return mesh;
                }
            }

            return null;
        }

        private bool MaterialsAreAssigned(Renderer renderer)
        {
            var assigned = renderer.sharedMaterials;
            if (assigned == null || assigned.Length != effectMaterials.Length) return false;
            for (var i = 0; i < assigned.Length; i++) if (assigned[i] != effectMaterials[i]) return false;
            return true;
        }

        private void RestoreMaterials()
        {
            if (targetRenderer && originalMaterials != null && MaterialsAreAssigned(targetRenderer))
                targetRenderer.sharedMaterials = originalMaterials;
            targetRenderer = null;
            originalMaterials = null;
            effectMaterials = null;
        }

        private void ApplyToMaterials()
        {
            if (effectMaterials == null || effectMaterials.Length == 0) return;
            RebuildRamps();
            var bounds = GetLocalBounds();
            var uvRect = GetUvRect();
            var root = CreateViewMatrix(bounds, effectRotation);
            var gradRoot = CreateViewMatrix(bounds, GetGradationRotation());
            var blend = ConvertBlend(blendType, sourceBlend, destinationBlend);
            var spriteTexture = !sourceMaterial && targetRenderer is SpriteRenderer spriteRenderer && spriteRenderer.sprite
                ? spriteRenderer.sprite.texture
                : null;

            for (var i = 0; i < effectMaterials.Length; i++)
            {
                var material = effectMaterials[i];
                if (!material) continue;
                material.SetTexture(MainTex, spriteTexture ? spriteTexture : material.GetTexture(MainTex) ?? Texture2D.whiteTexture);
                material.SetColor(Tint, tint);
                material.SetVector(UvRect, uvRect);
                material.SetInt(SrcBlend, (int)blend.src);
                material.SetInt(DstBlend, (int)blend.dst);
                material.SetInt(TextureBlendSourceId, (int)textureBlendSource);
                material.SetInt(TextureBlendLayoutId, (int)textureBlendLayout);
                material.SetTexture(BlendingMask, blendingMask ? blendingMask : Texture2D.whiteTexture);
                material.SetVector(BlendingMaskST, new Vector4(blendingMaskScale.x, blendingMaskScale.y, blendingMaskOffset.x, blendingMaskOffset.y));
                ApplyTextureBlendLayer(material, BlendingTextureG, BlendingTextureGST, BlendingTextureGOpacity, BlendingTextureGMode, blendingLayerG);
                ApplyTextureBlendLayer(material, BlendingTextureB, BlendingTextureBST, BlendingTextureBOpacity, BlendingTextureBMode, blendingLayerB);
                ApplyTextureBlendLayer(material, BlendingTextureWhite, BlendingTextureWhiteST, BlendingTextureWhiteOpacity, BlendingTextureWhiteMode, blendingLayerWhite);
                material.SetFloat(ToneIntensity, Mathf.Clamp01(toneIntensity));
                material.SetInt(ColorFilterId, (int)colorFilter);
                material.SetColor(ColorValue, color);
                material.SetFloat(ColorIntensity, Mathf.Clamp01(colorIntensity));
                material.SetInt(ColorGlow, colorGlow ? 1 : 0);
                material.SetFloat(SamplingIntensity, Mathf.Clamp01(samplingIntensity));
                material.SetFloat(SamplingWidth, Mathf.Max(0.5f, samplingWidth));
                material.SetFloat(SamplingScale, Mathf.Max(0.01f, samplingScale));
                material.SetTexture(TransitionTex, transitionTexture ? transitionTexture : Texture2D.whiteTexture);
                material.SetVector(TransitionTexST, new Vector4(transitionTextureScale.x, transitionTextureScale.y, transitionTextureOffset.x, transitionTextureOffset.y));
                material.SetVector(TransitionTexSpeed, transitionTextureSpeed);
                material.SetFloat(TransitionRate, Mathf.Clamp01(transitionRate));
                material.SetInt(TransitionReverse, transitionReverse ? 1 : 0);
                material.SetFloat(TransitionWidth, Mathf.Clamp01(transitionWidth));
                material.SetFloat(TransitionSoftness, Mathf.Clamp01(transitionSoftness));
                material.SetVector(TransitionRange, new Vector4(Mathf.Clamp01(transitionRange.x), Mathf.Clamp01(transitionRange.y), 0, 0));
                material.SetInt(TransitionColorFilter, (int)transitionColorFilter);
                material.SetColor(TransitionColor, transitionColor);
                material.SetInt(TransitionColorGlow, transitionColorGlow ? 1 : 0);
                material.SetInt(TransitionPatternReverse, transitionPatternReverse ? 1 : 0);
                material.SetFloat(TransitionAutoPlaySpeed, transitionAutoPlaySpeed);
                material.SetTexture(TransitionGradientTex, transitionRamp);
                material.SetColor(TargetColor, targetColor);
                material.SetFloat(TargetRange, Mathf.Clamp01(targetRange));
                material.SetFloat(TargetSoftness, Mathf.Clamp01(targetSoftness));
                material.SetFloat(EdgeWidth, Mathf.Clamp01(edgeWidth));
                material.SetFloat(EdgeShinyRate, Mathf.Clamp01(edgeShinyRate));
                material.SetFloat(EdgeShinySpeed, edgeShinyAutoPlaySpeed);
                material.SetFloat(EdgeShinyWidth, Mathf.Clamp01(edgeShinyWidth));
                material.SetInt(EdgeColorFilter, (int)edgeColorFilter);
                material.SetColor(EdgeColor, edgeColor);
                material.SetInt(EdgeColorGlow, edgeColorGlow ? 1 : 0);
                material.SetInt(PatternAreaId, (int)patternArea);
                material.SetFloat(DetailIntensity, Mathf.Clamp01(detailIntensity));
                material.SetVector(DetailThreshold, detailThreshold);
                material.SetColor(DetailColor, detailColor);
                material.SetTexture(DetailTex, detailTexture ? detailTexture : Texture2D.whiteTexture);
                material.SetVector(DetailTexST, new Vector4(detailTextureScale.x, detailTextureScale.y, detailTextureOffset.x, detailTextureOffset.y));
                material.SetVector(DetailTexSpeed, detailTextureSpeed);
                material.SetFloat(GradationIntensity, Mathf.Clamp01(gradationIntensity));
                material.SetInt(GradationColorFilter, (int)gradationColorFilter);
                material.SetColor(GradationColor1, gradationColor1);
                material.SetColor(GradationColor2, gradationColor2);
                material.SetColor(GradationColor3, gradationColor3);
                material.SetColor(GradationColor4, gradationColor4);
                material.SetTexture(GradationTex, gradationRamp);
                material.SetVector(GradationTexST, GetGradationScaleAndOffset());
                material.SetInt(GradationRadial, IsRadial(gradation) ? 1 : 0);
                material.SetMatrix(RootViewMatrix, root);
                material.SetMatrix(GradViewMatrix, gradRoot);
                material.SetMatrix(CanvasToWorldMatrix, Matrix4x4.identity);
                material.SetFloat(UnishadeTime, animationTime);
                material.SetFloat(DitherIntensity, Mathf.Clamp01(ditherIntensity));
                material.SetFloat(DitherScale, Mathf.Max(0.01f, ditherScale));
                material.SetVector(VertexShakeSpeed, vertexShakeSpeed);
                material.SetFloat(VertexShakeSpeedMultiplier, Mathf.Max(0, vertexShakeSpeedMultiplier));
                material.SetVector(VertexShakeDisplacement, vertexShakeMaxDisplacement);
                material.SetFloat(VertexShakeBlend, Mathf.Clamp01(vertexShakeBlend));
                material.SetVector(ScrollTextureSpeed, scrollTextureSpeed);
                material.SetFloat(HandDrawnAmount, Mathf.Max(0, handDrawnAmount));
                material.SetFloat(HandDrawnSpeed, Mathf.Max(1, handDrawnSpeed));
                material.SetFloat(DistortionIntensity, Mathf.Clamp01(distortionIntensity));
                material.SetVector(DistortionWave, new Vector4(Mathf.Max(0, distortionWaveX), Mathf.Max(0, distortionWaveY), 0, 0));
                material.SetVector(DistortionAmount, new Vector4(Mathf.Clamp(distortionAmountX, 0, 0.25f), Mathf.Clamp(distortionAmountY, 0, 0.25f), 0, 0));
                material.SetFloat(DistortionSpeed, distortionSpeed);
                material.SetTexture(DistortionTex, distortionTexture ? distortionTexture : Texture2D.grayTexture);
                material.SetVector(DistortionTexST, new Vector4(distortionTextureScale.x, distortionTextureScale.y, distortionTextureOffset.x, distortionTextureOffset.y));
                material.SetVector(DistortionRippleCenter, new Vector4(Mathf.Clamp01(distortionRippleCenter.x), Mathf.Clamp01(distortionRippleCenter.y), 0, 0));
                material.SetFloat(DistortionRippleFrequency, Mathf.Max(0, distortionRippleFrequency));
                material.SetFloat(DistortionRippleStrength, Mathf.Max(0, distortionRippleStrength));
                material.SetFloat(DistortionRippleSpeed, distortionRippleSpeed);
                material.SetFloat(HologramIntensity, Mathf.Clamp01(hologramIntensity));
                material.SetFloat(HologramScanlineDensity, Mathf.Max(1, hologramScanlineDensity));
                material.SetFloat(HologramScanlineStrength, Mathf.Clamp01(hologramScanlineStrength));
                material.SetFloat(HologramNoise, Mathf.Clamp01(hologramNoise));
                material.SetFloat(HologramRgbShift, Mathf.Clamp01(hologramRgbShift));
                material.SetFloat(HologramSpeed, hologramSpeed);
                material.SetColor(HologramColor, hologramColor);
                material.SetInt(HologramMode, (int)hologram);
                material.SetFloat(GlitchIntensity, Mathf.Clamp01(glitchIntensity));
                material.SetFloat(GlitchBlockSize, Mathf.Clamp(glitchBlockSize, 0.005f, 0.5f));
                material.SetFloat(GlitchSpeed, glitchSpeed);
                material.SetInt(GlitchMode, (int)glitch);
                material.SetFloat(InnerGlowIntensity, Mathf.Clamp01(innerGlowIntensity));
                material.SetFloat(InnerGlowSize, Mathf.Clamp(innerGlowSize, 0.25f, 8));
                material.SetColor(InnerGlowColor, innerGlowColor);
                material.SetFloat(OuterGlowIntensity, Mathf.Clamp(outerGlowIntensity, 0, 4));
                material.SetFloat(OuterGlowSize, Mathf.Clamp(outerGlowSize, 0.25f, 16));
                material.SetFloat(OuterGlowSoftness, Mathf.Clamp01(outerGlowSoftness));
                material.SetColor(OuterGlowColor, outerGlowColor);

                SetKeyword(material, ToneKeywords, (int)tone);
                SetKeyword(material, SamplingKeywords, (int)sampling);
                SetKeyword(material, TransitionKeywords, TransitionKeywordIndex(transition));
                SetKeyword(material, EdgeKeywords, (int)edge);
                SetKeyword(material, DetailKeywords, DetailKeywordIndex(detail));
                SetKeyword(material, TargetKeywords, (int)target);
                SetKeyword(material, new[] { "", "COLOR_FILTER" }, colorFilter == ColorFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "GRADATION_GRADIENT", "GRADATION_COLOR2", "GRADATION_COLOR4" }, GradationKeywordIndex(gradation));
                SetKeyword(material, DistortionKeywords, (int)distortion);
                SetKeyword(material, new[] { "", "DITHER_EFFECT" }, dither == DitherFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "VERTEX_SHAKE_EFFECT" }, vertexShake == VertexShakeFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "SCROLL_TEXTURE_EFFECT" }, scrollTexture == ScrollTextureFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "HAND_DRAWN_EFFECT" }, handDrawn == HandDrawnFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "HOLOGRAM_EFFECT" }, hologram == HologramFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "GLITCH_EFFECT" }, glitch == GlitchFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "INNER_GLOW_EFFECT" }, innerGlow == InnerGlowFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "OUTER_GLOW_EFFECT" }, outerGlow == OuterGlowFilter.None ? 0 : 1);
                SetKeyword(material, new[] { "", "TEXTURE_BLEND" }, IsTextureBlendEnabled() ? 1 : 0);
            }
        }

        private void EnsureTextureBlendLayers()
        {
            if (blendingLayerG == null) blendingLayerG = new TextureBlendLayer();
            if (blendingLayerB == null) blendingLayerB = new TextureBlendLayer();
            if (blendingLayerWhite == null) blendingLayerWhite = new TextureBlendLayer();
        }

        private bool IsTextureBlendEnabled()
        {
            var layer = textureBlendLayout == TextureBlendLayout.RGB ? blendingLayerG : blendingLayerWhite;
            var secondLayer = textureBlendLayout == TextureBlendLayout.RGB ? blendingLayerB : null;
            return HasTextureBlendLayer(layer) || HasTextureBlendLayer(secondLayer);
        }

        private static bool HasTextureBlendLayer(TextureBlendLayer layer)
        {
            return layer != null && layer.texture && layer.opacity > 0.0001f;
        }

        private static void ApplyTextureBlendLayer(Material material, int textureId, int textureStId, int opacityId, int modeId, TextureBlendLayer layer)
        {
            var texture = layer != null && layer.texture ? layer.texture : Texture2D.whiteTexture;
            var opacity = layer != null && layer.texture ? Mathf.Clamp01(layer.opacity) : 0;
            var scale = layer != null ? layer.scale : Vector2.one;
            var offset = layer != null ? layer.offset : Vector2.zero;
            var mode = layer != null ? layer.blendMode : TextureBlendMode.Normal;
            material.SetTexture(textureId, texture);
            material.SetVector(textureStId, new Vector4(scale.x, scale.y, offset.x, offset.y));
            material.SetFloat(opacityId, opacity);
            material.SetInt(modeId, (int)mode);
        }

        private void MigrateLegacyModes()
        {
            if (hologram == HologramFilter.None && hologramEnabled) hologram = HologramFilter.Hologram;
            if (glitch == GlitchFilter.None && glitchEnabled) glitch = GlitchFilter.BlocksAndLines;
            if (innerGlow == InnerGlowFilter.None && innerGlowEnabled) innerGlow = InnerGlowFilter.Additive;
            if (edge == EdgeFilter.None && edgeEnabled) edge = edgeShiny ? EdgeFilter.Shiny : EdgeFilter.Plain;
            hologramEnabled = false;
            glitchEnabled = false;
            innerGlowEnabled = false;
            edgeEnabled = false;
            edgeShiny = false;
        }

        private void RebuildRamps()
        {
            if (!gradationRamp) gradationRamp = CreateRamp("Unishade Gradation", gradationGradient);
            else FillRamp(gradationRamp, gradationGradient);
            if (!transitionRamp) transitionRamp = CreateRamp("Unishade Transition", transitionGradient);
            else FillRamp(transitionRamp, transitionGradient);
        }

        private static Texture2D CreateRamp(string name, Gradient gradient)
        {
            var texture = new Texture2D(256, 1, TextureFormat.RGBA32, false, true)
            {
                name = name,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };
            FillRamp(texture, gradient);
            return texture;
        }

        private static void FillRamp(Texture2D texture, Gradient gradient)
        {
            if (!texture || gradient == null) return;
            var pixels = new UnityEngine.Color[texture.width];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = gradient.Evaluate(i / (pixels.Length - 1f));
            texture.SetPixels(pixels);
            texture.Apply(false, false);
        }

        private void DestroyRuntimeObjects()
        {
            if (effectMaterials != null)
                for (var i = 0; i < effectMaterials.Length; i++) DestroyObject(effectMaterials[i]);
            DestroyObject(gradationRamp);
            DestroyObject(transitionRamp);
            effectMaterials = null;
            gradationRamp = null;
            transitionRamp = null;
        }

        private static new void DestroyObject(UnityEngine.Object value)
        {
            if (!value) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(value);
            else UnityEngine.Object.DestroyImmediate(value);
        }

        private Bounds GetLocalBounds()
        {
            if (targetRenderer is SpriteRenderer sprite && sprite.sprite) return sprite.sprite.bounds;
            if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
            if (meshFilter && meshFilter.sharedMesh) return meshFilter.sharedMesh.bounds;
            return new Bounds(Vector3.zero, Vector3.one);
        }

        private Vector4 GetUvRect()
        {
            if (!(targetRenderer is SpriteRenderer sprite) || !sprite.sprite || !sprite.sprite.texture) return new Vector4(0, 0, 1, 1);
            var texture = sprite.sprite.texture;
            var rect = sprite.sprite.textureRect;
            return new Vector4(rect.xMin / texture.width, rect.yMin / texture.height, rect.xMax / texture.width, rect.yMax / texture.height);
        }

        private Matrix4x4 CreateViewMatrix(Bounds bounds, float rotation)
        {
            var size = bounds.size;
            size.x = Mathf.Max(0.0001f, size.x);
            size.y = Mathf.Max(0.0001f, size.y);
            var scale = new Vector3(1f / size.x, 1f / size.y, 1);
            if (keepAspectRatio) scale.x = scale.y = Mathf.Min(scale.x, scale.y);
            return Matrix4x4.TRS(new Vector3(0.5f, 0.5f), Quaternion.Euler(0, 0, rotation), scale) * Matrix4x4.Translate(-bounds.center);
        }

        private float GetGradationRotation()
        {
            switch (gradation)
            {
                case GradationMode.DiagonalToLeftBottom: return 135;
                case GradationMode.DiagonalToRightBottom: return 45;
                case GradationMode.Vertical: return 90;
                case GradationMode.VerticalGradient: return -90;
                case GradationMode.Angle:
                case GradationMode.AngleGradient: return gradationRotation;
                default: return 0;
            }
        }

        private Vector4 GetGradationScaleAndOffset()
        {
            var scale = 1f / Mathf.Max(0.01f, gradationScale);
            var offset = gradationOffset * (scale + 1) * 0.5f - scale * 0.5f + 0.5f;
            if (gradation == GradationMode.HorizontalGradient || gradation == GradationMode.VerticalGradient || gradation == GradationMode.AngleGradient)
                offset = -0.5f * (scale + 1) - gradationOffset;
            if (IsRadial(gradation)) offset = gradationOffset;
            if (gradationReverse)
            {
                scale = -scale;
                offset = 1 - offset;
            }
            return new Vector4(scale, 1, offset, 0);
        }

        private static bool IsRadial(GradationMode mode) => mode == GradationMode.Radial || mode == GradationMode.RadialGradient;
        private static int TransitionKeywordIndex(TransitionFilter filter) => filter == TransitionFilter.Blaze ? 8 : filter == TransitionFilter.Pattern ? 9 : (int)filter;
        private static int DetailKeywordIndex(DetailFilter filter) => filter == DetailFilter.Subtractive ? 6 : filter == DetailFilter.Replace ? 4 : filter == DetailFilter.MultiplyAdditive ? 5 : (int)filter;
        private static int GradationKeywordIndex(GradationMode mode)
        {
            if (mode == GradationMode.None) return 0;
            if (mode == GradationMode.HorizontalGradient || mode == GradationMode.VerticalGradient || mode == GradationMode.AngleGradient || mode == GradationMode.RadialGradient) return 1;
            if (mode == GradationMode.Horizontal || mode == GradationMode.Vertical || mode == GradationMode.Radial || mode == GradationMode.Angle) return 2;
            return 3;
        }

        private static void SetKeyword(Material material, string[] keywords, int index)
        {
            for (var i = 0; i < keywords.Length; i++)
            {
                if (string.IsNullOrEmpty(keywords[i])) continue;
                if (i == index) material.EnableKeyword(keywords[i]);
                else material.DisableKeyword(keywords[i]);
            }
        }

        private static (BlendMode src, BlendMode dst) ConvertBlend(BlendType type, BlendMode src, BlendMode dst)
        {
            switch (type)
            {
                case BlendType.AlphaBlend: return (BlendMode.One, BlendMode.OneMinusSrcAlpha);
                case BlendType.Multiply: return (BlendMode.DstColor, BlendMode.OneMinusSrcAlpha);
                case BlendType.Additive: return (BlendMode.One, BlendMode.One);
                case BlendType.SoftAdditive: return (BlendMode.OneMinusDstColor, BlendMode.One);
                case BlendType.MultiplyAdditive: return (BlendMode.DstColor, BlendMode.One);
                default: return (src, dst);
            }
        }
    }
}
