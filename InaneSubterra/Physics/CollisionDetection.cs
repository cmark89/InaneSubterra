using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InaneSubterra.Scenes;

namespace InaneSubterra.Physics
{
    public class CollisionDetection
    {

        #region Fields

        // axisList stores all ICollidable objects in the game and sorts them along their X axis.
        public List<ICollidable> axisList;

        public List<ICollidable> sleepingList;

        // activeList is used during sweep and prune.
        public List<ICollidable> activeList;

        public GameScene thisScene;

        #endregion

        // Static constructor instantiates the lists required for sweep and prune collision detection.
        public CollisionDetection(GameScene newScene)
        {
            thisScene = newScene;

            axisList = new List<ICollidable>();
            sleepingList = new List<ICollidable>();
            activeList = new List<ICollidable>();
        }


        // Adds an object to the axisList.
        public void AddObject(ICollidable obj)
        {
            axisList.Add(obj);
        }


        // Removes an object from the axisList.
        public void RemoveObject(ICollidable obj)
        {
            axisList.Remove(obj);
        }


        // Sorts the axisList along the X axis.
        public void SortAxisList()
        {
            // Use a QuickSort algorithm here to sort by X position...
            // This algorithm is for testing purposes and is VERY temporary.  Replace with QuickSort ASAP.

            for (int i = 0; i < axisList.Count; i++)
            {
                for (int j = i + 1; j < axisList.Count; j++)
                {
                    // If the latter element's X comes before the current element's X
                    if(axisList[j].Hitbox.X < axisList[i].Hitbox.X)
                    {
                        ICollidable tempCollidable = axisList[i];
                        axisList[i] = axisList[j];
                        axisList[j] = tempCollidable;
                    }   
                }
            }
        }


        public List<ICollidable> QuickSort(List<ICollidable> listToSort)
        {
            if (listToSort.Count < 2)
            {
                // List is already sorted or non existent, so return it
                return listToSort;
            }
            // Otherwise, perform the quicksort.
            else
            {
                // Select a pivot point from the list and remove it
                ICollidable pivot = listToSort[listToSort.Count / 2];
                listToSort.Remove(pivot);

                List<ICollidable> lessList = new List<ICollidable>();
                List<ICollidable> moreList = new List<ICollidable>();
                
                foreach (ICollidable ic in listToSort)
                {
                    if (ic.Hitbox.X < pivot.Hitbox.X)
                        lessList.Add(ic);
                    else
                        moreList.Add(ic);
                }

                // Sort recursively
                listToSort.Clear();
                listToSort.AddRange(QuickSort(lessList));
                listToSort.Add(pivot);
                listToSort.AddRange(QuickSort(moreList));

                return listToSort;
            }
        }


        public void Update(GameTime gameTime)
        {
            // Run the broadphase, and then perform narrow phase on the sets of objects that result.
            if (axisList.Count > 1)
            {
                //Console.Clear();
                //Console.WriteLine("Objects: " + axisList.Count);
                //Console.WriteLine("Bubblesort operations: " + (axisList.Count * axisList.Count).ToString());
                //Console.WriteLine("Quicksort operations: " + (axisList.Count * Math.Log10(axisList.Count)).ToString());

                // First, find objects that should sleep
                //foreach (ICollidable ic in axisList.FindAll(x => x.Hitbox.X + x.Hitbox.Width < thisScene.ScreenArea.X))
                //{
                    // Put them in the house of sleep
                    //ic.Sleeping = true;
                    //sleepingList.Add(ic);
                //}

                // Find sleeping objects that should awake
                //foreach (ICollidable ic in axisList.FindAll(x => x.Hitbox.X + x.Hitbox.Width > thisScene.ScreenArea.X && x.Sleeping))
                //{
                    // Wake them up
                    //ic.Sleeping = false;
                    //axisList.Add(ic);
                //}

                // Now make sure each list only contains the proper elements


                //SortAxisList();
                QuickSort(axisList);
                NarrowphaseCollisionDetection(BroadphaseCollisionDetection());
            }
        }


        // BroadphaseCollisionDetection returns a list of ICollidable pairs to be checked for collision.
        public List<ICollidable[]> BroadphaseCollisionDetection()
        {
            // Abort if the axisList contains less than 2 objects
            if (axisList.Count < 2)
                return null;

            List<ICollidable[]> possibleCollisionList = new List<ICollidable[]>();

            // Clear the activeList
            activeList.Clear();

            // Clear the possibleCollisionList
            possibleCollisionList.Clear();

            // Iterate through each ICollidable axisList and check for possible collisions, adding each pair to the possibleCollisionList.
            for(int i = 0; i < axisList.Count; i++)
            {
                if (axisList[i].Sleeping)
                    continue;

                // Make absolutely sure the active list is clear
                activeList.Clear();

                // Add this object to the axis list.
                activeList.Add(axisList[i]);

                // Check for possible collisions with each element after the active element
                for (int j = i + 1; j < axisList.Count; j++)
                {
                    if (axisList[i].Sleeping)
                        continue;

                    // If the ICollidable's X falls within the span of the active element's width...
                    if (axisList[j].Hitbox.X < (axisList[i].Hitbox.X + axisList[i].Hitbox.Width))
                    {
                        // Add it to the active list
                        activeList.Add(axisList[j]);
                    }
                    else
                    {
                        // Otherwise, move on to returning the possible collisions and continue the broadphase.
                        break;
                    }
                }

                // If there is at least one possible collision...
                if (activeList.Count > 1)
                {
                    // For each additional element in the possible collision list...
                    for (int k = 1; k < activeList.Count; k++)
                    {
                        // ...add that element and the root element to the possible collision list.
                        possibleCollisionList.Add(new ICollidable[]{ activeList[0], activeList[k] });
                    }
                }
                else
                    continue;
            }

            // Return all possible pairs of colliding objects.
            return possibleCollisionList;
        }


        // NarrowphaseCollisionDetection takes a list of pairs of possibly colliding objects and tests to see if they collide.
        public void NarrowphaseCollisionDetection(List<ICollidable[]> possibleCollisions)
        {
            // Abort if there are no possible collisions or the list is null.
            if (possibleCollisions == null || possibleCollisions.Count < 1)
                return;

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

        // CheckForCollision checks a specific area for collision against objects in the scene and returns overlapping objects
        public ICollidable[] CheckForCollision(Rectangle searchRect)
        {
            List<ICollidable> overlappingObjects = new List<ICollidable>();
            List<ICollidable> nearObjects = new List<ICollidable>();

            ICollidable tempCollidable = new BlankHitbox(searchRect);
            axisList.Add(tempCollidable);
            QuickSort(axisList);

            int index = axisList.FindIndex(x => x == tempCollidable);
            
            activeList.Clear();
            activeList.Add(tempCollidable);


            // Find all ICollidables that begin within the search box's X, but do not exceed it.
            for(int j = 0; j < axisList.Count; j++)
            {
                if (axisList[j].Hitbox.X < (tempCollidable.Hitbox.X + tempCollidable.Hitbox.Width) || axisList[j].Hitbox.X + axisList[j].Hitbox.Width < (tempCollidable.Hitbox.X + tempCollidable.Hitbox.Width))
                {
                    activeList.Add(axisList[j]);
                    continue;
                }
                else
                {
                    break;
                }
            }
            
            // For each object in the new list, perform a narrow check for collisions
            for(int i = 0; i < activeList.Count; i++)
            {
                if (activeList[i] == tempCollidable) continue;

                if (tempCollidable.Hitbox.Intersects(activeList[i].Hitbox))
                {
                    overlappingObjects.Add(activeList[i]);
                }
            }

            // Remove the temporary collision from the axisList
            axisList.Remove(tempCollidable);
            activeList.Clear();
            
            // Return all collisions
            return overlappingObjects.ToArray();
        }
    }
}
