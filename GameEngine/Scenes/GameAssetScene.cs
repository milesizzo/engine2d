using GameEngine.Content;
using GameEngine.Graphics;
using GameEngine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Scenes
{
    // any scene that requires game assets
    public abstract class GameAssetScene : IScene
    {
        private readonly GraphicsDevice graphics;
        private readonly string name;
        private bool sceneEnded;
        private readonly UISystem ui;
        public event SceneEndedHandler SceneEnd;

        protected GameAssetScene(string name, GraphicsDevice graphics)
        {
            this.name = name;
            this.sceneEnded = false;
            this.ui = new UISystem(graphics);
            this.graphics = graphics;
        }

        public GraphicsDevice Graphics { get { return this.graphics; } }

        public string Name { get { return this.name; } }

        public bool SceneEnded
        {
            get { return this.sceneEnded; }
            protected set
            {
                this.sceneEnded = value;
                if (value) this.SceneEnd?.Invoke(this);
            }
        }

        public UISystem UI { get { return this.ui; } }

        public virtual void SetUp()
        {
            this.sceneEnded = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            this.ui.Update(gameTime);
        }

        public virtual void PreDraw(Renderer renderer)
        {
            renderer.Screen.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
            this.ui.Draw(renderer);
        }

        public virtual void PostDraw(Renderer renderer)
        {
            renderer.Screen.End();
        }
    }
}
