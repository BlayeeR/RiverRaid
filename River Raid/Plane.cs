using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River_Raid
{
    public class Plane
    {
        private Texture2D texture;
        private Rectangle hHitbox, vHitbox, rectangle;
        private Vector2 position;
        public Vector2 Center { get { return new Vector2(size.X / 2 + position.X, size.Y / 2 + position.Y); } }
        private Point size = new Point(60);
        private bool isPlayer = false;
        private TimeSpan lastBulletTime = new TimeSpan(0);
        private List<Bullet> bullets = new List<Bullet>();
        private int avoidanceSafe=100, avoidanceDangerous=50, avoidanceVeryDangerous=0, previousMovement=1, previousDangerLevel, attackSearch, attackNearby, attackShoot;
        private Game game;
        public Plane(Game _game, Vector2 _position, bool _isPlayer)
        {
            position = _position;
            isPlayer = _isPlayer;
            game = _game;
            if(isPlayer)
                texture = game.Content.Load<Texture2D>("plane");
            else
                texture = game.Content.Load<Texture2D>("enemyPlane");
            rectangle = new Rectangle(position.ToPoint(), size);
            if (isPlayer)
            {
                
                vHitbox = new Rectangle((int)Math.Round(size.X * 0.433 + position.X), (int)position.Y, (int)(size.X * 0.134), (int)size.Y);
                hHitbox = new Rectangle((int)position.X, (int)(size.Y * 0.308 + position.Y), size.X, (int)Math.Round(size.Y * 0.192));
            }
            else
            {
                vHitbox = new Rectangle((int)Math.Round(size.X * 0.433 + position.X), (int)position.Y, (int)(size.X * 0.134), size.Y);
                hHitbox = new Rectangle((int)position.X, (int)(size.Y * 0.5 + position.Y), size.X, (int)Math.Round(size.Y * 0.192));
            }
        }

        public bool Intersects(Rectangle obstacle)
        {
            return obstacle.Intersects(hHitbox) || obstacle.Intersects(vHitbox);
        }


        public bool IsPlayer { get => isPlayer; }
        public Texture2D Texture { get => texture; }

        public Vector2 Position { get => position; set {
                position = value;
                if (isPlayer)
                {
                    hHitbox.Location = new Point((int)position.X, (int)Math.Round(size.Y * 0.308 + position.Y));
                    vHitbox.Location = new Point((int)Math.Round(size.X * 0.433 + position.X), (int)position.Y);
                }
                else
                {
                    hHitbox.Location = new Point((int)position.X, (int)Math.Round(size.Y * 0.500 + position.Y));
                    vHitbox.Location = new Point((int)Math.Round(size.X * 0.433 + position.X), (int)position.Y);
                }
                rectangle.Location = position.ToPoint();
            }
        }
        public Point Size { get => size;}
        public float VerticalCenter
        {
            get => position.X -2 + size.X / 2;
        }
        public List<Bullet> Bullets { get => bullets; set => bullets = value; }
        public void AddBullet()
        {
            Bullets.Add(new Bullet(this));
        }
        public void MoveLeft(float distance)
        {
            Position = new Vector2(Position.X - distance, Position.Y);
            previousMovement =-1;
        }
        public void MoveRight(float distance)
        {
            Position = new Vector2(Position.X + distance, Position.Y);
            previousMovement =1;
        }
        public void MoveForward(float distance)
        {
            Position = new Vector2(Position.X, isPlayer?Position.Y-distance: Position.Y + distance);
        }
        public void MoveBackward(float distance)
        {
            Position = new Vector2(Position.X, isPlayer ? Position.Y + distance : Position.Y - distance);
        }
        public TimeSpan LastBulletTime { get => lastBulletTime; set => lastBulletTime = value; }
        public Rectangle Rectangle { get => rectangle; set => rectangle = value; }
        public Rectangle HorizontalHitbox { get => hHitbox; }
        public Rectangle VerticalHitbox { get => vHitbox; }
        public int PreviousMovement { get => previousMovement;}
        public int AvoidanceSafe { get => avoidanceSafe; set => avoidanceSafe = value; }
        public int AvoidanceDangerous { get => avoidanceDangerous; set => avoidanceDangerous = value; }
        public int AvoidanceVeryDangerous { get => avoidanceVeryDangerous; set => avoidanceVeryDangerous = value; }
        public int AttackSearch { get => attackSearch; set => attackSearch = value; }
        public int AttackNearby { get => attackNearby; set => attackNearby = value; }
        public int AttackShoot { get => attackShoot; set => attackShoot = value; }
        public int PreviousDangerLevel { get => previousDangerLevel; set => previousDangerLevel = value; }
    }
}
