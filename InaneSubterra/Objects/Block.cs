using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Scenes;
using InaneSubterra.Physics;

namespace InaneSubterra.Objects
{
    public class Block : GameObject
    {
        // Bool that stores whether the block is visible and should be drawn
        //private bool isVisible = true;

        // The color of the sequence the block was spawned under.
        Color color;

        public List<Block> linkedBlocks;

        // Controls whether or not this block will fall or not
        public bool FallingBlock { get; set; }

        public bool falling;
        public float fallCount;
        public float fallTime = 2f;

        public Block(GameScene scene, Vector2 newPos)
        {
            // Initialize fields.
            thisScene = scene;
            Position = newPos;
            Texture = thisScene.BlockTexture;

            color = thisScene.SequenceColors[thisScene.CurrentSequence];

            // Run initialization logic.
            Initialize();
        }


        new public void Initialize()
        {
            // Set the block to be unconcerned with gravity.
            UsesGravity = false;
            
            Solid = true;
            Name = "Block";

            // Set the OnCollision event to resolve collisions only with other blocks.
            OnCollision += delegate(object sender, CollisionEventArgs e)
            {
                if (FallingBlock && e.CollidedObject.Name == "Player" && Hitbox.Y > e.CollidedObject.Hitbox.Y) 
                {
                    falling = true;
                }
            };

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            if (falling)
            {
                fallCount += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fallCount > fallTime)
                {
                    falling = false;

                    UsesGravity = true;
                    foreach (Block b in linkedBlocks)
                        b.UsesGravity = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position - thisScene.Camera, color);
        }
    }
}
