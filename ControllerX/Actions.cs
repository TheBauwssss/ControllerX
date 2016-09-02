using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsInput.Native;
using SharpDX.XInput;

namespace ControllerX
{
    class MoveAction : StickAction
    {
        internal MoveAction() { }

        public MoveAction(ControllerMapping parent) : base(parent) { }

        public override void Handler_OnThumbStickMoved(decimal x, decimal y, decimal magnitude)
        {
            decimal threshold = MappingParent.Configuration.StickMoveThreshold;

            if (x < -1 * threshold)
                Simulator.Keyboard.KeyDown(VirtualKeyCode.VK_A); //left
            else Simulator.Keyboard.KeyUp(VirtualKeyCode.VK_A);

            if (x > threshold)
                Simulator.Keyboard.KeyDown(VirtualKeyCode.VK_D); //right
            else Simulator.Keyboard.KeyUp(VirtualKeyCode.VK_D);

            if (y < -1 * threshold)
                Simulator.Keyboard.KeyDown(VirtualKeyCode.VK_S); //down
            else Simulator.Keyboard.KeyUp(VirtualKeyCode.VK_S);

            if (y > threshold)
                Simulator.Keyboard.KeyDown(VirtualKeyCode.VK_W); //up
            else Simulator.Keyboard.KeyUp(VirtualKeyCode.VK_W);
        }
    }

    class LookAction : StickAction
    {
        internal LookAction()
        {
            _thread = new Thread(CursorThreadCode);
            _thread.IsBackground = true;
            _thread.Name = "ControllerX Cursor Govenor";
            _thread.Start();
        }

        private DateTime _dt;
        private decimal deltaX = 0;
        private decimal deltaY = 0;
        private Thread _thread;

        decimal movementX = 0;
        decimal movementY = 0;

        private bool _running = true;

        public LookAction(ControllerMapping parent) : base(parent)
        {
            _thread = new Thread(CursorThreadCode);
            _thread.IsBackground = true;
            _thread.Name = "ControllerX Cursor Govenor";
            _thread.Start();
            _handler.OnThumbStickReleased += () =>
            {
                Console.Out.WriteLine("Thumb released.");
                movementX = 0;
                movementY = 0;
                deltaX = 0;
                deltaY = 0;
            };
        }

        ~LookAction()
        {
            _running = false;
        }

        public void CursorThreadCode()
        {

            

            while (_running)
            {
                Thread.Sleep(10);

                if (_dt == null)
                {
                    _dt = DateTime.Now;
                    continue;
                }

                if (deltaX != 0 || deltaY != 0)
                {

                    decimal maxSpeed = MappingParent.Configuration.CursorSensitivity;

                    decimal speedX = maxSpeed * deltaX;
                    decimal speedY = maxSpeed * deltaY;
                   
                    decimal elapsed = (decimal)(DateTime.Now - _dt).TotalMilliseconds; //elapsed time (millis) since last loop

                    decimal fraction = elapsed / 1000;

                    //part

                    movementX = speedX * fraction;
                    movementY = speedY * fraction;

                    if (movementX != 0 || movementY != 0)
                    {

                        System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

                        System.Drawing.Point n = new System.Drawing.Point(p.X + (int)movementX, p.Y - (int)movementY);

                        System.Windows.Forms.Cursor.Position = n;
                    }
                }




                //    movementX += speedX * fraction;
                //    movementY += speedY * fraction;

                //    decimal integralX = Math.Truncate(Math.Floor(movementX));
                //    decimal integralY = Math.Truncate(Math.Floor(movementY));

                //    if (integralX != 0 || integralY != 0)
                //    {

                //        System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

                //        System.Drawing.Point n = new System.Drawing.Point(p.X + (int)integralX, p.Y - (int)integralY);

                //        movementX -= integralX;
                //        movementY -= integralY;

                //        System.Windows.Forms.Cursor.Position = n;
                //    }
                //}
                _dt = DateTime.Now; 
            }
        }

        public override void Handler_OnThumbStickMoved(decimal x, decimal y, decimal magnitude)
        {
            deltaX = x;
            deltaY = y;
        }
    }

    class ButtonAction : Action
    {
        internal ButtonAction(VirtualKeyCode key)
        {
            Key = key;
        }

        public ButtonAction(VirtualKeyCode key, ControllerMapping parent) : base(parent)
        {
            Key = key;
        }

        public VirtualKeyCode Key { get; set; }

        public override void Execute(State state, Mapping mapping)
        {
            if (state.Gamepad.Buttons.HasFlag(mapping.Control))
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

        internal MouseAction(MouseOption option)
        {
            Option = option;
        }

        public MouseAction(MouseOption option, ControllerMapping parent) : base(parent)
        {
            Option = option;
        }

        public MouseOption Option { get; set; }

        public override void Execute(State state, Mapping mapping)
        {

            if (state.Gamepad.Buttons.HasFlag(mapping.Control))
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
