using System;
using CMFL.MVVM.Services;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //注册页面
            SimpleIoc.Default.Register<LoginWindowViewModel>();
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<LiteWindowViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<PlazaPageViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<GamePageViewModel>();
            SimpleIoc.Default.Register<IntroPage1ViewModel>();
            SimpleIoc.Default.Register<IntroPage2ViewModel>();
            SimpleIoc.Default.Register<IntroPage3ViewModel>();
            SimpleIoc.Default.Register<IntroPage4ViewModel>();
            SimpleIoc.Default.Register<IntroPage5ViewModel>();
            SimpleIoc.Default.Register<WarningPageViewModel>();
            SimpleIoc.Default.Register<GameErrorPageViewModel>();
            SimpleIoc.Default.Register<UpgradePageViewModel>();
            SimpleIoc.Default.Register<AboutPageViewModel>();
            SimpleIoc.Default.Register<ExceptionPageViewModel>();
            SimpleIoc.Default.Register<LoadingPageViewModel>();
            SimpleIoc.Default.Register<PlazaPageViewModel>();
            SimpleIoc.Default.Register<SystemLoadControlViewModel>();
            SimpleIoc.Default.Register<GameTimeCounterViewModel>();
            SimpleIoc.Default.Register<DonatePageViewModel>();
            SimpleIoc.Default.Register<FeedbackPageViewModel>();

            //注册导航服务对象
            SimpleIoc.Default.Register(InitNavigationService);
        }

        public static LoginWindowViewModel LoginWindowViewModel =>
            ServiceLocator.Current.GetInstance<LoginWindowViewModel>();

        public static MainWindowViewModel MainWindowViewModel =>
            ServiceLocator.Current.GetInstance<MainWindowViewModel>();

        public static LiteWindowViewModel LiteWindowViewModel =>
            ServiceLocator.Current.GetInstance<LiteWindowViewModel>();

        public static HomeViewModel HomePageViewModel => ServiceLocator.Current.GetInstance<HomeViewModel>();
        public static PlazaPageViewModel PlazaPageViewModel => ServiceLocator.Current.GetInstance<PlazaPageViewModel>();
        public static SettingViewModel SettingPageViewModel => ServiceLocator.Current.GetInstance<SettingViewModel>();
        public static GamePageViewModel GamePageViewModel => ServiceLocator.Current.GetInstance<GamePageViewModel>();

        public static IntroPage1ViewModel IntroPage1ViewModel =>
            ServiceLocator.Current.GetInstance<IntroPage1ViewModel>();

        public static IntroPage2ViewModel IntroPage2ViewModel =>
            ServiceLocator.Current.GetInstance<IntroPage2ViewModel>();

        public static IntroPage3ViewModel IntroPage3ViewModel =>
            ServiceLocator.Current.GetInstance<IntroPage3ViewModel>();

        public static IntroPage4ViewModel IntroPage4ViewModel =>
            ServiceLocator.Current.GetInstance<IntroPage4ViewModel>();

        public static IntroPage5ViewModel IntroPage5ViewModel =>
            ServiceLocator.Current.GetInstance<IntroPage5ViewModel>();

        public static WarningPageViewModel WarningPageViewModel =>
            ServiceLocator.Current.GetInstance<WarningPageViewModel>();

        public static GameErrorPageViewModel GameErrorPageViewModel =>
            ServiceLocator.Current.GetInstance<GameErrorPageViewModel>();

        public static UpgradePageViewModel UpgradePageViewModel =>
            ServiceLocator.Current.GetInstance<UpgradePageViewModel>();

        public static AboutPageViewModel AboutPageViewModel => ServiceLocator.Current.GetInstance<AboutPageViewModel>();

        public static ExceptionPageViewModel ExceptionPageViewModel =>
            ServiceLocator.Current.GetInstance<ExceptionPageViewModel>();

        public static LoadingPageViewModel LoadingPageViewModel =>
            ServiceLocator.Current.GetInstance<LoadingPageViewModel>();

        public static SystemLoadControlViewModel SystemLoadControlViewModel =>
            ServiceLocator.Current.GetInstance<SystemLoadControlViewModel>();

        public static GameTimeCounterViewModel GameTimeCounterViewModel =>
            ServiceLocator.Current.GetInstance<GameTimeCounterViewModel>();

        public static DonatePageViewModel DonatePageViewModel =>
            ServiceLocator.Current.GetInstance<DonatePageViewModel>();

        public static FeedbackPageViewModel FeedbackPageViewModel =>
            ServiceLocator.Current.GetInstance<FeedbackPageViewModel>();

        /// <summary>
        ///     创建NavigationService对象
        /// </summary>
        /// <returns>NavigationService</returns>
        private static INavigationService InitNavigationService()
        {
            var service = new FrameNavigationService();
            service.Configure("HomePage", new Uri("pack://application:,,,/Views/HomePage.xaml"));
            service.Configure("PlazaPage", new Uri("pack://application:,,,/Views/PlazaPage.xaml"));
            service.Configure("SettingPage", new Uri("pack://application:,,,/Views/SettingView.xaml"));
            service.Configure("GamePage", new Uri("pack://application:,,,/Views/GamePage.xaml"));
            service.Configure("WarningPage", new Uri("pack://application:,,,/Views/WarningPage.xaml"));
            service.Configure("LostFilesFixPage", new Uri("pack://application:,,,/Views/LostFilesFixPage.xaml"));
            service.Configure("IntroPage1", new Uri("pack://application:,,,/Views/IntroPage1.xaml"));
            service.Configure("IntroPage2", new Uri("pack://application:,,,/Views/IntroPage2.xaml"));
            service.Configure("IntroPage3", new Uri("pack://application:,,,/Views/IntroPage3.xaml"));
            service.Configure("IntroPage4", new Uri("pack://application:,,,/Views/IntroPage4.xaml"));
            service.Configure("IntroPage5", new Uri("pack://application:,,,/Views/IntroPage5.xaml"));
            service.Configure("GameErrorPage", new Uri("pack://application:,,,/Views/GameErrorPage.xaml"));
            service.Configure("UpgradePage", new Uri("pack://application:,,,/Views/UpgradePage.xaml"));
            service.Configure("AboutPage", new Uri("pack://application:,,,/Views/AboutPage.xaml"));
            service.Configure("ExceptionPage", new Uri("pack://application:,,,/Views/ExceptionPage.xaml"));
            service.Configure("LoadingPage", new Uri("pack://application:,,,/Views/LoadingPage.xaml"));
            service.Configure("DonatePage", new Uri("pack://application:,,,/Views/DonatePage.xaml"));
            service.Configure("FeedbackPage", new Uri("pack://application:,,,/Views/FeedbackPage.xaml"));
            return service;
        }

        public static void CleanUp()
        {
            MainWindowViewModel.Dispose();
            LiteWindowViewModel.Dispose();
            HomePageViewModel.Dispose();
            PlazaPageViewModel.Dispose();
            SettingPageViewModel.Dispose();
            GamePageViewModel.Dispose();
            IntroPage1ViewModel.Dispose();
            IntroPage2ViewModel.Dispose();
            IntroPage3ViewModel.Dispose();
            IntroPage4ViewModel.Dispose();
            IntroPage5ViewModel.Dispose();
            WarningPageViewModel.Dispose();
            GameErrorPageViewModel.Dispose();
            UpgradePageViewModel.Dispose();
            AboutPageViewModel.Dispose();
            ExceptionPageViewModel.Dispose();
            LoadingPageViewModel.Dispose();
            SystemLoadControlViewModel.Dispose();
            GameTimeCounterViewModel.Dispose();
            DonatePageViewModel.Dispose();
        }
    }
}