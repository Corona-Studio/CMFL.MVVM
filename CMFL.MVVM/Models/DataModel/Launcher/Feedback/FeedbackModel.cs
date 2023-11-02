using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher.Feedback
{
    public class FeedbackModel
    {
        [JsonProperty("date")] public string Date { get; set; }

        [JsonProperty("user")] public string User { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("content")] public string Content { get; set; }

        [JsonProperty("user_tag")] public int UserTag { get; set; }

        [JsonProperty("admin_tag")] public string AdminTag { get; set; }

        [JsonProperty("admin_reply")] public string AdminReply { get; set; }
    }
}