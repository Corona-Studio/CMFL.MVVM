using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CMFL.MVVM.Models.DataModel.GameData;
using Microsoft.Win32;

namespace CMFL.MVVM.Class.Helper.Game
{
    internal class Searcher
    {
        public static Dictionary<string, string> GetJavas()
        {
            var result = new Dictionary<string, string>();
            var pregkey =
                Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            var nameKeys = new Dictionary<string, RegistryKey>();
            foreach (var item in pregkey.GetSubKeyNames())
                if (!nameKeys.ContainsKey(item))
                    nameKeys.Add(item, pregkey);

            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                using var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var pregkey64 =
                    localMachine.OpenSubKey(@"Software\\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (var item in pregkey64.GetSubKeyNames())
                    if (!nameKeys.ContainsKey(item))
                        nameKeys.Add(item, pregkey64);
            }


            RegistryKey currentKey = null;
            foreach (var item in nameKeys.Keys)
            {
                currentKey = nameKeys[item].OpenSubKey(item);
                if (currentKey.GetValue("DisplayName") is string displayName && displayName.ToLower().Contains("java"))
                {
                    Console.WriteLine(displayName);
                    var installSource = currentKey.GetValue("InstallLocation") as string; //获取路径
                    var lenght = installSource.LastIndexOf(@"\", StringComparison.Ordinal);
                    if (lenght < 1)
                        continue;
                    installSource =
                        installSource.Substring(0, installSource.LastIndexOf(@"\", StringComparison.Ordinal)) +
                        "\\bin\\javaw.exe";
                    result.Add(displayName, installSource);
                }
            }

            return result;
        }

        /// <summary>
        ///     搜索文件
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<T> SearchItem<T>(string path, bool isDirectory) where T : ItemData, new()
        {
            var result = new List<T>();
            var t = new T();
            path = path + t.FileLocation + "\\";
            if (!Directory.Exists(path))
                return result;
            switch (isDirectory)
            {
                case false:
                {
                    var files = Directory.EnumerateFiles(path);

                    result.AddRange(files.Select(item => new T {AbsolutelyFilePath = item}));
                    break;
                }
                case true:
                {
                    var directory = Directory.EnumerateDirectories(path);

                    result.AddRange(directory.Select(item => new T {AbsolutelyDirectory = item}));
                    break;
                }
            }

            return result;
        }

        public static List<string> GetFileName(string dirName, Regex regex)
        {
            //文件夹信息
            var dir = new DirectoryInfo(dirName);
            var result = new List<string>();
            //如果非根路径且是系统文件夹则跳过
            if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System", StringComparison.Ordinal) > -1)
                return result;

            if (!dir.Exists)
                return result;
            //取得所有文件
            var finfo = dir.GetFiles();
            foreach (var file in finfo)
            {
                var fName = file.Name;
                //判断文件是否包含查询名
                if (regex.Match(fName).Success)
                    if (!result.Contains(file.DirectoryName + @"\" + fName))
                        result.Add(file.DirectoryName + @"\" + fName);
            }

            //取得所有子文件夹
            var dinfo = dir.GetDirectories();
            foreach (var t in dinfo)
                result.AddRange(GetFileName(t.FullName, regex));

            return result;
        }
    }
}