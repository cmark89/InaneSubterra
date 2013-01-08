using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ObjectivelyRadical.Controls;
using InaneSubterra.Physics;
using InaneSubterra.Objects;

namespace InaneSubterra.Scenes
{
    public class GameScene : Scene
    {
        #region Fields

        // Content cached in these fields for now...
        public Texture2D BackgroundTexture { get; private set; }
        public Texture2D BlockTexture { get; private set; }

        // Colors for the various sequences are stored here...
        public List<Color> SequenceColors { get; private set; }
        public int CurrentSequence { get; private set; }

        // Game physics components...
        public CollisionDetection collisionDetection;
        public Gravity gravity;

        // Store a list of all objects in the scene which will require updating...
        public List<GameObject> gameObjects;

        // The player object
        Player player;

        #endregion
        public GameScene()
        {
        }

        public override void Initialize()
        {
            // Set up the list of colors to be used as the player progresses through sequences.
            if (SequenceColors == null)
            {
                SequenceColors = new List<Color>()
                {
                    new Color(),
                    Color.AliceBlue,
                    Color.DarkOliveGreen,
                    Color.YellowGreen,
                    Color.DarkOrange,
                    Color.OrangeRed,
                    Color.IndianRed,
                    Color.DarkRed
                };
            }
            
            // Set the sequence to 1.  This has the effect of ignoring the first color in the Color list.
            CurrentSequence = 1;

            // Instantiate a new collision detection module
            collisionDetection = new CollisionDetection();
            
            // Instantiate a new gravity
            gravity = new Gravity(this);

            // Create a new list to store the game objects
            gameObjects = new List<GameObject>();
        }


        public override void LoadContent(ContentManager content)
        {
            // Move this to another static class for efficiency and organization
            if(BackgroundTexture == null)
                BackgroundTexture = content.Load<Texture2D>("Graphics/background");

            if(BlockTexture == null)
                BlockTexture = content.Load<Texture2D>("Graphics/block");

            TestAddBlock();
        }


        public override void Update(GameTime gameTime)
        {
            foreach (GameObject go in gameObjects)
            {
                // FIRST THING, update the object's last known position.
                go.SetLastPosition();

                // Then, run the normal update.
                go.Update(gameTime);

                if (go.UsesGravity)
                {
                    gravity.GravityUpdate(gameTime, go);
                }
            }

            // Check for any collisions and trigger relevant events.
            collisionDetection.Update(gameTime);

            foreach (GameObject go in gameObjects)
            {
                if (go.RequestsFloorCollisionCheck)
                {
                    go.CheckForFloor();
                    go.RequestsFloorCollisionCheck = false;
                }
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (GameObject go in gameObjects)
                go.Draw(spriteBatch);
        }


        public override void Unload()
        {
        }
        

        // Add blocks to the scene to test collision detection.
        private void TestAddBlock()
        {
            player = new Objects.Player(this, Vector2.Zero);

            new Objects.Platform(this, new Vector2(32, 500), 8, 1);


            new Objects.Block(this, new Vector2(150, 0)).ObjectState = ObjectState.Falling;
        }
    }


}
