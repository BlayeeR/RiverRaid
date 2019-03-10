using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River_Raid
{
    public class Bullet
    {
        private Plane _plane;
        private Texture2D texture;
        private Rectangle rectangle;
        private Vector2 position;
        private Point size = new Point(5, 5);
        private int speed = 5
                                         ;
        public Bullet(Plane plane)
        {
            _plane = plane;
            texture = new Texture2D(plane.Texture.GraphicsDevice, 1, 1);
            texture.SetData( new[] { Color.Yellow } );
            position = new Vector2(plane.VerticalCenter - (size.X/2), plane.Position.Y);
            rectangle = new Rectangle(position.ToPoint(), size);
        }

        public Plane Plane { get => _plane; set => _plane = value; }
        public Texture2D Texture { get => texture; }
        public Rectangle Rectangle { get => rectangle; }
        public Vector2 Position
        {
            get => position; set
            {
                position = value;
                rectangle.Location = position.ToPoint();
            }
        }
        public Point Size { get => size; }
        public int Speed { get => speed; }
    }
}
