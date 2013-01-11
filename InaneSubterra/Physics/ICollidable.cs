using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InaneSubterra.Physics
{
    public interface ICollidable
    {
        // Each ICollidable must declare a name for collision testing purposes.
        string Name { get; }

        // Each ICollidable must declare a hitbox for calculating collisions with other objects.
        Rectangle Hitbox { get; }

        // Each ICollidable must declare a CollisionEventHandler and a function to raise that event.  How that event is handled is up to the object.
        void Collision(CollisionEventArgs e);

        // Determines whether the object totally blocks movement
        bool Solid { get; }

        // Used to disable objects that are no longer needed for collision detection
        bool Sleeping { get; set; }
    }

    // CollisionEventHandler is used to raise the OnCollision event.
    public delegate void CollisionEventHandler(object sender, CollisionEventArgs e);

    // Collision event args contains the object that collided with this one.
    public class CollisionEventArgs : EventArgs
    {
        public ICollidable CollidedObject { get; private set; }

        public CollisionEventArgs(ICollidable otherObject)
        {
            CollidedObject = otherObject;
        }
    }
}
