using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Scenes;
using InaneSubterra.Core;
using InaneSubterra.Physics;

namespace InaneSubterra.Objects
{
    public class SequenceCrystal : GameObject
    {
        Color color;
        AnimatedSprite sprite;
        bool triggered = false;

        public SequenceCrystal(GameScene newScene, Vector2 newPos)
        {
            thisScene = newScene;
            color = thisScene.SequenceColors[thisScene.CurrentSequence + 1];
            Position = newPos;

            FrameWidth = 48;
            FrameHeight = 48;

            Initialize();
        }


        new public void Initialize()
        {
            Solid = false;
            UsesGravity = false;
            Name = "Sequence Crystal";

            thisScene.crystalAppeared = true;

            // Setup the animations for the crystal
            SetupAnimations();

            // Subscribe the CrystalReached function to the OnCollision event, but only trigger it for the player
            OnCollision += delegate(object sender, CollisionEventArgs e)
            {
                if (e.CollidedObject.Name == "Player" && !triggered)
                    CrystalReached(sender, e);
            };

            base.Initialize();
        }


        public void SetupAnimations()
        {
            // Build the animated sprite
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            animations.Add("Stand", new Animation(thisScene, thisScene.CrystalTexture, 48, 48, new int[] { 0, 1, 2, 3 }, 8f, true));

            sprite = new AnimatedSprite(thisScene, animations);
            sprite.PlayAnimation("Stand");
            sprite.Position = Position;
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, color);
            
            //Figure out the proper hitbox with this
            //spriteBatch.Draw(thisScene.BlockTexture, new Rectangle((int)(Hitbox.X - thisScene.Camera.X), (int)(Hitbox.Y - thisScene.Camera.Y), Hitbox.Width, Hitbox.Height), new Color(0f,.4f,.8f,.5f));
        }


        // This is called when the player collides with the crystal
        public void CrystalReached(object sender, CollisionEventArgs e)
        {
            thisScene.SequenceUp();
            triggered = true;
            Destroy();
        }
    }
}
