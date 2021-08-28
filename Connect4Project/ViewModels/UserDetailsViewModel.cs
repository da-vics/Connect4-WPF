using Caliburn.Micro;
using Connect4Project.Models;
using Connect4Project.SqlHandler;
using System;
using System.Windows;

namespace Connect4Project.ViewModels
{
    public class UserDetailsViewModel : Screen
    {

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

        private SqlDataHandler sqlDataHandler;

        public UserDetailsViewModel()
        {
            sqlDataHandler = new SqlDataHandler();
        }

        public bool CanPlayGameCommand(string player1, string player2)
        {
            if (string.IsNullOrEmpty(player1) || string.IsNullOrEmpty(player2) || player1 == player2)
                return false;

            return true;
        }

        public void PlayGameCommand(string player1, string player2)
        {
            var conductor = this.Parent as IConductor;
            bool Player1_result = false;
            bool Player2_result = false;

            try
            {
                Player1_result = sqlDataHandler.VerifyUser(player1);
                Player2_result = sqlDataHandler.VerifyUser(player2);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialogAsync(new CustomAlertViewModel("DB Error!"));
            }

            if (Player1_result && Player2_result)
            {
                var gamePlayInput = new GamePlayInput(Player1, Player2);
                conductor.ActivateItemAsync(new GamePlayViewModel(gamePlayInput));
            }

            else
            {
                WindowManager windowManager = new WindowManager();
                if (!Player1_result)
                    windowManager.ShowDialogAsync(new CustomAlertViewModel($"{player1} not Registered!"));

                if (!Player2_result)
                    windowManager.ShowDialogAsync(new CustomAlertViewModel($"{player2} not Registered!"));
            }

        }
    }
}
