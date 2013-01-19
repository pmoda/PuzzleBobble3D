using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBobbleA2
{
    static class Stats
    {
        public const int NUM_COLORS = 7;

        public static int[] bubbles = new int[NUM_COLORS];
        public static int bubbleCount;
        public static int ballsFired;
        public static int score;

        public static void reset()
        {
            bubbles = new int[NUM_COLORS];
            bubbleCount = 0;
            ballsFired = 0;
            score = 0;
        }

        public static void recordBubble(Bubble bubble)
        {
            bubbleCount++;
            //ballsFired++;
            bubbles[(int)bubble.color]++;
        }
        public static void removeBubble(Bubble bubble)
        {
            bubbleCount--;
            if (bubbles[(int)bubble.color] != 0)
            bubbles[(int)bubble.color]--;
            score += 10;
        }
        public static void cascadeBubbles(List<Bubble> bubblesCollapsed)
        {
            bubbleCount -= bubblesCollapsed.Count;
            foreach (Bubble b in bubblesCollapsed) 
            {
                if (bubbles[(int)b.color] != 0)
                bubbles[(int)b.color]--;
            }
            score += (int)Math.Pow(2.0 , bubblesCollapsed.Count) * 10;
        }
        public static float getColorPercentage(Bubble.Colors color)
        {
            return bubbles[(int)color] / bubbleCount;
        }
        public static int computeScore()
        {
            int currentScore = score;
            return currentScore;
        }


    }
}
