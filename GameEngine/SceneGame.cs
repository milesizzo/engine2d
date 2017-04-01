using GameEngine.Content;
using GameEngine.Graphics;
using GameEngine.Scenes;
using GameEngine.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameEngine
{
    public class SceneGame : Game
    {
        private readonly TemplateStore<IScene> scenes = new TemplateStore<IScene>();
        private IScene currentScene;
        private Renderer renderer;
        private double frameRate;
        private readonly Store store;

        public SceneGame()
        {
            this.renderer = new Renderer(this);

            this.Content.RootDirectory = "Content";
            this.store = new Store(this.Content);
        }
        
        protected IScene CurrentScene
        {
            get { return this.currentScene; }
            set
            {
                this.currentScene = value;
                this.currentScene.SetUp();
            }
        }

        protected TemplateStore<IScene> Scenes
        {
            get { return this.scenes; }
        }

        protected void SetCurrentScene(string name)
        {
            this.currentScene = this.Scenes[name];
            this.currentScene.SetUp();
        }

        protected double FPS
        {
            get { return this.frameRate; }
        }

        protected Store Store
        {
            get { return this.store; }
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.currentScene != null)
            {
                if (this.IsActive)
                {
                    if (this.currentScene.SceneEnded)
                    {
                        this.currentScene = null;
                    }
                    else
                    {
                        this.currentScene.Update(gameTime);
                    }
                }
            }
            base.Update(gameTime);
        }

        protected sealed override void Draw(GameTime gameTime)
        {
            if (gameTime.GetElapsedSeconds() > 0)
            {
                this.frameRate = 1 / gameTime.GetElapsedSeconds();
            }

            if (this.currentScene != null)
            {
                this.currentScene.PreDraw(this.renderer);
                try
                {
                    this.currentScene.Draw(this.renderer);
                    this.Draw(this.renderer);
                }
                finally
                {
                    this.currentScene.PostDraw(this.renderer);
                }
            }
            base.Draw(gameTime);
        }

        protected virtual void Draw(Renderer renderer)
        {
            // do nothing
        }
    }
}
