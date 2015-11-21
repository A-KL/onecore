namespace Microsoft.Iot.Extended.Lcd
{
    using System;
    using System.Threading.Tasks;

    using Windows.Foundation;
    using Windows.UI;

    using Microsoft.Iot.Extended.Graphics;

    enum DisplayMode
    {
        //reset the above effect and turn the data to ON at the corresponding gray level.
        NormalDisplay = 0xA4,
        //forces the entire display to be at "GS63"
        DisplayOn = 0xA5,
        //forces the entire display to be at gray level "GS0"
        DisplayOff = 0xA6,
        //swap the gray level of display data
        InverseDisplay = 0xA7
    };

    enum DisplayPower
    {
        DimMode = 0xAC,
        SleepMode = 0xAE,
        NormalMode = 0xAF
    };

    enum ScollingDirection
    {
        Horizontal = 0x00,
        Vertical = 0x01,
        Diagonal = 0x02
    };

    public class SSD1331 : CoreGraphics, IScreen
    {
        public enum Command : byte
        {
            CMD_DRAW_LINE = 0x21,
            CMD_DRAW_RECTANGLE = 0x22,
            CMD_COPY_WINDOW = 0x23,
            CMD_DIM_WINDOW = 0x24,
            CMD_CLEAR_WINDOW = 0x25,
            CMD_FILL_WINDOW = 0x26,
            DISABLE_FILL = 0x00,
            ENABLE_FILL = 0x01,
            CMD_CONTINUOUS_SCROLLING_SETUP = 0x27,
            CMD_DEACTIVE_SCROLLING = 0x2E,
            CMD_ACTIVE_SCROLLING = 0x2F,

            CMD_SET_COLUMN_ADDRESS = 0x15,
            CMD_SET_ROW_ADDRESS = 0x75,
            CMD_SET_CONTRAST_A = 0x81,
            CMD_SET_CONTRAST_B = 0x82,
            CMD_SET_CONTRAST_C = 0x83,
            CMD_MASTER_CURRENT_CONTROL = 0x87,
            CMD_SET_PRECHARGE_SPEED_A = 0x8A,
            CMD_SET_PRECHARGE_SPEED_B = 0x8B,
            CMD_SET_PRECHARGE_SPEED_C = 0x8C,
            CMD_SET_REMAP = 0xA0,
            CMD_SET_DISPLAY_START_LINE = 0xA1,
            CMD_SET_DISPLAY_OFFSET = 0xA2,
            CMD_NORMAL_DISPLAY = 0xA4,
            CMD_ENTIRE_DISPLAY_ON = 0xA5,
            CMD_ENTIRE_DISPLAY_OFF = 0xA6,
            CMD_INVERSE_DISPLAY = 0xA7,
            CMD_SET_MULTIPLEX_RATIO = 0xA8,
            CMD_DIM_MODE_SETTING = 0xAB,
            CMD_SET_MASTER_CONFIGURE = 0xAD,
            CMD_DIM_MODE_DISPLAY_ON = 0xAC,

            CMD_DISPLAY_OFF = 0xAE,
            CMD_NORMAL_BRIGHTNESS_DISPLAY_ON = 0xAF,
            CMD_POWER_SAVE_MODE = 0xB0,
            CMD_PHASE_PERIOD_ADJUSTMENT = 0xB1,
            CMD_DISPLAY_CLOCK_DIV = 0xB3,
            CMD_SET_GRAy_SCALE_TABLE = 0xB8,
            CMD_ENABLE_LINEAR_GRAY_SCALE_TABLE = 0xB9,
            CMD_SET_PRECHARGE_VOLTAGE = 0xBB,
            CMD_SET_V_VOLTAGE = 0xBE
        }        

        private readonly byte[] InitializeCommands =
                        {
                            (byte)Command.CMD_DISPLAY_OFF,
                            (byte)Command.CMD_SET_CONTRAST_A, 0x91,
                            (byte)Command.CMD_SET_CONTRAST_B, 0x50,
                            (byte)Command.CMD_SET_CONTRAST_C, 0x7D,

                            (byte)Command.CMD_MASTER_CURRENT_CONTROL, 0x06,

                            (byte)Command.CMD_SET_PRECHARGE_SPEED_A, 0x64,
                            (byte)Command.CMD_SET_PRECHARGE_SPEED_B, 0x78,
                            (byte)Command.CMD_SET_PRECHARGE_SPEED_C, 0x64,

                            (byte)Command.CMD_SET_REMAP, 0x76,

                            (byte)Command.CMD_SET_DISPLAY_START_LINE, 0x00,

                            (byte)Command.CMD_SET_DISPLAY_OFFSET, 0x00,

                            (byte)Command.CMD_NORMAL_DISPLAY,

                            (byte)Command.CMD_SET_MULTIPLEX_RATIO, 0x3F,

                            (byte)Command.CMD_SET_MASTER_CONFIGURE, 0x8E,

                            (byte)Command.CMD_POWER_SAVE_MODE, 0x0B,

                            (byte)Command.CMD_PHASE_PERIOD_ADJUSTMENT, 0x31,

                            (byte)Command.CMD_DISPLAY_CLOCK_DIV, 0xF0,

                            (byte)Command.CMD_SET_PRECHARGE_VOLTAGE, 0x3A,

                            (byte)Command.CMD_SET_V_VOLTAGE, 0x3E,

                           // (byte)Command.CMD_DEACTIVE_SCROLLING,

                            (byte)Command.CMD_NORMAL_BRIGHTNESS_DISPLAY_ON
                        };

        public SSD1331(IScreenBus connection) 
            : base(new Size(96, 64))
        {
            this.Connection = connection;
        }        
            
        public async Task Initialize()
        {
            await this.Reset();

            this.Connection.SendCommands(this.InitializeCommands);                      
        }

        public Task Reset()
        {
            return this.Connection.Reset();
        }

        public override void DrawLine(Point from, Point to, Color color)
        {
            if (!this.Canvas.Contains(from))
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            if (!this.Canvas.Contains(to))
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }

            this.Connection.SendCommand((byte)Command.CMD_DRAW_LINE);
            this.SendPoint(from);
            this.SendPoint(to);
            this.SendColorCommand(color);
        }

        public override void DrawRectangle(Point position, Size dimentions, Color fillColor, Color borderColor)
        {
            if (!this.Canvas.Contains(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if (this.Canvas.Width < dimentions.Width + position.X)
            {
                throw new ArgumentOutOfRangeException("dimentions.X");
            }

            if (this.Canvas.Height < dimentions.Height + position.Y)
            {
                throw new ArgumentOutOfRangeException("dimentions.Y");
            }

            this.Connection.SendCommand((byte)Command.CMD_FILL_WINDOW);
            this.Connection.SendCommand((byte)Command.ENABLE_FILL);
            this.Connection.SendCommand((byte)Command.CMD_DRAW_RECTANGLE);
            this.SendPoint(position);

            var endPoint = new Point(position.X + dimentions.Width, position.Y + dimentions.Height);
            this.SendPoint(endPoint);

            this.SendColorCommand(borderColor);
            this.SendColorCommand(fillColor);
        }

        public void DrawPoint(Point position, Color color)
        {
            this.GoTo(position);
            this.SendColorData(color);
        }

        public void DrawBitmap(Point position, Size size, byte[] data)
        {
            var expected = (int)(size.Width * size.Height * 2); // RGB565

            if (expected != data.Length)
            {
                throw new ArgumentException(nameof(data));
            }

            this.GoTo(position);
            this.SendColorData(data);
        }

        public void Clear()
        {
            this.Clear(this.Canvas);
        }

        private void Clear(Rect window)
        {
            this.Connection.SendCommand((byte)Command.CMD_CLEAR_WINDOW);
            this.Connection.SendCommand((byte)window.X);
            this.Connection.SendCommand((byte)window.Y);
            this.Connection.SendCommand((byte)window.Width);
            this.Connection.SendCommand((byte)window.Height);
        }

        protected IScreenBus Connection { get; }

        private void SendPoint(Point point)
        {
            this.Connection.SendCommand((byte)point.X);
            this.Connection.SendCommand((byte)point.Y);
        }

        private void SendColorCommand(Color color)
        {
            this.Connection.SendCommand(color.R);
            this.Connection.SendCommand(color.G);
            this.Connection.SendCommand(color.B);
        }

        private void SendColorData(Color color)
        {
            this.Connection.SendData(color.ToRgb565());
        }

        private void SendColorData(byte[] data)
        {
            var colorData = new byte[2];
            
            for (var i = 0; i < data.Length; i += 2)
            {
                colorData[0] = data[i];
                colorData[1] = data[i+1];

                this.Connection.SendData(colorData);
            }            
        }

        private void GoTo(Point position)
        {
            if (!this.Canvas.Contains(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            this.SetWindow(position, this.Size);
        }

        private void SetWindow(Point position, Size size)
        {
            this.Connection.SendCommand((byte)Command.CMD_SET_COLUMN_ADDRESS);
            this.Connection.SendCommand((byte)position.X);
            this.Connection.SendCommand((byte)(size.Width - 1));

            this.Connection.SendCommand((byte)Command.CMD_SET_ROW_ADDRESS);
            this.Connection.SendCommand((byte)position.Y);
            this.Connection.SendCommand((byte)(size.Height - 1));
        }
    }
}
