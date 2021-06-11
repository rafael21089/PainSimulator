using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IPCA.MonoGame
{
    public class OBBCollider : Collider
    {
        // Center -> disponivel na classe pai (_center)
        // Direction 
        private Vector2 _direction;
        // Extends (size/2)
        private Vector2 _extends;

        public Vector2 Extends
        {
            get => _extends;
            set => _extends = value;
        }
        
        public OBBCollider(GameObject go, bool temporary = false) : base(go, temporary)
        {
            _extends = go.Size / 2f;
            _direction = Vector2.UnitX;
        }

        public override bool CollidesWith(Collider other)
        {
            switch (other)
            {
                case OBBCollider obb:
                    return OverlapOBB(obb);
                case CircleCollider c:
                    return OverlapCircle(c);
                case AABBCollider aabb:
                    return CollidesWith(aabb.ToOBB());
            }

            return false;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_debug)
            {
                Vector2 pt1 = _center + _direction * _extends.X + _direction.Normal() * _extends.Y;
                Vector2 pt2 = _center + _direction * _extends.X - _direction.Normal() * _extends.Y;
                Vector2 pt3 = _center - _direction * _extends.X + _direction.Normal() * _extends.Y;
                Vector2 pt4 = _center - _direction * _extends.X - _direction.Normal() * _extends.Y;

                pt1 = Camera.Position2Pixels(pt1);
                pt2 = Camera.Position2Pixels(pt2);
                pt3 = Camera.Position2Pixels(pt3);
                pt4 = Camera.Position2Pixels(pt4);
                
                _draw.DrawLine(spriteBatch, pt1, pt2, Color.Red);
                _draw.DrawLine(spriteBatch, pt2, pt4, Color.Green);
                _draw.DrawLine(spriteBatch, pt3, pt4, Color.Blue);
                _draw.DrawLine(spriteBatch, pt3, pt1, Color.Black);
            }
        }

        private bool OverlapOBB(OBBCollider b)
        {
            OBBCollider a = this;
            const float EPSILON = 0.0001f;
            
            Vector2[] a_u = {a._direction, a._direction.Normal()};
            Vector2[] b_u = {b._direction, b._direction.Normal()};

            float ra, rb;
            float[,] R = new float[2, 2], AbsR = new float[2, 2];

            // Compute rotation matrix expressing b in a’s coordinate frame
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                    R[i, j] = Vector2.Dot(a_u[i], b_u[j]);
            }

            // Compute translation vector t
            Vector2 t = b._center - a._center;
            // Bring translation into a’s coordinate frame
            t = new Vector2(Vector2.Dot(t, a_u[0]), Vector2.Dot(t, a_u[1]));
            
            // Compute common subexpressions. Add in an epsilon term to
            // counteract arithmetic errors when two edges are parallel and
            // their cross product is (near) null (see text for details)
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                    AbsR[i, j] = MathF.Abs(R[i, j]) + EPSILON;
            }
            
            // Test axes L = A0, L = A1, L = A2
            for (int i = 0; i < 2; i++) {
                ra = a._extends.Coord(i);
                rb = b._extends.Coord(0) * AbsR[i, 0] + b._extends.Coord(1) * AbsR[i, 1];
                if (MathF.Abs(t.Coord(i)) > ra + rb) return false;
            }

            // Test axes L = B0, L = B1, L = B2
            for (int i = 0; i < 2; i++) {
                ra = a._extends.Coord(0) * AbsR[0,i] + a._extends.Coord(1) * AbsR[1,i];
                rb = b._extends.Coord(i);
                if (MathF.Abs(t.Coord(0) * R[0,i] + t.Coord(1) * R[1,i] ) > ra + rb) return false;
            }

            return true;
        }
        
        private bool OverlapCircle(CircleCollider c)
        {
            if ((_center - c.Location).LengthSquared() < c.Radius * c.Radius)
                return true;
          
            Vector2 pt1 = _center + _direction * _extends.X + _direction.Normal() * _extends.Y;
            Vector2 pt2 = _center + _direction * _extends.X - _direction.Normal() * _extends.Y;
            Vector2 pt3 = _center - _direction * _extends.X + _direction.Normal() * _extends.Y;
            Vector2 pt4 = _center - _direction * _extends.X - _direction.Normal() * _extends.Y;

            return IntersectSegmentCircle(pt1, pt2, c)
                   || IntersectSegmentCircle(pt2, pt4, c)  // <==
                   || IntersectSegmentCircle(pt3, pt4, c)
                   || IntersectSegmentCircle(pt3, pt1, c);  // <==
        }
    }
}