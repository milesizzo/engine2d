using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Serializing
{
    public static partial class CommonSerialize
    {
        public static void Write(ISerializer context, Rectangle rect)
        {
            context.Write("x", rect.X);
            context.Write("y", rect.Y);
            context.Write("w", rect.Width);
            context.Write("h", rect.Height);
        }

        public static void Read(IDeserializer context, out Rectangle rect)
        {
            var x = context.Read<int>("x");
            var y = context.Read<int>("y");
            var width = context.Read<int>("w");
            var height = context.Read<int>("h");
            rect = new Rectangle(x, y, width, height);
        }
    }
}
