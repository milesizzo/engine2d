using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision.Shapes;

namespace GameEngine.Templates
{
    public class SingleSpriteFromSheetTemplate : ISpriteTemplate
    {
        private readonly SpriteSheetTemplate parent;
        private readonly Rectangle source;
        private readonly string name;

        public SingleSpriteFromSheetTemplate(string name, SpriteSheetTemplate parent, Rectangle source)
        {
            this.name = $"{parent.Name}.{name}";
            this.parent = parent;
            this.source = source;
        }

        public SpriteSheetTemplate Parent { get { return this.parent; } }

        public Rectangle Source { get { return this.source; } }

        public int NumberOfFrames { get { return 1; } }

        public Vector2 Origin
        {
            get { return this.parent.Origin; }
            set { throw new NotImplementedException(); }
        }

        public int Width
        {
            get { return this.parent.SpriteWidth; }
        }

        public int Height
        {
            get { return this.parent.SpriteHeight; }
        }

        public Texture2D Texture
        {
            get { return this.parent.Texture; }
            set { throw new NotImplementedException(); }
        }

        public int FPS
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Shape Shape
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return this.name; }
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

        public void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            sb.Draw(this.Texture, position, this.source, colour, rotation, this.Origin, scale, effects, depth);
        }
    }

    public class SpriteSheetTemplate : SpriteTemplate
    {
        public enum ParseMethod
        {
            VerticalFirst,
            HorizontalFirst
        }

        private readonly Texture2D texture;
        private readonly int spriteWidth;
        private readonly int spriteHeight;
        private readonly int border; // for serialization only
        protected readonly List<SingleSpriteFromSheetTemplate> sprites = new List<SingleSpriteFromSheetTemplate>();

        public SpriteSheetTemplate(string name, Texture2D texture, int spriteWidth, int spriteHeight, int border = -1, int numSprites = -1, ParseMethod method = ParseMethod.HorizontalFirst) : base(name)
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
            numSprites = numSprites == -1 ? gridWidth * gridHeight : numSprites;
            var x = 0;
            var y = 0;
            do
            {
                var source = new Rectangle(x * (this.spriteWidth + border) + border, y * (this.spriteHeight + border) + border, this.spriteWidth, this.spriteHeight);
                this.sprites.Add(new SingleSpriteFromSheetTemplate($"{this.sprites.Count}", this, source));
            } while (this.Parse(ref x, ref y, ref numSprites, gridWidth, gridHeight, method));
            this.border = border;
        }

        private bool Parse(ref int x, ref int y, ref int numSprites, int gridWidth, int gridHeight, ParseMethod method)
        {
            switch (method)
            {
                case ParseMethod.HorizontalFirst:
                    x++;
                    if (x >= gridWidth)
                    {
                        x = 0;
                        y++;
                    }
                    break;
                case ParseMethod.VerticalFirst:
                    y++;
                    if (y >= gridHeight)
                    {
                        y = 0;
                        x++;
                    }
                    break;
            }
            numSprites--;
            return numSprites > 0;
        }

        public IReadOnlyList<SingleSpriteFromSheetTemplate> Sprites { get { return this.sprites; } }

        public int IndexOf(SingleSpriteFromSheetTemplate sprite)
        {
            return this.sprites.IndexOf(sprite);
        }

        public int SpriteWidth { get { return this.spriteWidth; } }

        public int SpriteHeight { get { return this.spriteHeight; } }

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
