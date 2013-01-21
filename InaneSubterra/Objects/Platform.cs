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
        private Block[,] blockGrid;

        public float Y
        {
            get
            {
                return blocks[0].Hitbox.Y;
            }
        }

        public float X
        {
            get
            {
                return blocks[0].Hitbox.X;
            }
        }

        public float Width
        {
            get
            {
                return xWidth * thisScene.BlockTexture.Width;
            }
        }

        public float MiddleOfPlatform()
        {
            return (blocks[0].Position.X + (Width / 2));
        }

        public int xWidth;

        GameScene thisScene;
        Random rand;

        // Creates a platform, which is nothing more than a cohesive set of blocks
        public Platform(GameScene newScene, Vector2 position, int width, int height)
        {
            xWidth = width;
            thisScene = newScene;
            blocks = new List<Block>();
            blockGrid = new Block[height, width];

            rand = new Random();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Block newBlock = new Block(thisScene, new Vector2(position.X + (x * 32), position.Y + (y * 32)));
                    blockGrid[y, x] = newBlock;
                    blocks.Add(newBlock);
                }
            }

            if (thisScene.CurrentSequence < 11)
            {
                // Check for special item spawns...

                // Check to see if a sequence crystal should spawn
                if (thisScene.sequenceLength > thisScene.sequenceCrystalMinLength && !thisScene.crystalAppeared)
                {
                    // Store the probability of spawn based on the current length of the sequence
                    float probability = (thisScene.sequenceLength - thisScene.sequenceCrystalMinLength) / (thisScene.sequenceCrystalMaxLength - thisScene.sequenceCrystalMinLength);
                    Console.WriteLine("Sequence Progress: " + probability + "% chance of a Sequence Crystal spawning");
                    if (rand.NextDouble() < probability)
                    {
                        Console.WriteLine("A Sequence Crystal appears!");
                        // Spawn a sequence crystal in the middle of the platform
                        new SequenceCrystal(thisScene, new Vector2(MiddleOfPlatform() - 24, Y - 48));

                        // Remove all falling blocks from the platform.
                    }
                }
                else
                {
                    Console.WriteLine("Sequence Progress: " + thisScene.sequenceLength + " / " + thisScene.sequenceCrystalMinLength);

                    // Check to see if the platform generated should roll for falling blocks
                    if (thisScene.CurrentSequence >= 3)
                    {
                        float probability = (.05f * thisScene.CurrentSequence);
                        if (rand.NextDouble() < probability)
                        {
                            // The platform rolls for falling blocks now.
                            for (int x = 0; x < xWidth; x++)
                            {
                                if (rand.NextDouble() < probability)
                                {
                                    Console.WriteLine("Create a falling block!");

                                    // The block is a falling block.
                                    blockGrid[0, x].FallingBlock = true;
                                    blockGrid[0, x].fallTime = 1.7f - ((thisScene.CurrentSequence - 3) * .2f);
                                    blockGrid[0, x].linkedBlocks = new List<Block>();

                                    if (height > 1)
                                    {
                                        for (int y = 1; y < height; y++)
                                        {
                                            blockGrid[0, x].linkedBlocks.Add(blockGrid[y, x]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // It's literally the 11th hour, so create the final "boss"
                thisScene.evilPlayer = new EvilPlayer(thisScene, new Vector2(X + 2000, Y - thisScene.player.Hitbox.Height));
                thisScene.player.SetPosition(new Vector2(X + 500, Y - thisScene.player.Hitbox.Height));
            }
        } 
    }
}
