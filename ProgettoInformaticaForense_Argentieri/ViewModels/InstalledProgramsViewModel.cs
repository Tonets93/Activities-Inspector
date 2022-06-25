using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProgettoInformaticaForense_Argentieri.Messages;
using ProgettoInformaticaForense_Argentieri.Models;
using ProgettoInformaticaForense_Argentieri.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProgettoInformaticaForense_Argentieri.ViewModels
{
    public class InstalledProgramsViewModel : ViewModelBase
    {
        #region Proprietà

        private ObservableCollection<InstallEntry> _installEntries;

        public ObservableCollection<InstallEntry> InstallEntries
        {
            get => _installEntries;
            set 
            {
                var changed = Set(nameof(InstallEntries), ref _installEntries, value);

                if (changed)
                {
                    ExportCommand.RaiseCanExecuteChanged();
                }
            }   
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                var changed = Set(nameof(IsBusy), ref _isBusy, value);

                if (changed)
                {
                    LoadInstallEntriesCommand.RaiseCanExecuteChanged();
                    ExportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Comandi

        private RelayCommand _loadInstallEntriesCommand;
        public RelayCommand LoadInstallEntriesCommand => _loadInstallEntriesCommand
            ?? (_loadInstallEntriesCommand = new RelayCommand(ExecuteLoadInstallEntriesCommandAsync,
                CanExecuteLoadInstallEntriesCommand));

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand => _exportCommand
            ?? (_exportCommand = new RelayCommand(ExecuteExportCommandAsync,
                CanExecuteExportCommandAsync));

        #endregion

        private readonly IInstallEntriesBuilder _installEntriesBuilder;
        private readonly IDialogService _dialogService;
        private readonly IEntriesExporter _entriesExporter;
        private readonly IMessenger _messenger;

        public InstalledProgramsViewModel(IInstallEntriesBuilder installEntriesBuilder, IDialogService dialogService,
            IEntriesExporter entriesExporter, IMessenger messenger)
        {
            _installEntriesBuilder = installEntriesBuilder;
            _dialogService = dialogService;
            _entriesExporter = entriesExporter;
            _messenger = messenger;
        }

        private bool CanExecuteLoadInstallEntriesCommand()
            => IsBusy ? false : true;

        private async void ExecuteLoadInstallEntriesCommandAsync()
        {
            if (InstallEntries != null) InstallEntries.Clear();
            IsBusy = true;

            var getInstallEntriesResult = await _installEntriesBuilder.GetInstallEntries();

            if (getInstallEntriesResult.IsSuccess)
            {
                InstallEntries = new ObservableCollection<InstallEntry>(getInstallEntriesResult.Value);

                _messenger.Send(new OnInstallEntriesChangedMessage(InstallEntries.ToList()));
            }
            else
            {
                _dialogService.ShowError(getInstallEntriesResult.Error);
            }

            IsBusy = false;
        }

        private bool CanExecuteExportCommandAsync()
            => IsBusy == false && InstallEntries != null;

        private async void ExecuteExportCommandAsync()
        {
            try
            {
                var exportResult = await _entriesExporter.SaveEntriesDataAsync(InstallEntries, EntryType.InstalledPrograms);

                if (exportResult.IsSuccess)
                {
                    _dialogService.ShowInfo(Properties.Resources.ExportCommand_ExportComplete_Message);
                }
                else
                {
                    _dialogService.ShowError(exportResult.Error);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }
    }
}
