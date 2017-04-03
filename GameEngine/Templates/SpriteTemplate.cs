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
    public abstract class SpriteTemplate : ITemplate
    {
        private Vector2 origin;
        public int FPS = 5;
        private Shape shape;
        private readonly string name;

        protected SpriteTemplate(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public abstract Texture2D Texture { get; set; }

        public abstract int NumberOfFrames { get; }

        public virtual int Width { get { return this.Texture.Width; } }

        public virtual int Height { get { return this.Texture.Height; } }

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
            get
            {
                return this.shape ?? this.DefaultShape;
            }
            set
            {
                this.shape = value;
            }
        }

        public virtual Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        public void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.DrawSprite(sb, frame, position, colour, rotation, scale, effects, 0f);
        }

        public void DrawSprite(SpriteBatch sb, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            this.DrawSprite(sb, 0, position, colour, rotation, scale, effects, 0f);
        }

        public void DrawSprite(SpriteBatch sb, Vector2 position, Color colour, float rotation, Vector2 scale)
        {
            this.DrawSprite(sb, 0, position, colour, rotation, scale, SpriteEffects.None, 0f);
        }

        public abstract void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth);
    }

    public class AnimatedSpriteTemplate : SpriteTemplate
    {
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public AnimatedSpriteTemplate(string name, IEnumerable<Texture2D> textures) : base(name)
        {
            float averageWidth = 0, averageHeight = 0;
            foreach (var texture in textures)
            {
                averageWidth += texture.Width;
                averageHeight += texture.Height;
                this.textures.Add(texture);
            }
            averageWidth /= this.textures.Count;
            averageHeight /= this.textures.Count;
            this.Origin = new Vector2(averageWidth / 2, averageHeight / 2);
        }

        public IReadOnlyList<Texture2D> Textures
        {
            get { return this.textures.AsReadOnly(); }
        }

        public override Texture2D Texture
        {
            get { return this.textures.First(); }
            set
            {
                this.textures.Clear();
                this.textures.Add(value);
            }
        }

        public override int NumberOfFrames
        {
            get { return this.textures.Count; }
        }

        public void AddTexture(Texture2D texture)
        {
            this.textures.Add(texture);
        }

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            sb.Draw(this.textures[frame], position, null, colour, rotation, this.Origin, scale, effects, depth);
        }
    }

    public class SingleSpriteFromSheetTemplate : SpriteTemplate
    {
        private readonly SpriteSheetTemplate parent;
        private readonly Rectangle source;

        public SingleSpriteFromSheetTemplate(string name, SpriteSheetTemplate parent, Rectangle source) : base($"{parent.Name}.{name}")
        {
            this.parent = parent;
            this.source = source;
        }

        public override int NumberOfFrames { get { return 1; } }

        public override Vector2 Origin
        {
            get { return this.parent.Origin; }
            set { throw new NotImplementedException(); }
        }

        public override Texture2D Texture
        {
            get { return this.parent.Texture; }
            set { throw new NotImplementedException(); }
        }

        public void DrawSprite(SpriteBatch sb, Vector2 position, float depth)
        {
            this.DrawSprite(sb, 0, position, Color.White, 0, Vector2.One, SpriteEffects.None, depth);
        }

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            sb.Draw(this.Texture, position, this.source, colour, rotation, this.Origin, scale, effects, depth);
        }
    }

    public class SpriteSheetTemplate : SpriteTemplate
    {
        private readonly Texture2D texture;
        private readonly int spriteWidth;
        private readonly int spriteHeight;
        private readonly int border; // for serialization only
        protected readonly List<SingleSpriteFromSheetTemplate> sprites = new List<SingleSpriteFromSheetTemplate>();

        public SpriteSheetTemplate(string name, Texture2D texture, int spriteWidth, int spriteHeight, int border = -1) : base(name)
        {
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.texture = texture;
            if (border < 0)
            {
                if (this.texture.Width % this.spriteWidth != 0)
                {
                    // there is probably a border around each sprite
                    // TODO: detect borders of width > 1
                    border = 1;
                }
                else
                {
                    border = 0;
                }
            }
            int gridWidth, gridHeight;
            if (border == 0)
            {
                gridWidth = this.texture.Width / this.spriteWidth;
                gridHeight = this.texture.Height / this.spriteHeight;
            }
            else
            {
                gridWidth = (this.texture.Width - border) % (this.spriteWidth + border);
                gridHeight = (this.texture.Height - border) % (this.spriteHeight + border);
            }
            this.Origin = new Vector2(this.spriteWidth / 2, this.spriteHeight / 2);
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var source = new Rectangle(x * (this.spriteWidth + border) + border, y * (this.spriteHeight + border) + border, this.spriteWidth, this.spriteHeight);
                    this.sprites.Add(new SingleSpriteFromSheetTemplate($"{this.sprites.Count}", this, source));
                }
            }
            this.border = border;
        }

        public IReadOnlyList<SingleSpriteFromSheetTemplate> Sprites { get { return this.sprites; } }

        public override int Width { get { return this.spriteWidth; } }

        public override int Height { get { return this.spriteHeight; } }

        public override int NumberOfFrames { get { return 1; } }

        public int Border { get { return this.border; } } // for serialization only

        public override Texture2D Texture
        {
            get { return this.texture; }
            set { throw new NotImplementedException(); }
        }

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            this.sprites[frame].DrawSprite(sb, 0, position, colour, rotation, scale, effects, depth);
        }
    }

    public class AnimatedSpriteSheetTemplate : SpriteSheetTemplate
    {
        private readonly int numFrames;

        public AnimatedSpriteSheetTemplate(string name, Texture2D texture, int spriteWidth, int spriteHeight, int border = -1, int numFrames = -1) : base(name, texture, spriteWidth, spriteHeight, border)
        {
            this.numFrames = numFrames == -1 ? this.Sprites.Count : numFrames;
        }

        public override int NumberOfFrames { get { return this.numFrames; } }
    }

    public class SingleSpriteTemplate : SpriteTemplate
    {
        private Texture2D texture;

        public SingleSpriteTemplate(string name, Texture2D texture) : base(name)
        {
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.texture = texture;
        }

        public override Texture2D Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public override int NumberOfFrames
        {
            get
            {
                return 1;
            }
        }

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            sb.Draw(this.texture, position, null, colour, rotation, this.Origin, scale, effects, depth);
        }
    }
}
