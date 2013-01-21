using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Core;
using InaneSubterra.Scenes;
using InaneSubterra.Physics;
using Microsoft.Xna.Framework.Input;
using ObjectivelyRadical.Controls;

namespace InaneSubterra.Objects
{
    public class EvilPlayer : GameObject
    {
        AnimatedSprite sprite;

        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X + 12, (int)Position.Y, 26, 43);
            }
        }

        public EvilPlayer(GameScene scene, Vector2 newPos)
        {
            thisScene = scene;
            Position = newPos;

            Texture = thisScene.PlayerTexture;
            FrameWidth = 48;
            FrameHeight = 48;

            Initialize();
        }

        new public void Initialize()
        {
            // Setup the basic attributes of the object
            UsesGravity = false;
            ObjectState = ObjectState.None;

            Solid = false;

            Name = "EvilPlayer";

            // Set up the player's animations here
            LoadAnimations();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Update the position
            sprite.Position = Position;

            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }

        public void LoadAnimations()
        {
            Dictionary<string, Animation> playerAnimations = new Dictionary<string, Animation>();

            playerAnimations.Add("Stand", new Animation(thisScene, Texture, 48, 48, new int[] { 0 }, 10, true));
            playerAnimations.Add("Jump", new Animation(thisScene, Texture, 48, 48, new int[] { 1 }, 10, true));
            playerAnimations.Add("Fall", new Animation(thisScene, Texture, 48, 48, new int[] { 2 }, 10, true));
            playerAnimations.Add("Walk", new Animation(thisScene, Texture, 48, 48, new int[] { 3, 4, 5, 6, 7, 8 }, 10, true));
            playerAnimations.Add("Shoot", new Animation(thisScene, Texture, 48, 48, new int[] { 9 }, 10, true));
            
            sprite = new AnimatedSprite(thisScene, playerAnimations);
            sprite.Flipped = true;
            sprite.PlayAnimation("Stand");
        }

        public void Shoot()
        {
            sprite.PlayAnimation("Shoot");
        }
    }
}
