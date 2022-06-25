using ProgettoInformaticaForense_Argentieri.Models;
using ProgettoInformaticaForense_Argentieri.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProgettoInformaticaForense_Argentieri.Utility
{
    public class EntryWriter : StreamWriter
    {
        private readonly IEntryFormatter _entryFormatter;

        public EntryWriter(string path, bool append, Encoding encoding, IEntryFormatter entryFormatter) : base(path, append, encoding)
        {
            _entryFormatter = entryFormatter;
        }

        public void WriteEntries(IEnumerable<Entry> entries, EntryType entryType)
        {
            WriteLine(BuildHeader(entryType));

            foreach(var entry in entries)
            {
                WriteLine(_entryFormatter.AsCsv(entry));
            }
        }

        private string BuildHeader(EntryType entryType)
        {
            switch (entryType)
            {
                case EntryType.InstalledPrograms:
                case EntryType.Recents:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3}",
                        Properties.Resources.MainWindows_InstallEntry_FileName,
                        Properties.Resources.MainWindows_InstallEntry_DataSource,
                        Properties.Resources.MainWindows_InstallEntry_FullPath,
                        Properties.Resources.MainWindows_InstallEntry_installDate);

                case EntryType.Prefetch:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3}",
                        Properties.Resources.MainWindows_Prefetch_ExecutableFileName,
                        Properties.Resources.MainWindows_Prefetch_SourceFileName,
                        Properties.Resources.MainWindows_Prefetch_Extension,
                        Properties.Resources.MainWindows_Prefetch_LastRunTime);

                case EntryType.ShellBags:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3} ; {4}",
                        Properties.Resources.MainWindows_ShellBags_AbsolutePath,
                        Properties.Resources.MainWindows_ShellBags_AccessedDate,
                        Properties.Resources.MainWindows_ShellBags_CreationDate,
                        Properties.Resources.MainWindows_ShellBags_LastRegistryWriteDate,
                        Properties.Resources.MainWindows_ShellBags_RegistryPath);

                case EntryType.TimeIntervals:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3}",
                        Properties.Resources.MainWindows_TimeIntervals_Start,
                        Properties.Resources.MainWindows_TimeIntervals_End,
                        Properties.Resources.MainWindows_TimeIntervals_Duration,
                        Properties.Resources.MainWindows_TimeIntervals_MachineName);

                case EntryType.Sessions:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3} ; {4} ; {5} ; {6} ; {7}",
                        Properties.Resources.MainWindows_Sessions_UserName,
                        Properties.Resources.MainWindows_Sessions_Group,
                        Properties.Resources.MainWindows_Sessions_MachineName,
                        Properties.Resources.MainWindows_Sessions_LogOnTime,
                        Properties.Resources.MainWindows_Sessions_LogOffTime,
                        Properties.Resources.MainWindows_Sessions_Duration,
                        Properties.Resources.MainWindows_Sessions_NetworkAddress,
                        Properties.Resources.MainWindows_Sessions_AccessType);

                case EntryType.SystemTimeChanged:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3}",
                        Properties.Resources.MainWindows_SystemTimeChanged_UserName,
                        Properties.Resources.MainWindows_SystemTimeChanged_EventTime,
                        Properties.Resources.MainWindows_SystemTimeChanged_OldTime,
                        Properties.Resources.MainWindows_SystemTimeChanged_NewTime);

                case EntryType.Usb:
                    return string.Format(
                        "{0} ; {1} ; {2} ; {3}",
                        Properties.Resources.MainWindows_SystemTimeChanged_PluggedTime,
                        Properties.Resources.MainWindows_SystemTimeChanged_UnpluggedTime,
                        Properties.Resources.MainWindows_SystemTimeChanged_DeviceName,
                        Properties.Resources.MainWindows_SystemTimeChanged_MachineName);

                default: throw new ArgumentException(nameof(entryType));
            }
        }
    }
}
