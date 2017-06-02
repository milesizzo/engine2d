using GameEngine.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Scenes
{
    public abstract class UIOnlyScene : GameAssetScene
    {
        protected UIOnlyScene(string name, GraphicsDevice graphics) : base(name, graphics) { }
    }
}
