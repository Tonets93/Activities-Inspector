namespace ProgettoInformaticaForense_Argentieri.Models
{
    public class ShellBagEntry : Entry
    {
        public string AbsolutePath { get; set; }
        public string AccessedDate { get; set; }
        public string CreationDate { get; set; }
        public string LastRegistryWriteDate { get; set; }
        public string RegistryPath { get; set; }

        public ShellBagEntry(string absolutePath, string accessedDate, string creationDate, string lastRegistryWriteDate, 
            string registryPath)
        {
            AbsolutePath = absolutePath;
            AccessedDate = accessedDate;
            CreationDate = creationDate;
            LastRegistryWriteDate = lastRegistryWriteDate;
            RegistryPath = registryPath;
        }
    }
}
