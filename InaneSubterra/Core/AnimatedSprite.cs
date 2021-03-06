﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InaneSubterra.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InaneSubterra.Core
{
    public class AnimatedSprite
    {
        private Vector2 _position;
        // Property automatically update the position of the current animation to reflect changes to the sprite
        public Vector2 Position 
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                if(CurrentAnimation != null)
                    CurrentAnimation.Position = value;
            }
        }

        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationName { get; private set; }
        public Dictionary<string, Animation> Animations { get; private set; }
        public bool Flipped { get; set; }
        public GameScene thisScene;

        public AnimatedSprite(GameScene newScene)
        {
            thisScene = newScene;
            Animations = new Dictionary<string, Animation>();
        }

        public AnimatedSprite(GameScene newScene, Dictionary<string, Animation> dictionary)
        {
            thisScene = newScene;

            Animations = new Dictionary<string, Animation>();
            AddAnimations(dictionary);
        }


        public void AddAnimations(Dictionary<string, Animation> dictionary)
        {
            foreach (KeyValuePair<string, Animation> entry in dictionary)
            {
                Animations.Add(entry.Key, entry.Value);
            }
        }

        public void AddAnimations(string name, Animation anim)
        {
            Animations.Add(name, anim);
        }


        public void Update(GameTime gameTime)
        {
            if (CurrentAnimation != null)
            {
                CurrentAnimation.Position = Position;
                CurrentAnimation.Update(gameTime);

                if (Flipped != CurrentAnimation.HorizontalFlip)
                {
                    CurrentAnimation.HorizontalFlip = Flipped;
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            if (CurrentAnimation != null)
                CurrentAnimation.Draw(spriteBatch);
        }


        public void Draw(SpriteBatch spriteBatch, Color drawColor)
        {
            
            if (CurrentAnimation != null)
                CurrentAnimation.Draw(spriteBatch, drawColor);
        }


        public void PlayAnimation(string animName)
        {
            if (CurrentAnimationName != animName)
            {
                Animation newAnim;
                Animations.TryGetValue(animName, out newAnim);

                CurrentAnimation = newAnim;
                CurrentAnimation.Reset();
                CurrentAnimationName = animName;
            }
            
        }
    }
}
