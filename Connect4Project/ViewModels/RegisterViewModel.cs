using Caliburn.Micro;
using Connect4Project.SqlHandler;
using Connect4Project.SqlHandler.DataModels;
using System;

namespace Connect4Project.ViewModels
{
    class RegisterViewModel : Screen
    {
        private string _username { get; set; }
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                NotifyOfPropertyChange();
            }
        }

        private string _country { get; set; }
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                NotifyOfPropertyChange();
            }
        }

        private SqlDataHandler sqlDataHandler;

        public RegisterViewModel()
        {
            sqlDataHandler = new SqlDataHandler();
        }


        public bool CanRegister(string username, string country)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(country))
                return false;

            return true;
        }


        public void Register(string username, string country)
        {

            UserInfo userInfo = new UserInfo { username = Username, country = Country };
            bool errorFound = false;
            string errorMessage = "DB Error!";

            try
            {
                sqlDataHandler.RegisterUser(userInfo);
            }

            catch (Exception err)
            {
                var error = err.Message;
                errorMessage = "Username already exisits";
                errorFound = true;
            }

            if (errorFound == false)
            {
                Username = string.Empty;
                Country = string.Empty;

                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialogAsync(new CustomAlertViewModel("Registration done"));
            }//

            else
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialogAsync(new CustomAlertViewModel(errorMessage));
            }

        }
    }
}
