using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace ControllerX
{
    class ControllerMapping
    {

        public ControllerMapping()
        {
            _controlMapping = new List<Mapping>();
        }

        private Mapping GetMappingFor(GamepadButtonFlags control)
        {
            Mapping temp = null;

            foreach(Mapping m in _controlMapping)
                if (m.Control.ToString() == control.ToString())
                    temp = m;

            return temp;
        }

        private List<Mapping> _controlMapping;

        public void Process(State state)
        {
            foreach (GamepadButtonFlags flag in Enum.GetValues(typeof(GamepadButtonFlags)))
            {
                if (state.Gamepad.Buttons.HasFlag(flag))
                {
                    GetMappingFor(flag)?.Response?.Execute(state, flag);
                }
             }
        }


        public static ControllerMapping Default()
        {
            ControllerMapping mapping = new ControllerMapping();
            mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags.RightThumb, new LookAction()));
            mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags.LeftThumb, new MoveAction()));

            mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags.RightShoulder, new MouseAction(Action.MouseOption.Left)));
            mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags.LeftShoulder, new MouseAction(Action.MouseOption.Right)));
            mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags.B, )

            //mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags., new MouseAction(Action.MouseOption.Left)));


            return mapping;
        }

    }
}
