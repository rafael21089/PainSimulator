using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IPCA.MonoGame
{
    public class Scene
    {
        private List<Sprite> _sprites;

        public Scene(Game game, string name)
        {
            string filename = $"Content/scenes/{name}.dt";
            _sprites = new List<Sprite>();
            using (StreamReader reader = File.OpenText(filename))
            {
                JObject sceneJson = (JObject) JToken.ReadFrom(new JsonTextReader(reader));
                JArray spriteJson = (JArray) sceneJson["composite"]["sImages"];
                foreach (JObject image in spriteJson)
                {
                    float x = (float) (image["x"] ?? 0);
                    float y = (float) (image["y"] ?? 0);
                    string imageName = (string) image["imageName"];
                    string imageFilename = $"assets/orig/images/{imageName}";

                    // Load texture here, and send it to the sprite object
                    Texture2D texture = game.Content.Load<Texture2D>(imageFilename);
                    Sprite sprite = new Sprite(imageFilename, texture, new Vector2(x, y), true);
                    
                    _sprites.Add(sprite);
                    sprite.AddRectangleBody(game.Services.GetService<World>(), 
                        isKinematic: true);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Sprite sprite in _sprites)
            {
                sprite.Draw(spriteBatch, gameTime);
            }
        }
    }
}