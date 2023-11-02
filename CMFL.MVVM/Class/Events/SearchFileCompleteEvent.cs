using System;
using System.Collections.Generic;

namespace CMFL.MVVM.Class.Events
{
    public delegate void SearchFileCompleteEventHandler(object sender, SearchFileCompleteEventArgs e);

    public class SearchFileCompleteEventArgs : EventArgs
    {
        public SearchFileCompleteEventArgs(List<string> paths)
        {
            Paths = paths;
        }

        public List<string> Paths { get; set; } = new List<string>();
    }
}