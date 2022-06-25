using CSharpFunctionalExtensions;
using Microsoft.Win32;
using ProgettoInformaticaForense_Argentieri.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProgettoInformaticaForense_Argentieri.Services
{
    public class InstallEntriesBuilder : IInstallEntriesBuilder
    {
        private const string LOCAL_MACHINE_X86_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string CURRENT_USER_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public async Task<Result<List<InstallEntry>>> GetInstallEntries()
        {
            var taskCompletionSource = new TaskCompletionSource<Result<List<InstallEntry>>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                await Task.Run(() =>
                {
                    var locals = GetFromLocalMachine(LOCAL_MACHINE_X86_KEY);
                    var users = GetFromCurrentUser(CURRENT_USER_KEY);
                    var events = GetFromEvents();

                    taskCompletionSource.SetResult(Result.Success((locals.Concat(users)).Concat(events).ToList()));
                });
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetResult(Result.Failure<List<InstallEntry>>(ex.Message));
            }

            return taskCompletionSource.Task.Result;
        }

        private IEnumerable<InstallEntry> GetFromLocalMachine(string key)
        {
            using (RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(key))
            {
                if (rk == null) yield break;

                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        yield return BuildInstallEntry(sk);
                    }
                }
            }
        }

        private IEnumerable<InstallEntry> GetFromCurrentUser(string key)
        {
            using (RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(key))
            {
                if (rk == null) yield break;

                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        yield return BuildInstallEntry(sk);
                    }
                }
            }
        }

        private static IEnumerable<InstallEntry> GetFromEvents()
        {
            var events = GetSystemEvents();

            var installedPrograms = events.Where(ev => ev.EventID == 11707).ToList();

            var entries = new List<InstallEntry>();

            for (int i = 0; i < installedPrograms.Count; i++)
            {
                var substrings = installedPrograms[i].ReplacementStrings[0].Split(':');
                var substrings2 = substrings[1].Split(new char[] { '-', '-' });
                var fileName = substrings2[0].Trim();

                var date = installedPrograms[i].TimeGenerated.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var entry = new InstallEntry(fileName, string.Empty, string.Empty, date);

                if (entries.Any(ie => ie.FileName == entry.FileName && ie.InstallDate == entry.InstallDate)) continue;
                entries.Add(entry);
            }

            return entries;
        }

        private static IEnumerable<EventLogEntry> GetSystemEvents()
        {
            var myLog = new EventLog();
            myLog.Log = "Application";

            foreach (var @event in myLog.Entries)
            {
                var logEntry = (EventLogEntry)@event;
                yield return logEntry;
            }
        }

        private InstallEntry BuildInstallEntry(RegistryKey registryKey)
            => new InstallEntry(registryKey.GetValue("DisplayName")?.ToString(), registryKey.ToString(),
                registryKey.GetValue("InstallLocation")?.ToString(), registryKey.GetValue("InstallDate")?.ToString());
    }
}
