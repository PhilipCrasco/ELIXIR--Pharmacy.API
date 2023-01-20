using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = false)]
   // [EnableCors("CorsPolicy")]

    public class RoleController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> Get()
        {
            var roles = await _unitOfWork.Roles.GetAll();

            return Ok(roles);
        }

        [HttpGet]
        [Route("GetUntagModuleByRoleId/{id}/{menuid}")]
        public async Task<IActionResult> GetAllAvailableModule(int id, int menuid)

        {
            var roles = await _unitOfWork.Roles.GetUntagModuleByRoleId(id, menuid);

            return Ok(roles);
        }

        [HttpGet]
        [Route("GetAllRolesWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRolesWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var role = await _unitOfWork.Roles.GetAllRolessWithPagination(status, userParams);

            Response.AddPaginationHeader(role.CurrentPage, role.PageSize, role.TotalCount, role.TotalPages, role.HasNextPage, role.HasPreviousPage);

            var roleResult = new
            {
                role,
                role.CurrentPage,
                role.PageSize,
                role.TotalCount,
                role.TotalPages,
                role.HasNextPage,
                role.HasPreviousPage
            };

            return Ok(roleResult);
        }

        [HttpGet]
        [Route("GetAllRolesWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllUsersWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllRolesWithPagination(status, userParams);

            var role = await _unitOfWork.Roles.GetRoleByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(role.CurrentPage, role.PageSize, role.TotalCount, role.TotalPages, role.HasNextPage, role.HasPreviousPage);

            var roleResult = new
            {
                role,
                role.CurrentPage,
                role.PageSize,
                role.TotalCount,
                role.TotalPages,
                role.HasNextPage,
                role.HasPreviousPage
            };

            return Ok(roleResult);
        }

        [HttpGet]
        [Route("GetAllRoleModules")]
        public async Task<IActionResult> GetRoleModules()
        {
            var rolemodules = await _unitOfWork.Roles.GetAllRolewithModules();

            return Ok(rolemodules);
        }

        [HttpGet]
        [Route("GetbyId/{id}")]
        public async Task<IActionResult> GetRoles(int id)
        {
            var roles = await _unitOfWork.Roles.GetById(id);

            if (roles == null)
                return NotFound();

            return Ok(roles);
        }

        [HttpGet]
        [Route("GetRolesByStatus/{status}")]
        public async Task<IActionResult> GetRolesByStatus(bool status)
        {
            var roles = await _unitOfWork.Roles.GetRoleByStatus(status);

            if (roles == null)
                return NotFound();

            return Ok(roles);
        }

        [HttpPost]
        [Route("AddNewRole")]
        public async Task<IActionResult> CreateRole(UserRole role)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Roles.RoleAlreadyExist(role.RoleName))
                    return BadRequest("Role already exist, please try something else!");

                await _unitOfWork.Roles.AddNewRole(role);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetRoles", new { role.Id }, role);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UserRole role)
        {
            if (id != role.Id)
                return BadRequest();

            await _unitOfWork.Roles.UpdateRole(role);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Updated!");
        }

        [HttpPut]
        [Route("InActiveRole/{id}")]
        public async Task<IActionResult> InActiveRole(int id, [FromBody] UserRole role)
        {
            if (id != role.Id)
                return BadRequest();

            await _unitOfWork.Roles.InActiveRole(role);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Role!");
        }

        [HttpPut]
        [Route("ActivateRole/{id}")]
        public async Task<IActionResult> ActivateRole(int id, [FromBody] UserRole role)
        {
            if (id != role.Id)
                return BadRequest();

            await _unitOfWork.Roles.ActivateRole(role);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Role!");
        }

        [HttpGet]
        [Route("GetAllActiveRoles")]
        public async Task<IActionResult> GetAllActive()
        {
            var role = await _unitOfWork.Roles.GetAllActiveRoles();

            return Ok(role);
        }

        [HttpGet]
        [Route("GetAllInActiveRoles")]
        public async Task<IActionResult> GetAllInActive()
        {
            var role = await _unitOfWork.Roles.GetAllInActiveRoles();

            return Ok(role);
        }

        [HttpGet]
        [Route("GetRoleModulebyRole/{rolename}")]
        public async Task<IActionResult> GetRoleModulebyRole(string rolename)
        {
            var rolemodule = await _unitOfWork.Roles.GetRoleModuleByRole(rolename);

            return Ok(rolemodule);
        }

        [HttpGet]
        [Route("GetRoleModulebyId/{id}/{menuid}")]
        public async Task<IActionResult> GetRoleModuleById(int id, int menuid)
        {
            var rolemodule = await _unitOfWork.Roles.GetRoleModuleById(id, menuid);

            return Ok(rolemodule);
        }

        [HttpGet]
        [Route("GetRoleModuleByIdAndParentId/{id}/{status}/{menuid}")]
        public async Task<IActionResult> GetRoleModuleByIdAndParentId(int id, string status, int menuid)
        {
            var rolemodule = await _unitOfWork.Roles.GetRoleModuleByIdAndParentId(id, status, menuid);

            return Ok(rolemodule);
        }


        [HttpPost]
        [Route("TagandModules")]
        public async Task<IActionResult> TagModules([FromBody] UserRole_Modules[] roleModule)
        {
            foreach (UserRole_Modules module in roleModule)
            {

                var verifyId = await _unitOfWork.Roles.CheckRoleAndModuleId(module.RoleId, module.ModuleId);
                var verifyTagModule = await _unitOfWork.Roles.CheckRoleandTagModules(module);

                if (verifyId == false)
                    return BadRequest("Role or Module doesn't exist, Please check your data!");

                if (verifyTagModule == false)
                    return BadRequest("Role already contain this module!");

                module.IsActive = true;
                await _unitOfWork.Roles.TagModules(module);
                await _unitOfWork.CompleteAsync();
            }

            return new JsonResult("Successfully Tag Module!");
        }

        [HttpPut]
        [Route("UntagModule")]
        public async Task<IActionResult> UntagModule([FromBody] UserRole_Modules[] rolemodule)
        {

            foreach (UserRole_Modules module in rolemodule)
            {
                await _unitOfWork.Roles.UntagModuleinRole(module);
                await _unitOfWork.CompleteAsync();
            }

            return new JsonResult("Successfully Untag Module!");
        }

        [HttpPut]
        [Route("TagModuleinRole")]
        public async Task<IActionResult> ActivateTagModuleinRole([FromBody] UserRole_Modules[] rolemodule)
        {

            foreach (UserRole_Modules module in rolemodule)
            {

                var verifyTagModule = await _unitOfWork.Roles.CheckRoleandTagModules(module);

                if (verifyTagModule == false)
                    return BadRequest("Module already exist!");

                await _unitOfWork.Roles.TagAndUntagUpdate(module);
                await _unitOfWork.CompleteAsync();
            }
            return new JsonResult("Successfully Activated Tag Modules!");
        }

        [HttpGet]
        [Route("GetRoleModuleWithId/{id}")]
        public async Task<IActionResult> GetRoleModuleWithId(int id)
        {
            var rolemodule = await _unitOfWork.Roles.GetRoleModuleWithId(id);

            return Ok(rolemodule);
        }

    }
}
