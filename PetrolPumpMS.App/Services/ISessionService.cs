using PetrolPumpMS.Models;

namespace PetrolPumpMS.App.Services;

public interface ISessionService
{
    User? CurrentUser { get; }
    void SignIn(User user);
    void SignOut();
}

public class SessionService : ISessionService
{
    public User? CurrentUser { get; private set; }
    public void SignIn(User user) => CurrentUser = user;
    public void SignOut() => CurrentUser = null;
}
