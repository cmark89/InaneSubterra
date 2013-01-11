using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ObjectivelyRadical.Controls;
using InaneSubterra.Core;
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
        public Texture2D PlayerTexture { get; private set; }

        // Colors for the various sequences are stored here...
        public List<Color> SequenceColors { get; private set; }
        public int CurrentSequence { get; private set; }

        // Stores the camera position for drawing the game.
        public Vector2 Camera { get; private set; }

        // Stores the viewable area on the screen
        public Rectangle ScreenArea { get; private set; }

        // Game physics components...
        public CollisionDetection collisionDetection;
        public Gravity gravity;

        // Level generator
        WorldGenerator worldGenerator;

        // Store a list of all objects in the scene which will require updating...
        public List<GameObject> gameObjects;

        // Store the background for updating purposes.
        public List<ScrollingBackground> background;

        // The player object
        public Player player {get; private set;}

        // Stores the distance for which terrain has been generated.  When the camera exceeds this value, it creates more terrain and updates the value
        float levelLength;

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
                    Color.SkyBlue,
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
            collisionDetection = new CollisionDetection(this);
            
            // Instantiate a new gravity
            gravity = new Gravity(this);

            // Instantiate a new world generator
            worldGenerator = new WorldGenerator(this);

            // Create a new list to store the game objects
            gameObjects = new List<GameObject>();

            // Store the screen area
            Viewport view = InaneSubterra.graphics.GraphicsDevice.Viewport;
            ScreenArea = new Rectangle((int)Camera.X, (int)Camera.Y, view.Width, view.Height);
        }


        public override void LoadContent(ContentManager content)
        {
            // Move this to another static class for efficiency and organization
            if(BackgroundTexture == null)
                BackgroundTexture = content.Load<Texture2D>("Graphics/background");

            if(BlockTexture == null)
                BlockTexture = content.Load<Texture2D>("Graphics/block");

            if (PlayerTexture == null)
                PlayerTexture = content.Load<Texture2D>("Graphics/player");
            

            TestAddBlock();

            UpdateCamera();
            ScreenArea = new Rectangle((int)Camera.X, (int)Camera.Y, ScreenArea.Width, ScreenArea.Height);
            levelLength = ScreenArea.X + ScreenArea.Width + 1;

            background = new List<ScrollingBackground>();
            background.Add(new ScrollingBackground(this, (new Vector2(BackgroundTexture.Width / -2, 0))));
            background.Add(new ScrollingBackground(this, new Vector2(BackgroundTexture.Width / 2, 0)));

            UpdateCamera();
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

            UpdateCamera();

            if (ScreenArea.X + ScreenArea.Width > levelLength)
            {
                Console.WriteLine("Generating area!");
                worldGenerator.GenerateArea(ref levelLength);
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
            foreach (ScrollingBackground sb in background)
            {
                sb.Draw(spriteBatch);
            }

            foreach (GameObject go in gameObjects)
            {
                // Delay the player's draw until the end.
                if(go != player)
                    go.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);
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

        private void UpdateCamera()
        {
            // Cache the current camera position
            Vector2 lastCameraPos = Camera, cameraDelta;

            // Temporarily cache the viewport
            Viewport view = InaneSubterra.graphics.GraphicsDevice.Viewport;

            // Move the camera, centered on the player.
            Camera = new Vector2(player.Position.X - (view.Width / 2), Camera.Y);

            // Find how much the camera has changed position.
            cameraDelta = Camera - lastCameraPos;

            // If the camera has moved...
            if (cameraDelta.X != 0)
            {
                // Update the screen bounds to reflect the current viewport of the screen.
                ScreenArea = new Rectangle((int)Camera.X, (int)Camera.Y, view.Width, view.Height);

                // Update the background
                if(background != null)
                    UpdateBackground(cameraDelta);
            }
        }

        private void UpdateBackground(Vector2 cameraDelta)
        {
            List<ScrollingBackground> removeList = new List<ScrollingBackground>();
            foreach(ScrollingBackground sb in background)
            {
                // Scroll the background
                sb.Scroll(cameraDelta);

                // If the background image is too far to the left...
                if (sb.Right < (0))
                {
                    // Move it to the right side of the screen
                    sb.Position = new Vector2(sb.Position.X + ScreenArea.Width + sb.Texture.Width, sb.Position.Y);
                }
                // Otherwise, if it's too far to the right...
                else if (sb.Left > (ScreenArea.Width))
                {
                    // Move it to the left side of the screen
                    sb.Position = new Vector2(sb.Position.X - ScreenArea.Width - sb.Texture.Width, sb.Position.Y);
                }
            }
        }
    }


}
