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
    public class UsbViewModel : ViewModelBase
    {
        #region Proprietà

        private ObservableCollection<UsbEntry> _usbEntries;

        public ObservableCollection<UsbEntry> UsbEntries
        {
            get => _usbEntries;
            set
            {
                var changed = Set(nameof(UsbEntries), ref _usbEntries, value);

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
                    LoadUsbEntriesCommand.RaiseCanExecuteChanged();
                    ExportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Comandi

        private RelayCommand _loadUsbEntriesCommand;
        public RelayCommand LoadUsbEntriesCommand => _loadUsbEntriesCommand
            ?? (_loadUsbEntriesCommand = new RelayCommand(ExecuteLoadUsbEntriesCommandAsync,
                CanExecuteLoadUsbEntriesCommand));

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand => _exportCommand
            ?? (_exportCommand = new RelayCommand(ExecuteExportCommandAsync,
                CanExecuteExportCommandAsync));

        #endregion

        private readonly IUsbTrackingService _usbTrackingService;
        private readonly IDialogService _dialogService;
        private readonly IEntriesExporter _entriesExporter;
        private readonly IMessenger _messenger;

        public UsbViewModel(IUsbTrackingService usbTrackingService, IDialogService dialogService,
            IEntriesExporter entriesExporter, IMessenger messenger)
        {
            _usbTrackingService = usbTrackingService;
            _dialogService = dialogService;
            _entriesExporter = entriesExporter;
            _messenger = messenger;
        }

        private bool CanExecuteLoadUsbEntriesCommand()
            => IsBusy ? false : true;

        private async void ExecuteLoadUsbEntriesCommandAsync()
        {
            if (UsbEntries != null) UsbEntries.Clear();
            IsBusy = true;

            var getUsbPluggedResult = await _usbTrackingService.GetUsbEventsLogsAsync(2003);
            var getUsbUnpluggedResult = await _usbTrackingService.GetUsbEventsLogsAsync(2102);

            if (getUsbPluggedResult.IsSuccess && getUsbUnpluggedResult.IsSuccess)
            {
                var entries = _usbTrackingService.BuildUsbEntries(getUsbPluggedResult.Value, getUsbUnpluggedResult.Value);
                UsbEntries = new ObservableCollection<UsbEntry>(entries.ToList());

                _messenger.Send(new OnUsbEntriesChangedMessage(UsbEntries.ToList()));
            }
            else
            {
                _dialogService.ShowError("Errore durante il caricamento degli eventi per la funzionalità richiesta.");
            }

            IsBusy = false;
        }

        private bool CanExecuteExportCommandAsync()
            => IsBusy == false & UsbEntries != null;

        private async void ExecuteExportCommandAsync()
        {
            try
            {
                var exportResult = await _entriesExporter.SaveEntriesDataAsync(UsbEntries, EntryType.Usb);

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
