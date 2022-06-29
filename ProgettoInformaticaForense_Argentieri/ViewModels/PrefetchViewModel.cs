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
    public class PrefetchViewModel : ViewModelBase
    {
        #region Proprietà

        private ObservableCollection<PrefetchInfoEntry> _prefetchEntries;

        public ObservableCollection<PrefetchInfoEntry> PrefetchEntries
        {
            get => _prefetchEntries;
            set
            {
                var changed = Set(nameof(PrefetchEntries), ref _prefetchEntries, value);

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
                    LoadPrefetchInfoEntriesCommand.RaiseCanExecuteChanged();
                    ExportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Comandi

        private RelayCommand _loadPrefetchInfoEntriesCommand;
        public RelayCommand LoadPrefetchInfoEntriesCommand => _loadPrefetchInfoEntriesCommand
            ?? (_loadPrefetchInfoEntriesCommand = new RelayCommand(ExecuteLoadPrefetchInfoEntriesCommandAsync,
                CanExecuteLoadPrefetchInfoEntriesCommand));

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand => _exportCommand
            ?? (_exportCommand = new RelayCommand(ExecuteExportCommandAsync,
                CanExecuteExportCommandAsync));

        #endregion

        private readonly IPrefetchFileInfoBuilderService _prefetchFileInfoBuilderService;
        private readonly IDialogService _dialogService;
        private readonly IEntriesExporter _entriesExporter;
        private readonly IMessenger _messenger;

        public PrefetchViewModel(IPrefetchFileInfoBuilderService prefetchFileInfoBuilderService,
            IDialogService dialogService, IEntriesExporter entriesExporter, IMessenger messenger)
        {
            _prefetchFileInfoBuilderService = prefetchFileInfoBuilderService;
            _dialogService = dialogService;
            _entriesExporter = entriesExporter;
            _messenger = messenger;
        }

        private bool CanExecuteLoadPrefetchInfoEntriesCommand()
            => IsBusy ? false : true;

        private async void ExecuteLoadPrefetchInfoEntriesCommandAsync()
        {
            if (PrefetchEntries != null) PrefetchEntries.Clear();
            IsBusy = true;

            try
            {
                var getPrefetchFileInfosResult = await _prefetchFileInfoBuilderService.GetPrefetchFileInfosAsync();

                if (getPrefetchFileInfosResult.IsSuccess)
                {
                    PrefetchEntries = new ObservableCollection<PrefetchInfoEntry>(getPrefetchFileInfosResult.Value);

                    _messenger.Send(new OnPrefetchInfoEntriesChangedMessage(PrefetchEntries.ToList()));
                }
                else
                {
                    _dialogService.ShowError(getPrefetchFileInfosResult.Error);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message + "\n" + ex.StackTrace);
            }

            IsBusy = false;
        }

        private bool CanExecuteExportCommandAsync()
            => IsBusy == false & PrefetchEntries != null;

        private async void ExecuteExportCommandAsync()
        {
            try
            {
                var exportResult = await _entriesExporter.SaveEntriesDataAsync(PrefetchEntries, EntryType.Prefetch);

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
