using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Launcher.SocketServer;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.Models.DataModel.Launcher.Auth;
using GalaSoft.MvvmLight.Threading;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjCrypto.Class.Helper;

namespace CMFL.MVVM.ViewModels
{
    public class LoginWindowViewModel : PropertyChange
    {
        private bool _buttonBackVisibility = true;

        private bool _buttonCheckingVisibility;

        private bool _buttonLoginVisibility = true;

        private string _forgotEmail;

        private string _forgotPassword;
        private bool _isForgotPassword;

        private string _loginPassword;
        private string _loginUsername;

        private string _regButtonText = LanguageHelper.GetField("Registration");

        private string _regEmail;

        private string _regPassword;

        private string _regUsername;

        private bool _rememberMe = SettingsHelper.Settings.IsRememberMe;

        private string _resultDetail;

        private string _resultTitle;

        private string _verificationCode;

        public LoginWindowViewModel()
        {
            if (IsInDesignMode) return;

            if (!RememberMe) return;

            LoginUsername = SettingsHelper.Settings.LUsername;
            LoginPassword = StringEncryptHelper.AesDecrypt(SettingsHelper.Settings.LPassword);
        }

        public string LoginUsername
        {
            get => _loginUsername;
            set
            {
                _loginUsername = value;
                OnPropertyChanged(nameof(LoginUsername));
            }
        }

        public string LoginPassword
        {
            get => _loginPassword;
            set
            {
                _loginPassword = value;
                OnPropertyChanged(nameof(LoginPassword));
            }
        }

        public string RegUsername
        {
            get => _regUsername;
            set
            {
                _regUsername = value;
                OnPropertyChanged(nameof(RegUsername));
            }
        }

        public string RegPassword
        {
            get => _regPassword;
            set
            {
                _regPassword = value;
                OnPropertyChanged(nameof(RegPassword));
            }
        }

        public string RegEmail
        {
            get => _regEmail;
            set
            {
                _regEmail = value;
                OnPropertyChanged(nameof(RegEmail));
            }
        }

        public string ForgotPassword
        {
            get => _forgotPassword;
            set
            {
                _forgotPassword = value;
                OnPropertyChanged(nameof(ForgotPassword));
            }
        }

        public string ForgotEmail
        {
            get => _forgotEmail;
            set
            {
                _forgotEmail = value;
                OnPropertyChanged(nameof(ForgotEmail));
            }
        }

        public string VerificationCode
        {
            get => _verificationCode;
            set
            {
                _verificationCode = value;
                OnPropertyChanged(nameof(VerificationCode));
            }
        }

        public string ResultTitle
        {
            get => _resultTitle;
            set
            {
                _resultTitle = value;
                OnPropertyChanged(nameof(ResultTitle));
            }
        }

        public string ResultDetail
        {
            get => _resultDetail;
            set
            {
                _resultDetail = value;
                OnPropertyChanged(nameof(ResultDetail));
            }
        }

        public string RegButtonText
        {
            get => _regButtonText;
            set
            {
                _regButtonText = value;
                OnPropertyChanged(nameof(RegButtonText));
            }
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                OnPropertyChanged(nameof(RememberMe));
            }
        }

        public bool ButtonBackVisibility
        {
            get => _buttonBackVisibility;
            set
            {
                _buttonBackVisibility = value;
                OnPropertyChanged(nameof(ButtonBackVisibility));
            }
        }

        public bool ButtonLoginVisibility
        {
            get => _buttonLoginVisibility;
            set
            {
                _buttonLoginVisibility = value;
                OnPropertyChanged(nameof(ButtonLoginVisibility));
            }
        }

        public bool ButtonCheckingVisibility
        {
            get => _buttonCheckingVisibility;
            set
            {
                _buttonCheckingVisibility = value;
                OnPropertyChanged(nameof(ButtonCheckingVisibility));
            }
        }

        public ICommand LoginCommand
        {
            get { return new DelegateCommand(obj => { Login(); }); }
        }

        public ICommand RegCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    _isForgotPassword = false;
                    CheckRegistration();
                });
            }
        }

        public ICommand ForgotPasswordCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    _isForgotPassword = true;
                    var resultJson = LauncherSocketServerHelper
                        .Send("FORGET", new[] {ForgotEmail});
                    var result = JsonConvert.DeserializeObject<CommandResultModel>(resultJson);
                    if (result.Result?.Equals("SUCCEEDED", StringComparison.Ordinal) ?? false)
                    {
                        WindowLogin.Instance.ShowToast("发送邮箱验证码成功！", "请查看您的邮件", Color.FromRgb(2, 199, 83));
                        WindowLogin.Instance.GoToVerifyPage(null, null);
                    }
                    else
                    {
                        WindowLogin.Instance.ShowToast("发送邮箱验证码失败！", result.Message ?? "未知错误",
                            Color.FromRgb(255, 102, 102));
                    }
                });
            }
        }

        public ICommand VerifyCommand
        {
            get { return new DelegateCommand(obj => { StartVerification(); }); }
        }

        #region 注册代码

        /// <summary>
        ///     注册代码
        /// </summary>
        private void CheckRegistrationInformation()
        {
            if (string.IsNullOrEmpty(RegUsername) || string.IsNullOrEmpty(RegPassword) ||
                string.IsNullOrEmpty(RegEmail))
            {
                WindowLogin.Instance.ShowToast(LanguageHelper.GetFields("Registration|Failed"),
                    LanguageHelper.GetFields("Reason|RegistrationCheckFailedReason", ": "),
                    Color.FromRgb(255, 102, 102));
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ButtonBackVisibility = true;
                    RegButtonText = LanguageHelper.GetField("Registration");
                });
            }
            else
            {
                var resultJson =
                    LauncherSocketServerHelper.Send("REGISTER", new[] {RegEmail, RegUsername, RegPassword});

                if (string.IsNullOrEmpty(resultJson))
                {
                    WindowLogin.Instance.ShowToast("注册失败", "原因：Null。", Color.FromRgb(255, 102, 102));
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ButtonBackVisibility = true;
                        RegButtonText = LanguageHelper.GetField("Registration");
                    });
                    return;
                }

                var result = JsonConvert.DeserializeObject<CommandResultModel>(resultJson);
                if (!int.TryParse(result.Result, out var code))
                {
                    WindowLogin.Instance.ShowToast("注册失败", "原因：解析状态码时出现了问题。", Color.FromRgb(255, 102, 102));
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ButtonBackVisibility = true;
                        RegButtonText = LanguageHelper.GetField("Registration");
                    });
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ButtonBackVisibility = true;
                    RegButtonText = LanguageHelper.GetField("Registration");
                });

                switch ((RegisterResultType) code)
                {
                    case RegisterResultType.RegisterSuccess:
                        _isForgotPassword = false;
                        WindowLogin.Instance.ShowToast("注册成功", "请查看邮箱验证码。", Color.FromRgb(2, 199, 83));
                        break;
                    case RegisterResultType.DuplicateInfo:
                        WindowLogin.Instance.ShowToast("注册失败", "原因：已经有人使用相同的信息注册了。", Color.FromRgb(255, 102, 102));
                        return;
                    case RegisterResultType.Unknown:
                        WindowLogin.Instance.ShowToast("注册失败", "原因：未知错误，请联系管理员。", Color.FromRgb(255, 102, 102));
                        return;
                }

                WindowLogin.Instance.GoToVerifyPage(null, null);
            }
        }

        #endregion

        private void StartVerification()
        {
            string resultJson;

            if (_isForgotPassword)
                resultJson = LauncherSocketServerHelper
                    .Send("RESETPWD", new[] {ForgotEmail, VerificationCode, ForgotPassword});
            else
                resultJson = LauncherSocketServerHelper
                    .Send("REGVERIFY", new[] {RegEmail, VerificationCode});

            if (string.IsNullOrEmpty(resultJson))
            {
                WindowLogin.Instance.ShowToast("验证失败", "原因：未知错误，请联系管理员。", Color.FromRgb(255, 102, 102));
                return;
            }

            var result = JsonConvert.DeserializeObject<CommandResultModel>(resultJson);
            switch (result.Result)
            {
                case "SUCCEEDED":
                    ResultTitle = $"{(_isForgotPassword ? "找回" : "注册")}成功";
                    ResultDetail = $"{(_isForgotPassword ? "密码找回成功" : "CMFL账号创建成功！")}，赶快试试登录吧！";
                    if (!_isForgotPassword)
                        Analytics.TrackEvent(AnalyticsEventNames.RegisterNewLauncherAccount);
                    WindowLogin.Instance.GoToSuccessPage(null, null);
                    break;
                case "FAILED":
                    WindowLogin.Instance.ShowToast("验证失败", $"原因：{result.Message}", Color.FromRgb(255, 102, 102));
                    break;
            }
        }

        #region 登陆代码（验证用户名和密码）

        /// <summary>
        ///     登录_验证用户名与密码
        /// </summary>
        public async void CheckUsernamePassword()
        {
            if (string.IsNullOrEmpty(LoginUsername) || string.IsNullOrEmpty(LoginPassword))
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    WindowLogin.Instance.ShowToast("登录失败", "原因：用户名和密码不能为空！", Color.FromRgb(255, 102, 102));
                    ButtonLoginVisibility = true;
                    ButtonCheckingVisibility = false;
                });
                return;
            }

            var result = await LauncherHelper.CheckLauncherAuthInfo(LoginUsername, LoginPassword).ConfigureAwait(true);

            if (string.IsNullOrEmpty(result))
            {
                WindowLogin.Instance.ShowToast("验证失败", "原因：未知错误，请联系管理员。", Color.FromRgb(255, 102, 102));
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ButtonLoginVisibility = true;
                    ButtonCheckingVisibility = false;
                });
                return;
            }

            if (result.Equals("5", StringComparison.Ordinal))
            {
                LogHelper.WriteLogLine("成功登陆到CMFL账号", LogHelper.LogLevels.Info);
                if (RememberMe)
                {
                    SettingsHelper.Settings.LUsername = LoginUsername;
                    SettingsHelper.Settings.LPassword = StringEncryptHelper.AesEncrypt(LoginPassword);
                    SettingsHelper.Settings.IsRememberMe = true;
                    SettingsHelper.Save();
                }

                SettingsHelper.Settings.LoggedInToCMFL = true;
                ViewModelLocator.MainWindowViewModel.LoginFinished();
                DispatcherHelper.CheckBeginInvokeOnUI(WindowLogin.Instance.Close);
            }
            else
            {
                var authResult = Enum.TryParse(result, out AuthResultType auth) ? auth : AuthResultType.UnknownError;

                var reason = authResult switch
                {
                    AuthResultType.WrongCredential => "用户名或密码错误",
                    AuthResultType.UnknownError => "未知错误",
                    AuthResultType.AlreadyAuth => "您已经登陆了！",
                    AuthResultType.NotVerified => "邮箱未验证！",
                    _ => "未知错误"
                };
                SettingsHelper.Settings.LoggedInToCMFL = false;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    WindowLogin.Instance.ShowToast("登录失败", $"原因：{reason}", Color.FromRgb(255, 102, 102));
                });
                LogHelper.WriteLogLine($"无法登陆到CMFL账号，{reason}", LogHelper.LogLevels.Error);
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ButtonLoginVisibility = true;
                ButtonCheckingVisibility = false;
            });
        }

        #endregion

        #region 登录按钮事件

        /// <summary>
        ///     登录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login()
        {
            try
            {
                if (RememberMe)
                {
                    SettingsHelper.Settings.LUsername = LoginUsername;
                    SettingsHelper.Settings.LPassword = StringEncryptHelper.AesEncrypt(LoginPassword);
                    SettingsHelper.Settings.IsRememberMe = true;
                }
                else
                {
                    SettingsHelper.Settings.LUsername = string.Empty;
                    SettingsHelper.Settings.LPassword = string.Empty;
                    SettingsHelper.Settings.IsRememberMe = false;
                }
            }
            catch (ArgumentException ex)
            {
                NotifyHelper.ShowNotification("启动验证进程失败", "请查看日志", 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine("启动登陆进程失败", LogHelper.LogLevels.Error);
                LogHelper.WriteError(ex);
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ButtonLoginVisibility = false;
                ButtonCheckingVisibility = true;
            });

            if (Enumerable.Range(60, 10).RandomSample().Equals(66))
                NotifyHelper.ShowNotification("偷偷卖个萌嘤嘤嘤！", "一定看不到（跑", 1500, ToolTipIcon.Info); //日常随机彩蛋（跑

            CheckUsernamePassword();
        }

        #endregion

        #region 注册按钮事件

        private void CheckRegistration()
        {
            RegButtonText = "请稍候";
            ButtonBackVisibility = false;
            CheckRegistrationInformation();
        }

        #endregion
    }
}