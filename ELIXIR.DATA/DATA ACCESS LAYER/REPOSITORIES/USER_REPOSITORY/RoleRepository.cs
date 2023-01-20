using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.CORE.INTERFACES;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs;
using ELIXIR.DATA.DTOs.USER_DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class RoleRepository : GenericRepository<RoleDto>, IRoleRepository
    {
        private new readonly StoreContext _context;
        public RoleRepository(
                                StoreContext context
                             ) : base(context)
        {
            _context = context;
        }

        public new async Task<IReadOnlyList<RoleDto>> GetAll()
        {
            return await _context.Roles
                                       .Select(role => new RoleDto
                                       {
                                           Id = role.Id,
                                           RoleName = role.RoleName,
                                           IsActive = role.IsActive,
                                           DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                           AddedBy = role.AddedBy,
                                           ModifiedBy = role.ModifiedBy,
                                           DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                           Reason = role.Reason
                                       }).ToListAsync();
        }
        public override async Task<RoleDto> GetById(int id)
        {
            return await _context.Roles
                                       .Select(role => new RoleDto
                                       {
                                           Id = role.Id,
                                           RoleName = role.RoleName,
                                           IsActive = role.IsActive,
                                           DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                           AddedBy = role.AddedBy,
                                           ModifiedBy = role.ModifiedBy,
                                           DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                           Reason = role.Reason
                                       }).FirstOrDefaultAsync(x => x.Id == id);
        }
     
        public async Task<bool> AddNewRole(UserRole role)
        {

            role.DateAdded = DateTime.Now;
            role.DateModified = DateTime.Now;
            role.IsActive = true;

            await _context.Roles.AddAsync(role);
            return true;
        }

        public async Task<bool> UpdateRole(UserRole role)
        {

            var existingRole = await _context.Roles.Where(x => x.Id == role.Id)
                                                   .FirstOrDefaultAsync();

            existingRole.RoleName = role.RoleName;
            existingRole.IsActive = true;
            existingRole.DateModified = DateTime.Now;
            existingRole.ModifiedBy = role.ModifiedBy;

            if (role.ModifiedBy == null)
                existingRole.ModifiedBy = "Admin";
         
            return true;
        }

        public async Task<bool> RoleAlreadyExist(string role)
        {
            return await _context.Roles.AnyAsync(x => x.RoleName == role);
        }

        public async Task<bool> InActiveRole(UserRole role)
        {
            var existingRole = await _context.Roles.Where(x => x.Id == role.Id)
                                                   .FirstOrDefaultAsync();


            existingRole.RoleName = existingRole.RoleName;
            existingRole.IsActive = false;
            existingRole.DateModified = DateTime.Now;
            existingRole.ModifiedBy = role.ModifiedBy;
            existingRole.Reason = role.Reason;

            if (role.ModifiedBy == null)
                existingRole.ModifiedBy = "Admin";
         
            if (role.Reason == null)
                existingRole.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateRole(UserRole role)
        {
            var existingRole = await _context.Roles.Where(x => x.Id == role.Id)
                                                   .FirstOrDefaultAsync();

            existingRole.RoleName = existingRole.RoleName;
            existingRole.IsActive = true;
            existingRole.DateModified = DateTime.Now;
            existingRole.ModifiedBy = role.ModifiedBy;
            existingRole.Reason = role.Reason;

            if (role.ModifiedBy == null)
                existingRole.ModifiedBy = "Admin";

            if (role.Reason == null)
                existingRole.Reason = "Reopened Role";
            return true;
        }
        public async Task<IReadOnlyList<RoleDto>> GetAllActiveRoles()
        {
            return await _context.Roles
                                        .Select(role => new RoleDto
                                        {
                                            Id = role.Id,
                                            RoleName = role.RoleName,
                                            IsActive = role.IsActive,
                                            DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                            AddedBy = role.AddedBy,
                                            ModifiedBy = role.ModifiedBy,
                                            DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                            Reason = role.Reason
                                        }).Where(x => x.IsActive == true)
                                          .ToListAsync();
        }

        public async Task<IReadOnlyList<RoleDto>> GetAllInActiveRoles()
        {
            return await _context.Roles
                                        .Select(role => new RoleDto
                                        {
                                            Id = role.Id,
                                            RoleName = role.RoleName,
                                            IsActive = role.IsActive,
                                            DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                            AddedBy = role.AddedBy,
                                            ModifiedBy = role.ModifiedBy,
                                            DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                            Reason = role.Reason
                                        }).Where(x => x.IsActive == false)
                                          .ToListAsync();
        }

        public async Task<bool> CheckRoleAndModuleId(int roleid, int moduleid)
        {
            var roleId = await _context.Roles.FindAsync(roleid);
            var moduleId = await _context.Modules.FindAsync(moduleid);

            if (roleId == null || moduleId == null)
                return false;

            return true;
            
        }

        public async Task<bool> CheckRoleandTagModules(UserRole_Modules rolemodules)
        {
            var existingrolemodule = await _context.RoleModules.Where(x => x.RoleId == rolemodules.RoleId)
                                                               .Where(x => x.ModuleId == rolemodules.ModuleId)
                                                               .Where(x => x.IsActive == true)
                                                               .FirstOrDefaultAsync();
            if (existingrolemodule == null)
                return true;


            return false;
        }
        public async Task<IReadOnlyList<RolewithModulesDto>> GetAllRolewithModules()
        {
            var tagmodules = from rolemodule in _context.RoleModules
                          join role in _context.Roles on rolemodule.RoleId equals role.Id
                          join module in _context.Modules on rolemodule.ModuleId equals module.Id
                          select new RolewithModulesDto
                          {
                              RoleName = role.RoleName,
                              MainMenu = module.MainMenu.ModuleName,
                              SubMenu = module.SubMenuName,
                              ModuleName = module.ModuleName,
                              Id = module.Id,
                              IsActive = rolemodule.IsActive
                          };
            return await tagmodules.Where(x => x.IsActive == true)
                                   .ToListAsync();
        }
        public async Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleByRole(string rolename)
        {
            var rolemodules = from rolemodule in _context.RoleModules
                              join role in _context.Roles on rolemodule.RoleId equals role.Id
                              join module in _context.Modules on rolemodule.ModuleId equals module.Id
                              select new RolewithModulesDto
                              {
                                  RoleName = role.RoleName,
                                  MainMenu = module.MainMenu.ModuleName,
                                  MainMenuId = module.MainMenuId,
                                  SubMenu = module.SubMenuName,
                                  ModuleName = module.ModuleName,
                                  Id = module.Id,
                                  IsActive = rolemodule.IsActive
                              };

            return await rolemodules.Where(x => x.RoleName == rolename)
                                    .Where(x => x.IsActive == true)
                                    .ToListAsync();
        }

        public async Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleById(int id, int menuid)
        {
            var rolemodules = from rolemodule in _context.RoleModules
                              join role in _context.Roles on rolemodule.RoleId equals role.Id
                              join module in _context.Modules on rolemodule.ModuleId equals module.Id
                              select new RolewithModulesDto
                              {
                                  RoleName = role.RoleName,
                                  MainMenu = module.MainMenu.ModuleName,
                                  MainMenuId = module.MainMenuId,
                                  MenuPath = module.MainMenu.MenuPath,
                                  SubMenu = module.SubMenuName,
                                  ModuleName = module.ModuleName,
                                  Id = module.Id,
                                  IsActive = rolemodule.IsActive,
                                  RoleId = rolemodule.RoleId,
                                  ModuleStatus = module.ModuleStatus
                              };

            return await rolemodules.Where(x => x.RoleId == id)
                                    .Where(x => x.IsActive == true)
                                    .Where(x => x.MainMenuId == menuid)
                                    .ToListAsync();
        }

        public async Task<bool> TagModules(UserRole_Modules roleModule)
        {
            roleModule.IsActive = true;
            await _context.AddAsync(roleModule);
            return true;
        }
        public async Task<bool> TagAndUntagUpdate(UserRole_Modules rolemodule)
        {
            var rolemoduleStatus = await _context.RoleModules.Where(x => x.ModuleId == rolemodule.ModuleId)
                                                             .Where(x => x.RoleId == rolemodule.RoleId)
                                                             .FirstOrDefaultAsync();

            if (rolemoduleStatus == null)
                return await TagModules(rolemodule);

            if (rolemoduleStatus != null && rolemoduleStatus.IsActive == false)

                rolemoduleStatus.IsActive = true;

            return true;
        }
        public async Task<bool> UntagModuleinRole(UserRole_Modules rolemodule)
        {
            var existingrolemodule = await _context.RoleModules.Where(x => x.ModuleId == rolemodule.ModuleId)
                                                               .Where(x => x.RoleId == rolemodule.RoleId)
                                                               .FirstOrDefaultAsync();

            if(existingrolemodule == null)
                return false;

            existingrolemodule.IsActive = false;

            return true;
        }
        public async Task<IReadOnlyList<UntagModuleDto>> GetUntagModuleByRoleId(int id, int menuid)
        {
            var availablemodule = _context.Modules
                                                 .Where(x => x.MainMenuId == menuid)
                                                 .Where(x => !_context.RoleModules
                                                 .Where(x => x.RoleId == id)
                                                 .Where(x => x.IsActive == true)                                       
                                                 .Select(x => x.ModuleId)
                                                 .Contains(x.Id));

            return await availablemodule
                                       .Select(rolemodule => new UntagModuleDto
                                       {
                                           Remarks = "Untag",
                                           MainMenu = rolemodule.MainMenu.ModuleName,
                                           SubMenu = rolemodule.SubMenuName,
                                           RoleId = id,
                                           ModuleId = rolemodule.Id,
                                           IsActive = rolemodule.IsActive,
                                         
                                       })
                                            .Where(x => x.IsActive == true)
                                            .ToListAsync();
        }

        public async Task<IReadOnlyList<RoleDto>> GetRoleByStatus(bool status)
        {
            return await _context.Roles
                                        .Select(role => new RoleDto
                                        {
                                            Id = role.Id,
                                            RoleName = role.RoleName,
                                            IsActive = role.IsActive,
                                            DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                            AddedBy = role.AddedBy,
                                            ModifiedBy = role.ModifiedBy,
                                            DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                            Reason = role.Reason
                                        }).Where(x => x.IsActive == status)
                                          .ToListAsync();
        }

        public async Task<PagedList<RoleDto>> GetAllRolessWithPagination(bool status, UserParams userParams)
        {
            var roles = _context.Roles.OrderByDescending(x => x.DateAdded)
                                     .Select(role => new RoleDto
                                     {
                                         Id = role.Id,
                                         RoleName = role.RoleName,
                                         IsActive = role.IsActive,
                                         DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                         AddedBy = role.AddedBy,
                                         ModifiedBy = role.ModifiedBy,
                                         DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                         Reason = role.Reason
                                     }).Where(x => x.IsActive == status);
                  
                                      

            return await PagedList<RoleDto>.CreateAsync(roles, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<RoleDto>> GetRoleByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var roles = _context.Roles.OrderByDescending(x => x.DateAdded)
                                       .Select(role => new RoleDto
                                      {
                                          Id = role.Id,
                                          RoleName = role.RoleName,
                                          IsActive = role.IsActive,
                                          DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                          AddedBy = role.AddedBy,
                                          ModifiedBy = role.ModifiedBy,
                                          DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                          Reason = role.Reason
                                      }).Where(x => x.IsActive == status)
                                        .Where(x => x.RoleName.ToLower()
                                        .Contains(search.Trim().ToLower()));

            return await PagedList<RoleDto>.CreateAsync(roles, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleByIdAndParentId(int id, string status, int menuid)
        {
            var rolemodules = from rolemodule in _context.RoleModules
                              join role in _context.Roles on rolemodule.RoleId equals role.Id
                              join module in _context.Modules on rolemodule.ModuleId equals module.Id
                              select new RolewithModulesDto
                              {
                                  RoleName = role.RoleName,
                                  MainMenu = module.MainMenu.ModuleName,
                                  MainMenuId = module.MainMenuId,
                                  MenuPath = module.MainMenu.MenuPath,
                                  SubMenu = module.SubMenuName,
                                  ModuleName = module.ModuleName,
                                  Id = module.Id,
                                  IsActive = rolemodule.IsActive,
                                  RoleId = rolemodule.RoleId,
                                  ModuleStatus = module.ModuleStatus
                              };

            return await rolemodules.Where(x => x.RoleId == id)
                                    .Where(x => x.IsActive == true)
                                    .Where(x => x.ModuleStatus == status)
                                    .Where(x => x.MainMenuId == menuid)
                                    .ToListAsync();
        }

        public async Task<IReadOnlyList<RolewithModulesDto>> GetRoleModuleWithId(int id)
        {
            var rolemodules = from rolemodule in _context.RoleModules
                              join role in _context.Roles on rolemodule.RoleId equals role.Id
                              join module in _context.Modules on rolemodule.ModuleId equals module.Id
                              select new RolewithModulesDto
                              {
                                  RoleName = role.RoleName,
                                  MainMenu = module.MainMenu.ModuleName,
                                  MainMenuId = module.MainMenuId,
                                  MenuPath = module.MainMenu.MenuPath,
                                  SubMenu = module.SubMenuName,
                                  ModuleName = module.ModuleName,
                                  Id = module.Id,
                                  IsActive = rolemodule.IsActive,
                                  RoleId = rolemodule.RoleId,
                                  ModuleStatus = module.ModuleStatus
                              };

            return await rolemodules.Where(x => x.RoleId == id)
                                    .Where(x => x.IsActive == true)
                                    .ToListAsync();
        }
    }  
}