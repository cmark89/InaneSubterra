using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InaneSubterra.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InaneSubterra.Core
{

    public class Animation
    {
        public Vector2 Position { get; set; }
        public bool HorizontalFlip { get; set; }
        public Texture2D Texture { get; private set; }
        public int[] Frames { get; private set; }
        public int CurrentFrame { get; private set; }
        public float FrameRate { get; private set; }
        public bool Looping { get; private set; }

        private float time;
        private int frameWidth;
        private int frameHeight;

        GameScene thisScene;

        public Animation(GameScene newScene, Texture2D tex, int width, int height, int[] frames, float framesPerSecond, bool looping)
        {
            Texture = tex;
            Frames = frames;

            thisScene = newScene;
            
            if(Frames.Length > 0)
                CurrentFrame = Frames[0];

            FrameRate = 1 / framesPerSecond;
            Looping = looping;

            frameWidth = width;
            frameHeight = height;
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Set this to get the proper source rectangle
            if (!HorizontalFlip)
                spriteBatch.Draw(Texture, Position - thisScene.Camera, GetSourceRectangle(Frames[CurrentFrame]), Color.White);
            else
                spriteBatch.Draw(Texture, new Rectangle((int)(Position.X - thisScene.Camera.X), (int)(Position.Y - thisScene.Camera.Y), frameWidth, frameHeight), GetSourceRectangle(Frames[CurrentFrame]), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
        }


        // Draw overload takes a color to draw
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            // Set this to get the proper source rectangle
            if (!HorizontalFlip)
                spriteBatch.Draw(Texture, Position - thisScene.Camera, GetSourceRectangle(Frames[CurrentFrame]), color);
            else
                spriteBatch.Draw(Texture, new Rectangle((int)(Position.X - thisScene.Camera.X), (int)(Position.Y - thisScene.Camera.Y), frameWidth, frameHeight), GetSourceRectangle(Frames[CurrentFrame]), color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
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
