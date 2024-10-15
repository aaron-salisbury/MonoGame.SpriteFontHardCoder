using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace MonoGame.SpriteFontHardEncoder;

/// <summary>
/// Utility that hard-codes a <see cref="SpriteFont"/>, allowing its instantiation 
/// in code within another project. Without that project having to concern itself with content pipeline mechanics.
/// </summary>
/// <remarks>
/// Derived from <see href="https://github.com/willmotil/MonoGame-SpriteFont-HardEncoder-To-Class/tree/master">willmotil's offering</see>.
/// </remarks>
public static class SpriteFontHardCoder
{
    /// <summary>
    /// This breaks a <see cref="SpriteFont"/> down and then writes it as a C# file.
    /// When that generated class is instantiated and Load() is called on it, it then returns a hard-coded <see cref="Microsoft.Xna.Framework.Graphics.SpriteFont"/>.
    /// </summary>
    public static void Run(SpriteFont spriteFont, string fontName, string saveDirectoryPath)
    {
        fontName = Path.GetFileNameWithoutExtension(fontName.Replace(" ", string.Empty));

        SpriteFontData spriteFontData = DismantleSpriteFont(spriteFont);

        SpriteFontToClassFileEncoderDecoder.WriteFile(fontName, saveDirectoryPath, spriteFontData);
    }

    private static SpriteFontData DismantleSpriteFont(SpriteFont spriteFont)
    {
        int size = spriteFont.Texture.Width * spriteFont.Texture.Height;
        Color[] colors = new Color[size];
        // GetData() can't seem to handle font textures if the pipeline was set to use anything other than Color.
        // So anything compressed, like Dxt3, needs to have the TextureFormat changed to Color in the pipeline tool.
        spriteFont.Texture.GetData(colors); 

        Dictionary<char, SpriteFont.Glyph> glyphs = spriteFont.GetGlyphs();
        List<Rectangle> glyphBounds = [];
        List<Rectangle> glyphCroppings = [];
        List<char> glyphCharacters = [];
        List<Vector3> glyphKernings = [];

        foreach (KeyValuePair<char, SpriteFont.Glyph> kvp in glyphs)
        {
            glyphBounds.Add(kvp.Value.BoundsInTexture);
            glyphCroppings.Add(kvp.Value.Cropping);
            glyphCharacters.Add(kvp.Value.Character);
            glyphKernings.Add(new Vector3(kvp.Value.LeftSideBearing, kvp.Value.Width, kvp.Value.RightSideBearing));
        }

        char defaultChar = spriteFont.DefaultCharacter.HasValue ? spriteFont.DefaultCharacter.Value : '*';

        return new SpriteFontData()
        {
            Glyphs = glyphs,
            PixelColorData = [.. colors],
            GlyphBounds = glyphBounds,
            GlyphCroppings = glyphCroppings,
            GlyphKernings = glyphKernings,
            GlyphCharacters = glyphCharacters,
            NumberOfGlyphCharacters = glyphCharacters.Count,
            FontTexture = spriteFont.Texture,
            LineHeightSpacing = spriteFont.LineSpacing,
            Spacing = spriteFont.Spacing,
            Width = spriteFont.Texture.Width,
            Height = spriteFont.Texture.Height,
            DefaultChar = defaultChar,
            DefaultGlyph = spriteFont.DefaultCharacter.HasValue ? glyphs[defaultChar] : new SpriteFont.Glyph()
        };
    }
}
