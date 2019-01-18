using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dcbk34_Checkers
{
    class CheckerBoard
    {
        private List<BoardTile> squares = new List<BoardTile>();
        private List<GamePiece> checkers = new List<GamePiece>();
        private List<MustJump> forcedJumps = new List<MustJump>();

        public PlayerType Player1 = PlayerType.Player2;

        private Canvas playarea;
        private BoardTile hoverBoardTile;
        private GamePiece draggable;

        public List<BoardTile> Squares
        {
            get => squares;
            set => squares = value;
        }
        public List<GamePiece> GamePieces
        {
            get => checkers;
            set => checkers = value;
        }
        //creating the checkerboard
        public CheckerBoard(Canvas playarea)
        {
            this.playarea = playarea;
            GenerateCheckerBoardTiles();
            squares.ForEach(delegate (BoardTile square)
            {
                Canvas.SetTop(square.Shape, square.Location.Y * 100);
                Canvas.SetLeft(square.Shape, square.Location.X * 100);
                Canvas.SetZIndex(square.Shape, 0);
                playarea.Children.Add(square.Shape);
            });
            //creating the checker pieces
            GenerateCheckerPieces();
            checkers.ForEach(delegate (GamePiece gamepiece)
            {
                Canvas.SetTop(gamepiece.Shape, (gamepiece.Location.Y * 100) + gamepiece.Shape.Height / 3 / 2);
                Canvas.SetLeft(gamepiece.Shape, (gamepiece.Location.X * 100) + gamepiece.Shape.Width / 3 / 2);
                Canvas.SetZIndex(gamepiece.Shape, 1);
                playarea.Children.Add(gamepiece.Shape);
            });
            System.Windows.Application.Current.MainWindow.MouseMove += MainWindowCursorMoved;
            System.Windows.Application.Current.MainWindow.MouseUp += MainWindowMouseUnclicked;
        }

        //making the game board 8x8
        private void GenerateCheckerBoardTiles()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    squares.Add(new BoardTile(new Point(x, y)));
                }
            }
        }

        //generating 3 rows and 8 columns worth of checkers for the black pieces
        private void GenerateCheckerPieces()
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (((x + 1) + y) % 2 == 0)
                    {
                        GamePiece newChecker = new GamePiece(PlayerType.Player1, new Point(x, y));
                        newChecker.Shape.MouseDown += ShapeMouseClicked;
                        checkers.Add(newChecker);
                    }
                }
            }
            //generating 3 rows and 8 columns worth of checkers for the yellow pieces
            for (int y = 7; y > 4; y--)
            {
                for (int x = 7; x >= 0; x--)
                {
                    if (((x + 1) + y) % 2 == 0)
                    {
                        GamePiece newChecker = new GamePiece(PlayerType.Player2, new Point(x, y));
                        newChecker.Shape.MouseDown += ShapeMouseClicked;
                        checkers.Add(newChecker);
                    }
                }
            }
        }

        //allows the checker to be picked up
        private void ShapeMouseClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GamePiece checker = checkers.Find(p => p.Shape.Equals((Shape)sender));
            if (checker.PlayerPiece == Player1)
            {
                draggable = checker;
                Canvas.SetZIndex(draggable.Shape, 2);
            }
        }

        //highlights the area the checker is over with a yellow border
        //if the checker can't be placed in the spot where you are hovering
        //return it to its original postion
        private void MainWindowCursorMoved(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (draggable != null)
            {
                Point mousePoint = Mouse.GetPosition(playarea);
                Point mouseLocation = new Point((mousePoint.X - mousePoint.X % 100) / 100, (mousePoint.Y - mousePoint.Y % 100) / 100);

                if (hoverBoardTile != null)
                {
                    hoverBoardTile.Shape.Stroke = null;
                }
                hoverBoardTile = squares.Find(tile => tile.Location.Equals(mouseLocation));
                if (hoverBoardTile != null)
                {
                    hoverBoardTile.Shape.Stroke = new SolidColorBrush
                    {
                        Color = Color.FromRgb(255, 255, 0)
                    };
                }

                Canvas.SetTop(draggable.Shape, mousePoint.Y - draggable.Shape.ActualHeight / 2);
                Canvas.SetLeft(draggable.Shape, mousePoint.X - draggable.Shape.ActualWidth / 2);
            }
        }

        //allows the checker piece to be placed down in a new spot
        //and removing the checker from it previous space
        private void MainWindowMouseUnclicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (draggable != null)
            {
                bool nextTurn = false;
                Point mousePoint = Mouse.GetPosition(playarea);
                Point newLocation = new Point((mousePoint.X - mousePoint.X % 100) / 100, (mousePoint.Y - mousePoint.Y % 100) / 100);

                //checking to see if the new location is within the boundaries and rules
                if (newLocation.X >= 0 && newLocation.X < 8 && newLocation.Y >= 0 && newLocation.Y < 8)
                {
                    if (forcedJumps.Count > 0)
                    {
                        forcedJumps.ForEach(fj =>
                        {
                            if (newLocation.Equals(fj.MoveLocation) && draggable.Equals(fj.MoveGamePiece))
                            {
                                draggable.Location = newLocation;
                                nextTurn = true;
                                fj.RemovePieces.ForEach(checkerpiece =>
                                {
                                    playarea.Children.Remove(checkerpiece.Shape);
                                    checkers.Remove(checkerpiece);
                                });
                            }
                        });
                    }
                    else if (CheckMoveAllowed(newLocation, draggable.Location))
                    {
                        draggable.Location = newLocation;
                        nextTurn = true;
                    }
                }
                Canvas.SetTop(draggable.Shape, (draggable.Location.Y * 100) + draggable.Shape.Height / 3 / 2);
                Canvas.SetLeft(draggable.Shape, (draggable.Location.X * 100) + draggable.Shape.Width / 3 / 2);
                Canvas.SetZIndex(draggable.Shape, 1);
                draggable = null;
                if (hoverBoardTile != null)
                {
                    hoverBoardTile.Shape.Stroke = null;
                    hoverBoardTile = null;
                }
                if (nextTurn)
                {
                    NextTurn();
                }
            }
        }

        public void NextTurn()
        {
            Player1 = Player1 == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
            CheckMustJumps();
            squares.ForEach(square => square.Shape.Stroke = null);

            forcedJumps.ForEach(f =>
            {
                BoardTile square = squares.Find(s => s.Location.Equals(f.MoveLocation));
                if (square != null)
                {
                    square.Shape.Stroke = new SolidColorBrush
                    {
                        Color = Color.FromRgb(0, 255, 0)
                    };
                }
            });
        }

        public void CheckMustJumps()
        {
            forcedJumps.Clear();
            checkers.ForEach(checkerpiece =>
            {
                if (checkerpiece.PlayerPiece == Player1)
                {
                    Coordinates.COORDINATES.ForEach(delegate (Point coordinate)
                    {
                        Point directed_point = new Point(checkerpiece.Location.X + coordinate.X, checkerpiece.Location.Y + coordinate.Y);
                        GamePiece jumpable = checkers.Find(other => other.PlayerPiece != Player1 && other.Location.Equals(directed_point));
                        if (jumpable != null)
                        {
                            CheckJumpLanding(checkerpiece, coordinate, jumpable);
                        }
                    });
                }
            });
        }

        public bool CheckMoveAllowed(Point newLocation, Point prevLocation)
        {
            Point distance = new Point(newLocation.X - prevLocation.X, newLocation.Y - prevLocation.Y);

            // checking to see if the new location will be 
            //within the gameboard
            if ((newLocation.X + newLocation.Y) % 2 == 0)
            {
                return false;
            }
            //checking to see if the new move is backwards which isn't allowed
            //and returns the piece to its original position
            if ((distance.Y < 0 && Player1 == PlayerType.Player1) || (distance.Y > 0 && Player1 == PlayerType.Player2))
            {
                return false;
            }
            // checking to see if other pieces are in the same space that 
            //you are moving the current piece to.
            if (checkers.Find(piece => piece.Location.Equals(newLocation)) != null)
            {
                return false;
            }
            // checking to see if new move poistion is within the required
            // move distance of 1 space diagnally of the old space
            if (distance.X > 1 || distance.X < -1 || distance.Y > 1 || distance.Y < -1)
            {
                return false;
            }
            return true;
        }

        public static class Coordinates
        {
            public static Point N = new Point(0, 1);
            public static Point NE = new Point(1, 1);
            public static Point E = new Point(1, 0);
            public static Point SE = new Point(1, -1);
            public static Point S = new Point(0, -1);
            public static Point SW = new Point(-1, -1);
            public static Point W = new Point(-1, 0);
            public static Point NW = new Point(-1, 1);

            public static List<Point> COORDINATES = new List<Point>(new Point[] { N, NE, E, SE, S, SW, W, NW });

            public static string[] LetterPositions = { "A", "B", "C", "D", "E", "F", "G", "H" };
        }

        public void CheckJumpLanding(GamePiece gamepiece, Point coordinate, GamePiece jumpable, MustJump forcedJump = null)
        {
            Point jumpLanding = new Point(jumpable.Location.X + coordinate.X, jumpable.Location.Y + coordinate.Y);
            if (checkers.Find(other => other.Location.Equals(jumpLanding)) == null && jumpLanding.X >= 0 && jumpLanding.X < 8 && jumpLanding.Y >= 0 && jumpLanding.Y < 8)
            {
                Point difference = new Point(jumpLanding.X - gamepiece.Location.X, jumpLanding.Y - gamepiece.Location.Y);
                if ((Player1 == PlayerType.Player2 && difference.Y < 0) || (Player1 == PlayerType.Player1 && difference.Y > 0))
                {
                    if (forcedJump == null)
                    {
                        forcedJump = new MustJump(jumpLanding, gamepiece, jumpable);
                    }
                    else
                    {
                        forcedJump.MoveLocation = jumpLanding;
                        forcedJump.RemovePieces.Add(jumpable);
                    }
                    Point directed_point = new Point(jumpLanding.X + coordinate.X, jumpLanding.Y + coordinate.Y);
                    GamePiece nextJumpLanding = checkers.Find(other => other.PlayerPiece != Player1 && other.Location.Equals(directed_point));
                    if (nextJumpLanding != null)
                    {
                        CheckJumpLanding(gamepiece, coordinate, nextJumpLanding, forcedJump);
                    }
                    else
                    {
                        forcedJumps.Add(forcedJump);
                    }
                }
            }
            else
            {
                if (forcedJump != null)
                {
                    forcedJumps.Add(forcedJump);
                }
            }
        }

    }
}