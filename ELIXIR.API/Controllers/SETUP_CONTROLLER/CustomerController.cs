using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class CustomerController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        //----CUSTOMER---------

        [HttpGet]
        [Route("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var customer = await _unitOfWork.Customers.GetAll();

            return Ok(customer);
        }


        [HttpGet]
        [Route("GetAllCustomerWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var customer = await _unitOfWork.Customers.GetAllCustomerWithPagination(status, userParams);

            Response.AddPaginationHeader(customer.CurrentPage, customer.PageSize, customer.TotalCount, customer.TotalPages, customer.HasNextPage, customer.HasPreviousPage);

            var customerResult = new
            {
                customer,
                customer.CurrentPage,
                customer.PageSize,
                customer.TotalCount,
                customer.TotalPages,
                customer.HasNextPage,
                customer.HasPreviousPage
            };

            return Ok(customerResult);
        }

        [HttpGet]
        [Route("GetAllCustomerWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllCustomerWithPagination(status, userParams);

            var customer = await _unitOfWork.Customers.GetCustomerByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(customer.CurrentPage, customer.PageSize, customer.TotalCount, customer.TotalPages, customer.HasNextPage, customer.HasPreviousPage);

            var customerResult = new
            {
                customer,
                customer.CurrentPage,
                customer.PageSize,
                customer.TotalCount,
                customer.TotalPages,
                customer.HasNextPage,
                customer.HasPreviousPage
            };

            return Ok(customerResult);
        }

        [HttpGet]
        [Route("GetCustomerById/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _unitOfWork.Customers.GetById(id);

            return Ok(customer);
        }

        [HttpGet]
        [Route("GetAllActiveCustomer")]
        public async Task<IActionResult> GetAllActiveCustomer()
        {
            var customer = await _unitOfWork.Customers.GetAllActiveCustomer();

            return Ok(customer);
        }

        [HttpGet]
        [Route("GetAllInActiveCustomer")]
        public async Task<IActionResult> GetAllInActiveCustomer()
        {
            var customer = await _unitOfWork.Customers.GetAllInActiveCustomer();
            return Ok(customer);
        }

        [HttpPost]
        [Route("AddNewCustomer")]
        public async Task<IActionResult> CreateNewCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {

                var farmId = await _unitOfWork.Customers.ValidateFarmId(customer.FarmTypeId);

                if (farmId == false)
                    return BadRequest("Farm Type doesn't exist, Please add data first!");

                if (await _unitOfWork.Customers.CustomerCodeExist(customer.CustomerCode))
                    return BadRequest("Customer already Exist!, Please try something else!");

                await _unitOfWork.Customers.AddNewCustomer(customer);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllCustomer", new { customer.Id }, customer);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomerInfo(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            var farmId = await _unitOfWork.Customers.ValidateFarmId(customer.FarmTypeId);

            if (farmId == false)
                return BadRequest("Farm Type doesn't exist, Please add data first!");

            await _unitOfWork.Customers.UpdateCustomerInfo(customer);
            await _unitOfWork.CompleteAsync();

            return Ok(customer);
        }

        [HttpPut]
        [Route("InActiveCustomer/{id}")]
        public async Task<IActionResult> InActiveCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            await _unitOfWork.Customers.InActiveCustomer(customer);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Customer!");
        }

        [HttpPut]
        [Route("ActivateCustomer/{id}")]
        public async Task<IActionResult> ActivateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            await _unitOfWork.Customers.ActivateCustomer(customer);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Customer!");
        }

        //-----FARM TYPE----------
        [HttpGet]
        [Route("GetAllFarms")]
        public async Task<IActionResult> GetAllFarms()
        {
            var farm = await _unitOfWork.Customers.GetAllFarm();

            return Ok(farm);
        }

        [HttpGet]
        [Route("GetFarmById/{id}")]
        public async Task<IActionResult> GetFarmById(int id)
        {
            var farm = await _unitOfWork.Customers.GetFarmById(id);

            return Ok(farm);
        }

        [HttpGet]
        [Route("GetAllActiveFarms")]
        public async Task<IActionResult> GetAllActiveFarms()
        {
            var farm = await _unitOfWork.Customers.GetAllActiveFarm();

            return Ok(farm);
        }

        [HttpGet]
        [Route("GetAllInActiveFarms")]
        public async Task<IActionResult> GetAllInActiveFarms()
        {
            var farm = await _unitOfWork.Customers.GetAllInActiveFarm();

            return Ok(farm);
        }

        [HttpPost]
        [Route("AddNewFarm")]
        public async Task<IActionResult> CreateNewCustomer(FarmType farm)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Customers.FarmCodeExist(farm.FarmCode))
                    return BadRequest("Farm already exist!, please try something else.");

                await _unitOfWork.Customers.AddnewFarm(farm);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllFarms", new { farm.Id }, farm);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("UpdateFarm/{id}")]
        public async Task<IActionResult> UpdateFarm(int id, [FromBody] FarmType farm)
        {
            if (id != farm.Id)
                return BadRequest();

            await _unitOfWork.Customers.UpdateFarmType(farm);
            await _unitOfWork.CompleteAsync();

            return Ok(farm);
        }

        [HttpPut]
        [Route("InActiveFarm/{id}")]
        public async Task<IActionResult> InActiveFarm(int id, [FromBody] FarmType farm)
        {
            if (id != farm.Id)
                return BadRequest();

            await _unitOfWork.Customers.InActiveFarm(farm);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive Farm!");
        }

        [HttpPut]
        [Route("ActivateFarm/{id}")]
        public async Task<IActionResult> ActivateFarm(int id, [FromBody] FarmType farm)
        {
            if (id != farm.Id)
                return BadRequest();

            await _unitOfWork.Customers.ActivateFarm(farm);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate Farm!");
        }

        [HttpGet]
        [Route("GetAllFarmWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllFarmWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var farms = await _unitOfWork.Customers.GetAllFarmWithPagination(status, userParams);

            Response.AddPaginationHeader(farms.CurrentPage, farms.PageSize, farms.TotalCount, farms.TotalPages, farms.HasNextPage, farms.HasPreviousPage);

            var farmResult = new
            {
                farms,
                farms.CurrentPage,
                farms.PageSize,
                farms.TotalCount,
                farms.TotalPages,
                farms.HasNextPage,
                farms.HasPreviousPage
            };

            return Ok(farmResult);
        }

        [HttpGet]
        [Route("GetAllFarmWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllFarmWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllFarmWithPagination(status, userParams);

            var farms = await _unitOfWork.Customers.GetAllFarmWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(farms.CurrentPage, farms.PageSize, farms.TotalCount, farms.TotalPages, farms.HasNextPage, farms.HasPreviousPage);

            var farmResult = new
            {
                farms,
                farms.CurrentPage,
                farms.PageSize,
                farms.TotalCount,
                farms.TotalPages,
                farms.HasNextPage,
                farms.HasPreviousPage
            };

            return Ok(farmResult);
        }
    }
}
