using System;
using ChessLib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Linq;
using ChessLib.Engines;
using ChessLib.Utils;
using ChessLib.Data;
using SFML.Audio;
using ChessLib.Figures;

namespace ChessSFML
{
    internal class Program
    {
        private const int _CELL_LENGTH = 100;
        private static readonly RenderWindow _window;
        private static readonly RenderTexture _chessBoardTexture;
        private static readonly CircleShape _canMuveTo;
        private static readonly SoundBuffer _hrumSound, _checkSound, _turnSound, _tieSound, _matSound;
        private static readonly Sound _sound;
        private static readonly Sprite _chessBoardSprite;
        private static readonly RectangleShape _buttonExit, _buttonLocalMatch, _buttonOnlineMatch, _buttonJoinMenu, _buttonCreateMenu, _buttonJoin, _buttonCreate;
        private static readonly RenderTexture _pawnChoiceMenuTexture, _pauseMenuTexture, _newOnlineMenuTexture, _createOnlineMenuTexture, _joinOnlineMenuTexture, _resultsMenuTexture;
        private static readonly Sprite _pawnChoiceMenuSprite, _pauseMenuSprite, _newOnlineMenuSprite, _createOnlineMenuSprite, _joinOnlineMenuSprite, _resultsMenuSprite;
        private static readonly Font _font = new Font(@".\Resouces\Roboto-Bold.ttf");
        private static Text _textName, _textMatchId, _textWhiteName, _textBlackName, _textResult;
        private static readonly string _url = "https://chess.mrexpen.ru:4432";
        private static Text SelectedText;
        private static SkinProvider _skinProvider;
        private static IChessEngine _chess;
        private static ISelectManager _selectManager;
        private static GameState _gameState;
        private static EnumFigure _choice;

        static Program()
        {
            #region WindowInit
            _window = new RenderWindow(new VideoMode(800, 800), "Chess by MrExpen", Styles.Close | Styles.Titlebar);
            _window.Closed += (s, e) =>
            {
                if (_chess is IDisposable disposable)
                {
                    disposable?.Dispose();
                }
                _window.Close();
            };
            _window.SetVerticalSyncEnabled(true);
            _window.MouseButtonReleased += _window_MouseButtonReleased;
            _window.KeyPressed += _window_KeyPressed;
            _window.TextEntered += _window_TextEntered;
            #endregion

            #region Sounds
            _sound = new Sound();
            _hrumSound = new SoundBuffer(@".\Resouces\hrum.ogg");
            _checkSound = new SoundBuffer(@".\Resouces\check.ogg");
            _turnSound = new SoundBuffer(@".\Resouces\turn.ogg");
            _tieSound = new SoundBuffer(@".\Resouces\tie.ogg");
            _matSound = new SoundBuffer(@".\Resouces\mat.ogg");
            #endregion

            #region Board
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
            #endregion

            #region Skins
            _skinProvider = new SkinProvider(@".\Resouces\ChessPiecesArray.png");
            #endregion

            #region DefaultSettings
            Text text = new Text(string.Empty, _font, 30)
            {
                FillColor = SFML.Graphics.Color.Black
            };
            SFML.Graphics.Color ClearColor = SFML.Graphics.Color.White;
            SFML.Graphics.Color _buttonColor = new SFML.Graphics.Color(200, 200, 200);
            Vector2f ButtonSize = new Vector2f(_window.Size.X * 0.7f, _window.Size.Y * 0.1f);
            int Margin = 20;
            #endregion

            #region PawnChoice
            _pawnChoiceMenuTexture = new RenderTexture(_CELL_LENGTH * 5, _CELL_LENGTH * 2);
            _pawnChoiceMenuSprite = new Sprite(_pawnChoiceMenuTexture.Texture)
            {
                Origin = (Vector2f)_pawnChoiceMenuTexture.Size / 2f,
                Position = (Vector2f)_window.Size / 2f
            };
            #endregion

            #region Text
            _textName = new Text("Name", _font, 24) { FillColor = SFML.Graphics.Color.Black };
            _textName.Origin = new Vector2f(0, _textName.GetGlobalBounds().Height / 2f);

            _textMatchId = new Text("MatchId", _font, 24) { FillColor = SFML.Graphics.Color.Black };
            _textMatchId.Origin = new Vector2f(0, _textMatchId.GetGlobalBounds().Height / 2f);

            _textWhiteName = new Text("WhiteName", _font, 24) { FillColor = SFML.Graphics.Color.Black };
            _textWhiteName.Origin = new Vector2f(0, _textWhiteName.GetGlobalBounds().Height / 2f);

            _textBlackName = new Text("BlackName", _font, 24) { FillColor = SFML.Graphics.Color.Black };
            _textBlackName.Origin = new Vector2f(0, _textBlackName.GetGlobalBounds().Height / 2f);

            _textResult = new Text(string.Empty, _font, 24) { FillColor = SFML.Graphics.Color.Black, Position = (Vector2f)_window.Size / 2f };
            #endregion

            #region PauseMenu
            _pauseMenuTexture = new RenderTexture(_window.Size.X, _window.Size.Y);
            _pauseMenuTexture.Clear(ClearColor);

            _buttonLocalMatch = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonLocalMatch.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonLocalMatch.Position = ((Vector2f)_window.Size / 2f) - new Vector2f(0, ButtonSize.Y + Margin);

            _buttonOnlineMatch = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonOnlineMatch.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonOnlineMatch.Position = (Vector2f)_window.Size / 2f;

            _buttonExit = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonExit.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonExit.Position = ((Vector2f)_window.Size / 2f) + new Vector2f(0, ButtonSize.Y + Margin);

            _pauseMenuTexture.Draw(_buttonLocalMatch);
            _pauseMenuTexture.Draw(_buttonOnlineMatch);
            _pauseMenuTexture.Draw(_buttonExit);

            text.DisplayedString = "Новый локальный матч";
            var tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonLocalMatch.Position;
            _pauseMenuTexture.Draw(text);

            text.DisplayedString = "Играть онлайн";
            tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonOnlineMatch.Position;
            _pauseMenuTexture.Draw(text);

            text.DisplayedString = "Выйти";
            tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonExit.Position;
            _pauseMenuTexture.Draw(text);

            _pauseMenuTexture.Display();
            _pauseMenuSprite = new Sprite(_pauseMenuTexture.Texture);
            #endregion

            #region NewOnline

            _buttonJoinMenu = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonJoinMenu.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonJoinMenu.Position = (Vector2f)_window.Size / 2f - new Vector2f(0, (ButtonSize.Y + Margin) / 2f);

            _buttonCreateMenu = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonCreateMenu.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonCreateMenu.Position = (Vector2f)_window.Size / 2f + new Vector2f(0, (ButtonSize.Y + Margin) / 2f);

            _newOnlineMenuTexture = new RenderTexture(_window.Size.X, _window.Size.Y);
            _newOnlineMenuTexture.Clear(ClearColor);

            _newOnlineMenuTexture.Draw(_buttonJoinMenu);
            _newOnlineMenuTexture.Draw(_buttonCreateMenu);

            text.DisplayedString = "Присоединиться к матчу";
            tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonJoinMenu.Position;
            _newOnlineMenuTexture.Draw(text);

            text.DisplayedString = "Создать новый матч";
            tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonCreateMenu.Position;
            _newOnlineMenuTexture.Draw(text);

            _newOnlineMenuTexture.Display();

            _newOnlineMenuSprite = new Sprite(_newOnlineMenuTexture.Texture);
            #endregion

            #region Join
            _joinOnlineMenuTexture = new RenderTexture(_window.Size.X, _window.Size.Y);
            _joinOnlineMenuTexture.Clear(ClearColor);

            _buttonJoin = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonJoin.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonJoin.Position = (Vector2f)_window.Size / 2f + new Vector2f(0, ButtonSize.Y + Margin);

            _textName.Position = (Vector2f)_window.Size / 2f - new Vector2f(0, ButtonSize.Y + Margin);

            _textMatchId.Position = (Vector2f)_window.Size / 2f;

            _joinOnlineMenuTexture.Draw(_buttonJoin);

            text.DisplayedString = "Присоединиться";
            tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonJoin.Position;
            _joinOnlineMenuTexture.Draw(text);

            _joinOnlineMenuTexture.Display();
            _joinOnlineMenuSprite = new Sprite(_joinOnlineMenuTexture.Texture);

            #endregion

            #region Create
            _createOnlineMenuTexture = new RenderTexture(_window.Size.X, _window.Size.Y);
            _createOnlineMenuTexture.Clear(ClearColor);

            _buttonCreate = new RectangleShape(ButtonSize)
            {
                FillColor = _buttonColor
            };
            _buttonCreate.Origin = new Vector2f(ButtonSize.X / 2f, ButtonSize.Y / 2f);
            _buttonCreate.Position = (Vector2f)_window.Size / 2f + new Vector2f(0, ButtonSize.Y + Margin);

            _textWhiteName.Position = (Vector2f)_window.Size / 2f - new Vector2f(0, ButtonSize.Y + Margin);

            _textBlackName.Position = (Vector2f)_window.Size / 2f;

            _createOnlineMenuTexture.Draw(_buttonCreate);

            text.DisplayedString = "Создать";
            tgb = text.GetGlobalBounds();
            text.Origin = new Vector2f(tgb.Width / 2f, tgb.Height / 2f);
            text.Position = _buttonCreate.Position;
            _createOnlineMenuTexture.Draw(text);

            _createOnlineMenuTexture.Display();
            _createOnlineMenuSprite = new Sprite(_createOnlineMenuTexture.Texture);

            #endregion

            #region Result
            _resultsMenuTexture = new RenderTexture(_window.Size.X, _window.Size.Y);
            _resultsMenuTexture.Clear(ClearColor);
            _resultsMenuTexture.Display();
            _resultsMenuSprite = new Sprite(_resultsMenuTexture.Texture);
            #endregion

            #region ChessInit
            _choice = EnumFigure.None;
            _gameState = GameState.InPause;
            _chess = new LocalChessEngine();
            _chess.OnTurnChanged += _chess_OnTurnChanged;
            _selectManager = new SelectManager(_chess);
            #endregion
        }

        static void Main(string[] args)
        {

            while (_window.IsOpen)
            {
                _window.DispatchEvents();

                if (!_chess.InGame && _gameState == GameState.InGame)
                {
                    _gameState = GameState.ShowResults;
                    if (_chess.IsTie)
                    {
                        _textResult.DisplayedString = "Ничья.";
                    }
                    else
                    {
                        _textResult.DisplayedString = (_chess.Winner == ChessLib.Data.Color.White ? "Белый" : "Чёрный") + " цвет победил.";
                    }
                    var gb = _textResult.GetGlobalBounds();
                    _textResult.Origin = new Vector2f(gb.Width / 2f, gb.Height / 2f);
                }

                _window.Clear(SFML.Graphics.Color.White);
                switch (_gameState)
                {
                    case GameState.InPause:
                        {
                            _window.Draw(_pauseMenuSprite);
                        }
                        break;

                    case GameState.Spectate:
                    case GameState.InGame:
                        {
                            _window.Draw(_chessBoardSprite);
                            foreach (var Figure in _chess.Figures)
                            {
                                _skinProvider.Sprites[(Figure.EnumFigure, Figure.Color)].Position = new Vector2f(_CELL_LENGTH * Figure.Position.X + _CELL_LENGTH / 2, _CELL_LENGTH * 7 - _CELL_LENGTH * Figure.Position.Y + _CELL_LENGTH / 2);
                                _window.Draw(_skinProvider.Sprites[(Figure.EnumFigure, Figure.Color)]);
                            }
                            foreach (var position in _selectManager.MovesForSelected)
                            {
                                _canMuveTo.Position = new Vector2f(position.X * _CELL_LENGTH + _CELL_LENGTH / 2, _CELL_LENGTH * 7 - _CELL_LENGTH * position.Y + _CELL_LENGTH / 2);
                                _window.Draw(_canMuveTo);
                            }
                        }
                        break;

                    case GameState.NewOnline:
                        {
                            _window.Draw(_newOnlineMenuSprite);
                        }
                        break;

                    case GameState.JoinOnline:
                        {
                            _window.Draw(_joinOnlineMenuSprite);
                            _window.Draw(_textName);
                            _window.Draw(_textMatchId);
                        }
                        break;

                    case GameState.CreateOnline:
                        {
                            _window.Draw(_createOnlineMenuSprite);
                            _window.Draw(_textWhiteName);
                            _window.Draw(_textBlackName);
                        }
                        break;

                    case GameState.ShowResults:
                        {
                            _window.Draw(_resultsMenuSprite);
                            _window.Draw(_textResult);
                        }
                        break;
                }
                _window.Display();
            }
        }

        private static void _window_TextEntered(object sender, TextEventArgs e)
        {
            switch (_gameState)
            {
                case GameState.CreateOnline:
                    {
                        if (SelectedText == _textWhiteName || SelectedText == _textBlackName)
                        {
                            if (e.Unicode == "\b")
                            {
                                SelectedText.DisplayedString = SelectedText.DisplayedString.Length != 0 ? SelectedText.DisplayedString.Substring(0, SelectedText.DisplayedString.Length - 1) : string.Empty;
                            }
                            else
                            {
                                SelectedText.DisplayedString += string.Concat(e.Unicode.Where(x => char.IsLetterOrDigit(x)));
                            }
                        }
                    }
                    break;
                case GameState.JoinOnline:
                    {
                        if (SelectedText == _textName || SelectedText == _textMatchId)
                        {
                            if (e.Unicode == "\b")
                            {
                                SelectedText.DisplayedString = SelectedText.DisplayedString.Length != 0 ? SelectedText.DisplayedString.Substring(0, SelectedText.DisplayedString.Length - 1) : string.Empty;
                            }
                            else
                            {
                                SelectedText.DisplayedString += string.Concat(e.Unicode.Where(x => char.IsLetterOrDigit(x)));
                            }
                        }
                    }
                    break;
            }
        }
        private static void _window_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                switch (_gameState)
                {
                    case GameState.InPause:
                        _gameState = GameState.InGame;
                        break;

                    case GameState.Spectate:
                    case GameState.InGame:
                        _gameState = GameState.InPause;
                        break;

                    case GameState.CreateOnline:
                        _gameState = GameState.NewOnline;
                        break;

                    case GameState.JoinOnline:
                        _gameState = GameState.NewOnline;
                        break;

                    case GameState.NewOnline:
                        _gameState = GameState.InPause;
                        break;

                    case GameState.ShowResults:
                        _gameState = GameState.Spectate;
                        break;
                }
            }
        }
        private static void _window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            switch (_gameState)
            {
                case GameState.InPause:
                    {
                        if (_buttonLocalMatch.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            if (_chess is IDisposable disposable)
                            {
                                disposable?.Dispose();
                            }
                            _chess.OnTurnChanged -= _chess_OnTurnChanged;
                            _chess = new LocalChessEngine();
                            _chess.OnTurnChanged += _chess_OnTurnChanged;
                            _selectManager = new SelectManager(_chess);
                            _gameState = GameState.InGame;
                        }
                        else if (_buttonOnlineMatch.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            _gameState = GameState.NewOnline;
                        }
                        else if(_buttonExit.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            _window.Close();
                        }
                    }
                    break;

                case GameState.InGame:
                    {
                        if (e.Button == Mouse.Button.Right)
                        {
                            _selectManager.Selected = null;
                        }
                        else if (e.Button == Mouse.Button.Left)
                        {
                            if (!_selectManager.Selected.HasValue)
                            {
                                _selectManager.Selected = new ChessPosition(e.X / _CELL_LENGTH, 7 - e.Y / _CELL_LENGTH);
                            }
                            else
                            {
                                var pos = new ChessPosition(e.X / _CELL_LENGTH, 7 - e.Y / _CELL_LENGTH);
                                if (_selectManager.Selected.Value == pos)
                                {
                                    _selectManager.Selected = null;
                                }
                                else if (_chess.GetFigure(pos) is not null && _chess.GetFigure(pos).Color == _chess.Turn)
                                {
                                    _selectManager.Selected = pos;
                                }
                                else
                                {
                                    _chess.Move(_selectManager.Selected.Value, pos, Choice);
                                    _selectManager.Selected = null;
                                }
                            }
                        }
                    }
                    break;

                case GameState.NewOnline:
                    {
                        if (_buttonJoinMenu.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            _gameState = GameState.JoinOnline;
                        }
                        else if (_buttonCreateMenu.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            _gameState = GameState.CreateOnline;
                        }
                    }
                    break;

                case GameState.ShowResults:
                    break;

                case GameState.CreateOnline:
                    {
                        if (_buttonCreate.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            if (_chess is IDisposable disposable)
                            {
                                disposable?.Dispose();
                            }
                            var MatchId = HttpApi.CreateMatch(_url, _textWhiteName.DisplayedString, _textBlackName.DisplayedString);
                            _textMatchId.DisplayedString = MatchId.ToString();
                            _gameState = GameState.JoinOnline;
                        }
                        else if (_textWhiteName.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            SelectedText = _textWhiteName;
                        }
                        else if (_textBlackName.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            SelectedText = _textBlackName;
                        }
                    }
                    break;

                case GameState.JoinOnline:
                    {
                        if (_buttonJoin.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            if (_chess is IDisposable disposable)
                            {
                                disposable?.Dispose();
                            }
                            var chess = new SignalRChessEngine(_textName.DisplayedString, _url);

                            //TODO: checks
                            chess.JoinMatch(int.Parse(_textMatchId.DisplayedString));

                            _chess.OnTurnChanged -= _chess_OnTurnChanged;
                            _chess = chess;
                            _chess.OnTurnChanged += _chess_OnTurnChanged;
                            _selectManager = new SelectManager(_chess);
                            _gameState = GameState.InGame;
                        }
                        else if (_textName.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            SelectedText = _textName;
                        }
                        else if (_textMatchId.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            SelectedText = _textMatchId;
                        }
                    }
                    break;

                case GameState.InChoice:
                    int posx = (int)(e.X - _pawnChoiceMenuSprite.Position.X + _pawnChoiceMenuSprite.Origin.X) / _CELL_LENGTH;
                    int posy = (int)(e.Y - _pawnChoiceMenuSprite.Position.Y + _pawnChoiceMenuSprite.Origin.Y) / _CELL_LENGTH;
                    if (posy == 1)
                    {
                        switch (posx)
                        {
                            case 1:
                                _choice = EnumFigure.Queen;
                                break;
                            case 2:
                                _choice = EnumFigure.Rook;
                                break;
                            case 3:
                                _choice = EnumFigure.Bishop;
                                break;
                            case 4:
                                _choice = EnumFigure.Knight;
                                break;
                        }
                    }
                    break;
            }
        }

        private static void _chess_OnTurnChanged(object sender, TurnChangedEventArgs e)
        {
            if (_chess.IsTie)
            {
                _sound.SoundBuffer = _tieSound;
            }
            else if (e.IsChecked)
            {
                if (_chess.Winner != ChessLib.Data.Color.None)
                {
                    _sound.SoundBuffer = _matSound;
                }
                else
                {
                    _sound.SoundBuffer = _checkSound;
                }
            }
            else if (e.IsEat)
            {
                _sound.SoundBuffer = _hrumSound;
            }
            else
            {
                _sound.SoundBuffer = _turnSound;
            }
            _sound.Play();
        }

        private static EnumFigure Choice(ChessLib.Data.Color color)
        {
            _gameState = GameState.InChoice;
            while (_window.IsOpen)
            {
                _window.DispatchEvents();

                if (_choice != EnumFigure.None)
                {
                    var result = _choice;
                    _choice = EnumFigure.None;
                    _gameState = GameState.InGame;
                    return result;
                }

                _window.Draw(_chessBoardSprite);
                foreach (var Figure in _chess.Figures)
                {
                    _skinProvider.Sprites[(Figure.EnumFigure, Figure.Color)].Position = new Vector2f(_CELL_LENGTH * Figure.Position.X + _CELL_LENGTH / 2, _CELL_LENGTH * 7 - _CELL_LENGTH * Figure.Position.Y + _CELL_LENGTH / 2);
                    _window.Draw(_skinProvider.Sprites[(Figure.EnumFigure, Figure.Color)]);
                }
                foreach (var position in _selectManager.MovesForSelected)
                {
                    _canMuveTo.Position = new Vector2f(position.X * _CELL_LENGTH + _CELL_LENGTH / 2, _CELL_LENGTH * 7 - _CELL_LENGTH * position.Y + _CELL_LENGTH / 2);
                    _window.Draw(_canMuveTo);
                }

                _pawnChoiceMenuTexture.Clear(new SFML.Graphics.Color(200, 200, 200, 200));

                _skinProvider.Sprites[(EnumFigure.Queen, color)].Position = new Vector2f(_CELL_LENGTH * 1, _CELL_LENGTH * 1);
                _skinProvider.Sprites[(EnumFigure.Rook, color)].Position = new Vector2f(_CELL_LENGTH * 2, _CELL_LENGTH * 1);
                _skinProvider.Sprites[(EnumFigure.Bishop, color)].Position = new Vector2f(_CELL_LENGTH * 3, _CELL_LENGTH * 1);
                _skinProvider.Sprites[(EnumFigure.Knight, color)].Position = new Vector2f(_CELL_LENGTH * 4, _CELL_LENGTH * 1);

                _pawnChoiceMenuTexture.Draw(_skinProvider.Sprites[(EnumFigure.Queen, color)]);
                _pawnChoiceMenuTexture.Draw(_skinProvider.Sprites[(EnumFigure.Rook, color)]);
                _pawnChoiceMenuTexture.Draw(_skinProvider.Sprites[(EnumFigure.Bishop, color)]);
                _pawnChoiceMenuTexture.Draw(_skinProvider.Sprites[(EnumFigure.Knight, color)]);

                _pawnChoiceMenuTexture.Display();

                _window.Draw(_pawnChoiceMenuSprite);

                _window.Display();
            }
            _window.Close();
            throw new Exception();
        }
    }
}
