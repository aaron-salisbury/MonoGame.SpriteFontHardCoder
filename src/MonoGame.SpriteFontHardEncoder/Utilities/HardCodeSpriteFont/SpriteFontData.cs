using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame.SpriteFontHardEncoder;

/// <summary>
/// Holds the sprite font's data, including the pixel data.
/// </summary>
internal record SpriteFontData
{
    internal required Dictionary<char, SpriteFont.Glyph> Glyphs { get; init; }
    internal required List<Color> PixelColorData { get; init; }
    internal required List<Rectangle> GlyphBounds { get; init; }
    internal required List<Rectangle> GlyphCroppings { get; init; }
    internal required List<Vector3> GlyphKernings { get; init; }
    internal required List<char> GlyphCharacters { get; init; }
    internal required int NumberOfGlyphCharacters { get; init; }
    internal required Texture2D FontTexture { get; init; }
    internal required int LineHeightSpacing { get; init; }
    internal required float Spacing { get; init; }
    internal required int Width { get; init; }
    internal required int Height { get; init; }
    internal required char DefaultChar { get; init; }
    internal required SpriteFont.Glyph DefaultGlyph { get; init; }
}
