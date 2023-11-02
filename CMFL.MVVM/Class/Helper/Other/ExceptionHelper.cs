using System;
using System.Collections;
using System.Text;
using CMFL.MVVM.Class.Helper.Launcher;

namespace CMFL.MVVM.Class.Helper.Other
{
    public static class ExceptionHelper
    {
        public static void GetAllExceptionString(this Exception ex, ref StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException();

            while (true)
            {
                if (ex != null)
                {
                    sb.Append(LanguageHelper.GetField("UnhandledExceptionCaught")).Append(Environment.NewLine)
                        .Append("Type:").Append(Environment.NewLine)
                        .Append(ex.GetType()).Append(Environment.NewLine)
                        .Append(LanguageHelper.GetField("ExceptionInfo"))
                        .Append(Environment.NewLine)
                        .Append(ex.Message ?? "None").Append(Environment.NewLine)
                        .Append("Target Site:")
                        .Append(Environment.NewLine)
                        .Append(ex.TargetSite).Append(Environment.NewLine)
                        .Append("Source:")
                        .Append(Environment.NewLine)
                        .Append(ex.Source ?? "None").Append(Environment.NewLine)
                        .Append(LanguageHelper.GetField("ExceptionStack"))
                        .Append(Environment.NewLine)
                        .Append(ex.StackTrace ?? "None").Append(Environment.NewLine)
                        .Append("H Result: ")
                        .Append(Environment.NewLine)
                        .Append(ex.HResult).Append(Environment.NewLine)
                        .Append("Help Link: ")
                        .Append(Environment.NewLine)
                        .Append(ex.HelpLink ?? "None").Append(Environment.NewLine)
                        .Append(Environment.NewLine);

                    if (ex.Data.Count > 0)
                    {
                        sb.Append("Data(s):").Append(Environment.NewLine);
                        foreach (DictionaryEntry element in ex.Data)
                            sb.Append(element.Key).Append(": ").Append(element.Value).Append(Environment.NewLine);
                    }

                    if (ex.InnerException != null)
                    {
                        sb.Append("=> Inner Exception:").Append(Environment.NewLine);
                        ex = ex.InnerException;
                        continue;
                    }
                }

                break;
            }
        }
    }
}