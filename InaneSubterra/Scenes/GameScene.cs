﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using ObjectivelyRadical.Controls;
using InaneSubterra.Core;
using InaneSubterra.Physics;
using InaneSubterra.Objects;
using ObjectivelyRadical.Scripting;

namespace InaneSubterra.Scenes
{
    public class GameScene : Scene
    {
        #region Fields
        // Content cached in these fields for now...
        public Texture2D BackgroundTexture { get; private set; }
        public Texture2D BlockTexture { get; private set; }
        public Texture2D PlayerTexture { get; private set; }
        public Texture2D CrystalTexture { get; private set; }
        public Texture2D FadeTexture { get; private set; }

        public SpriteFont SequenceFont { get; private set; }
        public SpriteFont LargeFont { get; private set; }

        public Song BeneathThisDelusion;
        public Song DelusionAwakening;

        public SoundEffect gunshot;
        public SoundEffect fall;
        public SoundEffect jumpSound;
        public SoundEffect landSound;
        public SoundEffect deathSound;
        public SoundEffect platformFallSound;
        public SoundEffect sequenceUpSound;

        // Colors for the various sequences are stored here...
        public List<Color> SequenceColors { get; private set; }
        public List<string> SequenceTitles { get; private set; }
        public int CurrentSequence { get; private set; }

        // Stores the camera position for drawing the game.
        public Vector2 Camera { get; private set; }

        // Stores the viewable area on the screen
        public Rectangle ScreenArea { get; private set; }

        // Game physics components...
        public CollisionDetection collisionDetection;
        public Gravity gravity;

        // Use this to determine if the player should see the instructional text.
        public static bool tutorialSeen = false;

        // Check if the player is dead
        bool playerDead = false;

        // Level generator
        WorldGenerator worldGenerator;

        // Store a list of all objects in the scene which will require updating...
        public List<GameObject> gameObjects;

        // Store the background for updating purposes.
        public List<ScrollingBackground> background;

        // Store the background for updating purposes.
        public List<SequenceText> sequenceText;

        // The player object
        public Player player {get; private set;}

        // Stores the distance for which terrain has been generated.  When the camera exceeds this value, it creates more terrain and updates the value
        float levelLength;

        // Stores the length of the current sequence; used to determine when to advance to the next sequence
        public float sequenceLength;
        
        // Stores the length of the current sequence after which it will be possible for a Sequence Crystal to spawn, and the length at which it is certain
        public float sequenceCrystalMinLength = 2000f;
        public float sequenceCrystalMaxLength = 4000f;

        public bool crystalAppeared = false;

        // Store the player's reflection if it exists
        public EvilPlayer evilPlayer;

        // Variables related to the ending
        private float endingTimer;
        private bool reflectionMet;
        private bool shotFired;
        private bool playerDown;

        private bool screenFade;
        private Color fadeColor;
        private bool completedFade;
        private bool endingTextShown;
        private Color endingTextColor;

        private bool screenFlash;
        private Color flashColor;
        private float flashTime;

        private ScriptReader scriptReader;

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
                    Color.Aquamarine,               //Sequence 1: Curiosity
                    Color.SpringGreen,              //Sequence 2: Optimism
                    Color.YellowGreen,              //Sequence 3: Hope
                    Color.DarkOliveGreen,           //Sequence 4: Doubt
                    Color.DarkOrange,               //Sequence 5: Regret
                    Color.OrangeRed,                //Sequence 6: Fear
                    Color.MediumPurple,             //Sequence 7: Guilt
                    Color.DarkMagenta,              //Sequence 8: Despair
                    Color.Red,                       //Sequence 9: Hate
                    new Color(.1f, .1f, .1f, 1f),   //Sequence 10: Introspection
                    new Color(1f,1f,1f, .5f)        //Sequence 0: Truth
                };
            }

            // Set up the list of titles for the sequence
            if (SequenceTitles == null)
            {
                SequenceTitles = new List<string>()
                {
                    "",
                    "Curiosity",
                    "Optimism",
                    "Hope",
                    "Doubt",
                    "Denial",
                    "Fear",
                    "Guilt",
                    "Despair",
                    "Futility",
                    "Introspection",
                    "Truth"
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
            sequenceText = new List<SequenceText>();

            // Store the screen area
            Viewport view = InaneSubterra.graphics.GraphicsDevice.Viewport;
            ScreenArea = new Rectangle((int)Camera.X, (int)Camera.Y, view.Width, view.Height);

            scriptReader = new ScriptReader();
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

            if (CrystalTexture == null)
                CrystalTexture = content.Load<Texture2D>("Graphics/crystal");

            if (FadeTexture == null)
                FadeTexture = content.Load<Texture2D>("Graphics/fade");

            if (SequenceFont == null)
                SequenceFont = content.Load<SpriteFont>("Font/SequenceFont");

            if (LargeFont == null)
                LargeFont = content.Load<SpriteFont>("Font/TitleFont");

            if (BeneathThisDelusion == null)
                BeneathThisDelusion = content.Load<Song>("Audio/BeneathThisDelusion");

            if (DelusionAwakening == null)
                DelusionAwakening = content.Load<Song>("Audio/DelusionAwakening");

            if (gunshot == null)
                gunshot = content.Load<SoundEffect>("Audio/gunshot");

            if (fall == null)
                fall = content.Load<SoundEffect>("Audio/fall");

            if (sequenceUpSound == null)
                sequenceUpSound = content.Load<SoundEffect>("Audio/SequenceUp");

            if (deathSound == null)
                deathSound = content.Load<SoundEffect>("Audio/deathSound");

            if (jumpSound == null)
                jumpSound = content.Load<SoundEffect>("Audio/Jump");

            if (platformFallSound == null)
                platformFallSound = content.Load<SoundEffect>("Audio/PlatformFall");

            if (landSound == null)
                landSound = content.Load<SoundEffect>("Audio/Land");

            AddStartingBlocks();

            UpdateCamera();
            ScreenArea = new Rectangle((int)Camera.X, (int)Camera.Y, ScreenArea.Width, ScreenArea.Height);
            levelLength = ScreenArea.X + ScreenArea.Width + 1;

            background = new List<ScrollingBackground>();
            background.Add(new ScrollingBackground(this, (new Vector2(BackgroundTexture.Width / -2, 0))));
            background.Add(new ScrollingBackground(this, new Vector2(BackgroundTexture.Width / 2, 0)));

            UpdateCamera();
            PlaySong(BeneathThisDelusion);
            scriptReader.Execute(GameStart);
        }


        public override void Update(GameTime gameTime)
        {
            scriptReader.Update(gameTime);
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

            if (CurrentSequence < 11 && ScreenArea.X + ScreenArea.Width > levelLength)
            {
                // Tell the world generation to create a new platform
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

            // Update color changing for the background if needed
            foreach (ScrollingBackground sb in background)
            {
                sb.Update(gameTime);     
            }

            if (screenFlash)
            {
                flashTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (flashTime < 0)
                    screenFlash = false;
            }

            if (sequenceText.Count > 0)
            {
                foreach (SequenceText st in sequenceText)
                    st.Update(gameTime);

                foreach (SequenceText st in sequenceText.FindAll(x => !x.visible))
                    sequenceText.Remove(st);
            }

            if (player != null && player.controlEnabled && !playerDead && player.Hitbox.Y > ScreenArea.Height)
            {
                scriptReader.Execute(PlayerDeath);
            }


            // Ending update
            if (CurrentSequence == 11)
            {
                endingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (!reflectionMet && endingTimer > 1f)
                {
                    fadeColor = Color.Lerp(Color.White, Color.Transparent, (endingTimer - 1f) / 3f);
                    if ((endingTimer - 1f) / 3f >= 1)
                    {
                        screenFade = false;
                    }
                }
                if (!reflectionMet && Vector2.Distance(player.Position, evilPlayer.Position) < ScreenArea.Width / 3)
                {
                    reflectionMet = true;
                    player.StopWalking();
                    endingTimer = 0;
                }
                if (reflectionMet && !shotFired)
                {
                    if (endingTimer >= 11f)
                    {
                        shotFired = true;
                        evilPlayer.Shoot();
                        player.GetShot();
                        endingTimer = 0;
                        gunshot.Play(.5f, 0f, 0f);
                        MediaPlayer.Stop();
                    }
                    else
                    {
                        Camera = new Vector2(Camera.X + 15.9f * (float)gameTime.ElapsedGameTime.TotalSeconds, Camera.Y);
                    }
                }
                

                if (shotFired && !playerDown && endingTimer > 3f)
                {
                    playerDown = true;
                    player.Fall();
                    fall.Play(.65f, 0f, 0f);
                    endingTimer = 0;
                }

                if (!completedFade && playerDown && endingTimer > 2.5f)
                {
                    // Begin fading the screen here, and fading in the text.
                    if(!screenFade)
                        screenFade = true;

                    fadeColor = Color.Lerp(Color.Transparent, Color.White, (endingTimer - 2.5f) / 4f);

                    if (fadeColor == Color.White)
                    {
                        completedFade = true;
                        endingTimer = 0f;
                    }
                }

                if (completedFade)
                {
                    if (!endingTextShown)
                        endingTextShown = true;

                    endingTextColor = Color.Lerp(Color.Transparent, Color.Black, (endingTimer - 2.5f) / 5f);

                    if (endingTimer > 10f)
                    {
                        endingTextColor = Color.Lerp(Color.Black, Color.Transparent, (endingTimer - 10f) / 5f);
                        fadeColor = Color.Lerp(Color.White, Color.Black, (endingTimer - 10f) / 5f);

                        if (fadeColor == Color.Black)
                        {
                            InaneSubterra.SetScene(new TitleScene());
                        }
                    }
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

            if(player != null)
                player.Draw(spriteBatch);

            if (screenFlash)
                spriteBatch.Draw(FadeTexture, new Rectangle(0, 0, ScreenArea.Width, ScreenArea.Height), Color.Lerp(Color.Transparent, flashColor, flashTime));
                

            if (screenFade)
                spriteBatch.Draw(FadeTexture, new Rectangle(0, 0, ScreenArea.Width, ScreenArea.Height), fadeColor);

            foreach (SequenceText st in sequenceText)
            {
                st.Draw(spriteBatch);
            }

            if (endingTextShown)
                spriteBatch.DrawString(LargeFont, "The Sequence Continues...", new Vector2((ScreenArea.Width - LargeFont.MeasureString("The Sequence Continues...").X) / 2f, (ScreenArea.Height - LargeFont.MeasureString("The Sequence Continues...").Y) / 2), endingTextColor);
        }


        public override void Unload()
        {
        }
        

        // Add blocks to the scene to test collision detection.
        private void AddStartingBlocks()
        {
            player = new Objects.Player(this, new Vector2(0, ScreenArea.Height/2));

            new Objects.Platform(this, new Vector2(-160, 500), 11, 1);
        }

        private void UpdateCamera()
        {
            // Do not update camera once the ending has begun.
            if (reflectionMet)
                return;

            // Cache the current camera position
            Vector2 lastCameraPos = Camera, cameraDelta;

            // Temporarily cache the viewport
            Viewport view = InaneSubterra.graphics.GraphicsDevice.Viewport;

            // Move the camera, centered on the player.
            if(player != null)
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

        public void SequenceUp()
        {
            scriptReader.Execute(SequenceUpScript);
        }

        public IEnumerator<float> SequenceUpScript()
        {
            CurrentSequence++;
            sequenceLength = 0;

            sequenceUpSound.Play(.8f, 0f, 0f);
            screenFlash = true;
            flashColor = SequenceColors[CurrentSequence];
            flashTime = .5f;

            foreach (ScrollingBackground bg in background)
            {
                bg.ChangeColor(SequenceColors[CurrentSequence], 5f);
            }


            worldGenerator.SequenceUp();

            // Set this to 11, like the joke
            if (CurrentSequence == 11)
            {
                PlaySong(DelusionAwakening);
                //Create the final platform
                Platform finalPlatform = new Platform(this, new Vector2(levelLength + 1, 400), 98, 15);
                player.controlEnabled = false;
                player.BeginWalkingRight();

                screenFade = true;
                fadeColor = Color.White;
            }
            else
            {
                crystalAppeared = false;
            }

            yield return 1.3f;
            ShowSequenceText();
        }

        public void PlaySong(Song newSong)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(newSong);
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
        }

        public void ShowSequenceText()
        {
            int thisSequence = CurrentSequence;
            if (CurrentSequence == 11)
                thisSequence = 0;

            sequenceText.Add(new SequenceText(this, "Sequence " + thisSequence, new Vector2(0 - SequenceFont.MeasureString("Sequence " + CurrentSequence).X, 30), (ScreenArea.Width / 4f), ((ScreenArea.Width / 4f) * 3), 400, 60));
            sequenceText.Add(new SequenceText(this, SequenceTitles[CurrentSequence].PadLeft(11), new Vector2(ScreenArea.Width, 80), (ScreenArea.Width / 4f), ((ScreenArea.Width / 4f) * 3), -400, -60));
        }

        public IEnumerator<float> GameStart()
        {
            if (!tutorialSeen)
            {
                tutorialSeen = true;
                yield return .5f;
                sequenceText.Add(new SequenceText(this, "Arrow keys to move", new Vector2(0 - SequenceFont.MeasureString("Arrow keys to move").X, 500), (ScreenArea.Width / 4f), ((ScreenArea.Width / 4f) * 3), 400, 60));
                yield return 1.5f;
                sequenceText.Add(new SequenceText(this, "Spacebar to jump", new Vector2(0 - SequenceFont.MeasureString("Spacebar to jump").X, 550), (ScreenArea.Width / 4f), ((ScreenArea.Width / 4f) * 3), 400, 60));
                yield return 2f;
            }

            yield return 2f;
            
            ShowSequenceText();
        }

        public IEnumerator<float> PlayerDeath()
        {
            playerDead = true;
            player.Destroy();
            player = null;

            StopMusic();
            deathSound.Play();
            yield return 1.4f;
            InaneSubterra.SetScene(new TitleScene());
        }
    }


}
