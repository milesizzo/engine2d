using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Helpers
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        X1,
        X2,
    }

    public static class MouseHelper
    {
        private static Dictionary<MouseButton, bool> buttonHeld = new Dictionary<MouseButton, bool>();
        private static int? scrollValue = null;

        public static bool ButtonDown(MouseButton button)
        {
            var mouse = Mouse.GetState();
            var state = ButtonState.Released;
            switch (button)
            {
                case MouseButton.Left:
                    state = mouse.LeftButton;
                    break;
                case MouseButton.Middle:
                    state = mouse.MiddleButton;
                    break;
                case MouseButton.Right:
                    state = mouse.RightButton;
                    break;
                case MouseButton.X1:
                    state = mouse.XButton1;
                    break;
                case MouseButton.X2:
                    state = mouse.XButton2;
                    break;
            }
            return state == ButtonState.Pressed;
        }

        public static bool ButtonPressed(MouseButton button)
        {
            var mouse = Mouse.GetState();
            var state = ButtonState.Released;
            switch (button)
            {
                case MouseButton.Left:
                    state = mouse.LeftButton;
                    break;
                case MouseButton.Middle:
                    state = mouse.MiddleButton;
                    break;
                case MouseButton.Right:
                    state = mouse.RightButton;
                    break;
                case MouseButton.X1:
                    state = mouse.XButton1;
                    break;
                case MouseButton.X2:
                    state = mouse.XButton2;
                    break;
            }
            if (state == ButtonState.Pressed)
            {
                if (!buttonHeld.ContainsKey(button))
                {
                    buttonHeld[button] = true;
                    return true;
                }
            }
            else
            {
                buttonHeld.Remove(button);
            }
            return false;
        }

        public static int ScrollDirection
        {
            get
            {
                var value = Mouse.GetState().ScrollWheelValue;
                if (scrollValue == null)
                {
                    scrollValue = value;
                }
                else if (value > scrollValue)
                {
                    scrollValue = value;
                    return 1;
                }
                else if (value < scrollValue)
                {
                    scrollValue = value;
                    return -1;
                }
                return 0;
            }
        }
    }
}
