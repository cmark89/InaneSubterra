﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InaneSubterra.Core;
using InaneSubterra.Scenes;
using InaneSubterra.Physics;
using Microsoft.Xna.Framework.Input;
using ObjectivelyRadical.Controls;

namespace InaneSubterra.Objects
{
    public class Player : GameObject
    {
        AnimatedSprite sprite;
        PlayerState playerState;
        Facing facing;

        public float horizontalSpeed = 180;
        public float jumpVelocity = -400;

        public float xVelocity;

        public bool controlEnabled = true;

        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X + 12, (int)Position.Y, 26, 43);
            }
        }

        public Player(GameScene scene, Vector2 newPos)
        {
            thisScene = scene;
            Position = newPos;

            Texture = thisScene.PlayerTexture;
            FrameWidth = 48;
            FrameHeight = 48;

            Initialize();
        }

        new public void Initialize()
        {
            // Setup the basic attributes of the object
            UsesGravity = true;
            ObjectState = ObjectState.Falling;

            playerState = PlayerState.Fall;
            Solid = true;

            facing = Facing.Right;

            Name = "Player";

            // Add ResolveCollisions to the OnCollision event.
            OnCollision += ResolveCollisions;

            // Set up the player's animations here
            LoadPlayerAnimations();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (controlEnabled)
            {
                if (KeyboardManager.KeyDown(Keys.Left))
                {
                    Position += new Vector2((int)(horizontalSpeed * gameTime.ElapsedGameTime.TotalSeconds * -1), 0);
                    if (ObjectState != ObjectState.Jumping)
                        RequestsFloorCollisionCheck = true;

                    if (ObjectState == ObjectState.Grounded)
                    {
                        playerState = PlayerState.Walk;
                        sprite.PlayAnimation("Walk");
                    }

                    facing = Facing.Left;
                }

                if (KeyboardManager.KeyDown(Keys.Right))
                {
                    Position += new Vector2((int)(horizontalSpeed * gameTime.ElapsedGameTime.TotalSeconds), 0);
                    if (ObjectState != ObjectState.Jumping)
                        RequestsFloorCollisionCheck = true;

                    if (ObjectState == ObjectState.Grounded)
                    {
                        playerState = PlayerState.Walk;
                        sprite.PlayAnimation("Walk");
                    }

                    facing = Facing.Right;
                }

                if (KeyboardManager.KeyPressedDown(Keys.Space) && ObjectState == ObjectState.Grounded)
                {
                    YAcceleration = jumpVelocity;
                    ObjectState = ObjectState.Jumping;
                    playerState = PlayerState.Jump;
                    sprite.PlayAnimation("Jump");

                    thisScene.jumpSound.Play(.7f, 0f, 0f);
                }

                if (ObjectState == ObjectState.Jumping && KeyboardManager.KeyPressedUp(Keys.Space))
                {
                    YAcceleration = 0;
                    ObjectState = ObjectState.Falling;
                    playerState = PlayerState.Fall;
                    sprite.PlayAnimation("Fall");
                }

                if (ObjectState == ObjectState.Grounded && playerState == PlayerState.Fall)
                {
                    playerState = PlayerState.Stand;
                    sprite.PlayAnimation("Stand");
                }

                if (ObjectState == ObjectState.Grounded)
                {
                    YAcceleration = 0f;
                }

                if (ObjectState == ObjectState.Falling && playerState != PlayerState.Fall)
                {
                    playerState = PlayerState.Fall;
                    sprite.PlayAnimation("Fall");
                }

                if (KeyboardManager.KeyUp(Keys.Left) && KeyboardManager.KeyUp(Keys.Right))
                {
                    if (playerState == PlayerState.Walk)
                    {
                        playerState = PlayerState.Stand;
                        sprite.PlayAnimation("Stand");
                    }
                }
            }
            else
            {
                // Control is not enabled, so move the player based on what its horizontal speed is set to
                Position += new Vector2((int)(horizontalSpeed * gameTime.ElapsedGameTime.TotalSeconds), 0);
            }

            // Update the player sprite
            sprite.Position = Position;

            sprite.Flipped = (facing == Facing.Left);
            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }

        public void LoadPlayerAnimations()
        {
            Dictionary<string, Animation> playerAnimations = new Dictionary<string, Animation>();

            playerAnimations.Add("Stand", new Animation(thisScene, Texture, 48, 48, new int[] { 0 }, 10, true));
            playerAnimations.Add("Jump", new Animation(thisScene, Texture, 48, 48, new int[] { 1 }, 10, true));
            playerAnimations.Add("Fall", new Animation(thisScene, Texture, 48, 48, new int[] { 2 }, 10, true));
            playerAnimations.Add("Walk", new Animation(thisScene, Texture, 48, 48, new int[] { 3, 4, 5, 6, 7, 8 }, 10, true));
            playerAnimations.Add("Hurt", new Animation(thisScene, Texture, 48, 48, new int[] { 18 }, 10, true));
            playerAnimations.Add("Down", new Animation(thisScene, Texture, 48, 48, new int[] { 19 }, 10, true));

            sprite = new AnimatedSprite(thisScene, playerAnimations);
            sprite.Position = Position;

            sprite.PlayAnimation("Fall");
        }

        // Used in the game ending to force the player to begin moving against its will
        public void BeginWalkingRight()
        {
            playerState = PlayerState.Walk;
            ObjectState = ObjectState.Grounded;
            facing = Facing.Right;
            sprite.PlayAnimation("Walk");


            SetHorizontalSpeed(180f);
        }

        public void StopWalking()
        {
            playerState = PlayerState.Stand;
            ObjectState = ObjectState.Grounded;
            sprite.PlayAnimation("Stand");
            SetHorizontalSpeed(0f);
        }

        public void GetShot()
        {
            sprite.PlayAnimation("Hurt");
            SetHorizontalSpeed(-80f);
        }

        public void Fall()
        {
            sprite.PlayAnimation("Down");
            SetHorizontalSpeed(0f);
        }

        public void SetHorizontalSpeed(float speed)
        {
            horizontalSpeed = speed;
        }

        public enum PlayerState
        {
            Stand,
            Walk,
            Jump,
            Fall
        }
    }
}
