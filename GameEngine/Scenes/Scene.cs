using Microsoft.Xna.Framework;
using GameEngine.Content;
using GameEngine.Graphics;
using GameEngine.Templates;

namespace GameEngine.Scenes
{
    public delegate void SceneEndedHandler(IScene scene);

    public interface IScene : ITemplate
    {
        event SceneEndedHandler SceneEnd;

        bool SceneEnded { get; }

        void SetUp();

        void Update(GameTime gameTime);

        void PreDraw(Renderer renderer);

        void Draw(Renderer renderer, GameTime gameTime);

        void PostDraw(Renderer renderer);
    }
}
