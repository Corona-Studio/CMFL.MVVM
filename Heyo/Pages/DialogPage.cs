using System;
using System.Windows.Controls;

namespace Heyo.Pages
{
    public class DialogPage : Page
    {
        public delegate void ResultEventHandler(DialogPage sender, object result);

        public bool ShowBackButton { get; set; } = false;
        public bool ShowCloseButton { get; set; } = true;

        public event ResultEventHandler Result;
        public event EventHandler Closed;
        public new event EventHandler Loaded;

        public void LoadCompleted(object sender, EventArgs e)
        {
            if (Loaded == null)
                return;
            Loaded(sender, e);
        }

        protected void OutputResult(DialogPage sender, object result)
        {
            if (Result != null)
                Result(sender, result);
        }

        public void Close()
        {
            Closed(this, new EventArgs());
        }
    }
}