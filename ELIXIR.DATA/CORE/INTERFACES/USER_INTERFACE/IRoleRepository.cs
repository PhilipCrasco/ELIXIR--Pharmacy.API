using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DTOs;
using ELIXIR.DATA.DTOs.USER_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES
{
    public interface IRoleRepository : IGenericRepository<RoleDto>
    {
        Task<bool> RoleAlreadyExist(string role);
        Task<bool> AddNewRole(UserRole role);
        Task<bool> UpdateRole(UserRole role);
        Task<IReadOnlyList<RoleDto>> GetAllActiveRoles();
        Task<IReadOnlyList<RoleDto>> GetAllInActiveRoles();
        Task<bool> TagModules(UserRole_Modules roleModule);
        Task<bool> CheckRoleAndModuleId(int roleid, int moduleid);
        Task<bool> CheckRoleandTagModules(UserRole_Modules rolemodule);
        Task<IReadOnlyList<RolewithModulesDto>> GetAllRolewithModules();
        Task<bool> TagAndUntagUpdate(UserRole_Modules rolemodule);
        Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleByRole(string rolename);
        Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleById(int id, int menuid);
        Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleWithId(int id);

        Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleByIdAndParentId(int id, string status, int menuid);

        Task<bool> UntagModuleinRole(UserRole_Modules rolemodules);
        Task<IReadOnlyList<UntagModuleDto>> GetUntagModuleByRoleId(int id, int menuid);



        Task<bool> InActiveRole(UserRole role);
        Task<bool> ActivateRole(UserRole role);
        Task<IReadOnlyList<RoleDto>> GetRoleByStatus(bool status);





        Task<PagedList<RoleDto>> GetAllRolessWithPagination(bool status, UserParams userParams);
        Task<PagedList<RoleDto>> GetRoleByStatusWithPaginationOrig(UserParams userParams, bool status, string search);



    }
}
