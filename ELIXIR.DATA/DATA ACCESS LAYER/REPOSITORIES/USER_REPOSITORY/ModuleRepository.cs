using ELIXIR.DATA.CORE.INTERFACES;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class ModuleRepository : GenericRepository<ModuleDto>, IModuleRepository
    {
        private new readonly StoreContext _context;
        public ModuleRepository(
                        StoreContext context
                          ) : base(context)
        {
            _context = context;
        }

        //-------MODULE--------
        
        public override async Task<IReadOnlyList<ModuleDto>> GetAll()
        {
            return await _context.MainMenus
                                            .Join(_context.Modules,
                                             menu => menu.Id,
                                             module => module.MainMenuId,
                                            (menu, module) => new ModuleDto
                                            {
                                                Id = module.Id,
                                                MainMenu = menu.ModuleName,
                                                MainMenuId = menu.Id,
                                                SubMenuName = module.SubMenuName,
                                                ModuleName = module.ModuleName,
                                                DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                                AddedBy = module.AddedBy,
                                                IsActive = module.IsActive,
                                                ModifiedBy = module.ModifiedBy,
                                                Reason = module.Reason,
                                                ModuleStatus = module.ModuleStatus
                                            }).ToListAsync();
        }
         
        public override async Task<ModuleDto> GetById(int id)
        {
            return await _context.MainMenus
                                           .Join(_context.Modules,
                                            menu => menu.Id,
                                            module => module.MainMenuId,
                                           (menu, module) => new ModuleDto
                                           {

                                               Id = module.Id,
                                               MainMenu = menu.ModuleName,
                                               MainMenuId = menu.Id,
                                               SubMenuName = module.SubMenuName,
                                               ModuleName = module.ModuleName,
                                               DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                               AddedBy = module.AddedBy,
                                               IsActive = module.IsActive,
                                               ModifiedBy = module.ModifiedBy,
                                               Reason = module.Reason,
                                               ModuleStatus = module.ModuleStatus
                                           }).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> AddNewModule(Module module)
        {
            module.DateAdded = DateTime.Now;
            module.ModifiedBy = "Admin";
            module.IsActive = true;

            if (module.AddedBy == null)
                module.AddedBy = "Admin";
               

            await _context.Modules.AddAsync(module);
            return true;
        }

        public async Task<bool> UpdateModule(Module module)
        {

            var existingModule = await _context.Modules.Where(x => x.Id == module.Id)
                                                       .FirstOrDefaultAsync();

            existingModule.MainMenuId = module.MainMenuId;
            existingModule.SubMenuName = module.SubMenuName;
            existingModule.ModuleName = module.ModuleName;
            existingModule.ModifiedBy = module.ModifiedBy;
            existingModule.ModuleStatus = module.ModuleStatus;

            return true;

        }

        public async Task<IReadOnlyList<ModuleDto>> GetAllActiveModules()
        {
            return await _context.MainMenus
                                          .Join(_context.Modules,
                                           menu => menu.Id,
                                           module => module.MainMenuId,
                                          (menu, module) => new ModuleDto
                                          {
                                              Id = module.Id,
                                              MainMenu = menu.ModuleName,
                                              MainMenuId = menu.Id,
                                              SubMenuName = module.SubMenuName,
                                              ModuleName = module.ModuleName,
                                              DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                              AddedBy = module.AddedBy,
                                              IsActive = module.IsActive,
                                              ModifiedBy = module.ModifiedBy,
                                              Reason = module.Reason,
                                              ModuleStatus = module.ModuleStatus
                                          }).Where(x => x.IsActive == true)
                                            .ToListAsync();
        }

        public async Task<IReadOnlyList<ModuleDto>> GetAllInActiveModules()
        {
            return await _context.MainMenus
                                         .Join(_context.Modules,
                                          menu => menu.Id,
                                          module => module.MainMenuId,
                                         (menu, module) => new ModuleDto
                                         {
                                             Id = module.Id,
                                             MainMenu = menu.ModuleName,
                                             MainMenuId = menu.Id,
                                             SubMenuName = module.SubMenuName,
                                             ModuleName = module.ModuleName,
                                             DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                             AddedBy = module.AddedBy,
                                             IsActive = module.IsActive,
                                             ModifiedBy = module.ModifiedBy,
                                             Reason = module.Reason,
                                             ModuleStatus = module.ModuleStatus
                                         }).Where(x => x.IsActive == false)
                                           .ToListAsync();
        }

        public async Task<bool> SubMenuNameExist(string module)
        {
            return await _context.Modules.AnyAsync(x => x.SubMenuName == module);
        }

        public async Task<bool> CheckMainMenu(int id)
        {
            var mainMenuResult = await _context.MainMenus.FindAsync(id);

            if (mainMenuResult == null)
                return false;
            return true;
        }

        public async Task<bool> ModuleNameExist(string module)
        {
            return await _context.Modules.AnyAsync(x => x.ModuleName == module);
        }

        public async Task<bool> InActiveModule(Module module)
        {
            var existingModule = await _context.Modules.Where(x => x.Id == module.Id)
                                                       .FirstOrDefaultAsync();

            existingModule.MainMenu = existingModule.MainMenu;
            existingModule.SubMenuName = existingModule.SubMenuName;
            existingModule.ModuleName = existingModule.ModuleName;
            existingModule.ModifiedBy = module.ModifiedBy;
            existingModule.Reason = module.Reason;
            existingModule.IsActive = false;

            if (module.ModifiedBy == null )
                 existingModule.ModifiedBy = "Admin";
      
            if (module.Reason == null)
                existingModule.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateModule(Module module)
        {
            var existingModule = await _context.Modules.Where(x => x.Id == module.Id)
                                                       .FirstOrDefaultAsync();

            existingModule.MainMenu = existingModule.MainMenu;
            existingModule.SubMenuName = existingModule.SubMenuName;
            existingModule.ModuleName = existingModule.ModuleName;
            existingModule.ModifiedBy = module.ModifiedBy;
            existingModule.Reason = module.Reason;

            existingModule.IsActive = true;

            if (module.ModifiedBy == null)
                existingModule.ModifiedBy = "Admin";

            if (module.Reason == null)
                existingModule.Reason = "Reopened Module";

            return true;
        }

        public async Task<IReadOnlyList<ModuleDto>> GetModuleByStatus(bool status)
        {
            return await _context.MainMenus
                                        .Join(_context.Modules,
                                         menu => menu.Id,
                                         module => module.MainMenuId,
                                        (menu, module) => new ModuleDto
                                        {
                                            Id = module.Id,
                                            MainMenu = menu.ModuleName,
                                            SubMenuName = module.SubMenuName,
                                            ModuleName = module.ModuleName,
                                            DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                            AddedBy = module.AddedBy,
                                            IsActive = module.IsActive,
                                            ModifiedBy = module.ModifiedBy,
                                            Reason = module.Reason
                                        }).Where(x => x.IsActive == status)
                                          .ToListAsync();
        }


        //------MAIN MENU--------
        public async Task<IReadOnlyList<MainMenu>> GetAllMainMenu()
        {
            return await _context.MainMenus.ToListAsync();
        }

        public async Task<MainMenu> GetMainMenuById(int id)
        {
            return await _context.MainMenus.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddNewMainMenu(MainMenu menu)
        {
            menu.DateAdded = DateTime.Now;
            menu.IsActive = true;
            menu.AddedBy = "Admin";
            menu.ModifiedBy = "Admin";

            if (menu.AddedBy == null)
                menu.AddedBy = "Admin";
           
            await _context.MainMenus.AddAsync(menu);
            return true;
        }

        public async Task<bool> UpdateMainMenu(MainMenu menu)
        {
            try
            {
                var existingmenu = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                           .FirstOrDefaultAsync();

                existingmenu.ModuleName = menu.ModuleName;
                existingmenu.ModifiedBy = menu.ModifiedBy;
                existingmenu.MenuPath = menu.MenuPath;

                if (menu.ModifiedBy == null) 
                    existingmenu.ModifiedBy = "Admin";

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo}All method error", typeof(ModuleRepository));
                return false;
            }
        }

        public async Task<bool> InActiveMainMenu(MainMenu menu)
        {
            var existingMenu = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                         .FirstOrDefaultAsync();

            existingMenu.ModuleName = existingMenu.ModuleName;
            existingMenu.ModifiedBy = menu.ModifiedBy;
            existingMenu.Reason = menu.Reason;
            existingMenu.IsActive = false;

            if (menu.ModifiedBy == null)
                existingMenu.ModifiedBy = "Admin";

            if (menu.Reason == null)
                existingMenu.Reason= "Change Data";

           

            return true;
        }

        public async Task<bool> ActivateMainMenu(MainMenu menu)
        {
            var existingMenu = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                       .FirstOrDefaultAsync();

            existingMenu.ModuleName = existingMenu.ModuleName;
            existingMenu.ModifiedBy = menu.ModifiedBy;
            existingMenu.Reason = menu.Reason;
            existingMenu.IsActive = true;

            if (menu.ModifiedBy == null)
                existingMenu.ModifiedBy = "Admin";

            if (menu.Reason == null)
                existingMenu.Reason = "Reopened Menu";

            return true;
        }

        public async Task<IReadOnlyList<MainMenu>> GetAllActiveMainMenu()
        {
            return await _context.MainMenus.Where(x => x.IsActive == true)
                                           .ToListAsync();
        }

        public async Task<IReadOnlyList<MainMenu>> GetAllInActiveMainMenu()
        {
            return await _context.MainMenus.Where(x => x.IsActive == false)
                                           .ToListAsync();
        }

        public async Task<bool> MenuAlreadyExist(string menu)
        {
            return await _context.MainMenus.AnyAsync(x => x.ModuleName == menu);
        }

        public async Task<IReadOnlyList<MainMenu>> GetMenuByStatus(bool status)
        {
            return await _context.MainMenus.Where(x => x.IsActive == status)
                                           .ToListAsync();
        }

        public async Task<PagedList<ModuleDto>> GetAllModulessWithPagination(bool status, UserParams userParams)
        {
            var modules = _context.MainMenus.OrderByDescending(x => x.DateAdded)
                                           .Join(_context.Modules,
                                            menu => menu.Id,
                                            module => module.MainMenuId,
                                           (menu, module) => new ModuleDto
                                           {
                                               Id = module.Id,
                                               MainMenu = menu.ModuleName,
                                               MainMenuId = menu.Id,
                                               SubMenuName = module.SubMenuName,
                                               ModuleName = module.ModuleName,
                                               DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                               AddedBy = module.AddedBy,
                                               IsActive = module.IsActive,
                                               ModifiedBy = module.ModifiedBy,
                                               Reason = module.Reason,
                                               ModuleStatus = module.ModuleStatus
                                           })
                                             .Where(x => x.IsActive == status);
                                          

            return await PagedList<ModuleDto>.CreateAsync(modules, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ModuleDto>> GetModulesByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var modules = _context.MainMenus.OrderByDescending(x => x.DateAdded)
                                            .Join(_context.Modules,
                                             menu => menu.Id,
                                             module => module.MainMenuId,
                                            (menu, module) => new ModuleDto
                                            {
                                                Id = module.Id,
                                                MainMenu = menu.ModuleName,
                                                MainMenuId = menu.Id,
                                                SubMenuName = module.SubMenuName,
                                                ModuleName = module.ModuleName,
                                                DateAdded = (module.DateAdded).ToString("MM/dd/yyyy"),
                                                AddedBy = module.AddedBy,
                                                IsActive = module.IsActive,
                                                ModifiedBy = module.ModifiedBy,
                                                Reason = module.Reason,
                                                ModuleStatus = module.ModuleStatus
                                            })
                                              .Where(x => x.IsActive == status)
                                              .Where(x => x.SubMenuName.ToLower()
                                              .Contains(search.Trim().ToLower()));

            return await PagedList<ModuleDto>.CreateAsync(modules, userParams.PageNumber, userParams.PageSize);
        }
    }
}
