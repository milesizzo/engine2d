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
        public static void Write(ISerializer context, Color colour)
        {
            context.Write("value", colour.PackedValue);
        }

        public static void Read(IDeserializer context, out Color colour)
        {
            colour = new Color(context.Read<uint>("value"));
        }
    }
}
