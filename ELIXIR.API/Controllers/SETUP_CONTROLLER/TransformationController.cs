using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class TransformationController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransformationController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        //-----TRANSFORMATION FORMULA---------

        [HttpGet]
        [Route("GetAllTransformationFormula")]
        public async Task<IActionResult> GetAllTransformationFormula()
        {
            var formula = await _unitOfWork.Transforms.GetAll();

            return Ok(formula);
        }

        [HttpGet]
        [Route("GetAllFormulaWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<TransformationFormulaDto>>> GetAllFormulaWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var formula = await _unitOfWork.Transforms.GetAllFormulaWithPagination(status, userParams);

            Response.AddPaginationHeader(formula.CurrentPage, formula.PageSize, formula.TotalCount, formula.TotalPages, formula.HasNextPage, formula.HasPreviousPage);

            var formulaResult = new
            {
                formula,
                formula.CurrentPage,
                formula.PageSize,
                formula.TotalCount,
                formula.TotalPages,
                formula.HasNextPage,
                formula.HasPreviousPage
            };

            return Ok(formulaResult);
        }

        [HttpGet]
        [Route("GetAllFormulaWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<TransformationFormulaDto>>> GetAllSupplierWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllFormulaWithPagination(status, userParams);

            var formula = await _unitOfWork.Transforms.GetFormulaByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(formula.CurrentPage, formula.PageSize, formula.TotalCount, formula.TotalPages, formula.HasNextPage, formula.HasPreviousPage);

            var formulaResult = new
            {
                formula,
                formula.CurrentPage,
                formula.PageSize,
                formula.TotalCount,
                formula.TotalPages,
                formula.HasNextPage,
                formula.HasPreviousPage
            };

            return Ok(formulaResult);
        }

        [HttpGet]
        [Route("GetFormulaById/{id}")]
        public async Task<IActionResult> GetFormulaById(int id)
        {
            var formula = await _unitOfWork.Transforms.GetById(id);

            if (formula == null)
                return NotFound();

            return Ok(formula);
        }

        [HttpGet]
        [Route("GetAllActiveFormula")]
        public async Task<IActionResult> GetAllActiveFormula()
        {
            var formula = await _unitOfWork.Transforms.GetAllActiveFormula();

            return Ok(formula);
        }

        [HttpGet]
        [Route("GetAllInActiveFormula")]
        public async Task<IActionResult> GetAllInActiveFormula()
        {
            var formula = await _unitOfWork.Transforms.GetAllInActiveFormula();

            return Ok(formula);
        }

        [HttpPost]
        [Route("AddNewTransformationFormula")]
        public async Task<IActionResult> AddNewTransformationFormula(TransformationFormula formula)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Transforms.AddNewTransformationFormula(formula);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetAllTransformationFormula", new { formula.Id }, formula);
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPut]
        [Route("InActiveFormula/{id}")]
        public async Task<IActionResult> InActiveFormula(int id, [FromBody] TransformationFormula formula)
        {
            if (id != formula.Id)
                return BadRequest();

            await _unitOfWork.Transforms.InactiveTransformationFormula(formula);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inActive formula!");
        }

        [HttpPut]
        [Route("ActivateFormula/{id}")]
        public async Task<IActionResult> ActivateFormula(int id, [FromBody] TransformationFormula formula)
        {
            if (id != formula.Id)
                return BadRequest();

            await _unitOfWork.Transforms.ActivateTransformationFormula(formula);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate formula!");
        }

        //-----TRANSFORMATION REQUIREMENT---------

        [HttpGet]
        [Route("GetAllRequirementsWithFormula")]
        public async Task<IActionResult> GetAllRequirementsWithFormula()
        {
            var requirement = await _unitOfWork.Transforms.GetAllRequirementwithFormula();

            return Ok(requirement);
        }

        [HttpGet]
        [Route("GetAllFormulaWithRequirementByFormulaId/{id}")]
        public async Task<IActionResult> GetAllFormulaWithRequirementByFormulaId(int id)
        {
            var formula = await _unitOfWork.Transforms.GetAllFormulaWithRequirementsByFormulaId(id);

            if (formula == null)
                return NotFound();

            return Ok(formula);
        }

        [HttpPost]
        [Route("AddNewTransformationRequirementinFormula")]
        public async Task<IActionResult> AddNewTransformationRequirementinFormula([FromBody] TransformationRequirement[] requirement)
        {
            if (ModelState.IsValid)
            {

                foreach (TransformationRequirement items in requirement)
                {

                    var validateRawMaterial = await _unitOfWork.Transforms.ValidateRawMaterial(items.RawMaterialId);

                    var verifytagrequirement = await _unitOfWork.Transforms.ValidateTagRequirements(items);

                    var validateformula = await _unitOfWork.Transforms.ValidateFormulaIfActive(items.TransformationFormulaId);

                    if (validateRawMaterial == false)
                        return BadRequest("Raw Material doesn't exist, Please add data first!");

                    if (verifytagrequirement == false)
                        return BadRequest("Raw Material already added in the formula!");

                    if (validateformula == false)
                        return BadRequest("Formulation not exist, Please check your data!");

                    await _unitOfWork.Transforms.AddNewRequirementsInFormula(items);
                    await _unitOfWork.CompleteAsync();

                }

                return new JsonResult("Sucessfully added requriements");

            }

            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpDelete]
        [Route("DeleteRequirementinFormula/{id}")]
        public async Task<IActionResult> DeleteRequirementinFormula(int id, [FromBody] TransformationRequirement requirement)
        {
            if (id != requirement.Id)
                return BadRequest();

            await _unitOfWork.Transforms.DeleteRequirementInFormula(requirement.Id);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Deleted Requirement!");
        }

        [HttpPut]
        [Route("UpdateFormulaInfo/{id}")]
        public async Task<IActionResult> UpdateFormulaInfo(int id, [FromBody] TransformationFormula formula)
        {
            if (id != formula.Id)
                return BadRequest();

            await _unitOfWork.Transforms.UpdateFormulaCode(formula);
            await _unitOfWork.CompleteAsync();

            return Ok(formula);
        }

        [HttpPut]
        [Route("InActiveRequirements/{id}")]
        public async Task<IActionResult> InActiveRequirements(int id, [FromBody] TransformationRequirement requirement)
        {
            if (id != requirement.Id)
                return BadRequest();

            await _unitOfWork.Transforms.InActiveRequirement(requirement);
            await _unitOfWork.CompleteAsync();

            return Ok(requirement);
        }

        [HttpPut]
        [Route("ActivateRequirement/{id}")]
        public async Task<IActionResult> ActivateRequirement(int id, [FromBody] TransformationRequirement requirement)
        {
            if (id != requirement.Id)
                return BadRequest();

            await _unitOfWork.Transforms.ActivateRequirement(requirement);
            await _unitOfWork.CompleteAsync();

            return Ok(requirement);
        }

        [HttpGet]
        [Route("GetAllInActiveRequirement")]
        public async Task<IActionResult> GetAllInActiveRequirement([FromQuery] int id)
        {
            var formula = await _unitOfWork.Transforms.GetAllInActiveRequirements(id);


            if (formula == null)
                return NotFound();

            return Ok(formula);
        }

        [HttpPut]
        [Route("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] TransformationRequirement requirement)
        {
            if (id != requirement.Id)
                return BadRequest();

            await _unitOfWork.Transforms.UpdateQuantity(requirement);
            await _unitOfWork.CompleteAsync();

            return Ok(requirement);
        }


    }
}
