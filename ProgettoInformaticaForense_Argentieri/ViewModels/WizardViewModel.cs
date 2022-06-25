using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoInformaticaForense_Argentieri.ViewModels
{
    public class WizardViewModel : ViewModelBase
    {
        #region Proprietà

        private ObservableCollection<PageViewModelBase> _pages = new ObservableCollection<PageViewModelBase>();

        public ObservableCollection<PageViewModelBase> Pages
        {
            get => _pages;
            private set => Set(nameof(Pages), ref _pages, value);
        }

        private PageViewModelBase _currentPage;

        public PageViewModelBase CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (_currentPage == value) return;
            }
        }

        #endregion
    }
}
