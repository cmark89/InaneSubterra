using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using ObjectivelyRadical.Controls;
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

        public override void Initialize()
        {
            SequenceColors = new Color[]
            {
                Color.SpringGreen,              //Sequence 2: Optimism
                Color.YellowGreen,              //Sequence 3: Hope
                Color.DarkOliveGreen,           //Sequence 4: Doubt
                Color.DarkOrange,               //Sequence 5: Regret
                Color.OrangeRed,                //Sequence 6: Fear
                Color.MediumPurple,             //Sequence 7: Guilt
                Color.DarkMagenta,              //Sequence 8: Despair
                Color.Red,                       //Sequence 9: Hate
                new Color(.1f, .1f, .1f, 1f),   //Sequence 10: Introspection
                new Color(1f,1f,1f, .5f)        //Sequence 0: Truth
            };

            crystals = new List<TitleScreenAnimation>();
            ScreenArea = new Rectangle(0,0,InaneSubterra.graphics.GraphicsDevice.Viewport.Width, InaneSubterra.graphics.GraphicsDevice.Viewport.Height);
            titleAnimation = TitleAnimation();

            titleColor = Color.Transparent;
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

            backgroundColor = new Color(.6f, .6f, .6f, 1f);


            // Begin playing music here.
            PlaySong(titleMusic);
            
        }

        public override void Update(GameTime gameTime)
        {
            titleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (titleColor != Color.White)
            {
                titleColor = Color.Lerp(Color.Transparent, new Color(.5f, .5f, .5f, 1f), (titleTime - 2f) / 4f);
            }

            if(!titleAnimationCompleted)
                titleAnimation.MoveNext();

            // Update the animating crystals.
            foreach (TitleScreenAnimation tsa in crystals)
                tsa.Update(gameTime);


            // Check for input to start the game
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, ScreenArea, backgroundColor);
            //spriteBatch.DrawString(titleFont, "Inane Subterra", new Vector2((ScreenArea.Width - titleFont.MeasureString("Inane Subterra").X) / 2f, (ScreenArea.Height - titleFont.MeasureString("Inane Subterra").Y) / 2f), Color.Gold);
            spriteBatch.Draw(titleTexture, new Vector2((ScreenArea.Width - titleTexture.Width) / 2, (ScreenArea.Height - titleTexture.Height) / 2), titleColor);

            foreach (TitleScreenAnimation tsa in crystals)
                tsa.Draw(spriteBatch);
        }

        public IEnumerator<int> TitleAnimation()
        {
            Console.WriteLine("Yield until 1f");
            while (titleTime < 1f)
            {
                yield return 0;
            }

            Vector2 screenCenter = new Vector2((ScreenArea.Width - 48) / 2, (ScreenArea.Height - 48) / 2);
            TitleScreenAnimation newCrystal;
            float angle = 3.4f;
            float distance = 230f;

            for (int i = 0; i < 10; i++)
            {
                while (titleTime < (i * .8f) + 1f)
                    yield return 0;


                float newX = (float)Math.Cos(angle) * distance;
                float newY = (float)Math.Sin(angle) * distance;

                Console.WriteLine("Create crystal " + i);
                newCrystal = new TitleScreenAnimation(crystalTexture, 48, 48, new int[] { 0, 1, 2, 3 }, 6, true);
                newCrystal.Position = new Vector2(screenCenter.X + newX, screenCenter.Y + newY);
                newCrystal.ChangeColor(SequenceColors[i], 3f);
                crystals.Add(newCrystal);

                angle += (float)(Math.PI * 2) / 10;
            }

            

            Console.WriteLine("Finished!");

            titleAnimationCompleted = true;
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
