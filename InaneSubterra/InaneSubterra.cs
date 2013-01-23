using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ObjectivelyRadical;
using ObjectivelyRadical.Controls;

namespace InaneSubterra
{
    // my code don't steal
    public class InaneSubterra : Microsoft.Xna.Framework.Game
    {
        #region Fields
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Stores a static copy of the content manager
        public static ContentManager StaticContent { get; private set; }

        // Stores the current game scene
        public static Scene currentScene { get; private set; }





        #endregion

        public InaneSubterra()
        {
            graphics = new GraphicsDeviceManager(this);

            // Set the resolution to 800 x 600
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            // Initialize the game

            // For now, set the current scene to a new GameScene() instance.
            
            SetScene(new Scenes.GameScene());
            //SetScene(new Scenes.TitleScene());

            //Set the graphics resolution...

            
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the content
        }


        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            // Update the input manager classes.
            MouseManager.Update(gameTime);
            KeyboardManager.Update(gameTime);

            // Update the current scene if it exists.
            if (currentScene != null)
            {
                currentScene.Update(gameTime);
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();


            // If the current game scene exists, then draw everything for it.
            if (currentScene != null)
            {
                currentScene.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }


        protected void SetScene(Scene newScene)
        {
            if (currentScene != null)
                currentScene.Unload();

            currentScene = newScene;

            newScene.Initialize();
            newScene.LoadContent(Content);
        }
    }
}
