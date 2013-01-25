using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra;
using InaneSubterra.Scenes;
using InaneSubterra.Physics;

namespace InaneSubterra.Objects
{
    public abstract class GameObject : ICollidable
    {
        protected Rectangle floorHitbox;

        // Each GameObject must have a name
        public string Name { get; protected set; }

        // Each GameObject must store a reference to the scene containing it.
        public GameScene thisScene;

        // Each GameObject must declare a Texture
        public Texture2D Texture { get; protected set; }

        // Each GameObject must track its own position
        public Vector2 Position { get; protected set; }
        public Vector2 PreviousPosition { get; protected set; }

        // Determines whether the object can be moved through
        public bool Solid { get; protected set; }

        // Stores whether the object is sleeping or not
        public bool Sleeping { get; set; }

        // Stores whether the object is on the ground or not.
        public ObjectState ObjectState { get; set; }

        // Each GameObject must declare whether or not it is affected by gravity.
        public bool UsesGravity { get; set; }

        // Stores whether the object is jumping or not.
        public bool Jumping { get; set; }

        // This flags whether or not the object will require a late floor collision check this frame
        public bool RequestsFloorCollisionCheck = false;

        // Each GameObject has a Hitbox for dealing with collisions.  By default, it is the displayed sprite.

        public int FrameWidth = 32;
        public int FrameHeight = 32;
        public virtual Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, FrameWidth, FrameHeight);
            }
        }

        // Collision event handler
        public event CollisionEventHandler OnCollision;

        // YAcceleration used to apply gravity
        public double YAcceleration = 0f;
        public float TerminalVelocity = 500f;
        
        
        

        // Initialize will add the object to the gameScene's list of game objects.
        public void Initialize()
        {
            if(InaneSubterra.currentScene is Scenes.GameScene)
            {
                GameScene thisGameScene = InaneSubterra.currentScene as GameScene;

                if(!thisGameScene.gameObjects.Contains(this))
                    thisGameScene.gameObjects.Add(this);

                thisScene.collisionDetection.AddObject(this);
            }
        }


        public virtual void Update(GameTime gameTime)
        {
            SetLastPosition();
        }


        public void SetLastPosition()
        {
            PreviousPosition = Position;
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position - thisScene.Camera, Color.White);

            // Draw the hitbox
            spriteBatch.Draw(thisScene.BlockTexture, Position - thisScene.Camera, new Color(0f,.6f, 1f, .7f));
            //spriteBatch.Draw(Texture, Hitbox, new Color(0f, .8f, .3f, .5f));
        }


        public void Collision(CollisionEventArgs e)
        {
            if (OnCollision != null)
                OnCollision(this, e);
        }


        public void Destroy()
        {
            if (InaneSubterra.currentScene is Scenes.GameScene)
            {
                GameScene thisGameScene = InaneSubterra.currentScene as GameScene;

                if (thisGameScene.gameObjects.Contains(this))
                {
                    thisGameScene.gameObjects.Remove(this);
                    thisScene.collisionDetection.RemoveObject(this);
                }
            }
        }


        // This method will resolve collisions the object is involved in by moving it out of any intersecting solid objects.
        public void ResolveCollisions(object sender, CollisionEventArgs e)
        {
            ICollidable otherObject = e.CollidedObject;

            // If the hitboxes no longer intersect (due to a previous collision resolution etc.), prevent the resolution from occuring
            if (!Hitbox.Intersects(otherObject.Hitbox))
                return;

            if (otherObject.Solid)
            {
                // Find the dimensions of the overlap
                // Pick the shallow axis and move the object to its previous position along that axis only

                int x1, x2, y1, y2;

                x1 = Math.Max(Hitbox.X, otherObject.Hitbox.X);
                x2 = Math.Min(Hitbox.X + Hitbox.Width, otherObject.Hitbox.X + otherObject.Hitbox.Width);
                
                y1 = Math.Max(Hitbox.Y, otherObject.Hitbox.Y);  
                y2 = Math.Min(Hitbox.Y + Hitbox.Height, otherObject.Hitbox.Y + otherObject.Hitbox.Height);

                int xPen = x2 - x1;
                int yPen = y2 - y1;

                // If X penetration is shallower...
                if (xPen <= yPen)
                {
                    // Resolve along the X axis

                    //If collision comes from the right
                    if(PreviousPosition.X > Position.X)
                        Position = new Vector2(Position.X + (xPen + 1), Position.Y);
                    else
                        Position = new Vector2(Position.X - (xPen + 1), Position.Y);
                }
                else
                {
                    // Otherwise resolve around Y

                    // If the collision occurs from above...
                    if (PreviousPosition.Y < Position.Y)
                    {
                        Position = new Vector2(Position.X, Position.Y - yPen);
                        //Position = new Vector2(Position.X, otherObject.Hitbox.Y - Hitbox.Height);
                        CheckForFloor();
                    }
                    else
                    {
                        Position = new Vector2(Position.X, Position.Y + yPen);
                    }

                    // Make a recursive call in the fringe case that both sides have to be resolved.
                    ResolveCollisions(this, e);
                }
            }
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        public void CheckForFloor()
        {
            // There's no need to check for the floor if the object is jumping
            if (ObjectState == ObjectState.Jumping)
                return;

            floorHitbox = new Rectangle(Hitbox.X, Hitbox.Y + Hitbox.Height, Hitbox.Width, 1);
            List<ICollidable> floorObjectList = thisScene.collisionDetection.CheckForCollision(floorHitbox).ToList<ICollidable>().FindAll(x => x.Hitbox.Y <= Hitbox.Y + Hitbox.Height);
           
            if (floorObjectList.Contains(this))
                floorObjectList.Remove(this);

            if (floorObjectList.FindAll(x => x.Solid).Count > 0)
            {
                ObjectState = ObjectState.Grounded;

                // Special exception for blocks that are falling
                foreach(ICollidable ic in floorObjectList.FindAll(x => x is Block))
                {
                    Block b = ic as Block;
                    b.OnCollision(this, new CollisionEventArgs(this));
                }
            }
            else
            {
                ObjectState = ObjectState.Falling;
            }
        }
    }

    public enum ObjectState
    {
        None,
        Jumping,
        Falling,
        Grounded
    }

    public enum Facing
    {
        Left,
        Right
    }
}
