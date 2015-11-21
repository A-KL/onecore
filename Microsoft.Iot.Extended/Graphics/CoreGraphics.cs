namespace Microsoft.Iot.Extended.Graphics
{
    using System;

    using Windows.Foundation;
    using Windows.UI;

    public class CoreGraphics : IGraphics
    {
        private readonly Size size;
        private readonly Rect canvas;

        public CoreGraphics(Size size)
        {
            this.size = size;
            this.canvas = new Rect(new Point(0,0), this.size);
        }
        public Size Size { get; }

        public void DrawPixel(int x, int y, Color color)
        {
            
        }        

        public virtual void DrawLine(Point from, Point to, Color color)
        {
            if (!this.Canvas.Contains(from))
            {
                throw new ArgumentOutOfRangeException("from");
            }

            if (!this.Canvas.Contains(to))
            {
                throw new ArgumentOutOfRangeException("to");
            }

            throw new NotImplementedException("DrawLine not implemented use Lcd with hardware implementation");
        }

        public virtual void DrawRectangle(Point position, Size dimentions, Color fillColor, Color borderColor)
        {
            if (!this.Canvas.Contains(position))
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if (this.Canvas.Width < dimentions.Width + position.X)
            {
                throw new ArgumentOutOfRangeException("dimentions.X");
            }

            if (this.Canvas.Height < dimentions.Height + position.Y)
            {
                throw new ArgumentOutOfRangeException("dimentions.Y");
            }

            throw new NotImplementedException("DrawLine not implemented use Lcd with hardware implementation");
        }

        protected Rect Canvas
        {
            get
            {
                return this.canvas;
            }
        }
    }
}
