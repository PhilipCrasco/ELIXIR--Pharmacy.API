using ELIXIR.DATA.CORE.INTERFACES;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs;
using ELIXIR.DATA.DTOs.USER_DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class UserRepository : GenericRepository<UserDto>, IUserRepository
    {
        private new readonly StoreContext _context;

        public UserRepository(
                                StoreContext context,
                                ILogger logger
                              ) : base(context, logger)
        {
            _context = context;
        }

        //-----USER-------

        public override async Task<IReadOnlyList<UserDto>> GetAll()
        {
            try
            {
                var users = (from user in _context.Users
                             join role in _context.Roles on user.UserRoleId equals role.Id
                             join department in _context.Departments on user.DepartmentId equals department.Id
                         
                             select new UserDto
                             {
                                 Id = user.Id,
                                 FullName = user.FullName,
                                 UserName = user.UserName,
                                 Password = user.Password,
                                 Status = user.IsActive,
                                 UserRoleId = role.Id,
                                 UserRole = role.RoleName,
                                 DepartmentId = department.Id,
                                 Department = department.DepartmentName,     
                                 DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                                 AddedBy = user.AddedBy,
                                 ModifiedBy = user.ModifiedBy,
                                 Reason = user.Reason,
                             });
                return await users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo}All method error", typeof(UserRepository));
                return new List<UserDto>();
            }
        }

        public async Task<PagedList<UserDto>> GetAllUsersWithPagination( bool status, UserParams userParams)
        {

            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id
                         orderby user.DateAdded descending

                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRoleId = role.Id,
                             UserRole = role.RoleName,
                             DepartmentId = department.Id,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                             Reason = user.Reason,
                         }).Where(x => x.Status == status);

            return await PagedList<UserDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<UserDto>> GetUserByStatusWithPagination(UserParams userParams, bool status, string search)
        {
            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id
                         orderby user.DateAdded descending
                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRoleId = role.Id,
                             UserRole = role.RoleName,
                             DepartmentId = department.Id,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                             Reason = user.Reason,
                         }).Where(x => x.Status == status)
                           .Where(x => x.UserName.ToLower()
                           .Contains(search.Trim().ToLower()));

            return await PagedList<UserDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<UserDto>> GetUserByUserNameAsyncWithPagination(UserParams userParams, string username)
        {
            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id
                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRoleId = role.Id,
                             UserRole = role.RoleName,
                             DepartmentId = department.Id,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                             Reason = user.Reason,
                         }).Where(x => x.UserName.ToLower()
                           .Contains(username.Trim().ToLower()));

            return await PagedList<UserDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);

        }

        public override async Task<UserDto> GetById(int id)
        {
            var users = (from user in _context.Users
                        join role in _context.Roles on user.UserRoleId equals role.Id
                        join department in _context.Departments on user.DepartmentId equals department.Id
                        select new UserDto
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            UserName = user.UserName,
                            Password = user.Password,
                            Status = user.IsActive,
                            UserRoleId = role.Id,
                            UserRole = role.RoleName,
                            DepartmentId = department.Id,
                            Department = department.DepartmentName,
                            DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                            AddedBy = user.AddedBy,
                            ModifiedBy = user.ModifiedBy,
                            Reason = user.Reason,
                        });

            return await users.FirstOrDefaultAsync(x => x.Id == id);

        }
        public override async Task<bool> Delete(int id)
        {
            try
            {
                var exist = await _context.Users.Where(x => x.Id == id)
                                                .FirstOrDefaultAsync();

                if (exist != null)
                {
                    _context.Users.Remove(exist);
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo}All method error", typeof(UserRepository));
                return false;
            }
        }

        public async Task<IReadOnlyList<UserRole>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> UserAlreadyExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName);
        }

        public async Task<bool> UserAlreadyExistsbyId(int id)
        {
            var resut =  await _context.Users.FindAsync(id);

            return true;
        }

        public async Task<IReadOnlyList<UserDto>> GetUserByUserNameAsync(string username)
        {
            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id
                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRoleId = role.Id,
                             UserRole = role.RoleName,
                             DepartmentId = department.Id,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                             Reason = user.Reason,
                         });

            return await users               
                              .Where(x => x.UserName.ToLower()
                              .Contains(username.Trim().ToLower()))
                              .ToListAsync();
         
        }

        public async Task<IReadOnlyList<UserDto>> GetAllActiveUsers()
        {
            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id
                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRole = role.RoleName,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                             Reason = user.Reason
                         });

             return await users.Where(x => x.Status == true)
                                                 .ToListAsync();
        }

        public async Task<IReadOnlyList<UserDto>> GetAllInActiveUsers()
        {
            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id
                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRole = role.RoleName,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                              Reason = user.Reason
                         });

            return await users.Where(x => x.Status == false)
                                                .ToListAsync();
        }
        public async Task<bool> AddNewUser(User user)
        {
            user.DateAdded = DateTime.Now;
            user.IsActive = true;

            await _context.Users.AddAsync(user);
             return true;
        }
        public async Task<bool> CheckRoleData(int id)
        {
            var roleResult = await _context.Roles.FindAsync(id);

              if(roleResult == null)
                return false;
            return true;
        }

        public async Task<bool> CheckDepartmentData(int id)
        {
            var departmentResult = await _context.Departments.FindAsync(id);

            if (departmentResult == null)
                return false;
            return true;
        }

        public async Task<bool> UpdateUserInfo(User user)
        {
            try
            {
                var existingUser = await _context.Users.Where(x => x.Id == user.Id)
                                                       .Include(x => x.UserRole)
                                                       .Include(x => x.Department)           
                                                       .FirstOrDefaultAsync();
         
                existingUser.FullName = user.FullName;
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password;
                existingUser.UserRoleId = user.UserRoleId;
                existingUser.DepartmentId = user.DepartmentId;
                existingUser.ModifiedBy = user.ModifiedBy;

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo}All method error", typeof(UserRepository));
                return false;
            }
        }

        public async Task<bool> InActiveUser(User user)
        {
            var existingUser = await _context.Users.Where(x => x.Id == user.Id)
                                                   .FirstOrDefaultAsync();

            existingUser.FullName = existingUser.FullName;
            existingUser.Department = existingUser.Department;
            existingUser.UserName = existingUser.UserName;
            existingUser.Password = existingUser.Password;
            existingUser.UserRoleId = existingUser.UserRoleId;
            existingUser.DepartmentId = existingUser.DepartmentId;
            existingUser.ModifiedBy = user.ModifiedBy;
            existingUser.Reason = user.Reason;
            existingUser.IsActive = false;

            if (user.ModifiedBy == null)
                existingUser.ModifiedBy = "Admin";
  
            if (user.Reason == null)
                existingUser.Reason = "Resigned";

           

            return true;
        }

        public async Task<bool> ActivateUser(User user)
        {
            var existingUser = await _context.Users.Where(x => x.Id == user.Id)
                                                  .FirstOrDefaultAsync();

            existingUser.FullName = existingUser.FullName;
            existingUser.Department = existingUser.Department;
            existingUser.UserName = existingUser.UserName;
            existingUser.Password = existingUser.Password;
            existingUser.UserRoleId = existingUser.UserRoleId;
            existingUser.DepartmentId = existingUser.DepartmentId;

            existingUser.ModifiedBy = user.ModifiedBy;
            existingUser.Reason = user.Reason;
            existingUser.IsActive = true;

            if (user.ModifiedBy == null)
                existingUser.ModifiedBy = "Admin";

            if(user.Reason == null)
            existingUser.Reason = "Reopened Account";

            return true;
        }

        //Department 

        public async Task<IReadOnlyList<DepartmentDto>> GetAllDepartment()
        {
            return await _context.Departments.Select(dep => new DepartmentDto
            {
                Id = dep.Id,
                DepartmentName = dep.DepartmentName,
                AddedBy = dep.AddedBy,
                DateAdded = dep.DateAdded.ToString("MM/dd/yyyy"),
                IsActive = dep.IsActive
            })
            .ToListAsync();

        }

        public async Task<Department> GetDepartmentById(int id)
        {
            return await _context.Departments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddNewDepartment(Department dep)
        {

            dep.IsActive = true;
            dep.DateAdded = DateTime.Now;

            if (dep.AddedBy == null)
                dep.AddedBy = "Admin";
      
            await _context.Departments.AddAsync(dep);

            return true;
        }

        public async Task<bool> UpdateDepartmentInfo(Department dep)
        {
            try
            {
                var exisitngDep = await _context.Departments.Where(x => x.Id == dep.Id)
                                                            .FirstOrDefaultAsync();

                if (exisitngDep == null)
                    return await AddNewDepartment(dep);
          
                exisitngDep.DepartmentName = dep.DepartmentName;

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo}All method error", typeof(UserRepository));
                return false;
            }
        }

        public async Task<bool> InActiveDepartment(Department dep)
        {
            var exisitngdep = await _context.Departments.Where(x => x.Id == dep.Id)
                                                        .FirstOrDefaultAsync();

            exisitngdep.DepartmentName = exisitngdep.DepartmentName;
            exisitngdep.Reason = dep.Reason;
            exisitngdep.IsActive = false;

            if (dep.Reason == null)
                exisitngdep.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateDepartment(Department dep)
        {
            var exisitngdep = await _context.Departments.Where(x => x.Id == dep.Id)
                                                        .FirstOrDefaultAsync();

            exisitngdep.DepartmentName = exisitngdep.DepartmentName;
            exisitngdep.Reason = dep.Reason;
            exisitngdep.IsActive = true;

            if (dep.Reason == null)
                exisitngdep.Reason = "Reopened Department";
          

            return true;
        }




        public async Task<IReadOnlyList<Department>> GetAllActiveDeparment()
        {
            return await _context.Departments
                                              .Where(x => x.IsActive == true)
                                              .ToListAsync();
        }

        public async Task<IReadOnlyList<Department>> GetAllInActiveDeparment()
        {
            return await _context.Departments
                                             .Where(x => x.IsActive == false)
                                             .ToListAsync();
        }

        public async Task<bool> DepartmentAlreadyExist(string dep)
        {
            return await _context.Departments.AnyAsync(x => x.DepartmentName == dep);
        }

        public async Task<IReadOnlyList<UserDto>>GetUserByStatus(bool status)
        {
            var users = (from user in _context.Users
                         join role in _context.Roles on user.UserRoleId equals role.Id
                         join department in _context.Departments on user.DepartmentId equals department.Id           
                         select new UserDto
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             UserName = user.UserName,
                             Password = user.Password,
                             Status = user.IsActive,
                             UserRoleId = role.Id,
                             UserRole = role.RoleName,
                             DepartmentId = department.Id,
                             Department = department.DepartmentName,
                             DateAdded = (user.DateAdded).ToString("MM/dd/yyyy"),
                             AddedBy = user.AddedBy,
                             ModifiedBy = user.ModifiedBy,
                             Reason = user.Reason,
                         });

            return await users.Where(x => x.Status == status)
                              .ToListAsync();


        }

        public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentByStatus(bool status)
        {
            return await _context.Departments.Select(dep => new DepartmentDto
            {
                Id = dep.Id,
                DepartmentName = dep.DepartmentName,
                AddedBy = dep.AddedBy,
                DateAdded = dep.DateAdded.ToString("MM/dd/yyyy"),
                IsActive = dep.IsActive

            }).Where(x => x.IsActive == status)
              .ToListAsync();
        }

    }

}
