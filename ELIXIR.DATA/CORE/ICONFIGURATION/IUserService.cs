using ELIXIR.DATA.JWT.AUTHENTICATION;

namespace ELIXIR.DATA.CORE.ICONFIGURATION
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest request);
    }
}
