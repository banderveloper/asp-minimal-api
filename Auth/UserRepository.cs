namespace HotelsWebApi.Auth
{
    public class UserRepository : IUserRepository
    {
        private List<UserDto> _users = new()
        {
            new UserDto("kpssnik", "qwerty123")
        };

        public UserDto GetUser(UserModel userModel)
        {
            return _users.FirstOrDefault(user =>
                string.Equals(user.UserName, userModel.UserName) &&
                string.Equals(user.Password, userModel.Password)
            ) ?? throw new Exception("UserRepos get user error");
        }
    }
}