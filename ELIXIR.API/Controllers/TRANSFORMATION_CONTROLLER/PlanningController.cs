using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.TRANSFORMATION_CONTROLLER
{
    public class PlanningController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public PlanningController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("GetAllVersionByItemCode")]
        public async Task<IActionResult> GetAllVersionByItemCode([FromQuery] string itemcode)
        {
            var version = await _unitOfWork.Planning.GetAllVersionByItemCode(itemcode);

            return Ok(version);
        }

        [HttpPost]
        [Route("AddNewTransformationRequest")]
        public async Task<IActionResult> AddNewTransformationRequest(TransformationPlanning planning)
        {
            List<TransformationWithRequirements> outofStock = new List<TransformationWithRequirements>();

            if (ModelState.IsValid)
            {

                if (planning.Batch <= 0)
                    return BadRequest("Request failed! Please check your input in batch.");

                var validateDate = await _unitOfWork.Planning.ValidateInputDate(planning.ProdPlan.ToString());

                if (validateDate == false)
                    return BadRequest("Request failed! please check your input in prod plan date.");

                var getRequirement = await _unitOfWork.Planning.GetRequirementsStock(planning.ItemCode, planning.Version);

                foreach (var items in getRequirement)
                {

                    var validateStocks = await _unitOfWork.Planning.ValidateRequirement(items.ItemCode, planning.Batch, items.Quantity);

                    if (validateStocks == false)
                    {
                        var validateStock = await _unitOfWork.Planning.GetAllItemsWithStock(items.ItemCode);

                        items.WarehouseStock = validateStock;
                        items.QuantityNeeded = items.Quantity * planning.Batch;
                        outofStock.Add(items);
                    }

                    else
                    {
                        var validateFormulaCode = await _unitOfWork.Planning.ValidateFormulaCode(planning);

                        if (validateFormulaCode == false)
                            return BadRequest("Adding request failed! Formula not exist.");

                        var validateRequirement = await _unitOfWork.Planning.ValidateVersionInRequest(planning);

                        if (validateRequirement == false)
                            return BadRequest("Adding request failed. Please check if the formula have the requirements!");

                        await _unitOfWork.Planning.AddNewTransformationRequest(planning);
                    }

                }

                var resultList = new
                {
                    outofStock
                };


                if (outofStock.Count == 0)
                {
                    await _unitOfWork.CompleteAsync();
                    return Ok(planning);
                }
                else
                {
                    return BadRequest(resultList);
                }

            }

            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpPost]
        [Route("AddNewTransformationRequirements")]
        public async Task<IActionResult> AddNewTransformationRequirements(TransformationRequest request)
        {
            if (ModelState.IsValid)
            {
                var validateRequirement = await _unitOfWork.Planning.ValidateAllRequirementsWithFormula(request);

                if (validateRequirement == false)
                    return BadRequest("Adding request failed. Please check if the formula have the requirements!");

                var getRequirement = await _unitOfWork.Planning.GetAllRequirements(request);

                foreach (var items in getRequirement)
                {

                    request.Id = 0;

                    var validateRequest = await _unitOfWork.Planning.ValidateTransformationPlanning(request.TransformId);

                    request.ProdPlan = DateTime.Parse(validateRequest.ProdPlan);
                    request.Version = validateRequest.Version;
                    request.Batch = validateRequest.Batch;

                    request.ItemCode = items.ItemCode;
                    request.ItemDescription = items.ItemDescription;
                    request.Uom = items.Uom;
                    request.Quantity = items.Quantity * validateRequest.Batch;

                    await _unitOfWork.Planning.AddNewTransformationRequirements(request);
                    await _unitOfWork.CompleteAsync();
                }

                return Ok("Successfully add new request!");
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpGet]
        [Route("GetAllRequirementsByTransformId/{id}")]
        public async Task<IActionResult> GetAllRequirementsByTransformId(int id)
        {

            var requirement = await _unitOfWork.Planning.GetAllListOfRequirementsByTransformId(id);

            return Ok(requirement);

        }

        [HttpPut]
        [Route("ApproveTransformRequest/{id}")]
        public async Task<IActionResult> ApproveTransformRequest(int id, [FromBody] TransformationPlanning planning)
        {
            if (id != planning.Id)
                return BadRequest();


            var validate = await _unitOfWork.Planning.ValidateStatusRemarks(planning);

            if (validate == false)
                return BadRequest("Approved failed, please check your transformation request!");


            await _unitOfWork.Planning.ApproveTransformationRequest(planning);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully approved transformation request!");
        }

        [HttpGet]
        [Route("GetAllApprovedTransformationRequest")]
        public async Task<IActionResult> GetAllApprovedTransformationRequest()
        {

            var planning = await _unitOfWork.Planning.GetAllApprovedRequest();

            return Ok(planning);

        }

        [HttpGet]
        [Route("GetAllPendingRequest")]
        public async Task<IActionResult> GetAllPendingRequest([FromQuery] string status)
        {

            var planning = await _unitOfWork.Planning.GetAllPendingRequest(status);

            return Ok(planning);

        }

        [HttpGet]
        [Route("GetAllPendingRequestWithRequirements/{id}")]
        public async Task<IActionResult> GetAllPendingRequestWithRequirements(int id)
        {

            var requirement = await _unitOfWork.Planning.GetAllPendingRequestWithRequriements(id);

            return Ok(requirement);

        }

        [HttpPut]
        [Route("CancelTransformationRequest/{id}")]
        public async Task<IActionResult> CancelTransformationRequest(int id, [FromBody] TransformationPlanning planning)
        {
            if (id != planning.Id)
                return BadRequest();


            var validate = await _unitOfWork.Planning.ValidateIfPrepared(id);

            if (validate == false)
                return BadRequest("Cancel failed, transform Id already have prepared items!");

            await _unitOfWork.Planning.CancelTransformationRequest(planning);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully cancelled transformation request!");
        }

        [HttpGet]
        [Route("GetAllCancelledTransformationRequest")]
        public async Task<IActionResult> GetAllCancelledTransformationRequest()
        {

            var planning = await _unitOfWork.Planning.GetAllCancelledRequest();

            return Ok(planning);

        }

        [HttpPut]
        [Route("RejectTransformationRequest/{id}")]
        public async Task<IActionResult> RejectTransformationRequest(int id, [FromBody] TransformationReject reject)
        {

            if (id != reject.TransformId)
                return BadRequest();

            var validate = await _unitOfWork.Planning.RejectTransformationRequest(reject);

            if (validate == false)
                return BadRequest("Transform request not exist!");

            var validatePrepare = await _unitOfWork.Planning.ValidatePlanningRequestIfPrepared(reject.TransformId);

            if (validatePrepare == false)
                return BadRequest("Cannot reject request, transnform Id already have prepared materials!");

            var getRequest = await _unitOfWork.Planning.GetAllRequestForReject(reject.TransformId);

            foreach (var items in getRequest)
            {

                reject.Id = 0;

                reject.TransformId = items.TransformId;
                reject.FormulaCode = items.FormulaCode;
                reject.FormulaDescription = items.FormulaDescription;
                reject.Quantity = items.FormulaQuantity;
                reject.ProdPlan = Convert.ToDateTime(items.ProdPlan);
                reject.Uom = items.Uom;
                reject.Version = items.Version;
                reject.Batch = items.Batch;
                reject.RawmaterialCode = items.RawmaterialCode;
                reject.RawmaterialDescription = items.RawmaterialDescription;
                reject.Quantity = items.RawmaterialQuantity;
                reject.RejectedDate = DateTime.Now;
                reject.IsActive = true;

                await _unitOfWork.Planning.AddNewTransformationReject(reject);
                await _unitOfWork.CompleteAsync();
            }

            return Ok("Successfully reject request!");
        }

        [HttpPut]
        [Route("RequestRejectTransformationRequest/{id}")]
        public async Task<IActionResult> RequestRejectTransformationRequest(int id, [FromBody] TransformationPlanning planning)
        {
            if (id != planning.Id)
                return BadRequest();

            await _unitOfWork.Planning.RequestRejectTransformationRequest(planning);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully requested a material from reject list!");
        }

        [HttpPut]
        [Route("EditTransformationRequest/{id}")]
        public async Task<IActionResult> EditTransformationRequest(int id, [FromBody] TransformationRequest request)
        {
            if (id != request.TransformId)
                return BadRequest();

            List<TransformationWithRequirements> outofStock = new List<TransformationWithRequirements>();

            var validateApproved = await _unitOfWork.Planning.ValidateIfApproved(request.TransformId);

            if (validateApproved == false)
                return BadRequest("Edit failed, cannot edit approved request");

            await _unitOfWork.Planning.EditRejectTransformationPlanning(request);
            await _unitOfWork.CompleteAsync();

            var validateDate = await _unitOfWork.Planning.ValidateInputDate(request.ProdPlan.ToString());

            if (validateDate == false)
                return BadRequest("Request failed! please check your input in prod plan date.");


            var getRequirement = await _unitOfWork.Planning.GetRequirementsStock(request.ItemCode, request.Version);

            foreach (var items in getRequirement)
            {

                var validateStocks = await _unitOfWork.Planning.ValidateRequirement(items.ItemCode, request.Batch, items.Quantity);

                if (validateStocks == false)
                {
                    var validateStock = await _unitOfWork.Planning.GetAllItemsWithStock(items.ItemCode);

                    items.WarehouseStock = validateStock;
                    items.QuantityNeeded = items.Quantity * request.Batch;
                    outofStock.Add(items);
                }

                else
                {
                    request.Id = 0;

                    var validateRequest = await _unitOfWork.Planning.ValidateTransformationPlanning(request.TransformId);
                    request.ProdPlan = DateTime.Parse(validateRequest.ProdPlan);
                    request.Version = validateRequest.Version;
                    request.ItemCode = items.ItemCode;
                    request.ItemDescription = items.ItemDescription;
                    request.Uom = items.Uom;
                    request.Quantity = items.Quantity * request.Batch;

                    await _unitOfWork.Planning.AddNewTransformationRequirements(request);
                    await _unitOfWork.CompleteAsync();
                }

            }

            var resultList = new
            {
                outofStock
            };


            if (outofStock.Count == 0)
            {
                return Ok(request);
            }
            else
            {
                return BadRequest(resultList);
            }



            //foreach (var items in getRequirement)
            //{

            //    request.Id = 0;

            //    var validateRequest = await _unitOfWork.Planning.ValidateTransformationPlanning(request.TransformId);
            //    request.ProdPlan = DateTime.Parse(validateRequest.ProdPlan);
            //    request.Version = validateRequest.Version;
            //    request.ItemCode = items.ItemCode;
            //    request.ItemDescription = items.ItemDescription;
            //    request.Uom = items.Uom;
            //    request.Quantity = items.Quantity * request.Batch;

            //    await _unitOfWork.Planning.AddNewTransformationRequirements(request);
            //    await _unitOfWork.CompleteAsync();

            //}

            //return Ok("Sucessfully updated!");


        }

        [HttpGet]
        [Route("GetAllPendingTransformationRequest")]
        public async Task<IActionResult> GetAllPendingTransformationRequest()
        {
            var pending = await _unitOfWork.Planning.GetAllPlanningRequest();

            return Ok(pending);
        }

        [HttpGet]
        [Route("GetAllAvailableFormulaCode")]
        public async Task<IActionResult> GetAllAvailableFormulaCode()
        {
            var pending = await _unitOfWork.Planning.GetAllItemCode();

            return Ok(pending);
        }

        [HttpGet]
        [Route("GetAllRejectRequirementsStatus/{id}")]
        public async Task<IActionResult> GetAllRejectRequirementsStatus(int id)
        {
            var pending = await _unitOfWork.Planning.GetAllRejectRequirements(id);

            return Ok(pending);
        }

        [HttpGet]
        [Route("GetAllCancelRequirementsStatus/{id}")]
        public async Task<IActionResult> GetAllCancelRequirementsStatus(int id)
        {
            var pending = await _unitOfWork.Planning.GetAllCancelRequirements(id);

            return Ok(pending);
        }

        [HttpGet]
        [Route("GetAllRejectRequest")]
        public async Task<IActionResult> GetAllRejectRequest()
        {

            var planning = await _unitOfWork.Planning.GetAllRejectRequest();

            return Ok(planning);

        }

    }
}
