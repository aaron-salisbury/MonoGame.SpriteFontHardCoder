using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.SpriteFontHardEncoder;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //TODO: Update with your font. First make sure to:
        // - Place your font file in the Content/fonts folder.
        // - Create a *.spritefont file and place it next to your font file.
        // - Open the *.spritefont file in a text editor and change the font name line to match the TTF filename.
        // - Launch Content.mgcb, make sure the *.spritefont file is included, change its TextureFormat property to Color, and build the content.

        //SpriteFont font = Content.Load<SpriteFont>("fonts/RobotoMono-Regular");
        //string saveDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MonoGame.SpriteFontHardEncoder");
        //SpriteFontHardEncoder.Run(font, "RobotoMono", saveDirectory);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
