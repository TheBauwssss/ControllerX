using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace ControllerX
{
    abstract class Action
    {
        public enum MouseOption
        {
            Left = 1,
            Right = 2,
            ScrollUp = 3,
            ScrollDown = 4
        }

        public abstract void Execute(State state, GamepadButtonFlags mapping);
    }
}
