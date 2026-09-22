# Unishade

## Установка через Unity Package Manager

В Unity откройте `Window > Package Manager`, нажмите `+`, выберите `Add package from git URL...` и вставьте:

```text
https://github.com/smoketlt/Unishade.git?path=/Assets/Unishade#unishade-1.0
```

Имя пакета: `com.smoketlt.unishade`. Версия пакета: `1.0.1`.

## Быстрый старт

1. Добавьте `Unishade` на тот же GameObject, где находятся `SpriteRenderer` или `MeshRenderer`.
2. Оставьте `Renderer` в режиме `Auto` либо выберите нужный тип рендера вручную.
3. Включайте эффекты в Inspector выбором режима, отличного от `None`. Например: `Tone > Grayscale`, `Sampling > Blur Fast` или `Transition > Dissolve`.
4. Для эффектов растворения и паттернов назначьте текстуру в `Transition Texture`.
5. Для эффектов детализации назначьте текстуру в `Detail Texture`.
6. В разделе `Blending > Texture Blending` выберите источник маски и её раскладку, затем добавьте текстуры-слои. У каждого слоя есть собственные настройки прозрачности, режима смешивания, tiling и offset.
7. В компоненте также доступны `Dither`, `Vertex Shake`, `Scroll Texture`, `Hand Drawn` и режим `Water Ripple` для искажения.

Компонент создаёт отдельный приватный экземпляр материала для каждого слота рендера и восстанавливает исходные материалы при отключении или уничтожении компонента. Исходный shared material никогда не изменяется.

## Работа с координатами

Шейдер использует исходные UV текстуры, а координаты эффектов рассчитываются на основе локальных bounds спрайта или меша. Для упакованного спрайта компонент передаёт прямоугольник UV в атласе, поэтому выборки для blur и edge остаются внутри границ спрайта.

## Смешивание текстур

Основная текстура всегда берётся из `SpriteRenderer` или `MeshRenderer`:

- раскладка `RGB` использует зелёный канал маски для первого слоя и синий канал для второго;
- раскладка `Black & White` использует красный канал маски для одного слоя;
- каждый добавленный слой имеет независимые настройки `Opacity`, `Blend Mode`, `Tiling` и `Offset`;
- доступны режимы смешивания `Normal`, `Multiply`, `Additive`, `Subtractive`, `Screen` и `Overlay`;
- если в `Mask Source` выбран режим `Vertex Color`, маски слоёв берутся из цветов вершин рендера, а не из текстуры маски.

## Текущая область возможностей

Включает эффекты tone, color, sampling, transition, target color, gradient, edge, detail, маскированное смешивание нескольких текстур, анимированные смещения текстур и автоматически создаваемые градиентные рампы.

Дополнения, вдохновлённые набором возможностей 2DxFX: distortion (`wave`, `noise`, `twist`, `pinch`, `fisheye`, `water ripple`), dither, vertex shake, scrolling texture, hand-drawn motion, hologram со scanlines/noise/RGB shift, block/line glitch, inner glow и мягкий многослойный outer glow. Все они реализованы через shader keywords в том же проходе материала, поэтому могут комбинироваться с уже существующими разделами.

`Animation > Update Mode` определяет, как обновляется время в Edit Mode и Play Mode. В Edit Mode режим `Always` также постоянно перерисовывает Scene View, поэтому анимированные эффекты больше не зависят от движения мыши над Inspector. Режим `Manual` предназначен для детерминированного воспроизведения через `AdvanceAnimation(deltaTime)`.

UI-функции, которым требуется расширение вершин Canvas (UI-тени, зеркальная геометрия, маски и обработка raycast), намеренно не включены для `SpriteRenderer` и `MeshRenderer`.

## Runtime API

Каждый эффект доступен из кода через типизированный публичный API. Изменения проходят через те же настройки компонента и сразу применяются к приватным runtime-материалам. Поэтому пользователю не нужно обращаться к именам shader properties или к `sharedMaterial`.

Так как компонент сейчас находится в namespace `Unishade`, во внешнем скрипте удобно использовать alias:

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

После изменения свойства эффект автоматически применяется к рендереру. API покрывает renderer, animation, tone, color, sampling, transition, target, blending, gradient, edge, detail, distortion, dither, vertex shake, scrolling texture, hand-drawn motion, hologram, glitch, inner glow и outer glow. Слои текстур доступны через `unishade.Blending.Green`, `unishade.Blending.Blue` и `unishade.Blending.White`.
