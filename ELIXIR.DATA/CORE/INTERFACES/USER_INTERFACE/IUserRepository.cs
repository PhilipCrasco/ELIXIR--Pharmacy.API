using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DTOs;
using ELIXIR.DATA.DTOs.USER_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES
{
    public interface IUserRepository : IGenericRepository<UserDto>
    {
        //------USER----------
        Task<IReadOnlyList<UserRole>> GetRolesAsync();
        Task<bool> UserAlreadyExists(string userName);
        Task<bool> UserAlreadyExistsbyId(int id);
        Task<IReadOnlyList<UserDto>> GetUserByUserNameAsync(string  username);
        Task<IReadOnlyList<UserDto>> GetAllActiveUsers();
        Task<IReadOnlyList<UserDto>> GetAllInActiveUsers();
        Task<bool> AddNewUser(User user);
        Task<bool> UpdateUserInfo(User user);
        Task<bool> InActiveUser(User user);
        Task<bool> ActivateUser(User user);
        Task<bool> CheckRoleData(int id);
        Task<bool> CheckDepartmentData(int id);
        Task<IReadOnlyList<UserDto>> GetUserByStatus(bool status);

        //User with Pagination 

        Task<PagedList<UserDto>> GetAllUsersWithPagination(bool status, UserParams userParams);
        Task<PagedList<UserDto>> GetUserByUserNameAsyncWithPagination(UserParams userParams, string username);
        Task<PagedList<UserDto>> GetUserByStatusWithPagination(UserParams userParams, bool status, string search);


        //------DEPARTMENT---------
        Task<IReadOnlyList<DepartmentDto>> GetAllDepartment();
        Task<Department> GetDepartmentById(int id);
        Task<bool> AddNewDepartment(Department dep);
        Task<bool> UpdateDepartmentInfo(Department dep);
        Task<bool> InActiveDepartment(Department dep);
        Task<bool> ActivateDepartment(Department dep);
        Task<IReadOnlyList<Department>> GetAllActiveDeparment();
        Task<IReadOnlyList<Department>> GetAllInActiveDeparment();
        Task<bool> DepartmentAlreadyExist(string dep);
        Task<IReadOnlyList<DepartmentDto>> GetDepartmentByStatus(bool status);


    }
}
