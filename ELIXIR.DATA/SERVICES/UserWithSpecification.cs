using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;

namespace ELIXIR.DATA.SERVICES
{
    public class UserWithSpecification : BaseSpecification<User>
    {
        public UserWithSpecification()
        {
            AddInclude(x => x.UserRole);
            AddInclude(x => x.Department);
        }
    }

}
