using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InaneSubterra.Scenes;
using InaneSubterra.Objects;
using InaneSubterra.Physics;
using Microsoft.Xna.Framework;

namespace InaneSubterra.Core
{
    public class WorldGenerator
    {
        // These constants govern the minimum and maximum values that can be rolled for the /initial/
        // LevelGen variables.  This ensures that there is variety in the starting rules.
        #region LevelGen constants
        private const float MIN_GAP_CHANCE = .2f;
        private const float MAX_GAP_CHANCE = .4f;

        private const float MIN_MIN_GAP_WIDTH = 50f;
        private const float MAX_MIN_GAP_WIDTH = 100f;

        private const float MIN_MAX_GAP_WIDTH = 150f;
        private const float MAX_MAX_GAP_WIDTH = 310f;

        private const int MIN_MIN_PLATFORM_WIDTH = 2;
        private const int MAX_MIN_PLATFORM_WIDTH = 5;

        private const int MIN_MAX_PLATFORM_WIDTH = 6;
        private const int MAX_MAX_PLATFORM_WIDTH = 10;

        private const int MIN_MIN_PLATFORM_HEIGHT = 1;
        private const int MAX_MIN_PLATFORM_HEIGHT = 2;

        private const int MIN_MAX_PLATFORM_HEIGHT = 2;
        private const int MAX_MAX_PLATFORM_HEIGHT = 5;

        private const float MIN_VERTICAL_PLATFORM_CHANCE = 0f;
        private const float MAX_VERTICAL_PLATFORM_CHANCE = .5f;

        #endregion


        // These variables govern the minimum and maximum values that can be rolled for area generation.
        // These variables change when the Sequence increases.
        #region LevelGen variables

        private float gapChance;

        private float minGapWidth;
        private float maxGapWidth;

        private int minPlatformWidth;
        private int maxPlatformWidth;

        private int minPlatformHeight;
        private int maxPlatformHeight;

        private float verticalPlatformChance;
        #endregion


        // Random number generator
        private Random rand;

        // Store the scene, as all objects in the game must
        private GameScene thisScene;

        // Store the last platform to ensure that it is possible to jump to the new one from the last
        private Platform lastPlatform;

        // Store the last gap to retroactively add danger to the gap if need be (this allows prediction of player jumping paths)
        //private Rectangle lastGap;

        public WorldGenerator(GameScene newScene)
        {
            thisScene = newScene;
            rand = new Random();

            // Randomize the rules for platform generation in this world.
            gapChance = (float)(rand.NextDouble() * (MAX_GAP_CHANCE - MIN_GAP_CHANCE)) + MIN_GAP_CHANCE;

            minGapWidth = (float)(rand.NextDouble() * (MAX_MIN_GAP_WIDTH - MIN_MIN_GAP_WIDTH)) + MIN_MIN_GAP_WIDTH;
            maxGapWidth = (float)(rand.NextDouble() * (MAX_MAX_GAP_WIDTH - MIN_MAX_GAP_WIDTH)) + MIN_MAX_GAP_WIDTH;

            minPlatformWidth = rand.Next(MIN_MIN_PLATFORM_WIDTH, MAX_MIN_PLATFORM_WIDTH + 1);
            maxPlatformWidth = rand.Next(MIN_MAX_PLATFORM_WIDTH, MAX_MAX_PLATFORM_WIDTH + 1);

            minPlatformHeight = rand.Next(MIN_MIN_PLATFORM_HEIGHT, MAX_MIN_PLATFORM_HEIGHT + 1);
            maxPlatformHeight = rand.Next(MIN_MAX_PLATFORM_HEIGHT, MAX_MAX_PLATFORM_HEIGHT + 1);

            verticalPlatformChance = (float)(rand.NextDouble() * (MAX_VERTICAL_PLATFORM_CHANCE - MIN_VERTICAL_PLATFORM_CHANCE)) + MIN_VERTICAL_PLATFORM_CHANCE;
        }


        public void GenerateArea(ref float levelLength)
        {
            // First, roll to see if the game will generate a gap or not.
            if (rand.NextDouble() < gapChance)
            {
                GenerateGap(ref levelLength);
            }

            // Whether it generates a gap or not, generate a platform.
            GeneratePlatform(ref levelLength);

            // Depending on the sequence, do something with the platform that was just generated.
            // This will actually be handled in the Platform class's constructor.
            // Example: If the traversed distance is great enough, put a sequence orb on the platform that 
            // moves the game to the next sequence.
        }

        public void GenerateGap(ref float levelLength)
        {
            // Because the gap being generated is nothing, simply increase the levelLength by a random amount.
            float length = (float)(rand.NextDouble() * (maxGapWidth - minGapWidth)) + minGapWidth;
            levelLength += length;
            thisScene.sequenceLength += length;

            // If the current sequence is beyond a certain amount, roll to generate a hazard
        }

        public void GeneratePlatform(ref float levelLength)
        {
            float lastY;
            if (lastPlatform == null)
                lastY = 500;
            else
                lastY = lastPlatform.Y;

            // First, determine the X and Y start points of the new platform.
            float newX = levelLength + (thisScene.ScreenArea.X + thisScene.ScreenArea.Width - levelLength);
            float newY;

            float maxJumpHeight = (int)(Math.Max(0 + thisScene.player.Hitbox.Height, MaxJumpHeight(newX, lastY)));
            if(maxJumpHeight > thisScene.ScreenArea.Height - thisScene.BlockTexture.Height)
                maxJumpHeight = thisScene.ScreenArea.Height - thisScene.BlockTexture.Height;
            //if (lastY - MaxJumpHeight(newX) >= thisScene.ScreenArea.Height - thisScene.BlockTexture.Height)
                //newY = rand.Next(0 + thisScene.player.Hitbox.Height, thisScene.ScreenArea.Height - thisScene.BlockTexture.Height);
            //else
            newY = rand.Next((int)maxJumpHeight, thisScene.ScreenArea.Height - thisScene.BlockTexture.Height);


            // Possibly roll to see if a preset platform type gets created.  If not, then proceed to the next segment.


            // Roll the width and height
            int newWidth = rand.Next(minPlatformWidth, maxPlatformWidth + 1);
            int newHeight = rand.Next(minPlatformHeight, maxPlatformHeight + 1);

            // Roll to see if the platform is going to be a vertical or horizontal platform.
            if (rand.NextDouble() >= verticalPlatformChance)
            {
                // Horizontal platform: Generate normally.
                lastPlatform = new Platform(thisScene, new Vector2(newX, newY), newWidth, newHeight);
            }
            else
            {
                // Vertical platform: Swap height and width in the constructor.
                lastPlatform = new Platform(thisScene, new Vector2(newX, newY), newHeight, newWidth);
            }

            // Increase the level length by the total length of the new platform.
            levelLength += lastPlatform.Width;
            thisScene.sequenceLength += lastPlatform.Width;
        }


        // This is run every time the sequence increases, in order to make the platforming more difficult over time
        public void SequenceUp()
        {
            // Increase the gap chance by 10% if it's not yet 100%
            if (gapChance < 1)
                gapChance += .05f;

            // Decrease the maximum width of platforms if the current sequence is odd (decreases every 2 sequences)
            if (thisScene.CurrentSequence % 2 > 0 && maxPlatformWidth > 1)
                maxPlatformWidth -= 1;

            // If the minimum width of platforms is greater than 1, then decrease it on even sequences after 2
            if (thisScene.CurrentSequence % 2 == 0 && thisScene.CurrentSequence > 2 && minPlatformWidth > 1)
                minPlatformWidth -= 1;

            // Increase the maximum gap width by a random amount
            if(maxGapWidth < MAX_MAX_GAP_WIDTH)
                maxGapWidth += rand.Next(5, 10) + thisScene.CurrentSequence * 2;

            // Increase the minimum gap width by a random amount
            minGapWidth += rand.Next(0, 5) + thisScene.CurrentSequence;
        }


        // Returns the height the player can reach, given the new platform's X and the last platform's Y
        public float MaxJumpHeight(float newX, float lastY)
        {
            // Store the furthest right value of the last platform
            float platformEnd;
            if (lastPlatform != null)
                platformEnd = (lastPlatform.X + lastPlatform.Width);
            else
                platformEnd = 100;
            Console.WriteLine("Calculating maximum jump height...");
            // Get the distance between the platforms
            float gapLength = newX - platformEnd;
            Console.WriteLine("Gap Width: " + gapLength);
            // Find how long it would take in seconds for the player to jump between the gaps
            float jumpTime = gapLength / thisScene.player.horizontalSpeed;
            Console.WriteLine("Jump Time: " + jumpTime);

            // Find the maximum height the player can reach in that time given their jump velocity and gravity
            
            float finalHeight;
            if (jumpTime <= 1)
                finalHeight = lastY + (thisScene.player.jumpVelocity / 3f);
            else
                finalHeight = lastY + ((thisScene.player.jumpVelocity / 3f) + ((float)Gravity.gravityAcceleration * (jumpTime - 1)));

            Console.WriteLine("Final maximum height: " + finalHeight);
            return finalHeight;
        }

        // Returns the maximum width of a gap the player can clear, given the last platform's Y
        // Only run if the player is above the platform.
        public float MaxGapWidth(float newX, float lastY)
        {
            // Take player's last Y position
            // Add the jump velocity and multiply the result by (ScreenHeight - blockHeight) - Y, obviously factoring in Gravity time
            return 0;
        }
    }
}
