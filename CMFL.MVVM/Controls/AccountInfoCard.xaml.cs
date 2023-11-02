using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Graphic;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Other;
using CMFL.MVVM.Class.Helper.RandomAvatarHelper;
using CMFL.MVVM.Models.DataModel.Game.Account;
using GalaSoft.MvvmLight.Threading;
using Heyo.Class.Helper;
using ProjBobcat.Authenticator;
using ProjBobcat.Class.Helper;
using ProjCrypto.Class.Helper;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     AccountInfoCard.xaml 的交互逻辑
    /// </summary>
    public partial class AccountInfoCard : UserControl, IDisposable
    {
        public static readonly DependencyProperty AccountProperty =
            DependencyProperty.Register("AccountInfo", typeof(AccountInfo), typeof(AccountInfoCard),
                new PropertyMetadata(null));

        private readonly IEnumerable<string> _colors = new[]
        {
            "#536dfe",
            "#ef5350",
            "#00c853",
            "#ff5722",
            "#546e7a"
        };

        private readonly Dispatcher CurrentDispatcher = Dispatcher.CurrentDispatcher;


        public AccountInfoCard()
        {
            InitializeComponent();
            MainBorder.Background = new SolidColorBrush(ColorTranslator.FromHtml(_colors.RandomSample()).ToMedia());
        }

        /// <summary>
        ///     账户信息
        /// </summary>
        public AccountInfo AccountInfo
        {
            get => (AccountInfo) GetValue(AccountProperty);
            set => SetValue(AccountProperty, value);
        }

        private void Login()
        {
            if (AccountInfo is OnlineAccount online)
            {
                EmailText.Text = StringEncryptHelper.StringDecrypt(online.Email);
                AuthMethod.Text = LanguageHelper.GetField("OnlineMode");

                Task.Run(async () =>
                {
                    var dtNow = DateTime.Now;
                    if (dtNow.TimeDiff(online.LastAuthTime) <= 60 * 10 && dtNow.TimeDiff(online.LastAuthTime) != 0)
                    {
                        var tempAvatar =
                            await ImageHelper.GetImageSourceFromUri(
                                new Uri($"https://minotar.net/avatar/{online.Uuid.ToString()}")).ConfigureAwait(true);
                        tempAvatar.Freeze();
                        CurrentDispatcher.Invoke(() =>
                        {
                            DisplayName.Text = online.DisplayName;
                            Avatar.Source = tempAvatar;
                            AuthStatus.Text =
                                $"{(online.LastAuthState ? LanguageHelper.GetFields("Auth|Succeeded") : LanguageHelper.GetFields("Auth|Failed"))}（{LanguageHelper.GetField("Earlier")}）";
                            UUIDText.Text = $"{online.Uuid.ToString("D").Split('-').First()}-〇-〇-〇-〇";
                            UUIDText.ToolTip = online.Uuid.ToString();
                            ProgressIndicator.Visibility = Visibility.Hidden;
                        });
                        return;
                    }

                    var yggdrasilLogin = new YggdrasilAuthenticator
                    {
                        AuthServer = online.AuthServer?.TrimEnd('/'),
                        Email = StringEncryptHelper.StringDecrypt(online.Email),
                        Password = StringEncryptHelper.StringDecrypt(online.Password),
                        LauncherProfileParser = GameHelper.Core.VersionLocator.LauncherProfileParser
                    };

                    var authResult = await yggdrasilLogin.AuthTaskAsync(true).ConfigureAwait(true);

                    if (authResult.AuthStatus != ProjBobcat.Class.Model.AuthStatus.Succeeded)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            AuthStatus.Text = LanguageHelper.GetFields("Auth|Failed");
                        });

                        return;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        DisplayName.Text = authResult.SelectedProfile.Name;
                        AuthStatus.Text = LanguageHelper.GetField("AlreadyAuth");
                        var temp = new Guid(authResult.SelectedProfile.Id).ToString("D").Split('-');
                        UUIDText.Text = $"{temp.First()}-〇-〇-〇-〇";
                        UUIDText.ToolTip = authResult.SelectedProfile.Id;
                    });

                    var avatar =
                        await ImageHelper.GetImageSourceFromUri(
                                new Uri($"https://minotar.net/avatar/{authResult.SelectedProfile.Id}"))
                            .ConfigureAwait(false);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Avatar.Source = avatar;
                        online.LastAuthState = true;
                        online.UpdateAuthTime(DateTime.Now);
                        online.DisplayName = authResult.SelectedProfile.Name;
                        ProgressIndicator.Visibility = Visibility.Hidden;
                    });
                    online.Save();
                });
            }
        }

        private void GetAvatarFromDisk()
        {
            DisplayName.Text = AccountInfo.DisplayName;
            if (AccountInfo is OfflineAccount offline && !string.IsNullOrEmpty(offline.SkinPath))
            {
                using var headBitmap = BitmapHelper.SkinToHeadFile(offline.SkinPath, 128);
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Avatar.Source = headBitmap.ToImageSource();
                    headBitmap.Dispose();
                });
            }
            else
            {
                var randomBitmap = RandomAvatarBuilder.Build(100).SetPadding(10).ToImage();
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Avatar.Source = randomBitmap.DrawingImageToImageSource();
                });
                randomBitmap.Dispose();
            }
        }

        private void AccountInfoCard_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!AccountInfo.IsOnlineAccount())
            {
                EmailText.Text = LanguageHelper.GetField("None");
                AuthStatus.Text = LanguageHelper.GetField("OfflineMode");
                AuthMethod.Text = LanguageHelper.GetField("OfflineMode");
                var uuid = new Guid().ToString();
                UUIDText.Text = $"{uuid.Split('-').First()}-〇-〇-〇-〇";
                UUIDText.ToolTip = uuid;
                ProgressIndicator.Visibility = Visibility.Hidden;
                GetAvatarFromDisk();
            }
            else
            {
                Login();
            }
        }

        private void UseAccount(object sender, RoutedEventArgs e)
        {
            AccountInfo.SelectAccount();
            NotifyHelper
                .GetBasicMessageWithBadge(
                    $"\"{AccountInfo.DisplayName}\"{LanguageHelper.GetField("BecomingYourLoginAccount")}",
                    NotifyHelper.MessageType.Success).Queue();
        }

        private void DeleteAccount(object sender, RoutedEventArgs e)
        {
            if (SettingsHelper.Settings.AccountInfos.Count - 1 == 0)
            {
                NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("DeleteAccountFailedWarning"),
                        NotifyHelper.MessageType.Warning)
                    .Queue();
                return;
            }

            if (AccountInfo.IsOnlineAccount())
            {
                var online = AccountInfo as OnlineAccount;
                online?.Delete();
            }
            else
            {
                var offline = AccountInfo as OfflineAccount;
                offline?.Delete();
            }

            NotifyHelper
                .GetBasicMessageWithBadge(
                    $"\"{AccountInfo.DisplayName}\"{LanguageHelper.GetField("AccountDeleted")}",
                    NotifyHelper.MessageType.Success).Queue();
        }

        #region IDisposible Support

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~AccountInfoCard()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
        }

        #endregion
    }
}