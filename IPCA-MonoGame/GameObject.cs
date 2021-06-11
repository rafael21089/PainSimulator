using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IPCA.MonoGame
{
    // Father class for all game-objects, being them static or dynamic
    public class GameObject
    {
        protected float _rotation;
        protected Vector2 _position, _size;
        protected string _name;
        public Vector2 Position => _position;
        public Vector2 Size => _size;
        public string Name => _name;
        public Body Body;
        public bool Debug = true;
        

        public GameObject(string name) : this(name, Vector2.Zero)
        {
        }

        public GameObject(string name, Vector2 position)
        {
            _size = Vector2.One;
            _name = name;
            _position = position;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Body != null && !Body.IsKinematic)
            {
                _position = Body.Position;
                _rotation = -Body.Rotation;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Debug && Body != null)
            {
                Debug debug = new Debug();
                foreach (Fixture f in Body.FixtureList)
                {
                    switch (f.Shape)
                    {
                        case PolygonShape p:
                            for (int i = 0; i < p.Vertices.Count; i++)
                            {
                                Vector2 p1 = p.Vertices[i];
                                Vector2 p2 = p.Vertices[ (i + 1) % p.Vertices.Count ];
                                
                                p1 = p1.Rotate(_rotation) + _position;
                                p2 = p2.Rotate(_rotation) + _position;
                                
                                p1 = Camera.Position2Pixels(p1);
                                p2 = Camera.Position2Pixels(p2);
                                debug.DrawLine(spriteBatch, p1, p2, Color.Green);
                            }
                            break;
                        case CircleShape c:
                            Vector2 center = Camera.Position2Pixels(
                                Body.Position + c.Position);
                            float radius = Camera.Length2Pixels(new Vector2(c.Radius, 0)).X;
                            debug.DrawCircle(spriteBatch, center, radius, Color.Blue);
                            break;
                    }
                }
            }
        }

        public void Translate(float x, float y)
        {
            _position.X += x;
            _position.Y += y;
        }
        
        public void AddRectangleBody(
            World world,
            float width = 0f,
            float height = 0f,
            bool isKinematic = false)
        { 
            Body = BodyFactory.CreateRectangle(world,
                width > 0  ? width : _size.X,
                height > 0 ? height : _size.Y,
                1f, _position);
            
            Body.UserData = this;
            
            Body.BodyType = isKinematic ? BodyType.Kinematic : BodyType.Dynamic;
            Body.Friction = 0.5f;
            Body.Restitution = 0.1f;
            
            Body.FixedRotation = true;
        }
        

    }
}