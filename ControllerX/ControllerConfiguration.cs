using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerX
{
    public class ControllerConfiguration
    {

        private decimal _stickMoveThreshold;
        private decimal _stickLookThreshold;
        private short _stickLeftDeadzone;
        private short _stickRightDeadzone;

        private decimal _cursorSensitivity;

        public ControllerConfiguration()
        {
            StickMoveThreshold = 0.3m; //30% or 10000 units
            StickLookThreshold = 0.15m; //15% or 5000 units

            StickLeftDeadzone = 3500; //or 10%
            StickRightDeadzone = 3500; //or 10%

            CursorScalingMode = ThumbStickHandler.MovementScaling.Custom;
            CursorSensitivity = 1500; //pixels/sec
        }

        /// <summary>
        /// The stick movement required before a <see cref="MoveAction"/> is triggered.
        /// 
        /// Accepts <see cref="decimal"/> values from 0 to 1 with 0.3 being equal to 30%.
        /// </summary>
        public decimal StickMoveThreshold
        {
            get { return _stickMoveThreshold;}
            set
            {
                if (value >= 0 && value <= 1)
                    _stickMoveThreshold = value;
            }
        }

        /// <summary>
        /// The stick movement required before a <see cref="LookAction"/> is triggered.
        /// 
        /// Accepts <see cref="decimal"/> values from 0 to 1 with 0.3 being equal to 30%.
        /// </summary>
        public decimal StickLookThreshold
        {
            get { return _stickLookThreshold; }
            set
            {
                if (value >= 0 && value <= 1)
                    _stickLookThreshold = value;
            }
        }

        /// <summary>
        /// Circulair area centered on the left thumbstick in which it does not respond to interaction.
        /// 
        /// Accepts any <see cref="short"/> larger than 0.
        /// </summary>
        public short StickLeftDeadzone
        {
            get { return _stickLeftDeadzone; }
            set { if (value >= 0)
                    _stickLeftDeadzone = value;
            }
        }

        /// <summary>
        /// Circulair area centered on the right thumbstick in which it does not respond to interaction.
        /// Accepts any <see cref="short"/> larger than 0.
        /// </summary>
        public short StickRightDeadzone
        {
            get { return _stickRightDeadzone; }
            set
            {
                if (value >= 0)
                    _stickRightDeadzone = value;
            }
        }

        /// <summary>
        /// The scaling mode use to scale thumbstick movement to cursor movement.
        /// </summary>
        public ThumbStickHandler.MovementScaling CursorScalingMode { get; set; }

        /// <summary>
        /// The scaling mode use to scale thumbstick movement to character movement.
        /// </summary>
        public ThumbStickHandler.MovementScaling MovementScalingMode { get; set; }

        /// <summary>
        /// The maximum movement speed of the cursor in pixels per second.
        /// 
        /// Accepts <see cref="decimal"/> values from 100 to 20000.
        /// </summary>
        public decimal CursorSensitivity
        {
            get { return _cursorSensitivity; }
            set
            {
                if (value >= 100 && value <= 20000)
                    _cursorSensitivity = value;
            }
        }
    }
}
