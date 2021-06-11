using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IPCA.MonoGame
{
    public class Debug
    {
        private Texture2D _pixel;
        private static GraphicsDevice _graphicsDevice;

        public static void SetGraphicsDevice(GraphicsDevice gd)
        {
            _graphicsDevice = gd;
        }
        
        public Debug()
        {
            _pixel = new Texture2D(_graphicsDevice, 1, 1);
            _pixel.SetData(new [] { Color.White });
        }

        public void DrawPixel(SpriteBatch _spriteBatch, Vector2 position, Color color)
        {
            _spriteBatch.Draw(_pixel, position, color);
        }

        public void DrawLine(SpriteBatch _spriteBatch, Vector2 p0, Vector2 p1, Color color)
        {
            if ((int)p0.Y == (int)p1.Y) // Linha Horizontal
            {
                int x = (int) Math.Min(p0.X, p1.X);
                int w = (int) Math.Abs(p1.X - p0.X);
                _spriteBatch.Draw(_pixel, new Rectangle(x, (int) p0.Y, w, 1), color);
            }
            else if ((int) p0.X == (int) p1.X) // Linha Vertical
            {
                int y = (int) Math.Min(p0.Y, p1.Y);
                int h = (int) Math.Abs(p1.Y - p0.Y);
                _spriteBatch.Draw(_pixel, new Rectangle((int)p0.X, y, 1, h), color);
            }
            else // Diagonais
            {
                float m = (p1.Y - p0.Y) / (p1.X - p0.X);
                float b = p0.Y - p0.X * m;
                // p0.Y = p0.X * m + b
                // p1.Y = p1.X * m + b
                ///////
                // b = p0.Y - p0.X * m
                // p1.Y = p1.X * m + p0.Y - p0.X * m <=>
                // p1.Y - p0.Y = p1.X * m - p0.X * m <=>
                // p1.Y - p0.Y = m(p1.X - p0.X) <=>
                // m = (p1.Y - p0.Y) / (p1.X - p0.X)
                ////
                // b = p0.Y - p0.X * (p1.Y - p0.Y) / (p1.X - p0.X)
                if (Math.Abs(p0.X - p1.X) > Math.Abs(p0.Y - p1.Y)) // Diagonal "horizontal"
                {
                    int x0 = (int) Math.Min(p0.X, p1.X);
                    int x1 = (int) Math.Max(p0.X, p1.X);
                    for (int x = x0; x <= x1; x++)
                    {
                        Vector2 pos = new Vector2(x, m * x + b);
                        _spriteBatch.Draw(_pixel, pos, color);
                    }
                }
                else // Diagonal "vertical"
                {
                    int y0 = (int) Math.Min(p0.Y, p1.Y);
                    int y1 = (int) Math.Max(p0.Y, p1.Y);
                    for (int y = y0; y <= y1; y++)
                    {
                        // y = mx + b <=> mx = y - b <=> x = (y-b)/m
                        Vector2 pos = new Vector2( (y-b)/m, y);
                        _spriteBatch.Draw(_pixel, pos, color);
                    }
                }
            }
        }

        public void DrawRectangle(SpriteBatch _spriteBatch, RectangleF rectangle, Color color)
        {
            Vector2 tl = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 bl = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 tr = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 br = new Vector2(rectangle.Right, rectangle.Bottom);
            
            DrawLine(_spriteBatch, tl, tr, color);
            DrawLine(_spriteBatch, bl, br, color);
            DrawLine(_spriteBatch, tl, bl, color);
            DrawLine(_spriteBatch, tr, br, color);
        }
        
        public void DrawCircle(SpriteBatch _spriteBatch, Vector2 center, float  radius, Color color)
        {
            float inc = MathF.PI / (4*radius);
            for (float theta = 0; theta < MathF.PI * 2; theta += inc)
            {
                Vector2 vec = new Vector2(MathF.Cos(theta), MathF.Sin(theta)) * radius;
                _spriteBatch.Draw(_pixel, center + vec, color);
            }
        }
    }
}