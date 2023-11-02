namespace CMFL.MVVM.Models.DataModel.Launcher.Auth
{
    public enum AuthResultType
    {
        WrongCredential = 0,
        UnknownError,
        LoginRequired,
        AlreadyRegistered,
        AlreadyAuth,
        AuthSucceeded,
        NotVerified
    }
}