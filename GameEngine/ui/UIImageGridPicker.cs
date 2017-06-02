using GameEngine.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameEngine.UI
{
    public delegate void UIImageGridClicked(UIImageGridPicker caller, Point cell);

    public class UIImageGridPicker : UIPanel
    {
        private readonly List<ISpriteTemplate> sprites = new List<ISpriteTemplate>();
        private readonly int numColumns;
        private readonly int numRows;
        private int spriteWidth;
        private int spriteHeight;
        private Point? highlighted;
        private bool clicked;
        public Color HighlightColour;
        public Color ClickedColour;
        public event UIImageGridClicked GridClick;

        public UIImageGridPicker(int rows, int cols, UIElement parent) : base(parent)
        {
            this.numRows = rows;
            this.numColumns = cols;
        }

        public UIImageGridPicker(int rows, int cols) : base()
        {
            this.numRows = rows;
            this.numColumns = cols;
        }

        public override void Initialise()
        {
            base.Initialise();
            this.HighlightColour = Color.Red;
            this.ClickedColour = Color.DarkRed;
            this.Padding = Vector2.Zero;
            this.Border.Padding = 2;

            this.MouseEnter += (owner) =>
            {
            };
            this.MouseLeave += (owner) =>
            {
                this.highlighted = null;
                this.clicked = false;
            };
            this.MouseFocus += (owner) =>
            {
                var mouse = Mouse.GetState();
                var location = this.ScreenToLocal(new Vector2(mouse.X, mouse.Y));
                var x = (int)(location.X / this.spriteWidth);
                var y = (int)(location.Y / this.spriteHeight);
                this.highlighted = new Point(x, y);

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!this.clicked)
                    {
                        this.clicked = true;
                    }
                }
                else
                {
                    if (this.clicked)
                    {
                        this.clicked = false;
                        if (this.highlighted.HasValue)
                        {
                            var i = this.PointToIndex(this.highlighted.Value);
                            if (i >= 0 && i < this.sprites.Count)
                            {
                                this.GridClick?.Invoke(this, this.highlighted.Value);
                            }
                        }
                    }
                }
            };
        }

        public int PointToIndex(Point point)
        {
            return point.Y * this.numColumns + point.X;
        }

        public void AddSprites(IEnumerable<ISpriteTemplate> sprites)
        {
            foreach (var sprite in sprites)
            {
                this.AddSprite(sprite);
            }
        }

        public void AddSprite(ISpriteTemplate sprite)
        {
            if (this.sprites.Any())
            {
                if (sprite.Width != this.spriteWidth || sprite.Height != this.spriteHeight)
                {
                    throw new InvalidOperationException("All sprites need to be the same dimensions");
                }
            }
            else
            {
                this.spriteWidth = sprite.Width;
                this.spriteHeight = sprite.Height;
                this.MinimumSize = new Size2(this.numColumns * this.spriteWidth, this.numRows * this.spriteHeight);
            }
            this.sprites.Add(sprite);
        }

        protected override void Render(SpriteBatch screen)
        {
            base.Render(screen);
            var topLeft = this.ScreenTopLeft + this.Padding;
            var i = 0;
            for (var y = 0; y < this.numRows; y++)
            {
                var pos = new Vector2(topLeft.X, topLeft.Y + y * this.spriteHeight);
                for (var x = 0; x < this.numColumns; x++)
                {
                    var colour = Color.White;
                    if (this.highlighted.HasValue && this.highlighted.Value.X == x && this.highlighted.Value.Y == y)
                    {
                        colour = this.clicked ? this.ClickedColour : this.HighlightColour;
                    }
                    if (i < this.sprites.Count)
                    {
                        this.sprites[i].DrawSprite(screen, pos, colour, 0, Vector2.One);
                    }
                    pos.X += this.spriteWidth;
                    i++;
                }
            }
        }
    }
}
