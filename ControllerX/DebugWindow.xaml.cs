using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsInput.Native;
using SharpDX.XInput;

namespace ControllerX
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private Controller controller;
        private DispatcherTimer timer;
        private ControllerMapping mapping;

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            mapping = ControllerMapping.Default();

            numSensitivity.Value = (int)mapping.Configuration.CursorSensitivity;
            numMovementThreshold.Value = (int)(mapping.Configuration.StickMoveThreshold * 100);
            numCursorThreshold.Value = (int)(mapping.Configuration.StickLookThreshold * 100);
            numLeftDeadzone.Value = mapping.Configuration.StickLeftDeadzone;
            numRightDeadzone.Value = mapping.Configuration.StickRightDeadzone;

            controller = new Controller(UserIndex.One);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Stopwatch stp = new Stopwatch();
            //stp.Start();
            mapping.Process(controller.GetState());
            //stp.Stop();
            //Console.Out.WriteLine(stp.ElapsedMilliseconds);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();

            foreach (var item in Enum.GetValues(typeof(ThumbStickHandler.MovementScaling)))
            {
                list.Add(item.ToString());
            }

            comboCursorScaling.ItemsSource = list;
            comboCursorScaling.SelectedIndex = 2;

            comboMoveScaling.ItemsSource = list;
            comboMoveScaling.SelectedIndex = 0;
        }

        private void comboMoveScaling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.MovementScalingMode = (ThumbStickHandler.MovementScaling)Enum.Parse(typeof(ThumbStickHandler.MovementScaling), (sender as ComboBox).SelectedItem.ToString(), true);
        }

        private void comboCursorScaling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.CursorScalingMode = (ThumbStickHandler.MovementScaling)Enum.Parse(typeof(ThumbStickHandler.MovementScaling), (sender as ComboBox).SelectedItem.ToString(), true);
        }

        private void numSensitivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.CursorSensitivity = (decimal)numSensitivity.Value;
        }

        private void numMovementThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.StickMoveThreshold = (decimal) numMovementThreshold.Value / 100;
        }

        private void numCursorThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.StickLookThreshold = (decimal)numCursorThreshold.Value / 100;
        }

        private void numLeftDeadzone_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.StickLeftDeadzone = (short)numLeftDeadzone.Value;
        }

        private void numRightDeadzone_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mapping == null)
                return;

            mapping.Configuration.StickRightDeadzone = (short)numRightDeadzone.Value;
        }
    }
}
