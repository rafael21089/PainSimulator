using System;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace IPCA.MonoGame
{
    public static class ExtensionMethods
    {
        public static Vector2 Normal(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }
        
        // Vector2 v = new Vector2(x, y);
        // x = v.Coord(0) ==> v[0]
        // y = v.Coord(1) ==> v[1]
        public static float Coord(this Vector2 v, int i)
        {
            if (i == 0) return v.X;
            if (i == 1) return v.Y;
            throw new ArgumentOutOfRangeException("i", "Vector2.Coord");
        }

        public static GameObject GameObject(this Body b)
        {
            return b.UserData as GameObject;
        }

        public static GameObject GameObject(this Fixture f)
        {
            return f.Body.UserData as GameObject;
        }

        /// <summary>
        /// Rotates vector around origin.
        /// </summary>
        /// <param name="pt">The point to be rotated</param>
        /// <param name="angle">The angle to apply (radians)</param>
        /// <returns>A new vector after rotating around origin</returns>
        public static Vector2 Rotate(this Vector2 pt, float angle)
        {
            float c = MathF.Cos(-angle);
            float s = MathF.Sin(-angle);
            return new Vector2(pt.X * c - pt.Y * s, pt.X * s + pt.Y * c);
        }
    }
}