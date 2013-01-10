using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InaneSubterra.Scenes;

namespace InaneSubterra.Objects
{
    public class Platform
    {
        private List<Block> blocks;

        public float Y
        {
            get
            {
                return blocks[0].Hitbox.Y;
            }
        }

        public float Width
        {
            get
            {
                return xWidth * thisScene.BlockTexture.Width;
            }
        }

        public int xWidth;
        GameScene thisScene;

        // Creates a platform, which is nothing more than a cohesive set of blocks
        public Platform(GameScene newScene, Vector2 position, int width, int height)
        {
            xWidth = width;
            thisScene = newScene;
            blocks = new List<Block>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks.Add(new Block(thisScene, new Vector2(position.X + (x * 32), position.Y + (y * 32))));
                }
            }
        }
    }
}
