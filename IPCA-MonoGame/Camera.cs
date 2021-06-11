using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IPCA.MonoGame
{
    public class Camera
    {
        private static Camera _camera; // Instance from the Camera singleton
        
        private Vector2 _windowSize;
        private Vector2 _worldSize;
        private Vector2 _ratio;
        private Vector2 _target;

        public static Vector2 WorldSize => _camera._worldSize;
        
        public Camera(GraphicsDevice graphics, Vector2 worldSize):
            this(graphics, worldSize.X, worldSize.Y)
        { }
        
        public Camera(GraphicsDevice graphics, float width = 0f, float height = 0f)
        {
            if (_camera != null)
                throw new Exception("Camera constructor called more than once.");

            _camera = this; // Save reference to the unique instance in the singleton variable.
            _target = Vector2.Zero;
            _windowSize = graphics.Viewport.Bounds.Size.ToVector2();
            
                
            if (width == 0 && height == 0)
                // new Camera(_graphics)
                _worldSize = _windowSize;
            else if (width == 0)
                // new Camera(_graphics, height: 7.5f);
                // windowSize.Y == height
                // windowSize.X == ?? width
                _worldSize = new Vector2( _windowSize.X * height/_windowSize.Y, height);
            else if (height == 0)
                // new Camera(_graphics, width: 15f);
                // windowSize.X == width
                // windowSize.Y == ?? height
                _worldSize = new Vector2(width, _windowSize.Y * width / _windowSize.X);
            else
                // new Camera(_graphics, 15f, 7.5f);
                _worldSize = new Vector2(width, height);
            _ratio = _windowSize / _worldSize;
        }

        public static void Zoom(int zoom)
        {
            _camera._zoom(zoom);
        }

        private void _zoom(int zoom)
        {
            if (zoom >= 0)
                _worldSize *= MathF.Pow(0.9f, zoom);
            else
                _worldSize *= MathF.Pow(1.1f, -zoom);
            _ratio = _windowSize / _worldSize;
        }


        public static void LookAt(Vector2 target)
        {
            _camera._target = target;
        }

        public static Rectangle Rectangle2Pixels(Rectangle rect)
        {
            return _camera._rectangle2Pixels(rect);
        }
        
        private Rectangle _rectangle2Pixels(Rectangle rect)
        {
            Vector2 center = _position2Pixels(rect.Location.ToVector2());
            Vector2 dim = _length2Pixels(rect.Size.ToVector2());
            Vector2 location = center - dim/2f; 
            return new Rectangle(location.ToPoint(), dim.ToPoint());
        }
        public static RectangleF Rectangle2Pixels(RectangleF rect)
        {
            return _camera._rectangle2Pixels(rect);
        }

        private RectangleF _rectangle2Pixels(RectangleF rect)
        {
            Vector2 center = _position2Pixels(rect.Center);
            Vector2 dim = _length2Pixels(rect.Size);
            Vector2 location = center - dim/2f; 
            return new RectangleF(location, dim);
        }

        public static Vector2 Position2Pixels(Vector2 pos)
        {
            return _camera._position2Pixels(pos);
        }
        private Vector2 _position2Pixels(Vector2 pos)
        {
            // reposicionamento de acordo com o target
            Vector2 tmpPos = pos - (_target - _worldSize / 2f);
            // virtual 2 pixels
            Vector2 pixels = tmpPos * _ratio;
            // Inverter eixo dos Y
            return new Vector2(pixels.X, _windowSize.Y - pixels.Y);
        }

        public static Vector2 Length2Pixels(Vector2 len)
        {
            return _camera._length2Pixels(len);
        }
        private Vector2 _length2Pixels(Vector2 len)
        {
            return len * _ratio;
        }
    }
}