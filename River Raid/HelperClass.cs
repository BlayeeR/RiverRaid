using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River_Raid
{
    public static class HelperClass
    {
        private static Texture2D texture;

        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, texture, point1, distance, angle, color, thickness);
        }

        public static int CalculateDanger(Vector2 enemyPosition, List<Bullet> playerBullets, int leftWallPosition, int rightWallPosition)
        {
            int danger = 0, bulletsInRange =0;

            foreach (Bullet bullet in playerBullets)
            {
                if (Math.Sqrt(Math.Pow(bullet.Position.X - enemyPosition.X, 2) + Math.Pow(bullet.Position.Y - enemyPosition.Y, 2)) < 100 && bullet.Position.Y > enemyPosition.Y)
                {
                    bulletsInRange++;
                    int distance = (int)Math.Sqrt(Math.Pow(bullet.Position.X - enemyPosition.X, 2) + Math.Pow(bullet.Position.Y - enemyPosition.Y, 2));
                    danger += 100 - distance;
                }
            }
            danger *=bulletsInRange;
            if (enemyPosition.X - leftWallPosition < 100)
                danger += (int)(100-enemyPosition.X - leftWallPosition)*danger;
            else if (rightWallPosition - enemyPosition.X < 100)
                danger += (int)(100- (rightWallPosition -enemyPosition.X))*danger;
            return danger;
        }
        public static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (texture == null)
            {
                texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                texture.SetData(new[] { Color.White });
            }

            return texture;
        }
    }
}
