using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIR.DATA.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ModuleController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ModuleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        //------Module------

        [HttpGet]
        [Route("GetAllModules")]
        public async Task<IActionResult> Get()
        {
            var module = await _unitOfWork.Modules.GetAll();

            return Ok(module);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetModules(int id)
        {
            var modules = await _unitOfWork.Modules.GetById(id);

            if (modules == null)
                return NotFound();

            return Ok(modules);
        }

        [HttpGet]
        [Route("GetModuleByStatus/{status}")]
        public async Task<IActionResult> GetModuleByStatus(bool status)
        {
            var module = await _unitOfWork.Modules.GetModuleByStatus(status);

            if (module == null)
                return NotFound();

            return Ok(module);
        }

        [HttpGet]
        [Route("GetAllModulesWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllModulesWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var module = await _unitOfWork.Modules.GetAllModulessWithPagination(status, userParams);

            Response.AddPaginationHeader(module.CurrentPage, module.PageSize, module.TotalCount, module.TotalPages, module.HasNextPage, module.HasPreviousPage);

            var moduleResult = new
            {
                module,
                module.CurrentPage,
                module.PageSize,
                module.TotalCount,
                module.TotalPages,
                module.HasNextPage,
                module.HasPreviousPage
            };

            return Ok(moduleResult);
        }

        [HttpGet]
        [Route("GetAllModulesWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllUsersWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllModulesWithPagination(status, userParams);

            var module = await _unitOfWork.Modules.GetModulesByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(module.CurrentPage, module.PageSize, module.TotalCount, module.TotalPages, module.HasNextPage, module.HasPreviousPage);

            var moduleResult = new
            {
                module,
                module.CurrentPage,
                module.PageSize,
                module.TotalCount,
                module.TotalPages,
                module.HasNextPage,
                module.HasPreviousPage
            };

            return Ok(moduleResult);
        }

        [HttpPost]
        [Route("AddNewModule")]
        public async Task<IActionResult> CreateModule(Module module)
        {
            if (ModelState.IsValid)
            {
                var getMainMenuId = await _unitOfWork.Modules.CheckMainMenu(module.MainMenuId);

                if (getMainMenuId == false)
                    return BadRequest("MainMenu doesn't exist, Please input data first!");

                if (await _unitOfWork.Modules.SubMenuNameExist(module.SubMenuName))
                    return BadRequest("SubMenu Already Exist!, Please try something else!");

                if (await _unitOfWork.Modules.ModuleNameExist(module.ModuleName))
                    return BadRequest("ModuleName Already Exist!, Please try something else!");

                await _unitOfWork.Modules.AddNewModule(module);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetModules", new { module.Id }, module);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateModule/{id}")]
        public async Task<IActionResult> UpdateModuleById(int id, [FromBody] Module module)
        {
            if (id != module.Id)
                return BadRequest();

            await _unitOfWork.Modules.UpdateModule(module);
            await _unitOfWork.CompleteAsync();

            return Ok(module);
        }

        [HttpPut]
        [Route("InActiveModule/{id}")]
        public async Task<IActionResult> InActiveModule(int id, [FromBody] Module module)
        {
            if (id != module.Id)
                return BadRequest();

            await _unitOfWork.Modules.InActiveModule(module);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Module!");
        }

        [HttpPut]
        [Route("ActivateModule/{id}")]
        public async Task<IActionResult> ActivateModule(int id, [FromBody] Module module)
        {
            if (id != module.Id)
                return BadRequest();

            await _unitOfWork.Modules.ActivateModule(module);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Module!");
        }

        [HttpGet]
        [Route("GetAllActiveModules")]
        public async Task<IActionResult> GetAllActive()
        {
            var module = await _unitOfWork.Modules.GetAllActiveModules();

            return Ok(module);
        }

        [HttpGet]
        [Route("GetAllInActiveModules")]
        public async Task<IActionResult> GetAllInActive()
        {
            var module = await _unitOfWork.Modules.GetAllInActiveModules();

            return Ok(module);
        }

        //------Main Menu------

        [HttpGet]
        [Route("GetAllMainMenu")]
        public async Task<IActionResult> GetAllMainMenu()
        {
            var menu = await _unitOfWork.Modules.GetAllMainMenu();

            return Ok(menu);
        }

        [HttpGet]
        [Route("GetAllActiveMenu")]
        public async Task<IActionResult> GetAllActiveMenu()
        {
            var menu = await _unitOfWork.Modules.GetAllActiveMainMenu();

            return Ok(menu);
        }

        [HttpGet]
        [Route("GetAllInActiveMenu")]
        public async Task<IActionResult> GetAllInActiveMenu()
        {
            var menu = await _unitOfWork.Modules.GetAllInActiveMainMenu();

            return Ok(menu);
        }

        [HttpGet]
        [Route("GetMenuById/{id}")]
        public async Task<IActionResult> GetMenubyId(int id)
        {
            var menu = await _unitOfWork.Modules.GetMainMenuById(id);

            if (menu == null)
                return NotFound();

            return Ok(menu);
        }

        [HttpPost]
        [Route("AddNewMenu")]
        public async Task<IActionResult> CreateNewMenu(MainMenu menu)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Modules.MenuAlreadyExist(menu.ModuleName))
                    return BadRequest("Menu Already Exist!, Please try something else!");

                await _unitOfWork.Modules.AddNewMainMenu(menu);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllMainMenu", new { menu.Id }, menu);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateMenu/{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] MainMenu menu)
        {
            if (id != menu.Id)
                return BadRequest();

            await _unitOfWork.Modules.UpdateMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return Ok(menu);
        }

        [HttpPut]
        [Route("InActiveMenu/{id}")]
        public async Task<IActionResult> InActiveMenu(int id, [FromBody] MainMenu menu)
        {
            if (id != menu.Id)
                return BadRequest();

            await _unitOfWork.Modules.InActiveMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Menu!");
        }

        [HttpPut]
        [Route("ActivateMainMenu/{id}")]
        public async Task<IActionResult> ActivateMenu(int id, [FromBody] MainMenu menu)
        {
            if (id != menu.Id)
                return BadRequest();

            await _unitOfWork.Modules.ActivateMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Menu!");
        }

        [HttpGet]
        [Route("GetMenuByStatus/{status}")]
        public async Task<IActionResult> GetMenuByStatus(bool status)
        {
            var menu = await _unitOfWork.Modules.GetModuleByStatus(status);

            if (menu == null)
                return NotFound();

            return Ok(menu);
        }

    }
}
