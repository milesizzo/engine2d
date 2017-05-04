using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;

namespace GameEngine.Templates
{
    public class AnimatedSpriteSheetTemplate : SpriteSheetTemplate
    {
        private readonly int numFrames;

        public AnimatedSpriteSheetTemplate(string name, Texture2D texture, int spriteWidth, int spriteHeight, int border = -1, int numFrames = -1) : base(name, texture, spriteWidth, spriteHeight, border)
        {
            this.numFrames = numFrames == -1 ? this.Sprites.Count : numFrames;
        }

        public override int NumberOfFrames { get { return this.numFrames; } }
    }

    public class NamedAnimatedSpriteSheetTemplate : SpriteSheetTemplate
    {
        private class NASSWrapperTemplate : ISpriteTemplate
        {
            private readonly NamedAnimatedSpriteSheetTemplate owner;
            private readonly int[] frames;
            private readonly string key;
            private int fpsOverride;

            public NASSWrapperTemplate(string key, int[] frames, int fps, NamedAnimatedSpriteSheetTemplate owner)
            {
                this.key = key;
                this.owner = owner;
                this.frames = frames;
                //this.Shape = this.owner.Shape;
                this.fpsOverride = fps;
            }

            public Shape Shape
            {
                get { throw new InvalidOperationException(); }
                set { throw new InvalidOperationException(); }
            }

            public string Name { get { return $"{this.owner.Name}.{this.key}"; } }

            public int FPS
            {
                get { return this.fpsOverride == 0 ? this.owner.FPS : this.fpsOverride; }
                set { this.fpsOverride = value; }
            }

            public int NumberOfFrames { get { return this.frames.Length; } }

            public IEnumerable<int> Frames { get { return this.frames; } }

            public Texture2D Texture
            {
                get { return this.owner.Texture; }
                set { throw new NotImplementedException(); }
            }

            public int Width { get { return this.owner.SpriteWidth; } }

            public int Height { get { return this.owner.SpriteHeight; } }

            public Vector2 Origin
            {
                get { return this.owner.Origin; }
                set { throw new NotImplementedException(); }
            }

            public void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
            {
                this.owner.DrawSprite(sb, this.frames[frame], position, colour, rotation, scale, effects, depth);
            }
        }

        private readonly Dictionary<string, NASSWrapperTemplate> animations = new Dictionary<string, NASSWrapperTemplate>();
        private readonly int numFrames;
        private readonly int spriteWidth;
        private readonly int spriteHeight;

        public NamedAnimatedSpriteSheetTemplate(string name, Texture2D texture, int spriteWidth, int spriteHeight, int border = -1, int numFrames = -1) : base(name, texture, spriteWidth, spriteHeight, border)
        {
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.numFrames = numFrames == -1 ? (this.Width / this.SpriteWidth) * (this.Height / this.SpriteHeight) : numFrames;
        }

        public override int NumberOfFrames { get { return this.numFrames; } }

        public int SpriteWidth { get { return this.spriteWidth; } }

        public int SpriteHeight { get { return this.spriteHeight; } }

        public void AddAnimation(string key, int fps, params int[] frames)
        {
            this.animations[key] = new NASSWrapperTemplate(key, frames, fps, this);
        }

        public void AddAnimation(string key, int fps, IEnumerable<int> frames)
        {
            this.animations[key] = new NASSWrapperTemplate(key, frames.ToArray(), fps, this);
        }

        public void AddAnimation(string key, IEnumerable<int> frames)
        {
            this.animations[key] = new NASSWrapperTemplate(key, frames.ToArray(), 0, this);
        }

        public ISpriteTemplate GetAnimation(string key)
        {
            return this.animations[key];
        }

        public IEnumerable<int> Frames(string key)
        {
            return this.animations[key].Frames;
        }

        public IEnumerable<string> Keys { get { return this.animations.Keys; } }
    }
}
