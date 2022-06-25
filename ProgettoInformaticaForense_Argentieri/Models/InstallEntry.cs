namespace ProgettoInformaticaForense_Argentieri.Models
{
    public class InstallEntry : Entry
    {
        public string FileName { get; set; }
        public string DataSource { get; set; }
        public string FullPath { get; set; }
        public string InstallDate { get; set; }

        public InstallEntry(string fileName, string dataSource, string fullPath, string installDate)
        {
            FileName = fileName;
            DataSource = dataSource;
            FullPath = fullPath;
            InstallDate = installDate;
        }
    }
}
