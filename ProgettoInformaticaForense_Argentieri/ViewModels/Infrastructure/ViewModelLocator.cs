using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ProgettoInformaticaForense_Argentieri.Services;

namespace ProgettoInformaticaForense_Argentieri.ViewModels.Infrastructure
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IConfigParser, ConfigParser>();
            SimpleIoc.Default.Register<IInstallEntriesBuilder, InstallEntriesBuilder>();
            SimpleIoc.Default.Register<IPrefetchFileInfoBuilderService, PrefetchFileInfoBuilderService>();
            SimpleIoc.Default.Register<IPrefetchFileParserService, PrefetchFileParserService>();
            SimpleIoc.Default.Register<IRecentFilesService, RecentFilesService>();
            SimpleIoc.Default.Register<IShellBagsParserService, ShellBagsParserService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IUsageLogTimeService, UsageLogTimeService>();
            SimpleIoc.Default.Register<ILoggedInfoService, LoggedInfoService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IEntryFormatter, EntryFormatter>();
            SimpleIoc.Default.Register<IEntriesExporter, EntriesExporter>();
            SimpleIoc.Default.Register<ISystemTimeChangedService, SystemTimeChangedService>();
            SimpleIoc.Default.Register<IUsbTrackingService, UsbTrackingService>();
            SimpleIoc.Default.Register<INetService, NetService>();
            SimpleIoc.Default.Register<IReportService, ReportService>();
            SimpleIoc.Default.Register<IWindowFactory, WindowFactory>();
            SimpleIoc.Default.Register<IMessenger>(() => Messenger.Default);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<TimeIntervalsViewModel>();
            SimpleIoc.Default.Register<InstalledProgramsViewModel>();
            SimpleIoc.Default.Register<RecentFolderViewModel>();
            SimpleIoc.Default.Register<PrefetchViewModel>();
            SimpleIoc.Default.Register<ShellBagsViewModel>();
            SimpleIoc.Default.Register<SessionsViewModel>();
            SimpleIoc.Default.Register<SystemTimeChangedViewModel>();
            SimpleIoc.Default.Register<UsbViewModel>();
            SimpleIoc.Default.Register<ReportViewModel>();

            SimpleIoc.Default.GetInstance<ReportViewModel>();
        }

        public MainWindowViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        public TimeIntervalsViewModel TimeIntervalsViewModel => ServiceLocator.Current.GetInstance<TimeIntervalsViewModel>();
        public InstalledProgramsViewModel InstalledProgramsViewModel => ServiceLocator.Current.GetInstance<InstalledProgramsViewModel>();
        public RecentFolderViewModel RecentFolderViewModel => ServiceLocator.Current.GetInstance<RecentFolderViewModel>();
        public PrefetchViewModel PrefetchViewModel => ServiceLocator.Current.GetInstance<PrefetchViewModel>();
        public ShellBagsViewModel ShellBagsViewModel => ServiceLocator.Current.GetInstance<ShellBagsViewModel>();
        public SessionsViewModel SessionsViewModel => ServiceLocator.Current.GetInstance<SessionsViewModel>();
        public SystemTimeChangedViewModel SystemTimeChangedViewModel => ServiceLocator.Current.GetInstance<SystemTimeChangedViewModel>();
        public UsbViewModel UsbViewModel => ServiceLocator.Current.GetInstance<UsbViewModel>();
        public ReportViewModel ReportViewModel => ServiceLocator.Current.GetInstance<ReportViewModel>();

        public static void CleanUp() { }
    }
}
