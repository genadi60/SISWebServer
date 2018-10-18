namespace SIS.MvcFramework.Services.Contracts
{
    public interface IUserCookieService
    {
        string GetUserCookie(string userName);

        string GetUserData(string cookieContent);

        string DecryptString(string cipherText);
    }
}