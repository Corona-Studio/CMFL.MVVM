using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace CMFL.MVVM.Services
{
    public class FrameNavigationService : ViewModelBase, IFrameNavigationService
    {
        #region Fields

        private readonly Dictionary<string, Uri> _pagesByKey;
        private readonly List<string> _historic;
        private string _currentPageKey;

        #endregion

        #region Properties

        public string CurrentPageKey
        {
            get => _currentPageKey;

            private set
            {
                if (_currentPageKey == value) return;

                _currentPageKey = value;
                Set(ref _currentPageKey, value);
            }
        }

        public object Parameter { get; private set; }

        #endregion

        #region Ctors and Methods

        public FrameNavigationService()
        {
            _pagesByKey = new Dictionary<string, Uri>();
            _historic = new List<string>();
        }

        public void GoBack()
        {
            if (_historic.Count > 1)
            {
                _historic.RemoveAt(_historic.Count - 1);
                NavigateTo(_historic.Last(), null);
            }
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public virtual void NavigateTo(string pageKey, object parameter)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var frame = MainWindow.Instance.MainFrame;

                ViewModelLocator.LoadingPageViewModel.Tips =
                    $"({LanguageHelper.GetFields("Tips")}：{TipsHelper.Show()})";

                lock (_pagesByKey)
                {
                    frame.Source = _pagesByKey["LoadingPage"];
                }

                Task.Run(async () =>
                {
                    await Task.Delay(1000).ConfigureAwait(true);

                    lock (_pagesByKey)
                    {
                        if (!_pagesByKey.ContainsKey(pageKey))
                            throw new ArgumentException($"No such page: {pageKey} ", nameof(pageKey));
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { frame.Navigate(_pagesByKey[pageKey]); });
                        //frame.Source = _pagesByKey[pageKey];
                    }
                }).ConfigureAwait(false);

                Parameter = parameter;
                _historic.Add(pageKey);
                CurrentPageKey = pageKey;
            });
        }

        public void Configure(string key, Uri pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                    _pagesByKey[key] = pageType;
                else
                    _pagesByKey.Add(key, pageType);
            }
        }

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1) return null;

            for (var i = 0; i < count; i++)
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name) return frameworkElement;

                    frameworkElement = GetDescendantFromName(frameworkElement, name);
                    if (frameworkElement != null) return frameworkElement;
                }

            return null;
        }

        #endregion
    }
}