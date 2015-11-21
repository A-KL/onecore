namespace Microsoft.Iot.Extended.Hid
{
    using System;
    using System.Collections.Generic;
    using Windows.Devices.HumanInterfaceDevice;

    public enum ControllerDirection
    {
        None = 0, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft
    }

    public class ControllerVector : EventArgs
    {
        /// <summary>
        /// Get what direction the controller is pointing
        /// </summary>
        public ControllerDirection Direction { get; set; }

        /// <summary>
        /// Gets a value indicating the magnitude of the direction
        /// </summary>
        public int Magnitude { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ControllerVector otherVector = obj as ControllerVector;

            if (this.Magnitude == otherVector.Magnitude && this.Direction == otherVector.Direction)
            {
                return true;
            }

            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // disable overflow
            unchecked
            {
                int hash = 27;
                hash = (13 * hash) + this.Direction.GetHashCode();
                hash = (13 * hash) + this.Magnitude.GetHashCode();
                return hash;
            }
        }
    }

    public class Xbox360Controller
    {
        #region Members

        private readonly Dictionary<ControllerDirection, int> maxMagnitude = new Dictionary<ControllerDirection, int>();

        /// <summary>
        /// Tolerance to ignore around (0,0) for thumbstick movement
        /// </summary>
        private const double DeadzoneTolerance = 4000;

        #endregion

        #region Public

        /// <summary>
        /// Initializes a new instance of the XboxHidController class from a 
        /// HidDevice handle
        /// </summary>
        /// <param name="deviceHandle">Handle to the HidDevice</param>
        public Xbox360Controller(HidDevice deviceHandle)
        {
            this.deviceHandle = deviceHandle;
            deviceHandle.InputReportReceived += InputReportReceived;
            this.DirectionVector = new ControllerVector() {Direction = ControllerDirection.None, Magnitude = 10000};
            foreach (var direction in Enum.GetValues(typeof (ControllerDirection)))
            {
                this.maxMagnitude[(ControllerDirection) direction] = 0;
            }
        }

        /// <summary>
        /// Direction the controller is indicating.
        /// </summary>
        public ControllerVector DirectionVector { get; set; }

        /// <summary>
        /// Event raised when the controller input changes directions
        /// </summary>
        public event EventHandler<ControllerVector> DirectionChanged;

        #endregion

        #region Private

        /// <summary>
        /// Handle to the actual controller HidDevice
        /// </summary>
        private HidDevice deviceHandle { get; set; }

        /// <summary>
        /// Handler for processing/filtering input from the controller
        /// </summary>
        /// <param name="sender">HidDevice handle to the controller</param>
        /// <param name="args">InputReport received from the controller</param>
        private void InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            var dPad = (int) args.Report.GetNumericControl(0x01, 0x39).Value;

            var newVector = new ControllerVector()
            {
                Direction = (ControllerDirection) dPad,
                Magnitude = 10000
            };

            // DPad has priority over thumb stick, only bother with thumb stick 
            // values if DPad is not providing a value.
            if (newVector.Direction == ControllerDirection.None)
            {
                // If direction is None, magnitude should be 0
                newVector.Magnitude = 0;

                // Adjust X/Y so (0,0) is neutral position
                double stickX = args.Report.GetNumericControl(0x01, 0x30).Value - 32768;
                double stickY = args.Report.GetNumericControl(0x01, 0x31).Value - 32768;

                int stickMagnitude = (int) GetMagnitude(stickX, stickY);

                // Only process if the stick is outside the dead zone
                if (stickMagnitude > 0)
                {
                    newVector.Direction = CoordinatesToDirection(stickX, stickY);
                    newVector.Magnitude = stickMagnitude;
                    if (maxMagnitude[newVector.Direction] < newVector.Magnitude)
                    {
                        maxMagnitude[newVector.Direction] = newVector.Magnitude;
                    }
                }
            }

            // Only fire an event if the vector changed
            if (!this.DirectionVector.Equals(newVector))
            {
                if (null != this.DirectionChanged)
                {
                    this.DirectionVector = newVector;
                    this.DirectionChanged(this, this.DirectionVector);
                }
            }
        }

        /// <summary>
        /// Gets the magnitude of the vector formed by the X/Y coordinates
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        /// <returns>True if the coordinates are inside the dead zone</returns>
        private static double GetMagnitude(double x, double y)
        {
            var magnitude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            if (magnitude < DeadzoneTolerance)
            {
                magnitude = 0;
            }
            else
            {
                // Scale so deadzone is removed, and max value is 10000
                magnitude = ((magnitude - DeadzoneTolerance)/(32768 - DeadzoneTolerance))*10000;
                if (magnitude > 10000)
                {
                    magnitude = 10000;
                }
            }

            return magnitude;
        }

        /// <summary>
        /// Converts thumbstick X/Y coordinates centered at (0,0) to a direction
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        /// <returns>Direction that the coordinates resolve to</returns>
        private static ControllerDirection CoordinatesToDirection(double x, double y)
        {
            var radians = Math.Atan2(y, x);
            var orientation = (radians*(180/Math.PI));

            orientation = orientation
                          + 180 // adjust so values are 0-360 rather than -180 to 180
                          + 22.5 // offset so the middle of each direction has a +/- 22.5 buffer
                          + 270; // adjust so when dividing by 45, up is 1

            // Wrap around so that the value is 0-360
            orientation = orientation%360;

            // Dividing by 45 should chop the orientation into 8 chunks, which 
            // maps 0 to Up.  Shift that by 1 since we need 1-8.
            var direction = (int) ((orientation/45)) + 1;

            return (ControllerDirection) direction;
        }

        #endregion
    }
}