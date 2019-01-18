using System.Windows;
using System.Windows.Shapes;

namespace Dcbk34_Checkers
{
    public class BoardObject
    {
        private Shape shape;
        private Point location;

        public Shape Shape
        {
            get => shape;
            set => shape = value;
        }
        public Point Location
        {
            get => location;
            set => location = value;
        }
    }
}
