using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InaneSubterra.Physics
{
    // This class is used to create a hitbox that is not connected to a GameObject
    public class BlankHitbox : ICollidable
    {
        public Rectangle Hitbox { get; private set; }

        public string Name { get; private set; }

        public void Collision(CollisionEventArgs e)
        {
            return;
        }

        public bool Solid { get; private set; }

        public bool Sleeping { get; set; }


        public BlankHitbox(Rectangle rect)
        {
            Hitbox = rect;
            Solid = false;
        }
    }
}
