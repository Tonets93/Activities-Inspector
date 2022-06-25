using CSharpFunctionalExtensions;
using ProgettoInformaticaForense_Argentieri.Models;
using ProgettoInformaticaForense_Argentieri.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoInformaticaForense_Argentieri.Services
{
    public class EntriesExporter : IEntriesExporter
    {
        private readonly IEntryFormatter _entryFormatter;

        private const string _exportedDataRootPath = "Output";

        public EntriesExporter(IEntryFormatter entryFormatter)
        {
            _entryFormatter = entryFormatter;

            InitFileSystem();
        }

        public async Task<Result> SaveEntriesDataAsync(IEnumerable<Entry> entries, EntryType entryType)
        {
            var taskCompletionSource = new TaskCompletionSource<Result>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                await Task.Run(() =>
                {
                    var fileName = SetFileName(entryType);

                    using (var writer = new EntryWriter($"{_exportedDataRootPath}/{fileName}.csv", false, Encoding.Default, _entryFormatter))
                    {
                        writer.WriteEntries(entries, entryType);
                    }
                });

                taskCompletionSource.SetResult(Result.Success());
            }
            catch(Exception ex)
            {
                taskCompletionSource.SetResult(Result.Failure(ex.Message));
            }

            return taskCompletionSource.Task.Result;
        }

        private void InitFileSystem()
        {
            if (Directory.Exists(_exportedDataRootPath) == false)
                Directory.CreateDirectory(_exportedDataRootPath);
        }

        private string SetFileName(EntryType entryType)
        {
            switch (entryType)
            {
                case EntryType.TimeIntervals:
                    return Properties.Resources.Intervals_FileName;
                case EntryType.InstalledPrograms:
                    return Properties.Resources.InstalledPrograms_FileName;
                case EntryType.Recents:
                    return Properties.Resources.Recents_FileName;
                case EntryType.Prefetch:
                    return Properties.Resources.Prefetch_FileName;
                case EntryType.ShellBags:
                    return Properties.Resources.ShellBags_FileName;
                case EntryType.Sessions:
                    return Properties.Resources.Sessions_FileName;
                case EntryType.SystemTimeChanged:
                    return Properties.Resources.SystemTimeChanged_FileName;
                case EntryType.Usb:
                    return Properties.Resources.Usb_FileName;
                default: throw new ArgumentException();
            }
        }
    }
}
