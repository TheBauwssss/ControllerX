using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using WindowsInput.Native;
using SharpDX.XInput;

namespace ControllerX
{
    internal class Mapping
    {
        public GamepadButtonFlags Control { get; set; }
        public Action Response { get; set; }

        public static Mapping Create(GamepadButtonFlags control, Action response)
        {
            Mapping m = new Mapping();
            m.Control = control;
            m.Response = response;
            return m;
        }

        public static Mapping Create(GamepadButtonFlags control, ButtonAction response, VirtualKeyCode key)
        {
            Mapping m = new Mapping();
            m.Control = control;
            m.Response = response;
            (m.Response as ButtonAction).Key = key;
            return m;
        }

        public static Mapping Create(GamepadButtonFlags control, MouseAction response, Action.MouseOption option)
        {
            Mapping m = new Mapping();
            m.Control = control;
            m.Response = response;
            (m.Response as MouseAction).Option = option;
            return m;
        }
    }

}
