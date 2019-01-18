using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dcbk34_Checkers
{
    public class BoardTile : BoardObject
    {
        public BoardTile(Point location)
        {
            Location = location;
            Shape = new Rectangle
            {
                Width = 100,
                Height = 100,
                Fill = new SolidColorBrush
                {
                    Color = (location.X + location.Y) % 2 == 0 ? Color.FromRgb(0, 0, 0) : Color.FromRgb(0, 204, 204)
                },
                StrokeThickness = 4
            };
        }
    }
}
