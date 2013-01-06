using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Scenes;
using InaneSubterra.Physics;

namespace InaneSubterra.World
{
    public class Block : ICollidable
    {
        // Stores the parent scene to prevent making every field static
        GameScene parentScene;

        // Define the basic properties of a single block
        public Vector2 Position;
        public Texture2D Texture;
        public string Name = "Block";
        public Rectangle Hitbox;

        // Bool that stores whether the block is visible and should be drawn
        private bool isVisible;

        // Collision event handler
        public event CollisionEventHandler OnCollision;

        public Block(GameScene scene, Vector2 newPos)
        {
            parentScene = scene;
            Position = newPos;
            Texture = parentScene.BlockTexture;
        }


        public void Update(GameTime gameTime)
        {
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
                spriteBatch.Draw(Texture, Hitbox, parentScene.SequenceColors[parentScene.CurrentSequence]);
        }

        public void Collision(CollisionEventArgs e)
        {
            if(OnCollision != null)
                OnCollision(this, e);
        }
    }
}
