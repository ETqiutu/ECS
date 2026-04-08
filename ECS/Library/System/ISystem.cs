using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS.Library.Entity;

namespace ECS.Library.System
{
    public abstract class ISystem
    {
        public abstract void Update(GameTime gameTime, List<IEntity> entities);
        public virtual void Draw(SpriteBatch spriteBatch, List<IEntity> entities) { }
    }
}