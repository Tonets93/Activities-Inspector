using CSharpFunctionalExtensions;
using ProgettoInformaticaForense_Argentieri.Models;
using ProgettoInformaticaForense_Argentieri.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace ProgettoInformaticaForense_Argentieri.Services
{
    public class UsbTrackingService : IUsbTrackingService
    {
        public async Task<Result<List<EventRecord>>> GetUsbEventsLogsAsync(int eventId)
        {
            var taskCompletionSource = new TaskCompletionSource<Result<List<EventRecord>>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            string logType = "Microsoft-Windows-DriverFrameworks-UserMode/Operational";
            string query = $"*[System/EventID={eventId}]";

            var elQuery = new EventLogQuery(logType, PathType.LogName, query);
            var elReader = new EventLogReader(elQuery);

            var tmp = new List<EventRecord>();

            try
            {
                await Task.Run(() =>
                {
                    for (var eventInstance = elReader.ReadEvent(); eventInstance != null; eventInstance = elReader.ReadEvent())
                    {
                        tmp.Add(eventInstance);
                    }
                });

                taskCompletionSource.SetResult(Result.Success(tmp));
            }

            catch (Exception ex)
            {
                taskCompletionSource.SetResult(Result.Failure<List<EventRecord>>(ex.Message));
            }

            return taskCompletionSource.Task.Result;
        }

        public IEnumerable<UsbEntry> BuildUsbEntries(List<EventRecord> usbConnectedEvents,
            List<EventRecord> usbDisconnectedEvents)
        {
            foreach (var pluggedEvent in usbConnectedEvents)
            {
                var selectedDisconnectedEvent = usbDisconnectedEvents.Where
                    (de => de.Properties[0].Value.ToString() == pluggedEvent.Properties[0].Value.ToString()).FirstOrDefault();

                if (selectedDisconnectedEvent != null &&
                    selectedDisconnectedEvent.TimeCreated < pluggedEvent.TimeCreated) continue;

                var pluggedTime = DateBuilder.BuildFromString(pluggedEvent.TimeCreated.ToString());
                var unpluggedTime = DateBuilder.BuildFromString(selectedDisconnectedEvent?.TimeCreated.ToString() ?? string.Empty);
                var deviceName = pluggedEvent.Properties[1]?.Value ?? string.Empty;
                var machineName = pluggedEvent.MachineName;

                yield return new UsbEntry(pluggedTime, unpluggedTime, deviceName.ToString(), machineName);
            }
        }
    }
}
