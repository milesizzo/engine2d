using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.GameObjects;
using GameEngine.Graphics;
using GameEngine.Content;
using GameEngine.UI;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Scenes
{
    // a game asset scene that also includes a game context
    public abstract class GameScene<T> : GameAssetScene where T : IGameContext
    {
        private T context;
        private readonly Camera camera;

        protected GameScene(string name, GraphicsDevice graphics, ContentManager content) : this(name, graphics, new Store(content))
        {
        }

        protected GameScene(string name, GraphicsDevice graphics, Store store) : base(name, graphics, store)
        {
            this.camera = new Camera(graphics);
        }

        public Camera Camera { get { return this.camera; } }

        protected T Context { get { return this.context; } }

        public override void SetUp()
        {
            base.SetUp();
            this.context = this.CreateContext();
        }

        protected abstract T CreateContext();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.context.Update(gameTime);
        }

        public override void PreDraw(Renderer renderer)
        {
            base.PreDraw(renderer);
            renderer.World.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: camera.GetViewMatrix());
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            this.context.Draw(renderer);
        }

        public override void PostDraw(Renderer renderer)
        {
            renderer.World.End();
            base.PostDraw(renderer);
        }
    }
}
