using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CMFL.MVVM.Class.Helper.Launcher
{
    public static class LanguageHelper
    {
        private static Tuple<string, Uri> _currentLanguage;

        private static readonly Dictionary<string, Tuple<string, Uri>> Languages =
            new Dictionary<string, Tuple<string, Uri>>();

        private static readonly List<string> LanguageNamesList = new List<string>();

        private static readonly StringBuilder NamesSb = new StringBuilder();
        private static string _lastKey = string.Empty, _lastValue = string.Empty;

        static LanguageHelper()
        {
            RegisterLanguage("zh_CN", "简体中文", new Uri("/Assets/Language/zh_CN.xaml", UriKind.Relative));
            RegisterLanguage("zh_HK", "繁體中文（香港）", new Uri("/Assets/Language/zh_HK.xaml", UriKind.Relative));
            RegisterLanguage("zh_TW", "繁體中文（台灣）", new Uri("/Assets/Language/zh_TW.xaml", UriKind.Relative));
            RegisterLanguage("en_US", "English (US)", new Uri("/Assets/Language/en_US.xaml", UriKind.Relative));
            RegisterLanguage("en_UK", "English (UK)", new Uri("/Assets/Language/en_UK.xaml", UriKind.Relative));
            RegisterLanguage("es_SP", "Espanol", new Uri("/Assets/Language/es_SP.xaml", UriKind.Relative));

            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _currentLanguage = Languages[LanguageNamesList.Contains(lang) ? lang : "zh_CN"];
        }

        private static void RegisterLanguage(string langName, string name, Uri uri)
        {
            Languages.Add(langName, new Tuple<string, Uri>(name, uri));
            LanguageNamesList.Add(name);
        }

        public static List<string> GetAllLanguageNames()
        {
            return LanguageNamesList;
        }

        public static void ChangeLanguage(string name)
        {
            var changeLanguage = Languages
                .FirstOrDefault(dict => dict.Value.Item1.Equals(name, StringComparison.Ordinal)).Value;

            var resourceDictionary =
                Application.Current.Resources.MergedDictionaries.FirstOrDefault(d =>
                    d.Source.Equals(changeLanguage.Item2));

            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            _currentLanguage = changeLanguage;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetField(string name)
        {
            return Application.Current?.FindResource(name)?.ToString();
        }

        /// <summary>
        ///     拼接翻译文本，使用“|”分割
        /// </summary>
        /// <param name="names"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetFields(string names, string c = null, bool concatLast = false)
        {
            if (string.IsNullOrEmpty(names))
                return string.Empty;

            if (_lastKey.Equals(names, StringComparison.Ordinal))
                return _lastValue;

            NamesSb.Clear();
            var nameArr = names.Split('|');

            for (var i = 0; i < nameArr.Length; i++)
            {
                if (i == nameArr.Length - 1 && concatLast) NamesSb.Append(GetField(nameArr[i]));

                NamesSb.AppendFormat("{0}{1}", GetField(nameArr[i]), c);
            }

            _lastKey = names;
            _lastValue = NamesSb.ToString();
            return _lastValue;
        }
    }
}