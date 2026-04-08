// LiteECS/ISystem.cs
using Microsoft.Xna.Framework;

namespace LiteECS
{
    public interface ISystem
    {
        bool Enabled { get; set; }
        void Initialize(World world);
        void Update(GameTime gameTime);
    }

    public interface IDrawSystem : ISystem
    {
        void Draw(GameTime gameTime);
    }
}