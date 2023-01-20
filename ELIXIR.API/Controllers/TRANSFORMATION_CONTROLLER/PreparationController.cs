using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.TRANSFORMATION_CONTROLLER
{

    public class PreparationController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PreparationController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("GetAllRequirementsByTransformId")]
        public async Task<IActionResult> GetAllRequirementsByTransformId([FromQuery] TransformationPlanning planning)
        {

            var requirement = await _unitOfWork.Preparation.GetAllListOfTransformationByTransformationId(planning);

            return Ok(requirement);

        }

        [HttpPut]
        [Route("PrepareMaterialsForRequest/{id}")]
        public async Task<IActionResult> PrepareMaterialsForRequest(int id, [FromBody] TransformationPreparation preparation)
        {

            List<TransformationPreparation> PrepList = new List<TransformationPreparation>();
            TransformationPreparation usr = null;


            decimal tempWeighingScale = 0;


            if (id != preparation.TransformId)
                return BadRequest();


            var validate = await _unitOfWork.Preparation.ValidatePreparedMaterials(preparation.TransformId, preparation.ItemCode);

            if (validate == false)
                return BadRequest("Already prepared this material!");

            var validateApproved = await _unitOfWork.Preparation.ValidateIfApproved(preparation.TransformId);

            if (validateApproved == false)
                return BadRequest("Preparing material failed, please check if the request is approved");


            var getRemainingStocks = await _unitOfWork.Preparation.GetAllRemainingStocksPerReceivingId(preparation.ItemCode);

            var getInfoRequest = await _unitOfWork.Preparation.GetQuantityAndBatch(preparation.TransformId, preparation.ItemCode);


            foreach (var items in getRemainingStocks)
            {

                if (preparation.WeighingScale != 0)
                {

                    if (items.Remaining < preparation.WeighingScale)
                    {

                        usr = new TransformationPreparation();

                        usr.TransformId = preparation.TransformId;
                        usr.WarehouseId = items.WarehouseId;
                        usr.WeighingScale = items.Remaining;
                        usr.ItemCode = items.ItemCode;
                        usr.ItemDescription = items.ItemDescription;
                        usr.ManufacturingDate = Convert.ToDateTime(items.ManufacturingDate);
                        usr.ExpirationDate = Convert.ToDateTime(items.ExpirationDate);
                        usr.QuantityNeeded = getInfoRequest.QuantityNeeded;
                        usr.Batch = getInfoRequest.Batch;
                        usr.IsActive = true;
                        usr.PreparedDate = DateTime.Now;
                        usr.PreparedBy = preparation.PreparedBy;

                        PrepList.Add(usr);

                        tempWeighingScale = preparation.WeighingScale - items.Remaining;
                        preparation.WeighingScale = preparation.WeighingScale - items.Remaining;

                    }

                    else
                    {
                        usr = new TransformationPreparation();

                        usr.TransformId = preparation.TransformId;
                        usr.WeighingScale = preparation.WeighingScale;
                        usr.WarehouseId = items.WarehouseId;
                        usr.ItemCode = items.ItemCode;
                        usr.ItemDescription = items.ItemDescription;
                        usr.ManufacturingDate = Convert.ToDateTime(items.ManufacturingDate);
                        usr.ExpirationDate = Convert.ToDateTime(items.ExpirationDate);
                        usr.QuantityNeeded = getInfoRequest.QuantityNeeded;
                        usr.Batch = getInfoRequest.Batch;
                        usr.IsActive = true;
                        usr.PreparedDate = DateTime.Now;
                        usr.PreparedBy = preparation.PreparedBy;

                        PrepList.Add(usr);

                        preparation.WeighingScale = preparation.WeighingScale - preparation.WeighingScale;
                    }

                }

            }

            if (preparation.WeighingScale == 0)
            {

                if (PrepList.Count != 0)
                {
                    foreach (var item in PrepList)
                    {

                        await _unitOfWork.Preparation.PrepareTransformationMaterials(item);

                    }

                    await _unitOfWork.Preparation.UpdateRequestStatus(preparation);
                    await _unitOfWork.CompleteAsync();

                    await _unitOfWork.Preparation.UpdatePlanningStatus(preparation);
                    await _unitOfWork.CompleteAsync();

                }

            }
            else
                return BadRequest("Prepared failed, Not enough stocks!");


            return Ok(PrepList);


        }

        [HttpGet]
        [Route("GetAllRequestForMixing")]
        public async Task<IActionResult> GetAllRequestForMixing()
        {

            var mixing = await _unitOfWork.Preparation.GetAllListOfTransformationRequestForMixing();

            return Ok(mixing);

        }

        [HttpGet]
        [Route("GetRawmaterialDetailsInWarehouse")]
        public async Task<IActionResult> GetRawmaterialDetailsInWarehouse([FromQuery] int id, [FromQuery] string code)
        {

            var requirement = await _unitOfWork.Preparation.GetReceivingDetailsForRawmaterials(id, code);

            return Ok(requirement);

        }

        [HttpGet]
        [Route("GetTransformationFormula")]
        public async Task<IActionResult> GetTransformationFormula()
        {

            var planning = await _unitOfWork.Preparation.GetAllTransformationFormulaInformation();

            return Ok(planning);

        }

        [HttpGet]
        [Route("GetAllStocks")]
        public async Task<IActionResult> GetAllStocks()
        {

            var planning = await _unitOfWork.Preparation.GetAllAvailableStocks();

            return Ok(planning);

        }

        [HttpGet]
        [Route("GetRemaining")]
        public async Task<IActionResult> GetRemainingSample([FromQuery] string itemcode)
        {

            var planning = await _unitOfWork.Preparation.GetAllRemainingStocksPerReceivingId(itemcode);

            return Ok(planning);

        }

        [HttpPut]
        [Route("FinishedMixedMaterialsForWarehouse/{id}")]
        public async Task<IActionResult> FinishedMixedMaterialsForWarehouse(int id, [FromBody] WarehouseReceiving warehouse)
        {


            var validate = await _unitOfWork.Preparation.ValidateRequestAndPreparation(id);

            if (validate == false)
                return BadRequest("Mixing failed, you have materials left to prepared!");

            var validateMixing = await _unitOfWork.Preparation.FinishedMixedMaterialsForWarehouse(warehouse, id);

            if (validateMixing == false)
                return BadRequest("Already mixed all available batch for this request!");

            await _unitOfWork.CompleteAsync();

            await _unitOfWork.Preparation.CompareBatchCount(id);

            await _unitOfWork.CompleteAsync();

            return Ok(warehouse);

        }

        [HttpGet]
        [Route("GetTransformationFormulaPagination")]
        public async Task<ActionResult<IEnumerable<TransformationPlanningDto>>> GetAllFormulaWithPagination([FromQuery] UserParams userParams)
        {
            var preparation = await _unitOfWork.Preparation.GetAllTransformationFormulaInformationPagination(userParams);

            Response.AddPaginationHeader(preparation.CurrentPage, preparation.PageSize, preparation.TotalCount, preparation.TotalPages, preparation.HasNextPage, preparation.HasPreviousPage);

            var preparationResult = new
            {
                preparation,
                preparation.CurrentPage,
                preparation.PageSize,
                preparation.TotalCount,
                preparation.TotalPages,
                preparation.HasNextPage,
                preparation.HasPreviousPage
            };

            return Ok(preparationResult);
        }

        [HttpGet]
        [Route("GetAllTransformationForMixing")]
        public async Task<IActionResult> GetAllTransformationForMixing()
        {

            var planning = await _unitOfWork.Preparation.GetAllTransformationForMixing();

            return Ok(planning);

        }

        [HttpGet]
        [Route("GetAllTransformationForMixingPagination")]
        public async Task<ActionResult<IEnumerable<TransformationPlanningDto>>> GetAllTransformationForMixingPagination([FromQuery] UserParams userParams)
        {
            var mixing = await _unitOfWork.Preparation.GetAllTransformationForMixingPagination(userParams);

            Response.AddPaginationHeader(mixing.CurrentPage, mixing.PageSize, mixing.TotalCount, mixing.TotalPages, mixing.HasNextPage, mixing.HasPreviousPage);

            var mixingResult = new
            {
                mixing,
                mixing.CurrentPage,
                mixing.PageSize,
                mixing.TotalCount,
                mixing.TotalPages,
                mixing.HasNextPage,
                mixing.HasPreviousPage
            };

            return Ok(mixingResult);
        }

        [HttpGet]
        [Route("GetAllRequirementsForMixing")]
        public async Task<IActionResult> GetAllRequirementsForMixing([FromQuery] int id)
        {

            var requirement = await _unitOfWork.Preparation.GetAllRequirementsForMixing(id);

            return Ok(requirement);

        }

        [HttpGet]
        [Route("GetAllBatchRemainingPerMixing")]
        public async Task<IActionResult> GetBatchRemaining([FromQuery] int id)
        {

            var batch = await _unitOfWork.Preparation.CountBatch(id);

            return Ok(batch);

        }


    }
}
