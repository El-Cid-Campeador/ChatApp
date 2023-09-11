using ApiServer.Data;

namespace ApiServer.Repositories {
    public interface IUserRepository {
        Task PostUser(UserSignUpCred user);
        Task<LoggedInUserInfo?> FindUser(UserSignInCred user);
    }
}
