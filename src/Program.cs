using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace MonoGame.SpriteFontHardEncoder;

internal class Program
{
    public static void Main(string[] args)
    {
        int exitCode = 0;

        try
        {
            using Game game = new Game1();
            game.Run();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
            Debug.WriteLine(ex.StackTrace);
            exitCode = 1;
        }
        finally
        {
            Environment.Exit(exitCode);
        }
    }
}
