using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using SharpDX.XInput;

namespace ControllerX
{
    abstract class Action
    {
        static Action()
        {
            Simulator = new InputSimulator();
        }

        public ControllerMapping MappingParent { get; set; }

        protected Action() { }

        protected Action(ControllerMapping parent)
        {
            MappingParent = parent;
        }

        public enum MouseOption
        {
            Left = 1,
            Right = 2,
            ScrollUp = 3,
            ScrollDown = 4
        }

        public bool CurrentState { get; set; }

        public static InputSimulator Simulator { get; set; }

        public abstract void Execute(State state, Mapping mapping);
    }

    abstract class StickAction : Action
    {

        protected ThumbStickHandler _handler;

        protected StickAction()
        {
            _handler = new ThumbStickHandler();
            _handler.OnThumbStickMoved += Handler_OnThumbStickMoved;
        }

        protected StickAction(ControllerMapping parent) : base (parent)
        {
            _handler = new ThumbStickHandler();
            _handler.OnThumbStickMoved += Handler_OnThumbStickMoved;
        }

        public abstract void Handler_OnThumbStickMoved(decimal x, decimal y, decimal magnitude);

        public override void Execute(State state, Mapping mapping)
        {
            short x, y;

            if (mapping.Control == GamepadButtonFlags.LeftThumb)
            {
                x = state.Gamepad.LeftThumbX;
                y = state.Gamepad.LeftThumbY;
            }
            else
            {
                x = state.Gamepad.RightThumbX;
                y = state.Gamepad.RightThumbY;
            }

            _handler.RegisterValue(x, y);
        }
    }
}
