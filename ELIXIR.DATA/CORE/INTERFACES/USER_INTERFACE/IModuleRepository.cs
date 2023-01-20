using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIR.DATA.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES
{
    public interface IModuleRepository : IGenericRepository<ModuleDto>
    {
       //----MODULE-----
        Task<bool> AddNewModule(Module module);
        Task<bool> UpdateModule(Module module);
        Task<IReadOnlyList<ModuleDto>> GetAllActiveModules();
        Task<IReadOnlyList<ModuleDto>> GetAllInActiveModules();
        Task<bool> SubMenuNameExist(string module);
        Task<bool> ModuleNameExist(string module);
        Task<bool> CheckMainMenu(int id);
        Task<bool> InActiveModule(Module module);
        Task<bool> ActivateModule(Module module);
        Task<IReadOnlyList<ModuleDto>> GetModuleByStatus(bool status);





        //----MAIN MENU-----
        Task<IReadOnlyList<MainMenu>> GetAllMainMenu();
        Task<MainMenu> GetMainMenuById(int id);
        Task<bool> AddNewMainMenu(MainMenu menu);
        Task<bool> UpdateMainMenu(MainMenu menu);
        Task<bool> InActiveMainMenu(MainMenu menu);
        Task<bool> ActivateMainMenu(MainMenu menu);
        Task <IReadOnlyList<MainMenu>> GetAllActiveMainMenu();
        Task <IReadOnlyList<MainMenu>> GetAllInActiveMainMenu();
        Task<bool>MenuAlreadyExist(string menu);
        Task<IReadOnlyList<MainMenu>> GetMenuByStatus(bool status);



        Task<PagedList<ModuleDto>> GetAllModulessWithPagination(bool status, UserParams userParams);
        Task<PagedList<ModuleDto>> GetModulesByStatusWithPaginationOrig(UserParams userParams, bool status, string search);





    }
}
