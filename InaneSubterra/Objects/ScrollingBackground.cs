using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Scenes;

namespace InaneSubterra.Objects
{
    public class ScrollingBackground
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; set; }

        public float Left
        {
            get
            {
                return Position.X;
            }
        }

        public float Right
        {
            get
            {
                return Position.X + Texture.Width;
            }
        }

        public static float scrollFactor = .2f;
        GameScene thisScene;

        public ScrollingBackground(GameScene newScene, Vector2 newPosition)
        {
            thisScene = newScene;

            Texture = thisScene.BackgroundTexture;
            Position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, thisScene.SequenceColors[thisScene.CurrentSequence]);
        }

        public void Scroll(Vector2 scrollAmount)
        {
            // Move the background proportional to what the camera moves
            Position = Position - (scrollAmount * scrollFactor);
        }
    }
}
