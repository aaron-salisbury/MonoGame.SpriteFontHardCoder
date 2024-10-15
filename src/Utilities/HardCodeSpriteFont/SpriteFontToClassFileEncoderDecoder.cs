using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonoGame.SpriteFontHardEncoder;

/// <summary>
/// Encoding and Decoding methods. However, the decoder is placed within the output class file.
/// This allows the output class to be decoupled from the pipeline or even a TTF.
/// </summary>
/// <remarks>
/// Derived from <see href="https://github.com/willmotil/MonoGame-SpriteFont-HardEncoder-To-Class/tree/master">willmotil's offering</see>.
/// </remarks>
internal class SpriteFontToClassFileEncoderDecoder
{
    // Constants used for code formatting.
    private const int DIVIDER_AMOUNT = 10;
    private const int DIVIDER_AMOUNT_SINGLE_CHAR = 50;
    private const int DIVIDER_AMOUNT_SINGLE = 200;

    internal static void WriteFile(string fontName, string saveDirectoryPath, SpriteFontData fontData)
    {
        List<Color> colors = fontData.PixelColorData;
        RunLengthEncodingData runLengthEncodingData = EncodeColorArrayToDataRLE(colors);
        string chars = CharArrayToStringClassFormat("_chars", [.. fontData.GlyphCharacters]);
        string bounds = RectangleToStringClassFormat("_bounds", [.. fontData.GlyphBounds]);
        string croppings = RectangleToStringClassFormat("_croppings", [.. fontData.GlyphCroppings]);
        string kernings = Vector3ToStringClassFormat("_kernings", [.. fontData.GlyphKernings]);
        string runLengthEncoding = ByteArrayToStringClassFormat("_rleByteData", [.. runLengthEncodingData.Data]);

        StringBuilder theCSharpText = new();

        theCSharpText.AppendLine(
            "/*" +
            "\n    This file is programmatically generated. This class is hard-coded instance data for a instance of a Microsoft.Xna.Framework.Graphics.SpriteFont." +
            "\n    Use the GetFont method to load it." +
            "\n    Be aware, I believe you should dispose its texture in game1 unload as this won't have been loaded through the content manager." +
            "\n*/" +
            "\n");

        theCSharpText
            .AppendLine("using Microsoft.Xna.Framework;")
            .AppendLine("using Microsoft.Xna.Framework.Graphics;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine(string.Empty)
            .AppendLine("namespace MonoGame.SpriteFontHardEncoder;")
            .AppendLine(string.Empty)
            .AppendLine("/// <summary>")
            .AppendLine($"/// {fontName} font that has been converted into <see cref=\"SpriteFont\"/> and hard-coded.")
            .AppendLine("/// </summary>")
            .AppendLine("/// <remarks>")
            .AppendLine("/// Conversion tool made available by <see href=\"https://github.com/willmotil/MonoGame-SpriteFont-HardEncoder-To-Class/tree/master\">willmotil</see>.")
            .AppendLine("/// </remarks>")
            .AppendLine($"public class {fontName}")
            .AppendLine("{")
            .AppendLine($"\tprivate readonly int _width = {fontData.Width};")
            .AppendLine($"\tprivate readonly int _height = {fontData.Height};")
            .AppendLine($"\tprivate readonly char _defaultChar = char.Parse(\"{fontData.DefaultChar}\");")
            .AppendLine($"\tprivate readonly int _lineHeightSpacing = {fontData.LineHeightSpacing};")
            .AppendLine($"\tprivate readonly float _spacing = {fontData.Spacing};")
            .AppendLine(string.Empty)
            .AppendLine("\tpublic SpriteFont GetFont(GraphicsDevice device)")
            .AppendLine("\t{")
            .AppendLine("\t\tTexture2D fontTexture = DecodeToTexture(device, _rleByteData, _width, _height);")
            .AppendLine("\t\treturn new SpriteFont(fontTexture, _bounds, _croppings, _chars, _lineHeightSpacing, _spacing, _kernings, _defaultChar);")
            .AppendLine("\t}")
            .AppendLine(string.Empty)
            .AppendLine("\tprivate static Texture2D DecodeToTexture(GraphicsDevice device, List<byte> rleByteData, int width, int height)")
            .AppendLine("\t{")
            .AppendLine("\t\tColor[] colData = DecodeDataRLE(rleByteData);")
            .AppendLine("\t\tTexture2D texture = new(device, width, height);")
            .AppendLine("\t\ttexture.SetData<Color>(colData);")
            .AppendLine(string.Empty)
            .AppendLine("\t\treturn texture;")
            .AppendLine("\t}")
            .AppendLine(string.Empty)
            .AppendLine("\tprivate static Color[] DecodeDataRLE(List<byte> rleByteData)")
            .AppendLine("\t{")
            .AppendLine("\t\tList <Color> colors = [];")
            .AppendLine(string.Empty)
            .AppendLine("\t\tfor (int i = 0; i < rleByteData.Count; i++)")
            .AppendLine("\t\t{")
            .AppendLine("\t\t\tint val = (rleByteData[i] & 0x7F) * 2;")
            .AppendLine(string.Empty)
            .AppendLine("\t\t\tif (val > 252)")
            .AppendLine("\t\t\t{")
            .AppendLine("\t\t\t\tval = 255;")
            .AppendLine("\t\t\t}")
            .AppendLine(string.Empty)
            .AppendLine("\t\t\tColor color = val > 0 ? new Color(val, val, val, val) : new();")
            .AppendLine(string.Empty)
            .AppendLine("\t\t\tif ((rleByteData[i] & 0x80) > 0)")
            .AppendLine("\t\t\t{")
            .AppendLine("\t\t\t\tbyte runLength = rleByteData[i + 1];")
            .AppendLine("\t\t\t\tfor (int j = 0; j < runLength; j++)")
            .AppendLine("\t\t\t\t{")
            .AppendLine("\t\t\t\t\tcolors.Add(color);")
            .AppendLine("\t\t\t\t}")
            .AppendLine("\t\t\t\ti += 1;")
            .AppendLine("\t\t\t}")
            .AppendLine(string.Empty)
            .AppendLine("\t\t\tcolors.Add(color);")
            .AppendLine("\t\t}")
            .AppendLine(string.Empty)
            .AppendLine("\t\treturn [.. colors];")
            .AppendLine("\t}")
            .AppendLine(string.Empty)
            .AppendLine("\t#region Data")
            .Append(chars).Append("\n")
            .AppendLine(string.Empty)
            .Append(bounds).Append("\n")
            .AppendLine(string.Empty)
            .Append(croppings).Append("\n")
            .AppendLine(string.Empty)
            .Append(kernings).Append("\n")
            .AppendLine(string.Empty)
            .Append("\t// pixelsCompressed: ").Append(runLengthEncodingData.Pixels).Append(" bytesTallied: ").Append(runLengthEncodingData.BytesTallied).Append(" byteDataCount: ").Append(runLengthEncodingData.ByteDataCount).Append("\n")
            .Append(runLengthEncoding)
            .AppendLine(string.Empty)
            .AppendLine("\t#endregion")
            .AppendLine("}");

        string fullPath = Path.Combine(saveDirectoryPath, $"{fontName}.cs");
        File.WriteAllText(fullPath, theCSharpText.ToString());
    }

    /// <summary>
    /// Turns the pixel data into run length encode text for a CS file.
    /// Technically this is only compressing the alpha byte of a array.
    /// </summary>
    private static RunLengthEncodingData EncodeColorArrayToDataRLE(List<Color> colorArray)
    {
        List<byte> runLengthEncodingArray = [];
        int colorArrayIndex = 0;
        int colorArrayLength = colorArray.Count;
        int pixelsAccountedFor = 0;

        while (colorArrayIndex < colorArrayLength)
        {
            var colorMasked = (colorArray[colorArrayIndex].A / 2);
            var encodedValue = colorMasked;
            var runLength = 0;

            // Find the run length for this pixel.
            for (int i = 1; i < 255; i++)
            {
                int indexToTest = colorArrayIndex + i;
                if (indexToTest < colorArrayLength)
                {
                    var testColorMasked = (colorArray[indexToTest].A / 2);

                    if (testColorMasked == colorMasked)
                    {
                        runLength = i;
                    }  
                    else
                    {
                        i = 256; // Break on diff.
                    }   
                }
                else
                {
                    i = 256; // Break on maximum run length.
                } 
            }

            Console.WriteLine("colorArrayIndex: " + colorArrayIndex + "  runLengthEncodingArray index " + runLengthEncodingArray.Count + "  Alpha: " + colorMasked * 2 + "  runLength: " + runLength);

            if (runLength > 0)
            {
                encodedValue += 0x80;
                if (colorArrayIndex < colorArrayLength)
                {
                    runLengthEncodingArray.Add((byte)encodedValue);
                    runLengthEncodingArray.Add((byte)runLength);
                    pixelsAccountedFor += 1 + runLength;
                }
                else
                {
                    throw new Exception("Bug check; index write out of bounds.");
                }
                    
                colorArrayIndex = colorArrayIndex + 1 + runLength;
            }
            else
            {
                if (colorArrayIndex < colorArrayLength)
                {
                    runLengthEncodingArray.Add((byte)encodedValue);
                    pixelsAccountedFor += 1;
                }
                else
                {
                    throw new Exception("Encoding bug check; index write out of bounds.");
                }
                    
                colorArrayIndex++;
            }
        }

        Console.WriteLine("EncodeColorArrayToDataRLE: rleAry.Count " + runLengthEncodingArray.Count + " pixels accounted for " + pixelsAccountedFor + " bytes tallied " + pixelsAccountedFor * 4);

        return new RunLengthEncodingData()
        {
            Data = runLengthEncodingArray,
            ByteDataCount = runLengthEncodingArray.Count,
            Pixels = pixelsAccountedFor,
            BytesTallied = pixelsAccountedFor * 4
        };
    }

    /// <summary>
    /// This decodes the CS file's hard-coded Run Length Encoding pixel data to a texture.
    /// </summary>
    private static Texture2D DecodeRLEDataToTexture(GraphicsDevice device, List<byte> rleByteData, int _width, int _height)
    {
        Texture2D texture = new(device, _width, _height);
        texture.SetData(DecodeRLEDataToPixelData(rleByteData));

        return texture;
    }

    /// <summary>
    /// Decodes the class file hard-coded Run Length Encoding byte data to a color array.
    /// </summary>
    private static Color[] DecodeRLEDataToPixelData(List<byte> rleByteData)
    {
        List<Color> colors = [];

        for (int i = 0; i < rleByteData.Count; i++)
        {
            int val = (rleByteData[i] & 0x7F) * 2;
            if (val > 252)
            {
                val = 255;
            }
                
            Color color = val > 0 ? new Color(val, val, val, val) : new();

            if ((rleByteData[i] & 0x80) > 0)
            {
                byte runLength = rleByteData[i + 1];

                for (int j = 0; j < runLength; j++)
                {
                    colors.Add(color);
                }
                    
                i += 1;
            }

            colors.Add(color);
        }

        return [.. colors];
    }

    private static string CharArrayToStringClassFormat(string variableName, char[] charArray)
    {
        string charArrayString =
            "\t// Item count = " + charArray.Length +
            $"\n\tprivate readonly List<char> {variableName} = " +
            "\n\t[" +
            "\n\t\t";

        int divider = 0;
        for (int i = 0; i < charArray.Length; i++)
        {
            divider++;
            if (divider > DIVIDER_AMOUNT_SINGLE_CHAR)
            {
                divider = 0;
                charArrayString += "\n\t\t";
            }

            charArrayString += "(char)" + (int)charArray[i] + "";

            if (i < charArray.Length - 1)
            {
                charArrayString += ", ";
            }
        }

        charArrayString += "\n\t];";

        return charArrayString;
    }

    private static string RectangleToStringClassFormat(string variableName, Rectangle[] rectangleArray)
    {
        string rectangleArrayString =
            $"\tprivate readonly List<Rectangle> {variableName} = " +
            "\n\t[" +
            "\n\t\t";

        int divider = 0;
        for (int i = 0; i < rectangleArray.Length; i++)
        {
            divider++;
            if (divider > DIVIDER_AMOUNT)
            {
                divider = 0;
                rectangleArrayString += "\n\t\t";
            }

            rectangleArrayString += $"new Rectangle({rectangleArray[i].X},{rectangleArray[i].Y},{rectangleArray[i].Width},{rectangleArray[i].Height})";

            if (i < rectangleArray.Length - 1)
            {
                rectangleArrayString += ", ";
            }
        }

        rectangleArrayString += "\n\t];";

        return rectangleArrayString;
    }

    private static string Vector3ToStringClassFormat(string variableName, Vector3[] vector3Array)
    {
        string vector3ArrayString =
            $"\tprivate readonly List<Vector3> {variableName} = " +
            "\n\t[" +
            "\n\t\t";

        int divider = 0;
        for (int i = 0; i < vector3Array.Length; i++)
        {
            divider++;
            if (divider > DIVIDER_AMOUNT)
            {
                divider = 0;
                vector3ArrayString += "\n\t\t";
            }

            vector3ArrayString += $"new Vector3({vector3Array[i].X},{vector3Array[i].Y},{vector3Array[i].Z})";

            if (i < vector3Array.Length - 1)
            {
                vector3ArrayString += ", ";
            }
        }

        vector3ArrayString += "\n\t];";

        return vector3ArrayString;
    }

    private static string ByteArrayToStringClassFormat(string variableName, byte[] byteArray)
    {
        string byteArrayString =
            $"\tprivate readonly List<byte> {variableName} = " +
            "\n\t[" +
            "\n\t\t";

        int divider = 0;
        for (int i = 0; i < byteArray.Length; i++)
        {
            divider++;
            if (divider > DIVIDER_AMOUNT_SINGLE)
            {
                divider = 0;
                byteArrayString += "\n\t\t";
            }

            byteArrayString += byteArray[i];

            if (i < byteArray.Length - 1)
            {
                byteArrayString += ", ";
            }
        }

        byteArrayString += "\n\t];";

        return byteArrayString;
    }
}
