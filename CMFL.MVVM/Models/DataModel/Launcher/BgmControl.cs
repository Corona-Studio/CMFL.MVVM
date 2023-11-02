using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.ViewModels;
using NAudio.Wave;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public class BgmControl
    {
        private double _bgmVolume = SettingsHelper.Settings.BgmVolume;

        private bool _isMusicMuted;

        private string _musicUrl;
        private WaveOutEvent _playDevice = new WaveOutEvent();

        private int _saveCount;

        public BgmControl()
        {
            RegisterStoppedEvent();
        }

        public bool IsMusicMuted
        {
            get => _isMusicMuted;
            set
            {
                _isMusicMuted = value;

                if (value)
                    MusicEaseOut();
                else
                    MusicEaseIn();
            }
        }

        public double BgmVolume
        {
            get => _bgmVolume;
            set
            {
                _saveCount++;
                var volume = double.Parse(value.ToString("F"));
                _bgmVolume = volume;
                SettingsHelper.Settings.BgmVolume = volume;
                _playDevice.Volume = (float) value;

                if (_saveCount != 5) return;

                SettingsHelper.Save();
                _saveCount = 0;
            }
        }

        public string BgmName { get; set; }

        private void RegisterStoppedEvent()
        {
            _playDevice.PlaybackStopped += (sender, args) =>
            {
                ViewModelLocator.HomePageViewModel.BgmControl.SetMusicUri(_musicUrl);
            };
        }

        public void SetMusicUri(string url)
        {
            try
            {
                Task.Run(() =>
                {
                    _playDevice.Volume = 0;
                    _playDevice.Dispose();

                    _playDevice = new WaveOutEvent();
                    _playDevice.Init(new MediaFoundationReader(url));
                    _musicUrl = url;

                    RegisterStoppedEvent();
                    if (!IsMusicMuted)
                        MusicEaseIn();
                });
            }
            catch (COMException)
            {
            }
        }

        public void Play()
        {
            try
            {
                _playDevice.Play();
            }
            catch
            {
            }
        }

        public void MusicEaseIn()
        {
            Task.Run(async () =>
            {
                _playDevice.Volume = 0;
                _playDevice.Play();
                while (_playDevice.Volume <= (float) SettingsHelper.Settings.BgmVolume)
                {
                    if (SettingsHelper.Settings.BgmVolume - _playDevice.Volume <= 0.05)
                    {
                        _playDevice.Volume = (float) SettingsHelper.Settings.BgmVolume;
                        break;
                    }

                    _playDevice.Volume += 0.01F;
                    await Task.Delay(25).ConfigureAwait(true);
                }
            }).ConfigureAwait(false);
        }

        public void MusicEaseOut()
        {
            Task.Run(async () =>
            {
                while (_playDevice.Volume > 0)
                {
                    if (_playDevice.Volume <= 0.05F)
                    {
                        _playDevice.Volume = 0;
                        break;
                    }

                    _playDevice.Volume -= 0.01F;
                    await Task.Delay(25).ConfigureAwait(true);
                }

                _playDevice.Volume = 0;
            }).ConfigureAwait(false);
        }
    }
}