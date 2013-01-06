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
        string Name;

        // Each ICollidable must declare a hitbox for calculating collisions with other objects.
        Rectangle Hitbox;

        // Each ICollidable must declare whether it is a CollisionActor or not.  At least one CollisionActor is required for a collision. (This is to ensure inanimate objects do not try to collide with one another)
        bool CollisionActor;

        // Each ICollidable must declare a CollisionEventHandler and a function to raise that event.  How that event is handled is up to the object.
        CollisionEventHandler OnCollision;
        void Collision(CollisionEventArgs e);
    }

    public delegate void CollisionEventHandler(object sender, CollisionEventArgs e);

    /* Collision event args contains the object that collided with this one.
     *  One of the following will be the case:
     * 1. The Physics class checks for collisions each frame and then sends a new CollisionEventArgs to each object involved with the information of the other...OR
     * 2. Each object will keep track of its own collisions each frame using some manner sweep and prune, and then get the arguments based on that. */

    public class CollisionEventArgs : EventArgs
    {
        public ICollidable CollidedObject { get; private set; }

        public CollisionEventArgs(ICollidable otherObject)
        {
            CollidedObject = otherObject;
        }
    }
}
