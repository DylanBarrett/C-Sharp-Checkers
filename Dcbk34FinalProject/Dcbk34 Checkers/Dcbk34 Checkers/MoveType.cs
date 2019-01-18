using System.Collections.Generic;
using System.Windows;

namespace Dcbk34_Checkers
{
    public class MustJump
    {
        public List<GamePiece> RemovePieces = new List<GamePiece>();
        public GamePiece MoveGamePiece;
        public Point MoveLocation;

        public MustJump(Point location, GamePiece gamepiece, GamePiece remove)
        {
            MoveLocation = location;
            MoveGamePiece = gamepiece;
            RemovePieces.Add(remove);
        }
    }
}
