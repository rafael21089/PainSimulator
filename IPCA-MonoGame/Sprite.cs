using System;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IPCA.MonoGame
{
    // Game Object child
    // represents a game object that has a texture
    public class Sprite : GameObject
    {
        protected enum Direction
        {
            Left,
            Right,
        }
        protected Direction _direction = Direction.Right;

        // TODO: we should not duplicate textures on each instance
        protected Texture2D _texture;
        public Sprite(string name, Texture2D texture, Vector2 position,
            bool offset = false) : base(name, position)
        {
            _texture = texture;
            _size = _texture.Bounds.Size.ToVector2() / 128f;  // TODO: HARDCODED!
            if (offset)
                _position = position + new Vector2(_size.X , _size.Y) / 2f; // Anchor in the middle
        }

       
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 pos = Camera.Position2Pixels(_position);
            Vector2 anchor = _texture.Bounds.Size.ToVector2() / 2f;
           
            Vector2 scale = Camera.Length2Pixels(_size) / 128f; // TODO: HARDCODED!
            scale.Y = scale.X;  // FIXME! TODO: HACK HACK HACK
            
            spriteBatch.Draw(_texture, pos, null, Color.White,
                _rotation, anchor, scale, 
                _direction == Direction.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);

            base.Draw(spriteBatch, gameTime);
        }
    }
}