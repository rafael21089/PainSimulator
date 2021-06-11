using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dyno
{
    public interface ITempObject
    {
        bool IsDead();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}