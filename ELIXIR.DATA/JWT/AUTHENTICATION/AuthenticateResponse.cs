using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;

namespace ELIXIR.DATA.JWT.AUTHENTICATION
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            FullName = user.FullName;
            UserName = user.UserName;
            Password = user.Password;
            Role = user.UserRoleId;
            Token = token;
        }
    }
}
