using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = false)]

    //[Authorize]
    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public UserController(
            ILogger<UserController> logger,
            StoreContext context,
            IUnitOfWork unitOfWork
        )
        {
            _logger = logger;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        //------USER--------

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> Get()
        {
            var user = await _unitOfWork.Users.GetAll();

            return Ok(user);
        }

        [HttpGet]
        [Route("GetAllUsersWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var user = await _unitOfWork.Users.GetAllUsersWithPagination(status, userParams);

            Response.AddPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages, user.HasNextPage, user.HasPreviousPage);

            var userResult = new
            {
                user,
                user.CurrentPage,
                user.PageSize,
                user.TotalCount,
                user.TotalPages,
                user.HasNextPage,
                user.HasPreviousPage
            };

            return Ok(userResult);
        }

        [HttpGet]
        [Route("GetAllUsersWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllUsersWithPagination(status, userParams);

            var user = await _unitOfWork.Users.GetUserByStatusWithPagination(userParams, status, search);


            Response.AddPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages, user.HasNextPage, user.HasPreviousPage);

            var userResult = new
            {
                user,
                user.CurrentPage,
                user.PageSize,
                user.TotalCount,
                user.TotalPages,
                user.HasNextPage,
                user.HasPreviousPage
            };

            return Ok(userResult);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _unitOfWork.Users.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        [Route("GetUserByStatus/{status}")]
        public async Task<IActionResult> GetUserByStatus(bool status)
        {
            var user = await _unitOfWork.Users.GetUserByStatus(status);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        [Route("GetByUserName/{username?}")]
        public async Task<IActionResult> GetUserbyUserName(string username)
        {

            if (username == null)
                return await Get();

            var user = await _unitOfWork.Users.GetUserByUserNameAsync(username);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        [Route("AddNewUser")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                var getRoleId = await _unitOfWork.Users.CheckRoleData(user.UserRoleId);
                var getDepartmentId = await _unitOfWork.Users.CheckDepartmentData(user.DepartmentId);

                if (await _unitOfWork.Users.UserAlreadyExists(user.UserName))
                    return BadRequest("UserName Already Exist!, Please try something else!");


                if (getRoleId == false)
                    return BadRequest("Role doesn't exist, Please input data first!");

                if (getDepartmentId == false)
                    return BadRequest("Department doesn't exist, Please input data first!");

                await _unitOfWork.Users.AddNewUser(user);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetUser", new { user.Id }, user);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateUserInfo/{id}")]
        public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();

            await _unitOfWork.Users.UpdateUserInfo(user);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Updated!");
        }

        [HttpPut]
        [Route("InActiveUser/{id}")]
        public async Task<IActionResult> InActiveUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();

            await _unitOfWork.Users.InActiveUser(user);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive User!");
        }

        [HttpPut]
        [Route("ActivateUser/{id}")]
        public async Task<IActionResult> ActivateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();

            await _unitOfWork.Users.ActivateUser(user);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate User!");
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<ActionResult<IReadOnlyList<UserRole>>> GetUserRoles()
        {
            return Ok(await _unitOfWork.Users.GetRolesAsync());
        }

        [HttpGet]
        [Route("GetAllActiveUsers")]
        public async Task<IActionResult> GetAllActive()
        {
            var user = await _unitOfWork.Users.GetAllActiveUsers();

            return Ok(user);
        }

        [HttpGet]
        [Route("GetAllInActiveUsers")]
        public async Task<IActionResult> GetAllInActive()
        {
            var user = await _unitOfWork.Users.GetAllInActiveUsers();

            return Ok(user);
        }

        //-----DEPARTMENT-------

        [HttpGet]
        [Route("GetAllDepartments")]
        public async Task<ActionResult<IReadOnlyList<Department>>> GetDepartments()
        {
            return Ok(await _unitOfWork.Users.GetAllDepartment());
        }

        [HttpGet]
        [Route("GetAllDepartmentById/{id}")]
        public async Task<ActionResult<IReadOnlyList<Department>>> GetDepartmentById(int id)
        {
            var dep = await _unitOfWork.Users.GetDepartmentById(id);

            if (dep == null)
                return NotFound();

            return Ok(dep);
        }

        [HttpGet]
        [Route("GetDepartmentByStatus/{status}")]
        public async Task<IActionResult> GetDepartmentByStatus(bool status)
        {
            var dep = await _unitOfWork.Users.GetDepartmentByStatus(status);

            if (dep == null)
                return NotFound();

            return Ok(dep);
        }

        [HttpGet]
        [Route("GetAllActiveDepartment")]
        public async Task<IActionResult> GetAllActiveDepartment()
        {
            var dep = await _unitOfWork.Users.GetAllActiveDeparment();

            return Ok(dep);
        }

        [HttpGet]
        [Route("GetAllInActiveDepartment")]
        public async Task<IActionResult> GetAllInActiveDepartment()
        {
            var dep = await _unitOfWork.Users.GetAllInActiveDeparment();
            return Ok(dep);
        }

        [HttpPost]
        [Route("AddNewDepartment")]
        public async Task<IActionResult> CreateNewDepartment(Department dep)
        {
            if (ModelState.IsValid)
            {

                if (await _unitOfWork.Users.DepartmentAlreadyExist(dep.DepartmentName))
                    return BadRequest("Department Already Exist!, Please try something else!");

                await _unitOfWork.Users.AddNewDepartment(dep);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetDepartments", new { dep.Id }, dep);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateDepartmentInfo/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] Department dep)
        {
            if (id != dep.Id)
                return BadRequest();

            if (await _unitOfWork.Users.DepartmentAlreadyExist(dep.DepartmentName))
                return BadRequest("Department Already Exist!, Please try something else!");

            await _unitOfWork.Users.UpdateDepartmentInfo(dep);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Updated!");
        }

        [HttpPut]
        [Route("InActiveDepartment/{id}")]
        public async Task<IActionResult> InActiveDepartment(int id, [FromBody] Department dep)
        {
            if (id != dep.Id)
                return BadRequest();

            await _unitOfWork.Users.InActiveDepartment(dep);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Department!");
        }

        [HttpPut]
        [Route("ActivateDepartment/{id}")]
        public async Task<IActionResult> ActivateDepartment(int id, [FromBody] Department dep)
        {
            if (id != dep.Id)
                return BadRequest();

            await _unitOfWork.Users.ActivateDepartment(dep);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Department!");
        }



    }
}
