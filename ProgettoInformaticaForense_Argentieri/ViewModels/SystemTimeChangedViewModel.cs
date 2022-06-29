﻿using GalaSoft.MvvmLight;
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
    public class SystemTimeChangedViewModel : ViewModelBase
    {
        #region Proprietà

        private ObservableCollection<SystemTimeChangedEntry> _timeChangedEntries;

        public ObservableCollection<SystemTimeChangedEntry> TimeChangedEntries
        {
            get => _timeChangedEntries;
            set
            {
                var changed = Set(nameof(TimeChangedEntries), ref _timeChangedEntries, value);

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
                    LoadSystemTimeChangedCommand.RaiseCanExecuteChanged();
                    ExportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Comandi

        private RelayCommand _loadSystemTimeChangedCommand;
        public RelayCommand LoadSystemTimeChangedCommand => _loadSystemTimeChangedCommand
            ?? (_loadSystemTimeChangedCommand = new RelayCommand(ExecuteLoadSystemTimeChangedCommandAsync,
                CanExecuteLoadSystemTimeChangedCommand));

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand => _exportCommand
            ?? (_exportCommand = new RelayCommand(ExecuteExportCommandAsync,
                CanExecuteExportCommandAsync));

        #endregion

        private readonly ISystemTimeChangedService _timeChanged;
        private readonly IDialogService _dialogService;
        private readonly IEntriesExporter _entriesExporter;
        private readonly IMessenger _messenger;

        public SystemTimeChangedViewModel(ISystemTimeChangedService timeChanged, IDialogService dialogService,
            IEntriesExporter entriesExporter, IMessenger messenger)
        {
            _timeChanged = timeChanged;
            _dialogService = dialogService;
            _entriesExporter = entriesExporter;
            _messenger = messenger;
        }

        private bool CanExecuteLoadSystemTimeChangedCommand()
            => IsBusy ? false : true;

        private async void ExecuteLoadSystemTimeChangedCommandAsync()
        {
            if (TimeChangedEntries != null) TimeChangedEntries.Clear();
            IsBusy = true;

            try
            {
                var result = await _timeChanged.GetSystemTimeChangedEntriesAsync();

                if (result.IsSuccess)
                {
                    var events = result.Value;
                    TimeChangedEntries = new ObservableCollection<SystemTimeChangedEntry>(result.Value);

                    _messenger.Send(new OnSystemTimeChangedEntriesChangedMessage(TimeChangedEntries.ToList()));
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message + "\n" + ex.StackTrace);
            }

            IsBusy = false;
        }

        private bool CanExecuteExportCommandAsync()
            => IsBusy == false & TimeChangedEntries != null;

        private async void ExecuteExportCommandAsync()
        {
            try
            {
                var exportResult = await _entriesExporter.SaveEntriesDataAsync(TimeChangedEntries, EntryType.SystemTimeChanged);

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
