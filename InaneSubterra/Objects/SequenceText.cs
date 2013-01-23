using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Scenes;

namespace InaneSubterra.Objects
{
    public class SequenceText
    {
        string thisText;
        public Vector2 Position {get; private set;} 
        GameScene thisScene;

        float velocity;
        float maxSpeed;
        float minSpeed;

        float leftBarrier;
        float rightBarrier;

        public bool visible = true;

        public SequenceText(GameScene newScene, string text, Vector2 position, float leftLimit, float rightLimit, float highSpeed, float lowSpeed)
        {
            thisScene = newScene;
            thisText = text;
            Position = position;

            maxSpeed = highSpeed;
            minSpeed = lowSpeed;

            leftBarrier = leftLimit;
            rightBarrier = rightLimit;

            velocity = highSpeed;
        }

        public void Update(GameTime gameTime)
        {
            if (Position.X > leftBarrier && Position.X + thisScene.SequenceFont.MeasureString(thisText).X < rightBarrier)
            {
                velocity = minSpeed;
            }
            else
            {
                velocity = maxSpeed;
            }

            // Move the text
            Position = new Vector2(Position.X + (velocity * (float)gameTime.ElapsedGameTime.TotalSeconds), Position.Y);

            // Disable the text if it has flown past its limit
            if ((velocity > 0 && Position.X > thisScene.ScreenArea.Width) || (velocity < 0 && Position.X + thisScene.SequenceFont.MeasureString(thisText).X < 0))
            {
                visible = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(thisScene.SequenceFont, thisText, Position, Color.LightYellow);
        }
    }
}
