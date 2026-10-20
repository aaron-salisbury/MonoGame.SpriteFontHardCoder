using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace MonoGame.SpriteFontHardEncoder;

internal class Program
{
    public static int Main()
    {
        try
        {
            using Game game = new Game1();
            game.Run();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
            Debug.WriteLine(ex.StackTrace);
            return 1;
        }

        return 0;
    }
}
