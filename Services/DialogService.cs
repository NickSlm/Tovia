using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPlus.ViewModels;
using ToDoListPlus.Views;

namespace ToDoListPlus.Services
{

    public class DialogService: IDialogService
    {
        public bool? ShowDialog(PopupDialogViewModel viewModel)
        {
            var dialog = new PopupDialogView { DataContext = viewModel };

            viewModel.RequestClose += result =>
            {
                dialog.DialogResult = result;
                dialog.Close();
            };

            return dialog.ShowDialog();
        }

    }
}
