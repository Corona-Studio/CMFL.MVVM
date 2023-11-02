using System.Collections.Generic;

namespace CMFL.MVVM.Models.DataModel.Launcher.Feedback
{
    public class FeedbackBindingModel
    {
        public string Date { get; set; }

        public string User { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public List<TagModel> Tags { get; set; }

        public string AdminReply { get; set; }
    }
}