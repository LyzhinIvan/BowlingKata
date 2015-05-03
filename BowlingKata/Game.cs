using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NUnit.Framework;

namespace BowlingKata
{
    public class Game
    {
        public class Frame
        {
            public int? First { get; private set; }
            public int? Second { get; private set; }

            public void Roll(int pins)
            {
                if (First == null) First = pins;
                else if (Second == null && First!=10) Second = pins;
            }

            public bool IsSpare()
            {
                if (First == null || Second == null) return false;
                if (First != 10 && First + Second == 10) return true;
                return false;
            }

            public bool IsStrike()
            {
                if (First != null && First == 10) return true;
                return false;
            }

            public bool EndFrame()
            {
                if (IsStrike() || Second != null) return true;
                return false;
            }
        }
        
        private int score;
        private List<Frame> frames = new List<Frame>();

        public void Roll(int pins)
        {
            score += pins;
            if (frames.Count==0 || frames.Last().EndFrame())
            {
                if (frames.Count>0 && frames.Last().IsSpare())
                    score += pins;
                Frame frame = new Frame();
                frame.Roll(pins);
                frames.Add(frame);
            }
            else frames.Last().Roll(pins);
        }

        public int Score()
        {
            return score;
        }
    }

    [TestFixture]
    public class Game_should
    {
        [Test]
        public void ScoreZeroWhenNoRolls()
        {
            Game game = new Game();
            int score = game.Score();
            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        [TestCase(new []{2}, Result=2)]
        [TestCase(new []{2,5,10}, Result=17)]
        public int ScoreWhenNoStrikesAndNoSpares(params int[] rolls)
        {
            Game game = new Game();
            foreach (var pins in rolls)
                game.Roll(pins);
            return game.Score();
        }

        [Test]
        [TestCase(new []{1, 9, 3}, Result = 16)]
        [TestCase(new []{2,8,3,7,2}, Result = 27)]
        public int ScoreWithSpares(params int[] rolls)
        {
            Game game = new Game();
            foreach (var pins in rolls)
                game.Roll(pins);
            return game.Score();
        }
    }
}
