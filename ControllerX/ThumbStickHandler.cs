using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerX
{
    class ThumbStickHandler
    {
        public ThumbStickHandler()
        {
            InputDeadzone = 5000;
            SensitivityThreshold = 5000;
            Scaling = MovementScaling.Custom;
        }

        public enum MovementScaling
        {
            Lineair,        //X
            Exponential,    //X^2
            Custom
        }

        const int THUMB_MAX = 32767;
        const int THUMB_MIN = -32767;

        #region Events
        /// <summary>
        /// Raised when the position of the thumbstick changes.
        /// </summary>
        /// <param name="x">normalized movement along the X axis, -1 to 1</param>
        /// <param name="y">normalized movement along the Y axis, -1 to 1</param>
        /// <param name="magnitude">normalized movement of the stick, 0 to 1</param>
        public delegate void OnThumbStickMovedEventHandler(decimal x, decimal y, decimal magnitude);
        public event OnThumbStickMovedEventHandler OnThumbStickMoved;

        private decimal lastx = 0;
        private decimal lasty = 0;
        private decimal lastMagnitude = 0;

        private void RaiseThumbStickMovedEvent(decimal x, decimal y, decimal magnitude)
        {
            //Only raise if the values have changed
            if (x != lastx || y != lasty || magnitude != lastMagnitude)
                OnThumbStickMoved?.Invoke(x, y, magnitude);

            lastx = x;
            lasty = y;
            lastMagnitude = magnitude;
        }

        #endregion


        public int InputDeadzone { get; set; }
        public MovementScaling Scaling { get; set; }
        public int SensitivityThreshold { get; set; }

        private void CompensateForDeadzone(ref decimal value)
        {
            if (value < 0)
                value += InputDeadzone;
            else if (value > 0)
                value -= InputDeadzone;

            value = value/(32767 - InputDeadzone);
        }

        private void Scale(ref decimal value)
        {
            if (value == 0)
                return;

            bool neg = value < 0;
            switch (Scaling)
            {
                case MovementScaling.Exponential:
                    value = (decimal)Math.Pow((double)value, 2);
                    if (neg)
                        value = value * -1;
                    break;
                case MovementScaling.Custom:
                    decimal translation = 0.1m;

                    if (neg)
                        value = value - translation;
                    else value = value + translation;

                    value = (decimal) Math.Pow((double)value, 2);


                    if (neg)
                        value = value * -1;
                    break;
            }
           
        }

        private void Normalize(ref decimal value)
        {
            if (value < THUMB_MIN)
                value = THUMB_MIN;
            else if (value > THUMB_MAX)
                value = THUMB_MAX;
        }

        private bool ValidateMovement(ref decimal x, ref decimal y, out decimal normalizedMagnitude)
        {
            
            //determine how far the controller is pushed
            decimal magnitude = (decimal)Math.Sqrt((double)x*(double)x + (double)y*(double)y);

            //check if the controller is outside a circular dead zone
            if (magnitude > InputDeadzone)
            {
                //clip the magnitude at its expected maximum value
                if (magnitude > 32767) magnitude = 32767;

                //adjust magnitude relative to the end of the dead zone
                magnitude -= InputDeadzone;

                //optionally normalize the magnitude with respect to its expected range
                //giving a magnitude value of 0.0 to 1.0
                normalizedMagnitude = magnitude / (32767 - InputDeadzone);

                if (x > -1*SensitivityThreshold && x < SensitivityThreshold)
                    x = 0;

                if (y > -1 * SensitivityThreshold && y < SensitivityThreshold)
                    y = 0;

                return true;
            }
            else //if the controller is in the deadzone zero out the magnitude
            {
                normalizedMagnitude = 0;
                x = 0;
                y = 0;
                return false;
            }
        }

        public void RegisterValue(decimal x, decimal y)
        {
            //Normalize the values
            Normalize(ref x);
            Normalize(ref y);

            decimal magnitude;

            ValidateMovement(ref x, ref y, out magnitude);

            CompensateForDeadzone(ref x);
            CompensateForDeadzone(ref y);

            Scale(ref x);
            Scale(ref y);

            RaiseThumbStickMovedEvent(x,y,magnitude);

        }

    }
}
