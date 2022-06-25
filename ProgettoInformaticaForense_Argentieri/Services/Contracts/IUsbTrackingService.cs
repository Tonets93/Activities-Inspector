using CSharpFunctionalExtensions;
using ProgettoInformaticaForense_Argentieri.Models;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace ProgettoInformaticaForense_Argentieri.Services
{
    public interface IUsbTrackingService
    {
        Task<Result<List<EventRecord>>> GetUsbEventsLogsAsync(int eventId);

        IEnumerable<UsbEntry> BuildUsbEntries(List<EventRecord> usbConnectedEvents,
            List<EventRecord> usbDisconnectedEvents);
    }
}
