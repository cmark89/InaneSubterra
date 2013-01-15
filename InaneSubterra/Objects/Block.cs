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
            //OnCollision += delegate(object sender, CollisionEventArgs e)
            //{
                // Furthermore, only resolve the collision if this object's Y is higher than the colliding object's.
                // This will prevent falling blocks from nudging platform blocks out of place.
                //if (e.CollidedObject.Name == "Block" && Hitbox.Y < e.CollidedObject.Hitbox.Y) 
                    //ResolveCollisions(sender, e);
            //};

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position - thisScene.Camera, color);
        }
    }
}
