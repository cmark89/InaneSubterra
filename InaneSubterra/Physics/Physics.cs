using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InaneSubterra.Physics
{
    public static class Physics
    {
        // axisList stores all ICollidable objects in the game and sorts them along their X axis.
        static List<ICollidable> axisList;

        // activeList is used during sweep and prune.
        static List<ICollidable> activeList;


        // Adds an object to the axisList.
        public static void AddObject(ICollidable obj)
        {
            axisList.Add(obj);
            SortAxisList();
        }


        // Removes an object from the axisList.
        public static void RemoveObject(ICollidable obj)
        {
            axisList.Remove(obj);
        }


        // Sorts the axisList along the X axis.
        public static void SortAxisList()
        {
            // Use a QuickSort algorithm here to sort by X position...
        }


        public void Update(GameTime gameTime)
        {
            // Run the broadphase, and then perform narrow phase on the sets of objects that result.
            if (axisList.Count > 1)
            {
                SortAxisList();
                NarrowphaseCollisionDetection(BroadphaseCollisionDetection());
            }
        }


        // BroadphaseCollisionDetection returns a list of ICollidable pairs to be checked for collision.
        public List<ICollidable[]> BroadphaseCollisionDetection()
        {
            List<ICollidable[]> possibleCollisionList = new List<ICollidable[]>();

            // Iterate through each CollisionActor in the axisList and check for possible collisions, adding each pair to the possibleCollisionList.
            return possibleCollisionList;
        }


        // NarrowphaseCollisionDetection takes a list of pairs of possibly colliding objects and tests to see if they collide.
        public void NarrowphaseCollisionDetection(List<ICollidable[]> possibleCollisions)
        {
            foreach (ICollidable[] colPair in possibleCollisions)
            {
                if (colPair[0].Hitbox.Intersects(colPair[1].Hitbox))
                {
                    // These objects are colliding, so send the collision information to each...
                    colPair[0].Collision(new CollisionEventArgs(colPair[1]));
                    colPair[1].Collision(new CollisionEventArgs(colPair[0]));
                }
                else
                    continue;
            }
        }
    }
}
