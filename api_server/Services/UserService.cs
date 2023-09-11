using ApiServer.Data;
using ApiServer.Repositories;

namespace ApiServer.Services {
    public class UserService {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository){
            _userRepository = userRepository;
        }

        public async Task AddUser(UserSignUpCred user) {
            await _userRepository.PostUser(user);
        }

        public async Task<LoggedInUserInfo?> CheckCredentials(UserSignInCred user) {
            var res = await _userRepository.FindUser(user);

            if (res != null) {
                return res;
            }

            throw new Exception("User not found!");
        }
    }
}
