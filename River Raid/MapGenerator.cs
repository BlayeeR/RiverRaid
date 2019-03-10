using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace River_Raid
{
    public class MapGenerator
    {
        int lastRectangleIndex = 0, previous;
        public Rectangle[] leftSide, rightSide;
        byte[] hashBytesLeft, hashBytesRight;
        int margin = 150, speed,scale;    //, current = 0;
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private Point screenSize;
        public Rectangle LastLeftRectangle { get { return leftSide[lastRectangleIndex]; } }
        public Rectangle LastRightRectangle { get { return rightSide[lastRectangleIndex]; } }
        public MapGenerator(Point screenSize, int scale, int speed)
        {
            this.screenSize = screenSize;
            this.scale = scale;
            this.speed = speed;
            hashBytesLeft = new byte[(screenSize.Y / scale)];
            hashBytesRight = new byte[(screenSize.Y / scale)];
            leftSide = new Rectangle[(screenSize.Y / scale)];
            rightSide = new Rectangle[(screenSize.Y / scale)];
            rng.GetBytes(hashBytesLeft);// = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(rng.Next().ToString()));
            rng.GetBytes(hashBytesRight);// = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(rng.Next().ToString()));
            leftSide[0] = new Rectangle(new Point(0,screenSize.Y - scale), new Point(margin * 2, screenSize.Y));
            rightSide[0] = new Rectangle(new Point(screenSize.X - margin * 2, screenSize.Y - scale), new Point(screenSize.X, screenSize.Y));
            for (int i = 1; i < hashBytesLeft.Length; i++)
            {
                switch (hashBytesLeft[i])
                {
                    case byte n when (n < 105):
                        {
                            leftSide[i] = new Rectangle(new Point(0, leftSide[i - 1].Y - scale), (rightSide[i - 1].Left - leftSide[i - 1].Right) > margin ?
                                                                                            new Point(leftSide[i - 1].Right + scale, leftSide[i - 1].Y) :
                                                                                            leftSide[i - 1].Size);

                            break;

                        }
                    case byte n when (n > 150):
                        {
                            leftSide[i] = new Rectangle(new Point(0, leftSide[i - 1].Y - scale), leftSide[i].Size = leftSide[i - 1].Right > margin ?
                                                                                            new Point(leftSide[i - 1].Right - scale, leftSide[i - 1].Y) :
                                                                                            leftSide[i - 1].Size);
                            break;
                        }
                    default:
                        {
                            leftSide[i] = new Rectangle(new Point(0, leftSide[i - 1].Y - scale),
                            leftSide[i].Size = leftSide[i - 1].Size);
                            break;
                        }
                }

                switch (hashBytesRight[i])
                {
                    case byte n when (n < 105):
                        {


                            rightSide[i] = new Rectangle((rightSide[i - 1].Left - leftSide[i - 1].Right) > margin ?
                                                                        new Point(rightSide[i - 1].Left - scale, rightSide[i - 1].Y - scale) :
                                                                        new Point(rightSide[i - 1].X, rightSide[i - 1].Y - scale), new Point(screenSize.X, rightSide[i - 1].Top));
                            break;
                        }
                    case byte n when (n > 150):
                        {
                            rightSide[i] = new Rectangle(rightSide[i - 1].Left < (screenSize.X - margin) ?
                                                                        new Point(rightSide[i - 1].Left + scale, rightSide[i - 1].Y - scale) :
                                                                        new Point(rightSide[i - 1].X, rightSide[i - 1].Y - scale),
                            new Point(screenSize.X, rightSide[i - 1].Top));
                            break;
                        }
                    default:
                        {
                            rightSide[i] = new Rectangle(new Point(rightSide[i - 1].Left, rightSide[i - 1].Y - scale),
                            new Point(screenSize.X, rightSide[i - 1].Top));

                            break;
                        }
                }
            }
        }


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < leftSide.Count(); i++)
            {
                leftSide[i].Offset(new Point(0, speed));
                rightSide[i].Offset(new Point(0, speed));
            }
            lastRectangleIndex = Array.FindIndex(leftSide, x => x.Top == leftSide.Aggregate((i1, i2) => i1.Top > i2.Top ? i1 : i2).Top);
            previous = (lastRectangleIndex == 0 ? leftSide.Length - 1 : lastRectangleIndex - 1);
            if (leftSide[lastRectangleIndex].Top > screenSize.Y)
            {
                rng.GetBytes(hashBytesLeft);
                rng.GetBytes(hashBytesRight);
                switch (hashBytesLeft[lastRectangleIndex])
                {

                    case byte n when (n < 105):
                        {
                            leftSide[lastRectangleIndex].Location = new Point(0, 0);
                            leftSide[lastRectangleIndex].Size = (rightSide[previous].Left - leftSide[previous].Right) > margin ?
                                                                                            new Point(leftSide[previous].Right + scale, leftSide[previous].Y) :
                                                                                            new Point(leftSide[previous].Right, leftSide[previous].Top);

                            break;

                        }
                    case byte n when (n > 150):
                        {
                            leftSide[lastRectangleIndex].Location = new Point(0, 0);
                            leftSide[lastRectangleIndex].Size = leftSide[previous].Right > margin ?
                                                                                            new Point(leftSide[previous].Right - scale, leftSide[previous].Y) :
                                                                                            new Point(leftSide[previous].Right, leftSide[previous].Top);
                            break;
                        }
                    default:
                        {
                            leftSide[lastRectangleIndex].Location = new Point(0, 0);
                            leftSide[lastRectangleIndex].Size = new Point(leftSide[previous].Right, leftSide[previous].Top);
                            break;
                        }
                }

                switch (hashBytesRight[lastRectangleIndex])
                {

                    case byte n when (n < 105):
                        {


                            rightSide[lastRectangleIndex].Location = (rightSide[previous].Left - leftSide[previous].Right) > margin ?
                                                                        new Point(rightSide[previous].Left - scale, 0) :
                                                                        new Point(rightSide[previous].X, 0);
                            rightSide[lastRectangleIndex].Size = new Point(screenSize.X, rightSide[previous].Top);
                            break;
                        }
                    case byte n when (n > 150):
                        {
                            rightSide[lastRectangleIndex].Location = rightSide[previous].Left < (screenSize.X - margin) ?
                                                                        new Point(rightSide[previous].Left + scale, 0) :
                                                                        new Point(rightSide[previous].X, 0);
                            rightSide[lastRectangleIndex].Size = new Point(screenSize.X, rightSide[previous].Top);
                            break;
                        }
                    default:
                        {
                            rightSide[lastRectangleIndex].Location = new Point(rightSide[previous].Left, 0);
                            rightSide[lastRectangleIndex].Size = new Point(screenSize.X, rightSide[previous].Top);

                            break;
                        }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool debugMode)
        {
            for (int i = 0; i < leftSide.Length; i++)
            //foreach(Rectangle rectangle in leftSide)
            {
                if (debugMode)
                {
                    spriteBatch.Draw(HelperClass.GetTexture(spriteBatch), leftSide[i], (i == lastRectangleIndex) ? Color.Yellow : (i == previous) ? Color.Red : Color.Green);
                    spriteBatch.Draw(HelperClass.GetTexture(spriteBatch), rightSide[i], (i == lastRectangleIndex) ? Color.Yellow : (i == previous) ? Color.Red : Color.Green);
                }
                else
                {
                    spriteBatch.Draw(HelperClass.GetTexture(spriteBatch), leftSide[i], Color.Green);
                    spriteBatch.Draw(HelperClass.GetTexture(spriteBatch), rightSide[i], Color.Green);
                }
            }
            
        }

        public bool Intersects(Plane plane)
        {
            for (int i = 0; i < leftSide.Length; i++)
            {
                if (plane.Intersects(leftSide[i]) || plane.Intersects(rightSide[i]))
                    return true;
            }
            return false;
        }

        
    }
}
