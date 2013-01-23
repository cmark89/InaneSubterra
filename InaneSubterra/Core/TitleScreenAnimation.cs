using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InaneSubterra.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InaneSubterra.Core
{

    public class TitleScreenAnimation
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; private set; }
        public int[] Frames { get; private set; }
        public int CurrentFrame { get; private set; }
        public float FrameRate { get; private set; }
        public bool Looping { get; private set; }

        private float time;
        private int frameWidth;
        private int frameHeight;

        private bool changingColor = false;
        private Color color;
        private Color oldColor;
        private Color newColor;

        private float colorChangeTime;
        private float colorChangeDuration;



        public TitleScreenAnimation(Texture2D tex, int width, int height, int[] frames, float framesPerSecond, bool looping)
        {
            Texture = tex;
            Frames = frames;
            
            if(Frames.Length > 0)
                CurrentFrame = Frames[0];

            FrameRate = 1 / framesPerSecond;
            Looping = looping;

            frameWidth = width;
            frameHeight = height;

            color = Color.Transparent;
        }

        public void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time > FrameRate)
            {
                CurrentFrame++;

                time -= FrameRate;

                if (CurrentFrame == Frames.Length)
                    CurrentFrame = 0;
            }

            if (changingColor)
            {
                colorChangeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                color = Color.Lerp(oldColor, newColor, (colorChangeTime / colorChangeDuration));
                
                if(color == newColor)
                {
                    changingColor = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)(Position.X), (int)(Position.Y), frameWidth, frameHeight), GetSourceRectangle(Frames[CurrentFrame]), color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
        }

        public void ChangeColor(Color changeToColor, float time)
        {
            oldColor = color;
            newColor = changeToColor;
            colorChangeDuration = time;
            colorChangeTime = 0;

            changingColor = true;
        }

        public Rectangle GetSourceRectangle(int index)
        {
            int row, column;

            column = index % (Texture.Width / frameWidth);
            row = index / (Texture.Width / frameWidth);

            return (new Rectangle(column * frameWidth, row * frameHeight, frameWidth, frameHeight));
        }

        public void Reset()
        {
            CurrentFrame = 0;
            time = 0f;
        }
    }
}
