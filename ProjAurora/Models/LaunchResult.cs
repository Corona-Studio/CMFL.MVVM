namespace ProjAurora.Models
{
    public class Error
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public LaunchError LaunchError { get; set; }
    }

    public class LaunchResult
    {
        public int ExitCode { get; set; }
        public long GameTime { get; set; }
        public Error Error { get; set; }
    }
}