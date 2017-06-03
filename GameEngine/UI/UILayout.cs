using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameEngine.UI
{
    public class UIColumnLayout : UIElement
    {
        public int MaximumColumns;
        private readonly List<float> widths = new List<float>();

        public UIColumnLayout(UIElement parent, params float[] widths) : base(parent)
        {
            if (widths.Any())
            {
                if (widths.Sum() != 1f) throw new InvalidOperationException("Widths must sum to 1");
                this.widths.AddRange(widths);
            }
        }

        public UIColumnLayout() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Size.RelativeX = 1f;
            this.Size.RelativeY = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            var size = this.AbsoluteSize;
            if (!this.widths.Any())
            {
                foreach (var element in this.Elements)
                {
                    this.widths.Add(1f / this.Elements.Count);
                }
            }
            if (this.widths.Count != this.Elements.Count) throw new InvalidOperationException("Number of elements must match number of widths");
            var i = 0;
            UIElement last = null;
            foreach (var child in this.Elements)
            {
                child.Placement.X = last == null ? 0 : last.Placement.X + last.AbsoluteSize.X;
                child.Size.X = this.widths[i] * size.X;
                child.Size.Y = size.Y;
                last = child;
                i++;
            }

            /*
            var width = size.X / MathHelper.Max(this.Elements.Count, this.MaximumColumns);
            var height = size.Y;

            var fixedWidth = 0f;
            var numUnfixed = 0;

            foreach (var child in this.Elements)
            {
                var childMinWidth = child.MinimumSize.Width;
                if (childMinWidth > width)
                {
                    fixedWidth += childMinWidth;
                }
                else
                {
                    numUnfixed++;
                }
            }
            width = (size.X - fixedWidth) / numUnfixed;

            UIElement last = null;
            foreach (var child in this.Elements)
            {
                child.Placement.X = last == null ? 0 : last.Placement.X + last.AbsoluteSize.X;
                child.Size.X = MathHelper.Max(width, child.MinimumSize.Width);
                child.Size.Y = height;
                last = child;
            }
            */
            base.Update(gameTime);
        }

        protected override void Render(SpriteBatch screen)
        {
        }
    }

    public class UIRowLayout : UIElement
    {
        public int MaximumRows;

        public UIRowLayout(UIElement parent) : base(parent) { }

        public UIRowLayout() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Size.RelativeX = 1f;
            this.Size.RelativeY = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            var size = this.AbsoluteSize;
            var height = size.Y / MathHelper.Max(this.Elements.Count, this.MaximumRows);
            var width = size.X;

            var fixedHeight = 0f;
            var numUnfixed = 0;
            foreach (var child in this.Elements)
            {
                var childMinHeight = child.MinimumSize.Height;
                if (childMinHeight > height)
                {
                    fixedHeight += childMinHeight;
                }
                else
                {
                    numUnfixed++;
                }
            }
            height = (size.Y - fixedHeight) / numUnfixed;

            UIElement last = null;
            foreach (var child in this.Elements)
            {
                child.Placement.Y = last == null ? 0 : last.Placement.Y + last.AbsoluteSize.Y;
                child.Size.X = width;
                child.Size.Y = MathHelper.Max(height, child.MinimumSize.Height);
                last = child;
            }
            base.Update(gameTime);
        }

        protected override void Render(SpriteBatch screen)
        {
        }
    }
}
