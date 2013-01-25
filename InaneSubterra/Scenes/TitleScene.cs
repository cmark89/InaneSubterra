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
    public class TitleScene : Scene
    {
        SpriteFont titleFont;
        Rectangle ScreenArea;

        Texture2D backgroundTexture;
        Texture2D titleTexture;
        Color titleColor;
        Color backgroundColor;
        Texture2D crystalTexture;
        Color[] SequenceColors;

        List<TitleScreenAnimation> crystals;

        Song titleMusic;

        float titleTime = 0f;
        private bool titleAnimationCompleted = false;

        public IEnumerator<int> titleAnimation;
        ScriptReader scriptReader;

        public override void Initialize()
        {
            SequenceColors = new Color[]
            {
                Color.SpringGreen,            
                Color.Red,
                new Color(1f,1f,1f, .5f),
                Color.DarkMagenta,
                Color.DarkOrange, 
                Color.YellowGreen,
                Color.OrangeRed,
                new Color(.1f, .1f, .1f, 1f),
                Color.DarkOliveGreen,  
                Color.MediumPurple 
            };

            crystals = new List<TitleScreenAnimation>();
            ScreenArea = new Rectangle(0,0,InaneSubterra.graphics.GraphicsDevice.Viewport.Width, InaneSubterra.graphics.GraphicsDevice.Viewport.Height);

            titleColor = Color.Transparent;
            scriptReader = new ScriptReader();
        }

        public override void LoadContent(ContentManager content)
        {
            if (titleFont == null)
                titleFont = content.Load<SpriteFont>("Font/TitleFont");

            if (crystalTexture == null)
                crystalTexture = content.Load<Texture2D>("Graphics/crystal");

            if (backgroundTexture == null)
                backgroundTexture = content.Load<Texture2D>("Graphics/TitleBG");

            if (titleTexture == null)
                titleTexture = content.Load<Texture2D>("Graphics/InaneSubterraTitle");

            if (titleMusic == null)
                titleMusic = content.Load<Song>("Audio/DelusionInduction");

            backgroundColor = Color.Black;


            // Begin playing music here.
            PlaySong(titleMusic);

            scriptReader.Execute(TitleScreenAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            titleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (titleColor != new Color(.5f, .5f, .5f, 1f))
            {
                titleColor = Color.Lerp(Color.Transparent, new Color(.5f, .5f, .5f, 1f), (titleTime - 5f) / 4f);
            }

            if(!titleAnimationCompleted)
                scriptReader.Update(gameTime);

            // Update the animating crystals.
            foreach (TitleScreenAnimation tsa in crystals)
                tsa.Update(gameTime);


            // Check for input to start the game
            if (KeyboardManager.KeyPressedUp(Keys.Space) || KeyboardManager.KeyPressedUp(Keys.Enter))
            {
                if (!titleAnimationCompleted)
                {
                    titleAnimationCompleted = true;
                    ShowTitleScreen();
                }
                else
                {
                    InaneSubterra.SetScene(new GameScene());
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, ScreenArea, backgroundColor);
            //spriteBatch.DrawString(titleFont, "Inane Subterra", new Vector2((ScreenArea.Width - titleFont.MeasureString("Inane Subterra").X) / 2f, (ScreenArea.Height - titleFont.MeasureString("Inane Subterra").Y) / 2f), Color.Gold);
            spriteBatch.Draw(titleTexture, new Vector2((ScreenArea.Width - titleTexture.Width) / 2, (ScreenArea.Height - titleTexture.Height) / 2), titleColor);

            foreach (TitleScreenAnimation tsa in crystals)
                tsa.Draw(spriteBatch);
        }


        public IEnumerator<float> TitleScreenAnimation()
        {
            Vector2 screenCenter = new Vector2((ScreenArea.Width - 48) / 2, (ScreenArea.Height - 48) / 2);
            TitleScreenAnimation newCrystal;
            float angle = 3.4f;
            float distance = 230f;

            titleTime = 0;

            while(backgroundColor != new Color(.6f, .6f, .6f, 1f))
            {
                backgroundColor = Color.Lerp(Color.Black, new Color(.6f, .6f, .6f, 1f), titleTime / 3f);
                yield return 0f;
            }

            for (int i = 0; i < 10; i++)
            {
                yield return .4f;


                float newX = (float)Math.Cos(angle) * distance;
                float newY = (float)Math.Sin(angle) * distance;

                newCrystal = new TitleScreenAnimation(crystalTexture, 48, 48, new int[] { 0, 1, 2, 3 }, 6, true);
                newCrystal.Position = new Vector2(screenCenter.X + newX, screenCenter.Y + newY);
                newCrystal.ChangeColor(SequenceColors[i], 3f);
                crystals.Add(newCrystal);

                angle += (float)(Math.PI * 2) / 10;
            }

            titleAnimationCompleted = true;
        }

        public void ShowTitleScreen()
        {
            backgroundColor = new Color(.6f, .6f, .6f, 1f);
            titleColor = new Color(.5f, .5f, .5f, 1f);
            crystals.Clear();

            Vector2 screenCenter = new Vector2((ScreenArea.Width - 48) / 2, (ScreenArea.Height - 48) / 2);
            TitleScreenAnimation newCrystal;
            float angle = 3.4f;
            float distance = 230f;

            for (int i = 0; i < 10; i++)
            {
                float newX = (float)Math.Cos(angle) * distance;
                float newY = (float)Math.Sin(angle) * distance;

                newCrystal = new TitleScreenAnimation(crystalTexture, 48, 48, new int[] { 0, 1, 2, 3 }, 6, true);
                newCrystal.Position = new Vector2(screenCenter.X + newX, screenCenter.Y + newY);
                newCrystal.ChangeColor(SequenceColors[i], 0f);
                crystals.Add(newCrystal);

                angle += (float)(Math.PI * 2) / 10;
            }
        }

        public override void Unload()
        {
        }

        public void PlaySong(Song newSong)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(newSong);
        }
    }
}
