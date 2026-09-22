**# Unishade**

**## Install through Unity Package Manager**

In Unity, open `Window > Package Manager`, click `+`, choose `Add package from git URL...`, and enter:

```text
https://github.com/smoketlt/Unishade.git?path=/Assets/Unishade#unishade-1.0
```

The package name is `com.smoketlt.unishade` and the package version is `1.0.0`.

**## Quick Start**

1. Add `Unishade` to the same GameObject that contains a `SpriteRenderer` or `MeshRenderer`.

2. Leave `Renderer` set to `Auto`, or manually select the required renderer type.

3. Enable effects in the Inspector by selecting any mode other than `None`. For example: `Tone > Grayscale`, `Sampling > Blur Fast`, or `Transition > Dissolve`.

4. For dissolve and pattern effects, assign a texture to `Transition Texture`.

5. For detail effects, assign a texture to `Detail Texture`.

6. In `Blending > Texture Blending`, select the mask source and mask layout, then add texture layers. Each layer has its own opacity, blend mode, tiling, and offset settings.

7. The component also includes `Dither`, `Vertex Shake`, `Scroll Texture`, `Hand Drawn`, and the `Water Ripple` distortion mode.

The component creates a separate private material instance for each renderer material slot and restores the original materials when the component is disabled or destroyed. The original shared material is never modified.

**## Texture Blending**

The base texture is always taken from the `SpriteRenderer` or `MeshRenderer`.

* The `RGB` layout uses the green mask channel for the first layer and the blue channel for the second layer;

* The `Black & White` layout uses the red mask channel for a single layer;

* Each added layer has independent `Opacity`, `Blend Mode`, `Tiling`, and `Offset` settings;

* Available blend modes are `Normal`, `Multiply`, `Additive`, `Subtractive`, `Screen`, and `Overlay`;

* If `Vertex Color` is selected as the `Mask Source`, layer masks are taken from the renderer's vertex colors instead of a mask texture.

**## Current Feature Set**

Includes tone, color, sampling, transition, target color, gradient, edge, detail effects, masked multi-texture blending, animated texture offsets, and automatically generated gradient ramps.

`Animation > Update Mode` determines how time is updated in Edit Mode and Play Mode. In Edit Mode, the `Always` mode also continuously repaints the Scene View, so animated effects no longer depend on mouse movement over the Inspector. The `Manual` mode is intended for deterministic playback using `AdvanceAnimation(deltaTime)`.

**## Runtime API**

Every effect is also available through a typed public API. The API changes the same serialized settings and immediately updates the private runtime materials, so user code does not need to access shader property names or `sharedMaterial`.

Because the component currently lives in the `Unishade` namespace, use an alias in an external script:

```csharp
using UnityEngine;
using UnishadeComponent = Unishade.Unishade;

public sealed class CharacterEffects : MonoBehaviour
{
    [SerializeField] private UnishadeComponent unishade;

    private void Start()
    {
        unishade.Tone.Filter = UnishadeComponent.ToneSettings.FilterType.Grayscale;
        unishade.Tone.Intensity = 0.8f;

        unishade.Transition.Mode = UnishadeComponent.TransitionSettings.TransitionType.Dissolve;
        unishade.Transition.Amount = 0.5f;

        unishade.Detail.Mode = UnishadeComponent.DetailSettings.ModeType.Masking;
        unishade.Detail.Texture = detailTexture;

        unishade.OuterGlow.Mode = UnishadeComponent.OuterGlowSettings.ModeType.Additive;
        unishade.OuterGlow.Color = Color.cyan;
    }

    [SerializeField] private Texture detailTexture;
}
```

Changing a property automatically applies it to the renderer. The API covers renderer, animation, tone, color, sampling, transition, target, blending, gradient, edge, detail, distortion, dither, vertex shake, scrolling texture, hand-drawn motion, hologram, glitch, inner glow and outer glow. Texture layers are available through `unishade.Blending.Green`, `unishade.Blending.Blue` and `unishade.Blending.White`.

