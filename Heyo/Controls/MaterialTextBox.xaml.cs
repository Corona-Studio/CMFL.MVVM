using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Heyo.Controls
{
    /// <summary>
    ///     TextBoxEx.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialTextBox : UserControl
    {
        private const int AnimationSpeed = 6;
        private static readonly PowerEase easeFaunction = new PowerEase {EasingMode = EasingMode.EaseIn, Power = 4};

        private readonly DoubleAnimation lineX1Anim = new DoubleAnimation
            {EasingFunction = easeFaunction, SpeedRatio = AnimationSpeed};

        private readonly DoubleAnimation lineX2Anim = new DoubleAnimation
            {EasingFunction = easeFaunction, SpeedRatio = AnimationSpeed};

        private readonly ThicknessAnimation tipAnim = new ThicknessAnimation
            {EasingFunction = easeFaunction, SpeedRatio = AnimationSpeed};

        private string _password = "";
        public string[] chars = {"@", "#", "$", "%", "&"};

        private bool keyDown;

        public MaterialTextBox()
        {
            InitializeComponent();

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text.Length < 1)
                {
                    SetLineLength(5, ActualWidth);
                    SetTipMargin(new Thickness(0, 0, 0, 25));
                }
            };
            textBox.LostFocus += (s, e) =>
            {
                if (textBox.Text.Length < 1)
                {
                    SetLineLength(0, 0);
                    SetTipMargin(new Thickness(5, 20, 0, 0));
                }
            };

            textBox.TextChanged += TextBoxEx_TextChanged;
            textBox.PreviewKeyDown += TextBoxEx_PreviewKeyDown;
        }

        public object WarningContent
        {
            get => warnLabel.Content;
            set => warnLabel.Content = value;
        }

        public string Text
        {
            get => textBox.Text;
            set
            {
                textBox.Text = value;
                if (value.Length > 0)
                {
                    line.X1 = 5;
                    line.X2 = ActualWidth;
                    viewbox.Margin = new Thickness(0, 0, 0, 25);
                }
            }
        }

        public object Tip
        {
            get => tipLabel.Content;
            set => tipLabel.Content = value;
        }

        public string TipText { get; set; }
        public bool OnlyNumber { get; set; }

        public bool PasswordBox { get; set; } = false;

        public string Password
        {
            get => _password;
            set
            {
                if (value.Length - _password.Length > 1)
                {
                    Text = "";
                    for (var i = 0; i < value.Length; i++) Text += RandomString();
                }

                _password = value;
            }
        }

        public event TextChangedEventHandler TextChanged;

        private void SetLineLength(double x1, double x2)
        {
            lineX1Anim.To = x1;
            lineX2Anim.To = x2;
            line.BeginAnimation(Line.X1Property, lineX1Anim);
            line.BeginAnimation(Line.X2Property, lineX2Anim);
        }

        private void SetTipMargin(Thickness margin)
        {
            tipAnim.To = margin;
            viewbox.BeginAnimation(MarginProperty, tipAnim);
        }

        private void TextBoxEx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            keyDown = true;

            if (e.Key == Key.Space && (OnlyNumber || PasswordBox))
                e.Handled = true;

            if (PasswordBox)
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    e.Handled = true;
        }

        private void TextBoxEx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordBox && keyDown)
            {
                keyDown = false;
                var changes = e.Changes;
                foreach (var item in changes)
                {
                    if (item.AddedLength > 0)
                    {
                        var select = textBox.SelectionStart;
                        var addedString = textBox.Text.Substring(item.Offset, item.AddedLength);
                        Password = Password.Insert(item.Offset, addedString);
                        textBox.Text = textBox.Text.Remove(item.Offset, 1).Insert(item.Offset, RandomString());
                        textBox.SelectionStart = select;
                    }

                    if (item.RemovedLength > 0 && Password.Length >= item.RemovedLength)
                        Password = Password.Remove(item.Offset, item.RemovedLength);
                }
            }

            TextChanged?.Invoke(sender, e);
        }

        private string RandomString()
        {
            var r = new Random();
            return chars[r.Next(0, chars.Length - 1)];
        }

        private void textBox1_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string) e.DataObject.GetData(typeof(string));
                if (!isNumberic(text)) e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }

            if (PasswordBox) e.CancelCommand();
        }


        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (OnlyNumber)
            {
                if (!isNumberic(e.Text))
                    e.Handled = true;
                else
                    e.Handled = false;
            }
        }

        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (var c in _string)
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            return true;
        }
    }
}