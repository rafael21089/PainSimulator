using System;
using System.Linq;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Shared.Optimization;
using IPCA.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dyno
{
    public class Bullet : GameObject, ITempObject
    {
        private Texture2D _texture;
        private Vector2 _directon;
        private float _speed;
        private float _maxDistance = 10f; // tempo de vida
        private Vector2 _origin; // starting Point, to compute current distance
        private Vector2 _anchor;
        private bool _collided = false;
        
        public bool Collided => _collided;
        public bool IsDead() => 
            Collided
            || 
            (_origin - _position).LengthSquared() > _maxDistance * _maxDistance;

        public Vector2 ImpactPos;
        
        public Bullet(
            Texture2D texture, Vector2 startingPos, Vector2 direction,
            World world
            ) :
            base("bullet", startingPos)
        {
            _origin = startingPos;
            _texture = texture;
            _anchor = _texture.Bounds.Size.ToVector2() / 2f;
            // Speed
            _speed = direction.Length();
            // Normalized direction
            _directon = direction;
            _directon.Normalize();
            // Rotation
            _rotation = MathF.Atan2(-_directon.Y, _directon.X);
            _size = _texture.Bounds.Size.ToVector2() / 128f; // FIXME!!!!
            Body = BodyFactory.CreateCircle(
                world, _size.Y / 2f, 1f, 
                _position + (_size.X/2f-_size.Y/2f) * _directon,
                BodyType.Dynamic, this);
            Body.LinearVelocity = _directon * _speed;
            Body.IsBullet = true;
            Body.IgnoreGravity = true;
            Body.IsSensor = true;

            Body.OnCollision = (a, b, contact) =>
            {
                string[] ignore = {"idle", "bullet", "explosion"};
                if (!ignore.Contains(b.GameObject().Name))
                {
                    _collided = true;
                    ImpactPos = _position+(b.GameObject().Position-_position)/2f;
                }
            };
        }
        
        public override void Update(GameTime gameTime)
        {
            _position = Body.Position + (_size.Y / 2f - _size.X / 2f) * _directon;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 scale = Camera.Length2Pixels(_size) / 128f; // TODO: HARDCODED!
            scale.Y = scale.X;  // FIXME! TODO: HACK HACK HACK

            spriteBatch.Draw(_texture, 
                Camera.Position2Pixels(_position), null,
                Color.White, _rotation, 
                _anchor, scale, SpriteEffects.None,
                0);
            
            base.Draw(spriteBatch, gameTime);
        }
    }
}