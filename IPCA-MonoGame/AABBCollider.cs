using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IPCA.MonoGame
{
    public class AABBCollider : Collider
    {
        private RectangleF _collider;

        public override Vector2 Location
        {
            set {
                _center = value;
                _collider.Location = PositionFromCenter();
            }
            get => _center;
        } 
        
        private Vector2 PositionFromCenter()
        {
            return _center - _gameObject.Size * new Vector2(0.5f, 0.5f);
        }
        
        public AABBCollider(GameObject go) : base(go)
        {
            Vector2 location = PositionFromCenter();  // <====
            _collider = new RectangleF(location, go.Size); // <====
        }

        public override bool CollidesWith(Collider other)
        {
            switch (other)
            {
                case AABBCollider aabb:
                    return _collider.Intersects(aabb._collider);
                case CircleCollider c:
                    return OverlapCircle(c);
                case OBBCollider o:
                    return o.CollidesWith(this);
                default:
                    throw new Exception("AABBCollider CollidesWith with unknown type of collider");
            } 
        }

        public OBBCollider ToOBB()
        {
            OBBCollider obb = new OBBCollider(_gameObject, true);
            obb.Location = _collider.Center;
            obb.Extends = _collider.Size / 2f;
            return obb;
        }
      
        
        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_debug)
            {
                _draw.DrawRectangle(spriteBatch, Camera.Rectangle2Pixels(_collider), Color.Yellow);
            }
        }

        private bool OverlapCircle(CircleCollider c)
        {
            if ((_center - c.Location).LengthSquared() < c.Radius * c.Radius)
                return true;
            Vector2 pt1 = new Vector2(_collider.Left, _collider.Top);
            Vector2 pt2 = new Vector2(_collider.Right, _collider.Top);
            Vector2 pt3 = new Vector2(_collider.Right, _collider.Bottom);
            Vector2 pt4 = new Vector2(_collider.Left, _collider.Bottom);
            return IntersectSegmentCircle(pt1, pt2, c)
                   || IntersectSegmentCircle(pt2, pt3, c)
                   || IntersectSegmentCircle(pt3, pt4, c)
                   || IntersectSegmentCircle(pt4, pt1, c);
        }
    }
}