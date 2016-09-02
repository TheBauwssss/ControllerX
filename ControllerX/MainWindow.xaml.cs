using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsInput;
using WindowsInput.Native;
using DeviceManagement;
using SharpDX.XInput;
using Xceed.Wpf.Toolkit;
using Cursor = System.Windows.Forms.Cursor;

namespace ControllerX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            left = new ThumbStickHandler();
            right = new ThumbStickHandler();

            left.OnThumbStickMoved += Left_OnThumbStickMoved;
            right.OnThumbStickMoved += Right_OnThumbStickMoved;

            cursorTimer = new DispatcherTimer();
            cursorTimer.Interval = TimeSpan.FromMilliseconds(1);
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();

            sensitivity.Value = maxSpeed;
            sensitivityTHreshold.Value = left.SensitivityThreshold;
            deadzone.Value = left.InputDeadzone;

        }

        private decimal deltaX = 0;
        private decimal deltaY = 0;
        private int maxSpeed = 2000; //pixels/sec

        private InputSimulator sim = new InputSimulator();

        private DateTime dt;

        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            if (dt == null)
            {
                dt = DateTime.Now;
                return;
            }

            if (deltaX != 0 || deltaY != 0)
            {

                decimal speedX = maxSpeed*deltaX;
                decimal speedY = maxSpeed*deltaY;
                //Console.WriteLine("X: " + speedX + "\tY: " + speedY);

                double elapsed = (DateTime.Now - dt).TotalMilliseconds; //elapsed time (millis) since last loop

                double fraction = elapsed/1000;

                int movementX = (int) Decimal.Ceiling(speedX*(decimal) fraction);
                int movementY = (int) Decimal.Ceiling(speedY*(decimal) fraction);

                string debug = string.Format("Cursor ({2}):\n\tX: {0}\n\tY: {1}", movementX, movementY, left.Scaling);
                cursor.Content = debug;

                //Console.WriteLine("X: " + movementX + "\tY: " + movementY);

                System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

                System.Drawing.Point n = new System.Drawing.Point(p.X + movementX, p.Y - movementY);

                System.Windows.Forms.Cursor.Position = n;
            }
            dt = DateTime.Now;
        }

        private DispatcherTimer cursorTimer;

        private int precision = 5;

        private void Right_OnThumbStickMoved(decimal x, decimal y, decimal magnitude)
        {
            thumbR.Content = string.Format("Thumbstick Right:\n\t\tX: {0}\n\t\tY: {1}\n\t\tM: {2}", Math.Round(x, precision), Math.Round(y, precision), Math.Round(magnitude, precision));

            //Cursor stuff

            deltaX = x;
            deltaY = y;

          

        }

        private void Left_OnThumbStickMoved(decimal x, decimal y, decimal magnitude)
        {
            decimal threshold = 0.3m;

            thumbL.Content = string.Format("Thumbstick Left:\n\t\tX: {0}\n\t\tY: {1}\n\t\tM: {2}", Math.Round(x, precision), Math.Round(y, precision), Math.Round(magnitude, precision));

            if (x < -1*threshold)
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_A); //left
            else sim.Keyboard.KeyUp(VirtualKeyCode.VK_A);

            if (x > threshold)
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_D); //right
            else sim.Keyboard.KeyUp(VirtualKeyCode.VK_D);

            if (y < -1 * threshold)
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_S); //down
            else sim.Keyboard.KeyUp(VirtualKeyCode.VK_S);

            if (y > threshold)
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_W); //up
            else sim.Keyboard.KeyUp(VirtualKeyCode.VK_W);

        }

        private ThumbStickHandler left;
        ThumbStickHandler right;

        private DeviceInfo controllerDevice;

        private Controller controller;
        private DispatcherTimer timer;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // you need nuget pkg https://devicemgr.codeplex.com/
            // ClassName: "XB1UsbClass"
            // Desc: "Microsoft Xbox One Controller"
            var allClasses = DeviceInfoSet.GetAllClassesPresent();
            var devices = allClasses.GetDevices();
            controllerDevice = (from device in devices where device.ClassName == "XB1UsbClass" select device).FirstOrDefault();

            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            controller = new Controller(UserIndex.One);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private bool ldown = false;
        private bool rdown = false;

        private bool a_state = false;
        private bool b_state = false;
        private bool x_state = false;
        private bool y_state = false;

        private bool back_state = false;
        private bool start_state = false;

        private bool l_shoulder_state = false;
        private bool r_shoulder_state = false;

        public void CheckKey(State state, GamepadButtonFlags button, ref bool state_var, VirtualKeyCode key)
        {
            if (state.Gamepad.Buttons.HasFlag(button))
            {
                if (!state_var)
                {
                    state_var = true;
                    sim.Keyboard.KeyDown(key);
                }
            }
            else
            {
                if (state_var)
                {
                    state_var = false;
                    sim.Keyboard.KeyUp(key);
                }
            }
        }

        

        public void CheckKey(State state, GamepadButtonFlags button, ref bool state_var, System.Action action_down, System.Action action_up)
        {
           
            if (state.Gamepad.Buttons.HasFlag(button))
            {
                if (!state_var)
                {
                    state_var = true;
                    action_down?.Invoke();
                }
            }
            else
            {
                if (state_var)
                {
                    state_var = false;
                    action_up?.Invoke();
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var state = controller.GetState();

            var b = new StringBuilder();

            left.RegisterValue(state.Gamepad.LeftThumbX, state.Gamepad.LeftThumbY);
            right.RegisterValue(state.Gamepad.RightThumbX, state.Gamepad.RightThumbY);

            //Trigger stuff
            int triggerL = Convert.ToInt32(state.Gamepad.LeftTrigger);
            int triggerR = Convert.ToInt32(state.Gamepad.RightTrigger);

            if (triggerL > 30)
            {
                if (!ldown)
                {
                    sim.Mouse.RightButtonDown();
                    
                    ldown = true;
                }
            }
            else
            {
                if (ldown)
                {
                    sim.Mouse.RightButtonUp();
                    ldown = false;
                }
            }


            if (triggerR > 30)
            {
                if (!rdown)
                {
                    sim.Mouse.LeftButtonDown();
                    rdown = true;
                }
            }
            else
            {
                if (rdown)
                {
                    sim.Mouse.LeftButtonUp();
                    rdown = false;
                }
            }

            //CheckKey(state, GamepadButtonFlags.A, ref a_state, VirtualKeyCode.SPACE);
            CheckKey(state, GamepadButtonFlags.B, ref b_state, VirtualKeyCode.ESCAPE);
            //CheckKey(state, GamepadButtonFlags.X, ref b_state, VirtualKeyCode.SPACE);
            CheckKey(state, GamepadButtonFlags.Y, ref y_state, VirtualKeyCode.SPACE);

            CheckKey(state, GamepadButtonFlags.Back, ref back_state, VirtualKeyCode.VK_E);
            CheckKey(state, GamepadButtonFlags.Start, ref start_state, VirtualKeyCode.ESCAPE);



            CheckKey(state, GamepadButtonFlags.LeftShoulder, ref l_shoulder_state, ()=>
            {
                sim.Mouse.HorizontalScroll(-1);
            }, null);

            CheckKey(state, GamepadButtonFlags.RightShoulder, ref r_shoulder_state, () =>
            {
                sim.Mouse.HorizontalScroll(1);
            }, null);


            


            b.AppendFormat("Triggers: \n\t R:\t{0}\n\t L:\t{1}\n", triggerR, triggerL);

            //Right stick

            int rX = state.Gamepad.RightThumbX; //Normalize so value goes from 0 -> 32767          
            int rY = state.Gamepad.RightThumbY; //Normalize so value goes from 0 -> 32767
            b.AppendFormat("Stick Right: \n\t X:\t{0} \n\t Y:\t{1}\n", state.Gamepad.RightThumbX, state.Gamepad.RightThumbY);

            b.AppendFormat("Stick Left: \n\t X:\t{0} \n\t Y:\t{1}\n", state.Gamepad.LeftThumbX, state.Gamepad.LeftThumbY);

            //Buttons = string.Format("A: {0} B: {1} X: {2} Y: {3}", state.Gamepad.Buttons.ToString(), state.Gamepad.LeftThumbY);
            b.AppendLine("Buttons: " + string.Format("{0}", state.Gamepad.Buttons));

            output.Content = b;
        }

        void Test(State state)
        {
            int INPUT_DEADZONE = 4000;
            bool clipped = false;

            var b = new StringBuilder();

            var LX = state.Gamepad.LeftThumbX;
            var LY = state.Gamepad.LeftThumbY;

            //b.AppendFormat("X:{0}\t{1}\n", LX, LY);


            //determine how far the controller is pushed
            double magnitude = Math.Sqrt(LX*LX + LY*LY);


            //determine the direction the controller is pushed
            double normalizedLX = LX/magnitude;
            double normalizedLY = LY/magnitude;


            //test

            double normX = LX;

            if (normX < 0)
                normX += INPUT_DEADZONE;
            else
                normX -= INPUT_DEADZONE;

            normX = normX/(32767 - INPUT_DEADZONE);


            double normalizedMagnitude = 0;


            //check if the controller is outside a circular dead zone
            if (magnitude > INPUT_DEADZONE)
            {
                //clip the magnitude at its expected maximum value
                if (magnitude > 32767) magnitude = 32767;

                //adjust magnitude relative to the end of the dead zone
                magnitude -= INPUT_DEADZONE;

                //optionally normalize the magnitude with respect to its expected range
                //giving a magnitude value of 0.0 to 1.0
                normalizedMagnitude = magnitude/(32767 - INPUT_DEADZONE);
            }
            else //if the controller is in the deadzone zero out the magnitude
            {
                clipped = true;
            }


            var m = Math.Round(magnitude, 1);
            var nm = Math.Round(normalizedMagnitude, 4);


            b.AppendFormat("Stick Left: \n\t X:\t{0} \n\t Y:\t{1}\n\n", state.Gamepad.LeftThumbX, state.Gamepad.LeftThumbY);
            b.AppendFormat("Magnitude: {0} {1}\n", (clipped) ? 0 : m, (clipped) ? "(" + m + ")" : "");
            b.AppendFormat("Normalized: {0} {1}\n", (clipped) ? 0 : nm, (clipped) ? "(" + nm + ")" : "");
            b.AppendFormat("Clipped: {0}\n", clipped);

            b.AppendFormat("Normalized Stick Left: \n\t X:\t{0} \n\t Y:\t{1}\n", normalizedLX, normalizedLY);
            b.Append("X: " + normX);

            output.Content = b;
        }

        private void comboSensitivityScale_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();

            foreach (var item in Enum.GetValues(typeof (ThumbStickHandler.MovementScaling)))
            {
                list.Add(item.ToString());
            }

            comboSensitivityScale.ItemsSource = list;
            comboSensitivityScale.SelectedIndex = 2;
        }

        private void comboSensitivityScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThumbStickHandler.MovementScaling scaling = (ThumbStickHandler.MovementScaling) Enum.Parse(typeof (ThumbStickHandler.MovementScaling), (sender as ComboBox).SelectedItem.ToString(), true);

            left.Scaling = scaling;
            right.Scaling = scaling;
        }

        private void deadzone_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender == null || left == null || right == null)
                return;

            left.InputDeadzone = (int) (sender as IntegerUpDown).Value;
            right.InputDeadzone = (int) (sender as IntegerUpDown).Value;
        }

        private void sensitivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender == null)
                return;

            maxSpeed = (int) (sender as IntegerUpDown).Value;
        }

        private void sensitivityTHreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender == null || left == null || right == null)
                return;

            left.SensitivityThreshold = (int) (sender as IntegerUpDown).Value;
            right.SensitivityThreshold = (int) (sender as IntegerUpDown).Value;
        }
    }
}
