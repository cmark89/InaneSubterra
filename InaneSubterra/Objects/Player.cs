using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Scenes;
using InaneSubterra.Physics;
using Microsoft.Xna.Framework.Input;
using ObjectivelyRadical.Controls;

namespace InaneSubterra.Objects
{
    public class Player : GameObject
    {
        public Player(GameScene scene, Vector2 newPos)
        {
            thisScene = scene;
            Position = newPos;

            // Obviously temporary
            Texture = thisScene.BlockTexture;

            Initialize();
        }

        new public void Initialize()
        {
            // Setup the basic attributes of the object
            UsesGravity = true;
            ObjectState = ObjectState.Falling;
            Solid = true;

            Name = "Player";

            // Add ResolveCollisions to the OnCollision event.
            OnCollision += ResolveCollisions;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            //Console.Clear();
            //Console.WriteLine("Player Object State: " + ObjectState.ToString());
            //Console.WriteLine("Player Y Velocity: " + YAcceleration);

            if (KeyboardManager.KeyDown(Keys.Left))
            {
                Position += new Vector2(-3, 0);
                if (ObjectState != ObjectState.Jumping)
                    RequestsFloorCollisionCheck = true;
            }

            if (KeyboardManager.KeyDown(Keys.Right))
            {
                Position += new Vector2(3, 0);
                if (ObjectState != ObjectState.Jumping)
                    RequestsFloorCollisionCheck = true;
            }

            if (KeyboardManager.KeyPressedDown(Keys.Space) && ObjectState == ObjectState.Grounded)
            {
                Console.WriteLine("Jump!");
                YAcceleration = -400;
                ObjectState = ObjectState.Jumping;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //if(floorHitbox != null)
                //spriteBatch.Draw(Texture, floorHitbox, new Color(1f,1f,0f, .5f));
        }
    }
}
