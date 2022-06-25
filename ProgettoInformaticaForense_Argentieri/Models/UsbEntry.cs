namespace ProgettoInformaticaForense_Argentieri.Models
{
    public class UsbEntry : Entry
    {
        public string PluggedTime { get; private set; }
        public string UnpluggedTime { get; private set; }
        public string DeviceName { get; private set; }
        public string MachineName { get; private set; }

        public UsbEntry(string puggledTime, string unpluggedTime, string deviceName, string machineName)
        {
            PluggedTime = puggledTime;
            UnpluggedTime = unpluggedTime;
            DeviceName = deviceName;
            MachineName = machineName;
        }
    }
}
