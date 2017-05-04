using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using GameEngine.Graphics;
using System;

namespace GameEngine.Templates
{
    public interface ISpriteTemplate : ITemplate
    {
        Texture2D Texture { get; set; }

        int NumberOfFrames { get; }

        int Width { get; }

        int Height { get; }

        int FPS { get; set; }

        Vector2 Origin { get; set; }

        void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth);

        Shape Shape { get; set; }
    }

    public abstract class SpriteTemplate : ISpriteTemplate
    {
        private Vector2 origin;
        private Shape shape;
        private readonly string name;

        protected SpriteTemplate(string name)
        {
            this.name = name;
            this.FPS = 5;
        }

        public string Name { get { return this.name; } }

        public abstract Texture2D Texture { get; set; }

        public abstract int NumberOfFrames { get; }

        public virtual int Width { get { return this.Texture.Width; } }

        public virtual int Height { get { return this.Texture.Height; } }

        public virtual int FPS { get; set; }

        public Shape DefaultShape
        {
            get
            {
                return new PolygonShape(new Vertices(new[]
                {
                    -this.Origin,
                    new Vector2(this.Width - this.Origin.X, -this.Origin.Y),
                    new Vector2(this.Width - this.Origin.X, this.Height - this.Origin.Y),
                    new Vector2(-this.Origin.X, this.Height - this.Origin.Y)
                }), 1f);
            }
        }

        public Shape Shape
        {
            get { return this.shape ?? this.DefaultShape; }
            set { this.shape = value; }
        }

        public virtual Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        public abstract void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth);
    }

    public static class SpriteTemplateExtensions
    {
        public static void DrawSprite(this ISpriteTemplate sprite, SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            sprite.DrawSprite(sb, frame, position, colour, rotation, scale, effects, 0f);
        }

        public static void DrawSprite(this ISpriteTemplate sprite, SpriteBatch sb, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            sprite.DrawSprite(sb, 0, position, colour, rotation, scale, effects, 0f);
        }

        public static void DrawSprite(this ISpriteTemplate sprite, SpriteBatch sb, Vector2 position, Color colour, float rotation, Vector2 scale)
        {
            sprite.DrawSprite(sb, 0, position, colour, rotation, scale, SpriteEffects.None, 0f);
        }

        public static void DrawSprite(this ISpriteTemplate sprite, SpriteBatch sb, Vector2 position, float depth)
        {
            sprite.DrawSprite(sb, 0, position, Color.White, 0f, Vector2.One, SpriteEffects.None, depth);
        }

        public static void DrawSprite(this ISpriteTemplate sprite, SpriteBatch sb, int frame, Vector2 position, float depth)
        {
            sprite.DrawSprite(sb, frame, position, Color.White, 0f, Vector2.One, SpriteEffects.None, depth);
        }
    }
}
