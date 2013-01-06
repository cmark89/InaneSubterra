using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ObjectivelyRadical.Controls;

namespace InaneSubterra.Scenes
{
    public class GameScene : Scene
    {
        #region Fields

        // Content cached in these fields...
        public Texture2D BackgroundTexture { get; private set; }
        public Texture2D BlockTexture { get; private set; }

        // Colors for the various sequences are stored here...
        public List<Color> SequenceColors { get; private set; }
        public int CurrentSequence { get; private set; }

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
        }


        public override void LoadContent(ContentManager content)
        {
            // Move this to another static class for efficiency and organization
            if(BackgroundTexture == null)
                BackgroundTexture = content.Load<Texture2D>("/Graphics/background");

            if(BlockTexture == null)
                BlockTexture = content.Load<Texture2D>("/Graphics/block");
        }


        public override void Update(GameTime gameTime)
        {
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
