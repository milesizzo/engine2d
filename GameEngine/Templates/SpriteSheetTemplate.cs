using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Templates
{
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

        public override int Width
        {
            get { return this.parent.Width; }
        }

        public override int Height
        {
            get { return this.parent.Height; }
        }

        public override Texture2D Texture
        {
            get { return this.parent.Texture; }
            set { throw new NotImplementedException(); }
        }

        public void GetData<T>(Rectangle? rect, T[] data, int start, int count) where T : struct
        {
            var viewport = this.source;
            if (rect != null)
            {
                viewport.X += rect.Value.X;
                viewport.Y += rect.Value.Y;
                viewport.Width = rect.Value.Width;
                viewport.Height = rect.Value.Height;
            }
            this.Texture.GetData(0, viewport, data, start, count);
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
                gridWidth = (this.texture.Width - border) / (this.spriteWidth + border);
                gridHeight = (this.texture.Height - border) / (this.spriteHeight + border);
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
}
