using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;

namespace GameLibrary
{
    public class BasicViewModel : ViewModelBase
    {

        private string text;

        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        private ICommand clickMeCommand;

        public ICommand ClickMeCommand
        {
            get { return clickMeCommand; }
            set { SetProperty(ref clickMeCommand, value); }
        }

        public BasicViewModel() : base()
        {
            Text = "Hello GDS!";
            ClickMeCommand = new RelayCommand(new Action<object>(OnClicked));
        }

        private void OnClicked(object obj)
        {
            Text = "It works!";
        }
    }
}
