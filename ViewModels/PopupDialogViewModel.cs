using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace ToDoListPlus.ViewModels
{
    public class PopupDialogViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand SaveCommand => _saveCommand;
        public ICommand CancelCommand => _cancelCommand;


        private readonly DelegateCommand _saveCommand;
        private readonly DelegateCommand _cancelCommand;


        public string TaskTitle { get; set; }
        public DateTime? DueDate { get; set; }

        public event Action<bool>? RequestClose;

        public PopupDialogViewModel()
        {
            _saveCommand = new DelegateCommand(OnSave, Execute);
            _cancelCommand = new DelegateCommand(OnCancel, Execute);
        }

        private void OnSave(object commandParameter)
        {
            RequestClose?.Invoke(true);
        }

        private bool Execute(object commandParameter)
        {
            return true;
        }

        private void OnCancel(object commandParameter) {
            RequestClose?.Invoke(false);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

 }
