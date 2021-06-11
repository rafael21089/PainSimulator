using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IPCA.MonoGame
{
    public class CircleCollider : Collider
    {
        protected float _radius;
        public float Radius => _radius;

        public CircleCollider(GameObject go, float radius) : base(go)
        {
            _center = go.Position;
            _radius = radius;
        }

        public CircleCollider(GameObject go, Vector2 center, float radius) : base(go)
        {
            _center = center;
            _radius = radius;
        }

        public override bool CollidesWith(Collider other)
        {
            switch (other)
            {
                case CircleCollider c:
                    return (_center - c._center).LengthSquared() < MathF.Pow(_radius + c._radius, 2);
                case AABBCollider aabb:
                    return aabb.CollidesWith(this);
                case OBBCollider o:
                    return o.CollidesWith(this);
            }

            return false;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float radius = Camera.Length2Pixels(new Vector2(_radius, 0)).X;
            if (_debug)
            {
                _draw.DrawCircle(spriteBatch, 
                    Camera.Position2Pixels(_center), radius, Color.Yellow);
            }
        }
    }
}