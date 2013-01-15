using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InaneSubterra.Objects;
using InaneSubterra.Scenes;

namespace InaneSubterra.Physics
{
    public class Gravity
    {
        GameScene thisScene;
        public const double gravityAcceleration = 430f;


        public Gravity(GameScene gameScene)
        {
            thisScene = gameScene;
        }


        public virtual void GravityUpdate(GameTime gameTime, GameObject go)
        {
            if (go.ObjectState != ObjectState.Falling && go.ObjectState != ObjectState.Jumping)
                return;

            double gravityAccelerationChange = gravityAcceleration;
            if (go.ObjectState == ObjectState.Jumping)
            {
                gravityAccelerationChange *= 1.3f;

                if (go.YAcceleration > 0)
                    go.ObjectState = ObjectState.Falling;
            }


            go.YAcceleration += gravityAccelerationChange * gameTime.ElapsedGameTime.TotalSeconds;

            // Clamp acceleration between 1 and -1 times the object's terminal velocity
            if (go.YAcceleration > go.TerminalVelocity)
            {
                go.YAcceleration = go.TerminalVelocity;
            }
            else if (go.YAcceleration < -go.TerminalVelocity)
            {
                go.YAcceleration = -go.TerminalVelocity;
            }

            float newYPos = go.Position.Y + (float)(go.YAcceleration * gameTime.ElapsedGameTime.TotalSeconds);
            go.SetPosition(new Vector2(go.Position.X, newYPos));
        }
    }
}
