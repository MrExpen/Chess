using System;
using ChessLib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;


namespace ChessSFML
{
    internal class Program
    {
        private const int _CELL_LENGTH = 100;
        private static readonly RenderWindow _window;
        private static readonly RenderTexture _chessBoardTexture;
        private static readonly CircleShape _canMuveTo;
        private static readonly Sprite _chessBoardSprite;
        private static ChessUI _chess;
        static Program()
        {
            _window = new RenderWindow(new VideoMode(800, 800), "Chess by MrExpen", Styles.Close | Styles.Titlebar);
            _window.Closed += (s, e) => _window.Close();
            _window.MouseButtonReleased += _window_MouseButtonReleased;

            RectangleShape White = new RectangleShape(new Vector2f(_CELL_LENGTH, _CELL_LENGTH))
            {
                FillColor = new SFML.Graphics.Color(238, 238, 210)
            };
            RectangleShape Black = new RectangleShape(new Vector2f(_CELL_LENGTH, _CELL_LENGTH))
            {
                FillColor = new SFML.Graphics.Color(118, 150, 86)
            };
            _canMuveTo = new CircleShape(15);
            _canMuveTo.Origin = new Vector2f(_canMuveTo.Radius, _canMuveTo.Radius);
            _canMuveTo.FillColor = new SFML.Graphics.Color(120, 100, 100, 150);


            _chessBoardTexture = new RenderTexture(_window.Size.X, _window.Size.Y);
            _chessBoardTexture.Clear(SFML.Graphics.Color.White);
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Vector2f Position = new Vector2f(_CELL_LENGTH * x, _CELL_LENGTH * y);
                    if ((x + y) % 2 == 0)
                    {
                        White.Position = Position;
                        _chessBoardTexture.Draw(White);
                    }
                    else
                    {
                        Black.Position = Position;
                        _chessBoardTexture.Draw(Black);
                    }
                    
                }
            }
            _chessBoardTexture.Display();
            _chessBoardSprite = new Sprite(_chessBoardTexture.Texture);

            _chess = new ChessUI(new SkinProvider(@".\Resouces\ChessPiecesArray.png"));
        }

        static void Main(string[] args)
        {

            while (_window.IsOpen)
            {
                _window.DispatchEvents();


                _window.Clear(SFML.Graphics.Color.White);
                _window.Draw(_chessBoardSprite);
                foreach (var Figure in _chess.Board.Figures)
                {
                    if (Figure is null)
                    {
                        continue;
                    }
                    _chess.SkinProvider.Sprites[(Figure.EnumFigure, Figure.Color)].Position = new Vector2f(_CELL_LENGTH * Figure.Position.X + _CELL_LENGTH / 2, _CELL_LENGTH * 7 - _CELL_LENGTH * Figure.Position.Y + _CELL_LENGTH / 2);
                    _window.Draw(_chess.SkinProvider.Sprites[(Figure.EnumFigure, Figure.Color)]);
                }
                foreach (var position in _chess.Moves)
                {
                    _canMuveTo.Position = new Vector2f(position.X * _CELL_LENGTH + _CELL_LENGTH / 2, _CELL_LENGTH * 7 - _CELL_LENGTH * position.Y + _CELL_LENGTH / 2);
                    _window.Draw(_canMuveTo);
                }
                _window.Display();
            }
        }

        private static void _window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Right)
            {
                _chess.Selected = null;
            }
            else if (e.Button == Mouse.Button.Left)
            {
                if (!_chess.Selected.HasValue)
                {
                    _chess.Selected = new ChessPosition(e.X / _CELL_LENGTH, 7 - e.Y / _CELL_LENGTH);
                }
                else
                {
                    try
                    {
                        var pos = new ChessPosition(e.X / _CELL_LENGTH, 7 - e.Y / _CELL_LENGTH);
                        if (_chess.Board.Figures[pos.X, pos.Y] is not null && _chess.Board.Figures[pos.X, pos.Y].Color == _chess.Turn)
                        {
                            _chess.Selected = pos;
                        }
                        else
                        {
                            _chess.Move(_chess.Selected.Value, pos);
                            _chess.Selected = null;
                        }

                    }
                    catch (ArgumentException) { _chess.Selected = null; }
                }
            }
        }
    }
}
