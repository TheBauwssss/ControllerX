using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsInput.Native;
using SharpDX.XInput;

namespace ControllerX
{
    class MoveAction : Action
    {
        public override void Execute(State state, GamepadButtonFlags mapping)
        {
            throw new NotImplementedException();
        }
    }

    class LookAction : Action
    {
        public override void Execute(State state, GamepadButtonFlags mapping)
        {
            throw new NotImplementedException();
        }
    }

    class ButtonAction : Action
    {

        public ButtonAction(VirtualKeyCode key)
        {
            Key = key;
        }

        public VirtualKeyCode Key { get; set; }

        public override void Execute(State state, GamepadButtonFlags mapping)
        {
            if (state.Gamepad.Buttons.HasFlag(mapping))
            {
                if (!CurrentState)
                {
                    CurrentState = true;
                    Simulator.Keyboard.KeyDown(Key);
                }
            }
            else
            {
                if (CurrentState)
                {
                    CurrentState = false;
                    Simulator.Keyboard.KeyUp(Key);
                }
            }
        }
    }

    class MouseAction : Action
    {

        public MouseAction(MouseOption option)
        {
            Option = option;
        }

        public MouseOption Option { get; set; }

        public override void Execute(State state, GamepadButtonFlags mapping)
        {

            if (state.Gamepad.Buttons.HasFlag(mapping))
            {
                if (!CurrentState)
                {
                    CurrentState = true;

                    switch (Option)
                    {
                        case MouseOption.Left:
                            Simulator.Mouse.LeftButtonDown();
                            break;
                        case MouseOption.Right:
                            Simulator.Mouse.RightButtonDown();
                            break;
                        case MouseOption.ScrollUp:
                            Simulator.Mouse.VerticalScroll(1);
                            break;
                        case MouseOption.ScrollDown:
                            Simulator.Mouse.VerticalScroll(-1);
                            break;
                    }
                }
            }
            else
            {
                if (CurrentState)
                {
                    CurrentState = false;

                    switch (Option)
                    {
                        case MouseOption.Left:
                            Simulator.Mouse.LeftButtonUp();
                            break;
                        case MouseOption.Right:
                            Simulator.Mouse.RightButtonUp();
                            break;
                        case MouseOption.ScrollUp:
                            //nothing
                            break;
                        case MouseOption.ScrollDown:
                            //nothing
                            break;
                    }
                }
            
            }
        }
    }
}
