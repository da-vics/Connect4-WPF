using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;

namespace Connect4Project.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive
    {

        public ShellViewModel()
        {
            ActivateItemAsync(new GameMenuViewModel(CloseCommand));
        }

        public void MenuCommand()
        {
            Window mwindow = (Window)GetView();
            SystemCommands.ShowSystemMenu(mwindow, mwindow.PointToScreen(Mouse.GetPosition(mwindow)));
        }

        public void CloseCommand()
        {
            Window mwindow = (Window)GetView();
            mwindow.Close();
        }

        public void MinimiseCommand()
        {
            Window mwindow = (Window)GetView();
            mwindow.WindowState = WindowState.Minimized;
        }

        public void MaximizeCommand()
        {
            Window mwindow = (Window)GetView();
            mwindow.WindowState ^= WindowState.Maximized;
        }

        public void BackHomeCommand()
        {
            if (Items.Count > 1)
            {
                ActivateItemAsync(new GameMenuViewModel(CloseCommand));
                var homePage = Items[0];
                Items.Clear();
                ActivateItemAsync(homePage);
            }
        }

        public void BackOnePage()
        {
            if (Items.Count > 1)
            {
                Items.RemoveAt(Items.Count - 1);
                var index = Items.Count - 1;
                ActivateItemAsync(Items[index]);
            }
        }
    }
}
