using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Heyo.Controls
{
    /// <summary>
    ///     TextBox.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxEx : TextBox
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(TextBoxEx)
            );

        public static readonly DependencyProperty TipForegroundBrushProperty =
            DependencyProperty.Register(
                "TipForegroundBrush",
                typeof(Brush),
                typeof(TextBoxEx)
            );

        public static readonly DependencyProperty TipTextProperty =
            DependencyProperty.Register(
                "TipText",
                typeof(string),
                typeof(TextBoxEx)
            );

        private Brush _hintTextColor = new SolidColorBrush(Colors.Gray);

        private bool _keyDown;
        private Brush _originForeground;
        public string[] chars = {"@", "#", "$", "%", "&"};

        public TextBoxEx()
        {
            InitializeComponent();
            Password = "";
        }

        public string Password
        {
            get => (string) GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public Brush TipForegroundBrush
        {
            get => (Brush) GetValue(TipForegroundBrushProperty);
            set
            {
                SetValue(TipForegroundBrushProperty, value);
                _hintTextColor = value;
            }
        }

        public string TipText
        {
            get => (string) GetValue(TipTextProperty);
            set => SetValue(TipTextProperty, value);
        }

        public bool OnlyNumber { get; set; }

        public bool PasswordBox { get; set; } = false;

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _originForeground = Foreground.Clone();
            if (Foreground is SolidColorBrush)
            {
                var color = (Foreground as SolidColorBrush).Color;
                //_hintTextColor = BorderBrush;//new SolidColorBrush(Color.FromArgb((byte)(color.A * 0.9), color.R, color.G, color.B));
            }

            TextChanged += TextBoxEx_TextChanged;
            PreviewKeyDown += TextBoxEx_PreviewKeyDown;

            CaretBrush = Foreground;
            Foreground = _hintTextColor;
        }

        private void TextBoxEx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _keyDown = true;

            if (e.Key == Key.Space && (OnlyNumber || PasswordBox))
                e.Handled = true;

            if (!PasswordBox) return;
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) e.Handled = true;
        }


        private void TextBoxEx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordBox && _keyDown)
            {
                _keyDown = false;
                var changes = e.Changes;
                foreach (var item in changes)
                {
                    if (item.AddedLength > 0)
                    {
                        var select = SelectionStart;
                        var addedString = Text.Substring(item.Offset, item.AddedLength);
                        Password = Password.Insert(item.Offset, addedString);
                        Text = Text.Remove(item.Offset, 1).Insert(item.Offset, RandomString());
                        SelectionStart = select;
                    }

                    if (item.RemovedLength > 0 && Password.Length >= item.RemovedLength)
                        Password = Password.Remove(item.Offset, item.RemovedLength);
                }
            }
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
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会高10毫秒左右
                    return false;
            return true;
        }
    }
}