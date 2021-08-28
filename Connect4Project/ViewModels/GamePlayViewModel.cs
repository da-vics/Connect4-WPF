using Caliburn.Micro;
using Connect4Project.Models;
using Connect4Project.SqlHandler;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Connect4Project.ViewModels
{

    #region CustomStruct
    public struct Field
    {
        public int rol;
        public int col;

        public Field(int r, int c)
        {
            rol = r;
            col = c;
        }
    }
    #endregion

    public class GamePlayViewModel : Screen
    {

        const int winnerPoints = 100;  // player score on win

        private const int MaxRow = 6;
        private const int MaxColumn = 7;
        private char[,] _gameMap = new char[MaxRow, MaxColumn] {
                                                     {' ', ' ', ' ', ' ',' ',' ',' ' },
                                                     {' ', ' ', ' ', ' ',' ',' ',' ' },
                                                     {' ', ' ', ' ', ' ',' ',' ',' ' },
                                                     {' ', ' ', ' ', ' ',' ',' ',' ' },
                                                     {' ', ' ', ' ', ' ',' ',' ',' ' },
                                                     {' ', ' ', ' ', ' ',' ',' ',' ' }
                                                   };

        private bool? _switchPlayers { get; set; }
        private bool _canPlay { get; set; } = true;

        private string _player1 { get; set; }
        public string Player1
        {
            get => _player1;
            set
            {
                _player1 = value;
            }
        }

        private string _player2 { get; set; }
        public string Player2
        {
            get => _player2;
            set
            {
                _player2 = value;
            }
        }

        private string _gameState { get; set; }
        public string GameState
        {
            get => _gameState;
            set
            {
                _gameState = value;
                NotifyOfPropertyChange();
            }
        }

        private Dictionary<string, int> _gameMapIndexDictionary;
        private SqlDataHandler sqlDataHandler;


        public GamePlayViewModel(GamePlayInput playInput)
        {
            Player1 = playInput.Player1;
            Player2 = playInput.Player2;

            _switchPlayers = true;
            _gameMapIndexDictionary = new Dictionary<string, int>();

            _gameMapIndexDictionary.Add("col1", 0);
            _gameMapIndexDictionary.Add("col2", 1);
            _gameMapIndexDictionary.Add("col3", 2);
            _gameMapIndexDictionary.Add("col4", 3);
            _gameMapIndexDictionary.Add("col5", 4);
            _gameMapIndexDictionary.Add("col6", 5);
            _gameMapIndexDictionary.Add("col7", 6);

            GameState = $"{Player1}'s Turn";
            _canPlay = true;

            sqlDataHandler = new SqlDataHandler();

        }

        public void RunOperation(object obj)
        {
            if (_canPlay == false)
                return;

            var selectedButton = (Button)obj;
            var stackContainer = (StackPanel)selectedButton.Parent;
            var rowIndex = stackContainer.Children.IndexOf(selectedButton);
            var colIndex = _gameMapIndexDictionary[stackContainer.Name];

            if (_gameMap[rowIndex, colIndex] == 'r' || _gameMap[rowIndex, colIndex] == 'y')
                return;

            for (int i = MaxRow - 1; i >= 0; --i)     // check for unfilled in seleceted position
            {
                if (_gameMap[i, colIndex] == ' ')
                {
                    var calButton = (Button)stackContainer.Children[i];

                    if (_switchPlayers == true)
                    {
                        calButton.Foreground = Brushes.Red;
                        _gameMap[i, colIndex] = 'r';
                        var result = checkWin('r', stackContainer);
                        if (result)
                        {
                            _canPlay = false;
                            GameState = "Game Over";
                            sqlDataHandler.UpdatePlayerStats(Player1, Player2, winnerPoints);
                            WindowManager windowManager = new WindowManager();
                            windowManager.ShowDialogAsync(new CustomAlertViewModel($"{Player1} Won!"));
                        }
                    }

                    else if (_switchPlayers == false)
                    {
                        calButton.Foreground = Brushes.Yellow;
                        _gameMap[i, colIndex] = 'y';
                        var result = checkWin('y', stackContainer);
                        if (result)
                        {
                            _canPlay = false;
                            GameState = "Game Over";
                            sqlDataHandler.UpdatePlayerStats(Player2, Player1, winnerPoints);
                            WindowManager windowManager = new WindowManager();
                            windowManager.ShowDialogAsync(new CustomAlertViewModel($"{Player2} Won!"));
                        }
                    }

                    break;
                }
            }

            _switchPlayers = !_switchPlayers;

            if (_switchPlayers == true && _canPlay)
                GameState = $"{Player1}'s Turn";

            else if (_switchPlayers == false && _canPlay)
                GameState = $"{Player2}'s Turn";
        }


        private bool checkWin(char player, StackPanel container)
        {
            int count = 0;
            List<Field> points = new List<Field>();
            bool foundPattern = false;

            #region HorizontalCheck

            for (int i = 0; i < MaxRow; ++i)
            {
                count = 0;
                points.Clear();

                if (foundPattern)
                    break;

                for (int j = 0; j < MaxColumn; ++j)
                {
                    if (_gameMap[i, j] == player)
                    {
                        ++count;
                        points.Add(new Field(i, j));
                        if (count == 4) break;
                    }

                    else if (_gameMap[i, j] != player)
                    {
                        count = 0;
                        points.Clear();
                    }
                }//

                if (count == 4)
                {
                    var containerParent = (StackPanel)container.Parent;
                    foundPattern = true;

                    for (int p = 0; p < points.Count; ++p)
                    {
                        var col = points[p].col;
                        var innerContainer = (StackPanel)containerParent.Children[col];
                        var btn = (Button)innerContainer.Children[points[p].rol];
                        btn.Content = "X";
                    }
                }
            }//
            #endregion

            if (foundPattern)
                return foundPattern;


            #region VerticalCheck
            count = 0;
            points.Clear();

            for (int i = 0; i < MaxColumn; ++i)
            {
                count = 0;
                points.Clear();

                if (foundPattern)
                    break;

                for (int j = 0; j < MaxRow; ++j)
                {
                    if (_gameMap[j, i] == player)
                    {
                        ++count;
                        points.Add(new Field(j, i));
                        if (count == 4) break;
                    }

                    else if (_gameMap[j, i] != player)
                    {
                        count = 0;
                        points.Clear();
                    }
                }//

                if (count == 4)
                {
                    var containerParent = (StackPanel)container.Parent;
                    foundPattern = true;

                    for (int p = 0; p < points.Count; ++p)
                    {
                        var col = points[p].col;
                        var innerContainer = (StackPanel)containerParent.Children[col];
                        var btn = (Button)innerContainer.Children[points[p].rol];
                        btn.Content = "X";
                    }
                }
            }//
            #endregion

            if (foundPattern)
                return foundPattern;

            #region DiagonalWin_1

            count = 0;
            points.Clear();

            for (int col = 0; col < MaxColumn; ++col)
            {
                count = 0;
                points.Clear();

                if (foundPattern)
                    break;

                for (int rol = 3; rol < MaxRow; ++rol)
                {
                    count = 0;
                    points.Clear();

                    if (foundPattern)
                        break;

                    int i = rol;

                    for (int j = col; j < MaxColumn; ++j, --i)
                    {
                        if (i == -1)
                            break;
                        if (_gameMap[i, j] == player)
                        {
                            points.Add(new Field(i, j));
                            ++count;
                            if (count == 4)
                                break;
                        }

                        else if (_gameMap[i, j] != player)
                        {
                            count = 0;
                            points.Clear();
                        }
                    }

                    if (count == 4)
                    {
                        var containerParent = (StackPanel)container.Parent;
                        foundPattern = true;

                        for (int p = 0; p < points.Count; ++p)
                        {
                            var co = points[p].col;
                            var innerContainer = (StackPanel)containerParent.Children[co];
                            var btn = (Button)innerContainer.Children[points[p].rol];
                            btn.Content = "X";
                        }
                    }//

                }//
            }//

            #endregion

            if (foundPattern)
                return foundPattern;


            #region DiagonalWin_2

            count = 0;
            points.Clear();

            for (int col = 0; col < MaxColumn; ++col)
            {
                count = 0;
                points.Clear();

                if (foundPattern)
                    break;

                for (int rol = 3; rol < MaxRow; ++rol)
                {
                    count = 0;
                    points.Clear();

                    if (foundPattern)
                        break;

                    int i = rol;

                    for (int j = col; j >= 0; --j, --i)
                    {
                        if (i == -1)
                            break;
                        if (_gameMap[i, j] == player)
                        {
                            points.Add(new Field(i, j));
                            ++count;
                            if (count == 4)
                                break;
                        }

                        else if (_gameMap[i, j] != player)
                        {
                            count = 0;
                            points.Clear();
                        }
                    }

                    if (count == 4)
                    {
                        var containerParent = (StackPanel)container.Parent;
                        foundPattern = true;

                        for (int p = 0; p < points.Count; ++p)
                        {
                            var co = points[p].col;
                            var innerContainer = (StackPanel)containerParent.Children[co];
                            var btn = (Button)innerContainer.Children[points[p].rol];
                            btn.Content = "X";
                        }
                    }//
                }//
            }//

            #endregion

            if (foundPattern)
                return foundPattern;


            return false;
        }//
    }
}
