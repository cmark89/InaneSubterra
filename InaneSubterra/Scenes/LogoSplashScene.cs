using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using ObjectivelyRadical.Controls;
using ObjectivelyRadical.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using InaneSubterra.Core;
using InaneSubterra.Physics;
using InaneSubterra.Objects;


namespace InaneSubterra.Scenes
{
    public class LogoSplashScene : Scene
    {
        Rectangle ScreenArea;
        Texture2D splashTexture;
        Color splashColor;
        ScriptReader scriptReader;
        float titleTime;

        public override void Initialize()
        {
            
            ScreenArea = new Rectangle(0,0,InaneSubterra.graphics.GraphicsDevice.Viewport.Width, InaneSubterra.graphics.GraphicsDevice.Viewport.Height);
            scriptReader = new ScriptReader();

            splashColor = Color.Black;
        }

        public override void LoadContent(ContentManager content)
        {
            if (splashTexture == null)
                splashTexture = content.Load<Texture2D>("Graphics/ORSplash");

            scriptReader.Execute(SplashAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            titleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            scriptReader.Update(gameTime);

            // Check for input to skip the screen
            if (KeyboardManager.KeyPressedUp(Keys.Space) || KeyboardManager.KeyPressedUp(Keys.Enter))
            {
                InaneSubterra.SetScene(new TitleScene());
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(splashTexture, ScreenArea, splashColor);
        }


        public IEnumerator<float> SplashAnimation()
        {
            yield return 1f;

            titleTime = 0f;
            while (splashColor != Color.White)
            {
                splashColor = Color.Lerp(Color.Black, Color.White, (titleTime) / 2f);
                yield return 0f;
            }

            yield return 3f;

            titleTime = 0f;
            while (splashColor != Color.Black)
            {
                splashColor = Color.Lerp(Color.White, Color.Black, titleTime / 2f);
                yield return 0f;
            }

            yield return 1.6f;
            InaneSubterra.SetScene(new TitleScene());
        }

        public override void Unload()
        {
        }
    }
}
