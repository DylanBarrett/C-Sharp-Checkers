using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dcbk34_Checkers
{
    public class GamePiece : BoardObject
    {
        private PlayerType playerPiece;
        private PieceType piecetype;

        public PlayerType PlayerPiece
        {
            get => playerPiece;
            set => playerPiece = value;
        }
        public PieceType Type
        {
            get => piecetype;
            set => piecetype = value;
        }

        public GamePiece(PlayerType playerPiece, Point location)
        {
            this.playerPiece = playerPiece;
            Location = location;
            piecetype = PieceType.Pawn;
            Shape = new Ellipse
            {
                Width = 75,
                Height = 75,
                Fill = new SolidColorBrush
                {
                    Color = playerPiece == PlayerType.Player1 ? Color.FromRgb(255, 255, 0) : Color.FromRgb(0, 0, 0)
                },
                Stroke = new SolidColorBrush
                {
                    Color = Color.FromRgb(255, 255, 255)
                },
                StrokeThickness = 5
            };
        }
    }
}