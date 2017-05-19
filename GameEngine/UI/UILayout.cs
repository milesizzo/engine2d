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

        public UIColumnLayout(UIElement parent) : base(parent) { }

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
            var width = size.X / MathHelper.Max(this.Elements.Count, this.MaximumColumns);
            var height = size.Y;
            UIElement last = null;
            foreach (var child in this.Elements)
            {
                child.Placement.X = last == null ? 0 : last.Placement.X + last.Size.X;
                child.Size.X = width;
                child.Size.Y = height;
                last = child;
            }
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
            UIElement last = null;
            foreach (var child in this.Elements)
            {
                child.Placement.Y = last == null ? 0 : last.Placement.Y + last.Size.Y;
                child.Size.X = width;
                child.Size.Y = height;
                last = child;
            }
            base.Update(gameTime);
        }

        protected override void Render(SpriteBatch screen)
        {
        }
    }
}
