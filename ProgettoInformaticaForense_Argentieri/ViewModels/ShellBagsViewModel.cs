using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProgettoInformaticaForense_Argentieri.Messages;
using ProgettoInformaticaForense_Argentieri.Models;
using ProgettoInformaticaForense_Argentieri.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProgettoInformaticaForense_Argentieri.ViewModels
{
    public class ShellBagsViewModel : ViewModelBase
    {
        #region Proprietà

        private ObservableCollection<ShellBagEntry> _shellBagsEntries;

        public ObservableCollection<ShellBagEntry> ShellBagsEntries
        {
            get => _shellBagsEntries;
            set
            {
                var changed = Set(nameof(ShellBagsEntries), ref _shellBagsEntries, value);

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
                    LoadShellBagsEntriesCommand.RaiseCanExecuteChanged();
                    ExportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Comandi

        private RelayCommand _loadShellBagsEntriesCommand;
        public RelayCommand LoadShellBagsEntriesCommand => _loadShellBagsEntriesCommand
            ?? (_loadShellBagsEntriesCommand = new RelayCommand(ExecuteLoadShellBagsEntriesCommandAsync,
                CanExecuteLoadShellBagsEntriesCommand));

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand => _exportCommand
            ?? (_exportCommand = new RelayCommand(ExecuteExportCommandAsync,
                CanExecuteExportCommandAsync));

        #endregion

        private readonly IShellBagsParserService _shellBagsParserService;
        private readonly IDialogService _dialogService;
        private readonly IEntriesExporter _entriesExporter;
        private readonly IMessenger _messenger;

        public ShellBagsViewModel(IShellBagsParserService shellBagsParserService, IDialogService dialogService,
            IEntriesExporter entriesExporter, IMessenger messenger)
        {
            _shellBagsParserService = shellBagsParserService;
            _dialogService = dialogService;
            _entriesExporter = entriesExporter;
            _messenger = messenger;
        }

        private bool CanExecuteLoadShellBagsEntriesCommand()
            => IsBusy ? false : true;

        private async void ExecuteLoadShellBagsEntriesCommandAsync()
        {
            if (ShellBagsEntries != null) ShellBagsEntries.Clear();
            IsBusy = true;

            try
            {
                var shellbagsResult = await _shellBagsParserService.ParseShellBags();

                if (shellbagsResult.IsSuccess)
                {
                    var shellBags = shellbagsResult.Value;
                    var entries = GetShellBagsEntries(shellBags).ToList();

                    ShellBagsEntries = new ObservableCollection<ShellBagEntry>(entries);

                    _messenger.Send(new OnShellBagEntriesChangedMessage(ShellBagsEntries.ToList()));
                }
                else
                {
                    _dialogService.ShowError(shellbagsResult.Error);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message + "\n" + ex.StackTrace);
            }

            IsBusy = false;
        }

        private bool CanExecuteExportCommandAsync()
            => IsBusy == false & ShellBagsEntries != null;

        private async void ExecuteExportCommandAsync()
        {
            try
            {
                var exportResult = await _entriesExporter.SaveEntriesDataAsync(ShellBagsEntries, EntryType.ShellBags);

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

        private static IEnumerable<ShellBagEntry> GetShellBagsEntries(List<IShellItem> shellBags)
        {
            foreach (var item in shellBags)
            {
                var properties = item.GetAllProperties();

                var absPath = properties.ContainsKey("AbsolutePath") ? properties["AbsolutePath"] : string.Empty;
                var accDate = properties.ContainsKey("AccessedDate") ? properties["AccessedDate"] : string.Empty;
                var crDate = properties.ContainsKey("CreationDate") ? properties["CreationDate"] : string.Empty;
                var lrwDate = properties.ContainsKey("LastRegistryWriteDate") ? properties["LastRegistryWriteDate"] : string.Empty;
                var regPath = properties.ContainsKey("RegistryPath") ? properties["RegistryPath"] : string.Empty;

                yield return new ShellBagEntry(absPath, accDate, crDate, lrwDate, regPath);
            }
        }
    }
}
