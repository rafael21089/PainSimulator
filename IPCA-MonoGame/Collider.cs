using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace IPCA.MonoGame
{
    // Representa qualquer tipo de Collider (seja circle, AABB ou OBB)
    public abstract class Collider
    {
        protected GameObject _gameObject;
        protected bool _debug;
        protected Debug _draw;
        protected Vector2 _center; // <<===

        private static List<Collider> _colliders = new List<Collider>();
        public static List<Collider> Colliders => _colliders;  // Temporary stuff???

        public virtual Vector2 Location
        {
            set => _center = value;
            get => _center;
        }

        public Collider(GameObject go, bool temporary = false)
        {
            _center = go.Position;
            _gameObject = go;
            _draw = new Debug();
            if (!temporary) _colliders.Add(this);
        }
        
        public void EnableDebug(bool debug)
        {
            _debug = debug;   
        }
        
        public abstract bool CollidesWith(Collider other);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        
        // Intersects ray r = p + td, |d| = 1, with circle c 
        protected static bool IntersectSegmentCircle(Vector2 pt1, Vector2 pt2, CircleCollider circle)
        {
            float dist = (pt2 - pt1).Length();
            Vector2 p = pt1;
            Vector2 d = (pt2 - pt1) / dist;
            
            Vector2 m = p - circle.Location;
            float b = Vector2.Dot(m, d);
            float c = Vector2.Dot(m, m) - circle.Radius * circle.Radius;
            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0)
            if (c > 0.0f && b > 0.0f) return false;
            float discr = b*b - c; 
            // A negative discriminant corresponds to ray missing sphere
            if (discr < 0.0f) return false;
            
            // Ray now found to intersect sphere, compute smallest t value of intersection
            float t = - b - MathF.Sqrt(discr);
            // If t is negative, ray started inside sphere so clamp t to zero
            if (t < 0.0f) t = 0.0f;
            return t <= dist;
        }
    }
}