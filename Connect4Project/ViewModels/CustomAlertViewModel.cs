using Caliburn.Micro;

namespace Connect4Project.ViewModels
{
    class CustomAlertViewModel : Conductor<object>
    {

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyOfPropertyChange();
            }
        }

        public CustomAlertViewModel(string message)
        {
            Message = message;
        }

        public void CloseCommand()
        {
            TryCloseAsync();
        }

    }
}
