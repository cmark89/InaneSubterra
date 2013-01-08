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

        // Creates a platform, which is nothing more than a cohesive set of blocks
        public Platform(GameScene thisScene, Vector2 position, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    new Block(thisScene, new Vector2(position.X + (x * 32), position.Y + (y * 32)));
                }
            }
        }
    }
}
