using Caliburn.Micro;
using Connect4Project.SqlHandler;
using Connect4Project.SqlHandler.DataModels;
using System;

namespace Connect4Project.ViewModels
{
    class RegisterViewModel : Screen
    {
        private string _firstName { get; set; }
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                NotifyOfPropertyChange();
            }
        }

        private string _lastName { get; set; }
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                NotifyOfPropertyChange();
            }
        }

        private string _email { get; set; }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                NotifyOfPropertyChange();
            }
        }

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


        public bool CanRegister(string firstName, string lastName, string email, string username, string country)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(country))
                return false;

            return true;
        }


        public void Register(string firstName, string lastName, string email, string username, string country)
        {

            UserInfo userInfo = new UserInfo { firstName = FirstName, lastName = LastName, email = Email, username = Username, country = Country };
            bool errorFound = false;
            string errorMessage = "DB Connection Error!";

            try
            {
                sqlDataHandler.RegisterUser(userInfo);
            }

            catch (Exception err)
            {
                var error = err.Message;

                if (error.Contains("duplicate"))
                    errorMessage = "Username or Email Already Exisits";
                errorFound = true;
            }

            if (errorFound == false)
            {
                FirstName = string.Empty;
                LastName = string.Empty;
                Email = string.Empty;
                Username = string.Empty;
                Country = string.Empty;

                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialogAsync(new CustomAlertViewModel("Completed!"));
            }//

            else
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialogAsync(new CustomAlertViewModel(errorMessage));
            }

        }
    }
}
