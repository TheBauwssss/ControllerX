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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
