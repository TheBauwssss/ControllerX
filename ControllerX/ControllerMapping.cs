using System;
using System.Collections.Generic;
using System.Data;
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
            Configuration = new ControllerConfiguration();
        }

        private Mapping GetMappingFor(GamepadButtonFlags control)
        {
            Mapping temp = null;

            foreach(Mapping m in _controlMapping)
                if (m.Control.ToString() == control.ToString())
                    temp = m;

            return temp;
        }

        private readonly List<Mapping> _controlMapping;
        public ControllerConfiguration Configuration { get; set; }

        public void Map(Mapping mapping)
        {
            if (GetMappingFor(mapping.Control) != null)
                throw new DuplicateNameException("Only one action can be mapped to a single key");

            mapping.Response.MappingParent = this;

            _controlMapping.Add(mapping);
        }

        public void Process(State state)
        {
            foreach (Mapping mapping in _controlMapping)
                mapping.Response.Execute(state, mapping);
        }


        public static ControllerMapping Default()
        {
            ControllerMapping controllerMapping = new ControllerMapping();
            controllerMapping.Map(Mapping.Create(GamepadButtonFlags.RightThumb, new LookAction()));
            controllerMapping.Map(Mapping.Create(GamepadButtonFlags.LeftThumb, new MoveAction()));

            controllerMapping.Map(Mapping.Create(GamepadButtonFlags.RightShoulder, new MouseAction(Action.MouseOption.Left)));
            controllerMapping.Map(Mapping.Create(GamepadButtonFlags.LeftShoulder, new MouseAction(Action.MouseOption.Right)));

            //mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags., new MouseAction(Action.MouseOption.Right)));
            //mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags.LeftShoulder, new MouseAction(Action.MouseOption.Right)));

            //mapping._controlMapping.Add(Mapping.Create(GamepadButtonFlags., new MouseAction(Action.MouseOption.Left)));


           

            return controllerMapping;
        }

    }
}
