using Caliburn.Micro;
using Connect4Project.Models;
using Connect4Project.SqlHandler;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Connect4Project.ViewModels
{
    class LeaderBoardViewModel : Screen
    {

        private ObservableCollection<PlayerStats> _playerStats;
        public ObservableCollection<PlayerStats> PlayerStats
        {
            get => _playerStats;
            set
            {
                _playerStats = value;
                NotifyOfPropertyChange();
            }
        }

        private SqlDataHandler sqlDataHandler;

        public LeaderBoardViewModel()
        {
            sqlDataHandler = new SqlDataHandler();
            PlayerStats = sqlDataHandler.GetLeaderBoard();
        }

        public void DeleteItem(object obj)
        {
            var deletebtn = obj as Button;
            var parentContainer = (StackPanel)deletebtn.Parent;
            var BorderContainer = (Border)parentContainer.Children[0];
            var StackContainer = (StackPanel)BorderContainer.Child;
            var ListItems = (ListView)StackContainer.Children[2];
            var selected = (PlayerStats)ListItems.SelectedValue;

            if (selected != null)
            {
                sqlDataHandler.DeleteUser(selected.userName);
                PlayerStats = sqlDataHandler.GetLeaderBoard();
            }

            else
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialogAsync(new CustomAlertViewModel("No User Selected!"));
            }

        }
    }
}
