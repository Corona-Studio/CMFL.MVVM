using CMFL.MVVM.Class.Helper.Other;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CMFL.MVVM.Models.DataModel.LiteLoader
{
    public class LiteLoaderBuildModel :PropertyChange
    {
        private LiteLoaderVersionMetaModel _versionMeta;
        public LiteLoaderVersionMetaModel VersionMeta
        {
            get => _versionMeta;
            set
            {
                if (value == null) return;

                _versionMeta = value;
                Version = _versionMeta.Version;
                VersionType = _versionMeta.Type;

                VersionTypeBgColor = _versionMeta.Type switch
                {
                    "RELEASE" => (SolidColorBrush) Application.Current?.FindResource("SystemGreen1"),
                    "SNAPSHOT" => (SolidColorBrush) Application.Current?.FindResource("SystemPink1"),
                    _ => (SolidColorBrush) Application.Current?.FindResource("SystemBlack2")
                };

                Time = TimeHelper.LongDateTimeToDateTimeString(_versionMeta.Build.TimeStamp, false);
                McVersion = _versionMeta.McVersion;
            }
        }

        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        private string _versionType;

        public string VersionType
        {
            get => _versionType;
            set
            {
                _versionType = value;
                OnPropertyChanged(nameof(VersionType));
            }
        }

        private SolidColorBrush _versionTypeBgColor;

        public SolidColorBrush VersionTypeBgColor
        {
            get => _versionTypeBgColor;
            set
            {
                _versionTypeBgColor = value;
                OnPropertyChanged(nameof(VersionTypeBgColor));
            }
        }

        private string _time;

        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        private string _mcVersion;

        public string McVersion
        {
            get => _mcVersion;
            set
            {
                _mcVersion = value;
                OnPropertyChanged(nameof(McVersion));
            }
        }

        private string _installMessage;

        private bool _installSnackBarActive;

        public string InstallMessage
        {
            get => _installMessage;
            set
            {
                _installMessage = value;
                OnPropertyChanged(nameof(InstallMessage));
            }
        }

        public bool InstallSnackBarActive
        {
            get => _installSnackBarActive;
            set
            {
                _installSnackBarActive = value;
                OnPropertyChanged(nameof(InstallSnackBarActive));
            }
        }


        public ICommand DownloadLiteLoaderCommand
        {
            get { return new DelegateCommand(obj => { }); }
        }
    }
}