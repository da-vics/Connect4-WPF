using Caliburn.Micro;
using System.Windows.Input;

namespace Connect4Project.ViewModels
{
    class GameMenuViewModel : Screen
    {

        private System.Action _closeWindow;

        public GameMenuViewModel(System.Action closeWindow)
        {
            _closeWindow = closeWindow;
        }

        public void PlayGameCommand()
        {
            var conductor = this.Parent as IConductor;
            conductor.ActivateItemAsync(new UserDetailsViewModel());
        }

        public void QuitCommand()
        {
            _closeWindow?.Invoke();
        }

        public void LeaderBoard()
        {
            var conductor = this.Parent as IConductor;
            conductor.ActivateItemAsync(new LeaderBoardViewModel());
        }

        public void Register()
        {
            var conductor = this.Parent as IConductor;
            conductor.ActivateItemAsync(new RegisterViewModel());
        }

    }
}
