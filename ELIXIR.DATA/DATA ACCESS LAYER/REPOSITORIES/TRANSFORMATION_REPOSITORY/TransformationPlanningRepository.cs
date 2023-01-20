using ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.TRANSFORMATION_REPOSITORY
{
    public class TransformationPlanningRepository : ITransformationPlanning

    {
        private readonly StoreContext _context;

        public TransformationPlanningRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TransformationFormulaDto>> GetAllVersionByItemCode(string itemcode)
        {
            var version = _context.Formulas.Select(version => new TransformationFormulaDto
                                                   {
                           
                                                       Id = version.Id, 
                                                       ItemCode = version.ItemCode,
                                                       ItemDescription = version.ItemDescription,
                                                       Uom = version.Uom,
                                                       Version = version.Version,
                                                       Quantity = version.Quantity,
                                                       DateAdded = version.DateAdded.ToString("MM/dd/yyyy"),
                                                       AddedBy = version.AddedBy,
                                                       IsActive = version.IsActive,
                                                       Reason = version.Reason
                                                    });

            return await version.Where(x => x.ItemCode == itemcode)
                                .ToListAsync();
        }


        public async Task<bool> AddNewTransformationRequest(TransformationPlanning planning)
        {

            var validateFormula = await _context.Formulas.Where(x => x.ItemCode == planning.ItemCode)
                                                         .Where(x => x.Version == planning.Version)
                                                         .FirstOrDefaultAsync();


            planning.ItemCode = validateFormula.ItemCode;
            planning.ItemDescription = validateFormula.ItemDescription;
            planning.Quantity = validateFormula.Quantity;
            planning.Uom = validateFormula.Uom;
            planning.DateAdded = DateTime.Now;
            planning.Status = true;
            planning.StatusRequest = "Pending";

            await _context.Transformation_Planning.AddAsync(planning);


            return true;
        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllListOfTransformationRequest()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = planning.Quantity,
                AddedBy = planning.AddedBy,
                Status = planning.Status,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy")
            }).Where(x => x.Status == true)
              .ToListAsync();
        }

        public Task<bool> UpdateTransformPlanning(TransformationPlanning planning)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddNewTransformationRequirements(TransformationRequest request)
        {

            request.IsActive = true;
            request.IsReject = null;

            await _context.Transformation_Request.AddAsync(request);
            return true;


        }

        public async Task<IReadOnlyList<TransformationWithRequirements>> GetAllRequirements(TransformationRequest request)
        {
            var formula =  (from formulacode in _context.Formulas
                                 join requirements in _context.FormulaRequirements on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                                 from requirements in leftJ.DefaultIfEmpty()




                                 select new TransformationWithRequirements
                                 {
                                     Id = requirements.Id,
                                     FormulaCode = formulacode.ItemCode,
                                     Version = formulacode.Version,
                                     ItemCode = requirements.RawMaterial.ItemCode,
                                     ItemDescription = requirements.ItemDescription,
                                     Uom = formulacode.Uom,
                                     Quantity = requirements.Quantity,
                                     IsActive = requirements.IsActive
                                 });

            return await formula.Where(x => x.FormulaCode == request.ItemCode)
                                .Where(x => x.Version == request.Version)
                                .Where(x => x.IsActive == true)
                                .ToListAsync();
                                
        }

        public async Task<TransformationPlanningDto> ValidateTransformationPlanning(int id)
        {

            var validate = _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = planning.Quantity,
                AddedBy = planning.AddedBy,
                Status = planning.Status
            });


            return await validate.Where(x => x.Id == id)
                                 .FirstOrDefaultAsync();

        }

        public async Task<bool> ValidateAllRequirementsWithFormula(TransformationRequest request)
        {
            var formula = await (from formulacode in _context.Formulas
                           join requirements in _context.FormulaRequirements on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                           from requirements in leftJ.DefaultIfEmpty()
                           select new TransformationWithRequirements
                           {
                               Id = requirements != null ? requirements.Id : 0,
                               FormulaCode = formulacode.ItemCode,
                               Version = formulacode.Version,
                               ItemCode = requirements != null ? requirements.RawMaterial.ItemCode : null, 
                               ItemDescription = requirements != null ? requirements.ItemDescription : null,
                               Uom = formulacode.Uom,
                               Quantity = requirements != null  ? requirements.Quantity : 0

                           }).Where(x => x.FormulaCode == request.ItemCode)
                             .Where(x => x.Version == request.Version)
                             .ToListAsync();

            if (formula.Count == 1)
                return false;

            return true;
            
        }

        public async Task<bool> ValidateInputDate(string date)
        {

            var dateNow = DateTime.Now;

            if (Convert.ToDateTime(date.Trim()).Date < Convert.ToDateTime(Convert.ToString(dateNow).Trim()).Date)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<MaterialRequirements>> GetAllListOfRequirementsByTransformId(int id)
        {
           
            var requirement = _context.Transformation_Request.Select(x => new MaterialRequirements
            {
                Id = x.Id,
                TransformationId = x.TransformId,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                Batch = x.Batch,
                Version = x.Version,
                Quantity = Math.Round(Convert.ToDecimal(x.Quantity),2),
                ProdPlan = x.ProdPlan.ToString("MM/dd/yyyy"),
                IsActive = x.IsActive,
                IsPrepared = x.IsPrepared

            });

            return await requirement.Where(x => x.TransformationId == id)                  
                                    .Where(x => x.IsPrepared == false)
                                    .Where(x => x.IsActive == true)
                                    .ToListAsync();
        }                                   


        public async Task<bool> ValidateStocksInRequirement(TransformationPlanning planning)
        {

            var totalRequest = _context.Transformation_Request.GroupBy(x => new
            {
                x.ItemCode,
                x.IsActive

            }).Select(x => new MaterialRequirements
            {
                ItemCode = x.Key.ItemCode,
                Reserve = x.Sum(x => x.Quantity),
                IsActive = x.Key.IsActive

            }).Where(x => x.IsActive == true);


            var validateRequest = (from formula in _context.Formulas
                                   where formula.ItemCode == planning.ItemCode && formula.Version == planning.Version
                                   join requirements in _context.FormulaRequirements
                                   on formula.Id equals requirements.TransformationFormulaId into leftJ
                                   from requirements in leftJ.DefaultIfEmpty()

                                   select new TransformationWithRequirements
                                   {

                                       Id = requirements != null ? requirements.Id : 0,
                                       FormulaCode = formula.ItemCode,
                                       Version = formula.Version,
                                       ItemCode = requirements != null ? requirements.RawMaterial.ItemCode : null,
                                       ItemDescription = requirements != null ? requirements.ItemDescription : null,
                                       Uom = formula.Uom,
                                       Quantity = requirements != null ? requirements.Quantity : 0,

                                   });


            var validateStock = (from request in validateRequest
                                 join stock in _context.WarehouseReceived
                                 on request.ItemCode equals stock.ItemCode into leftJ
                                 from stock in leftJ.DefaultIfEmpty()

                                 join totalR in totalRequest on request.ItemCode equals totalR.ItemCode


                                 group stock by new
                                 {

                                     request.ItemCode,
                                     request.Quantity,
                                     totalR.Reserve,


                                 } into total

                                 select new MaterialRequirements
                                 {
                                     ItemCode = total.Key.ItemCode,
                                     Quantity = total.Key.Quantity,
                                     Reserve = total.Sum(x => x.ActualGood) - total.Key.Reserve

                                 }).Where(x => x.WarehouseItemStatus == true);


            foreach(var items in validateStock)
            {
                    if((items.Quantity * planning.Batch) > items.Reserve || (items.Quantity * planning.Batch) == 0)
                    return false;
            }

            return true;

        }

        public async Task<bool> ApproveTransformationRequest(TransformationPlanning planning)
        {

            var existingInfo = await _context.Transformation_Planning.Where(x => x.Id == planning.Id)
                                                                     .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;


            existingInfo.Is_Approved = true;
            existingInfo.DateApproved = DateTime.Now;
            existingInfo.StatusRequest = "Approved";

            return true;

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllApprovedRequest()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = Math.Round(Convert.ToDecimal(planning.Quantity),2),
                AddedBy = planning.AddedBy,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                Status = planning.Status,
                IsApproved = planning.Is_Approved != null,
                IsPrepared = planning.IsPrepared,
                IsMixed = planning.IsMixed != null
            }).Where(x => x.Status == true)
              .Where(x => x.IsApproved == true)
              .Where(x => x.IsPrepared == false)
              .Where(x => x.IsMixed == false)
              .ToListAsync();
        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllPendingRequest(string status)
        {

            var pendingRequest = _context.Transformation_Planning.OrderBy(x => x.ProdPlan)
                                                                    .Select(planning => new TransformationPlanningDto
                                                                    {
                                                                        Id = planning.Id,
                                                                        ItemCode = planning.ItemCode,
                                                                        ItemDescription = planning.ItemDescription,
                                                                        Uom = planning.Uom,
                                                                        ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                                                                        Version = planning.Version,
                                                                        Batch = planning.Batch,
                                                                        Quantity = Math.Round(Convert.ToDecimal(planning.Quantity), 2),
                                                                        AddedBy = planning.AddedBy,
                                                                        DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                                                                        Status = planning.Status,
                                                                        IsApproved = planning.Is_Approved != null,
                                                                        StatusRemarks = planning.StatusRequest

                                                                    });

            return await pendingRequest.Where(x => x.IsApproved != true)
                                       .ToListAsync();

        }

        public async Task<IReadOnlyList<MaterialRequirements>> GetAllPendingRequestWithRequriements(int id)
        {

            var requirement = _context.Transformation_Request.Select(x => new MaterialRequirements
            {
                Id = x.Id,
                TransformationId = x.TransformId,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                Batch = x.Batch,
                Version = x.Version,
                Quantity = Math.Round(Convert.ToDecimal(x.Quantity), 2),
                ProdPlan = x.ProdPlan.ToString("MM/dd/yyyy"),
                IsActive = x.IsActive,
                IsPrepared = x.IsPrepared

            });

            return await requirement.Where(x => x.TransformationId == id)
                                    .Where(x => x.IsActive == true)
                                    .Where(x => x.IsPrepared == false)
                                    .ToListAsync();

        }

        public async Task<bool> RejectTransformationRequest(TransformationReject reject)
        {

            var existingInfo = await _context.Transformation_Planning.Where(x => x.Id == reject.TransformId)
                                                                     .FirstOrDefaultAsync();

            var validateRequest = await _context.Transformation_Request.Where(x => x.TransformId == reject.TransformId)
                                                                       .Where(x => x.IsActive == true)
                                                                       .ToListAsync();

            if (existingInfo == null)
                return false;

            existingInfo.Status = false;
            existingInfo.Is_Approved = false;
            existingInfo.RejectedDate = DateTime.Now;
            existingInfo.StatusRequest = "Rejected";
            existingInfo.RejectRemarks = reject.RejectRemarks;

            foreach(var items in validateRequest)
            {
                items.IsActive = false;
                items.IsReject = true;
            }

            return true;

        }

        public async Task<bool> ValidateVersionInRequest(TransformationPlanning planning)
        {

            var formula = await (from formulacode in _context.Formulas
                                 join requirements in _context.FormulaRequirements on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                                 from requirements in leftJ.DefaultIfEmpty()
                                 select new TransformationWithRequirements
                                 {
                                     Id = requirements != null ? requirements.Id : 0,
                                     FormulaCode = formulacode.ItemCode,
                                     Version = formulacode.Version,
                                     ItemCode = requirements != null ? requirements.RawMaterial.ItemCode : null,
                                     ItemDescription = requirements != null ? requirements.ItemDescription : null,
                                     Uom = formulacode.Uom,
                                     Quantity = requirements != null ? requirements.Quantity : 0

                                 }).Where(x => x.FormulaCode == planning.ItemCode)
                                   .Where(x => x.Version == planning.Version)
                                   .ToListAsync();

            if (formula.Count <= 1)
                return false;

            return true;


        }

        public async Task<bool> ValidateFormulaCode(TransformationPlanning planning)
        {

            var validate = await _context.Formulas.Where(x => x.ItemCode == planning.ItemCode)
                                                        .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> CancelTransformationRequest(TransformationPlanning planning)
        {

            var existingInfo = await _context.Transformation_Planning.Where(x => x.Id == planning.Id)
                                                                     .FirstOrDefaultAsync();

      
            var existingRequirements = await _context.Transformation_Request.Where(x => x.TransformId == existingInfo.Id)
                                                                            .Where(x => x.IsActive == true)
                                                                            .ToListAsync();
  
            existingInfo.Status = false;
            existingInfo.Is_Approved = false;
            existingInfo.StatusRequest = "Cancelled";
            existingInfo.CancelRemarks = planning.CancelRemarks;

            foreach(var items in existingRequirements)
            {
                items.IsActive = false;
                items.IsCancelled = true;
            }

            return true;

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllCancelledRequest()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = Math.Round(Convert.ToDecimal(planning.Quantity), 2),
                AddedBy = planning.AddedBy,
                Status = planning.Status,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy")
            })
           .Where(x => x.Status == false)
           .ToListAsync();

        }

        public async Task<bool> AddNewTransformationReject(TransformationReject reject)
        {
            await _context.Transformation_Reject.AddAsync(reject);

            return true;
        }

        public async Task<IReadOnlyList<TransformationRejectDto>> GetAllRequestForReject(int id)
        {
            var request =  (from planning in _context.Transformation_Planning
                            where planning.Id == id
                            join requested in _context.Transformation_Request on planning.Id equals requested.TransformId
                            into leftJ
                            from requested in leftJ.DefaultIfEmpty()

                            select new TransformationRejectDto
                            {

                                TransformId = planning.Id,
                                FormulaCode = planning.ItemCode,
                                FormulaDescription = planning.ItemDescription,
                                FormulaQuantity = Math.Round(Convert.ToDecimal(planning.Quantity), 2),
                                Uom = planning.Uom,
                                Batch = planning.Batch,
                                Version = planning.Version,
                                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                                RawmaterialCode = requested.ItemCode,
                                RawmaterialDescription = requested.ItemDescription,
                                RawmaterialQuantity = Math.Round(Convert.ToDecimal(requested.Quantity), 2),
                                IsActive = planning.Status

                            });

            return await request.ToListAsync();

        }

        public async Task<bool> EditRejectTransformationPlanning(TransformationRequest request)
        {
            var existingInfo = await _context.Transformation_Planning.Where(x => x.Id == request.TransformId)
                                                                     .FirstOrDefaultAsync();

            var getRequirements = await _context.Transformation_Request.Where(x => x.TransformId == request.TransformId)
                                                                       .Where(x => x.IsReject == true)
                                                                       .Where(x => x.IsActive == false)
                                                                       .ToListAsync();

            var getFormulaInfo = await _context.Formulas.Where(x => x.ItemCode == request.ItemCode)
                                                        .Where(x => x.Version == request.Version)
                                                        .FirstOrDefaultAsync();


            if (existingInfo == null)
                return false;

            existingInfo.ItemCode = request.ItemCode;
            existingInfo.ItemDescription = getFormulaInfo.ItemDescription;
            existingInfo.Uom = getFormulaInfo.Uom;
            existingInfo.ProdPlan = request.ProdPlan;
            existingInfo.Version = request.Version;
            existingInfo.Batch = request.Batch;
            existingInfo.Quantity = getFormulaInfo.Quantity;
            existingInfo.Status = true;
            existingInfo.StatusRequest = "Pending";

            foreach(var items in getRequirements)
            {
                items.IsReject = null;
                items.IsActive = false;

            }

            return true;

        }

        public async Task<bool> RequestRejectTransformationRequest(TransformationPlanning planning)
        {
            var existing = await _context.Transformation_Planning.Where(x => x.Id == planning.Id)
                                                                 .FirstOrDefaultAsync();

            var validateReject = await _context.Transformation_Reject.Where(x => x.TransformId == planning.Id)
                                                                    .ToListAsync();

            var validateRequirements = await _context.Transformation_Request.Where(x => x.TransformId == planning.Id)
                                                                            .ToListAsync();
            if (existing == null)
                return false;

            existing.Status = true;
            existing.RejectedBy = null;
            existing.RejectedDate = null;

            existing.StatusRequest = "Pending";

            foreach(var items in validateReject)
            {
                items.IsActive = false;
            }

            foreach (var items in validateRequirements)
            {
                items.IsActive = true;
            }




            return true;
            
        }

        public async Task<bool> ValidateStatusRemarks(TransformationPlanning planning)
        {

            var validate = await _context.Transformation_Planning.Where(x => x.Id == planning.Id)
                                                                 .FirstOrDefaultAsync();

            if (validate.StatusRequest != "Pending")
                return false;


            return true;


        }

        public async Task<bool> ValidateIfApproved(int id)
        {
           
            var validate = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                 .FirstOrDefaultAsync();
            if (validate.StatusRequest == "Approved")
                return false;

            return true;

        }

        public async Task<bool> ValidatePlanningRequestIfPrepared(int id)
        {
            var validate = await _context.Transformation_Preparation.Where(x => x.TransformId == id)
                                                                    .ToListAsync();

            if (validate.Count != 0)
                return false;

            return true;

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllPlanningRequest()
        {

            var pending = _context.Transformation_Planning.OrderBy(x => x.ProdPlan)
                                                            .Select(request => new TransformationPlanningDto
                                                            {

                                                            Id = request.Id,
                                                            ItemCode = request.ItemCode,
                                                            ItemDescription = request.ItemDescription,
                                                            Uom = request.Uom,
                                                            ProdPlan = request.ProdPlan.ToString("MM/dd/yyyy"),
                                                            Version = request.Version,
                                                            Batch = request.Batch,
                                                            Quantity = Math.Round(Convert.ToDecimal(request.Quantity), 2),
                                                            DateAdded = request.DateAdded.ToString("MM/dd/yyyy"),
                                                            AddedBy = request.AddedBy,
                                                            StatusRemarks = request.StatusRequest,
                                                            IsPrepared = request.IsPrepared
                                                            });

            return await pending.Where(x => x.StatusRemarks == "Pending")
                                .ToListAsync();

        }

        public async Task<IReadOnlyList<TransformationFormulaDto>> GetAllItemCode()
        {
            var itemcode = _context.Formulas.GroupBy(x => new
            {
                x.ItemCode,
                x.IsActive,

            }).Select(x => new TransformationFormulaDto
            {
                ItemCode = x.Key.ItemCode,
                IsActive = x.Key.IsActive

            });

            return await itemcode.Where(x => x.IsActive == true)
                                 .ToListAsync();

        }

        public async Task<decimal> GetAllItemsWithStock(string itemcode)
        {
            var compute = await _context.WarehouseReceived.Where(x => x.ItemCode == itemcode)
                                                          .Where(x => x.IsActive == true)
                                                          .SumAsync(x => x.ActualGood);

            var computeRequest = await _context.Transformation_Request.Where(x => x.ItemCode == itemcode)
                                                                      .Where(x => x.IsActive == true)
                                                                      .SumAsync(x => x.Quantity);

            var computeOrderReserve = await _context.Orders.Where(x => x.ItemCode == itemcode)
                                                           .Where(x => x.IsActive == true)
                                                           .Where(x => x.PreparedDate != null)
                                                           .SumAsync(x => x.QuantityOrdered);

            var issueOut = await _context.MiscellaneousIssueDetails.Where(x => x.ItemCode == itemcode)
                                                                   .Where(x => x.IsActive == true)
                                                                    .Where(x => x.IsTransact == true)
                                                                    .SumAsync(x => x.Quantity);

            var final = compute - computeRequest - computeOrderReserve - issueOut;

            return final;
        }

        public async Task<bool> ValidateRequirement(string itemcode, int batch, decimal quantity)
        {

            var computeRequest = await _context.Transformation_Request.Where(x => x.ItemCode == itemcode)
                                                                      .Where(x => x.IsActive == true)
                                                                      .SumAsync(x => x.Quantity);

            var computeStock = await _context.WarehouseReceived.Where(x => x.ItemCode == itemcode)
                                                               .Where(x => x.IsActive == true)
                                                               .SumAsync(x => x.ActualGood);

            var issueOut = await _context.MiscellaneousIssueDetails.Where(x => x.ItemCode == itemcode)
                                                                   .Where(x => x.IsActive == true)
                                                                   .Where(x => x.IsTransact == true)
                                                                   .SumAsync(x => x.Quantity);

            var computeOrderReserve = await _context.Orders.Where(x => x.ItemCode == itemcode)
                                                           .Where(x => x.IsActive == true)
                                                           .Where(x => x.PreparedDate != null)
                                                           .SumAsync(x => x.QuantityOrdered);

            if ((quantity * batch) > (computeStock - computeRequest - computeOrderReserve - issueOut))
                return false;

            return true;
        }

        public async Task<TransformationPlanningDto> GetBatchByTransformId(int id)
        {
            return await _context.Transformation_Planning.Select(x => new TransformationPlanningDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                Batch = x.Batch, 
  
            })
           .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IReadOnlyList<TransformationWithRequirements>> GetRequirementsStock(string itemcode, int version)
        {
            var formula = (from formulacode in _context.Formulas
                           join requirements in _context.FormulaRequirements on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                           from requirements in leftJ.DefaultIfEmpty()

                           select new TransformationWithRequirements
                           {
                               Id = requirements.Id,
                               FormulaCode = formulacode.ItemCode,
                               Version = formulacode.Version,
                               ItemCode = requirements.RawMaterial.ItemCode,
                               ItemDescription = requirements.ItemDescription,
                               Uom = formulacode.Uom,
                               Quantity = Math.Round(Convert.ToDecimal(requirements.Quantity), 2),
                               IsActive = requirements.IsActive
                           });

            return await formula.Where(x => x.FormulaCode == itemcode)
                                .Where(x => x.Version == version)
                                .Where(x => x.IsActive == true)
                                .ToListAsync();
        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllPendingRequestNotif()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = planning.Quantity,
                AddedBy = planning.AddedBy,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                Status = planning.Status,
                IsApproved = planning.Is_Approved != null,
                StatusRemarks = planning.StatusRequest

            })
           .Where(x => x.StatusRemarks == "Pending")
           .ToListAsync();
        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllRejectRequestNotif()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = planning.Quantity,
                AddedBy = planning.AddedBy,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                Status = planning.Status,
                IsApproved = planning.Is_Approved != null,
                StatusRemarks = planning.StatusRequest

            })
        .Where(x => x.StatusRemarks == "Rejected")
        .ToListAsync();
        }

        public async Task<IReadOnlyList<MaterialRequirements>> GetAllRejectRequirements(int id)
        {
            var requirements = (from request in _context.Transformation_Request
                                where request.TransformId == id && request.IsReject == true && request.IsActive == false
                                join reject in _context.Transformation_Reject             
                                on request.TransformId equals reject.TransformId into leftJ

                                from reject in leftJ.DefaultIfEmpty()

                                group reject by new
                                {

                                   request.TransformId, 
                                   request.ItemCode, 
                                   request.ItemDescription,
                                   request.Uom,
                                   request.Batch,
                                   request.Version,
                                   request.Quantity,
                                   request.ProdPlan,
                                   request.IsPrepared,
                                   reject.IsActive

                                }into total

                                select new MaterialRequirements
                                {
                                    TransformationId = total.Key.TransformId,
                                    ItemCode = total.Key.ItemCode,
                                    ItemDescription = total.Key.ItemDescription,
                                    Uom = total.Key.Uom,
                                    Batch = total.Key.Batch,
                                    Version = total.Key.Version,
                                    Quantity = total.Key.Quantity,
                                    ProdPlan = total.Key.ProdPlan.ToString("MM/dd/yyyy"),
                                    IsActive = total.Key.IsActive,
                                    IsPrepared = total.Key.IsPrepared,
                                    RejectStatus = total.Key.IsActive
                                });

            return await requirements.ToListAsync();
        }

        public async Task<IReadOnlyList<MaterialRequirements>> GetAllCancelRequirements(int id)
        {

            var requirements = (from request in _context.Transformation_Request
                                where request.TransformId == id && request.IsCancelled == true
                                join planning in _context.Transformation_Planning
                                on request.TransformId equals planning.Id into leftJ

                                from planning in leftJ.DefaultIfEmpty()

                                group planning by new
                                {

                                    request.TransformId,
                                    request.ItemCode,
                                    request.ItemDescription,
                                    request.Uom,
                                    request.Batch,
                                    request.Version,
                                    request.Quantity,
                                    request.ProdPlan,
                                    request.IsPrepared,
                                    planning.CancelRemarks
                                
                                } into total

                                where total.Key.CancelRemarks != null

                                select new MaterialRequirements
                                {
                                    TransformationId = total.Key.TransformId,
                                    ItemCode = total.Key.ItemCode,
                                    ItemDescription = total.Key.ItemDescription,
                                    Uom = total.Key.Uom,
                                    Batch = total.Key.Batch,
                                    Version = total.Key.Version,
                                    Quantity = Math.Round(Convert.ToDecimal(total.Key.Quantity), 2),
                                    ProdPlan = total.Key.ProdPlan.ToString("MM/dd/yyyy"),
                                    IsPrepared = total.Key.IsPrepared,
                                    CancelRemarks = total.Key.CancelRemarks
                                });

            return await requirements.ToListAsync();

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllRejectRequest()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {

                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Version = planning.Version,
                Batch = planning.Batch,
                Quantity = Math.Round(Convert.ToDecimal(planning.Quantity),2),
                AddedBy = planning.AddedBy,
                Status = planning.Status,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                StatusRemarks = planning.StatusRequest,
                RejectRemarks = planning.RejectRemarks

            })
         .Where(x => x.StatusRemarks == "Rejected")
         .ToListAsync();
        }

        public async Task<bool> ValidateIfPrepared(int id)
        {
            var validate = await _context.Transformation_Preparation.Where(x => x.TransformId == id)
                                                                    .ToListAsync();


            if (validate.Count != 0)
                return false;


            return true;

        }

        public Task<bool> ValidateIfDecimal(int batch)
        {

            var validate = decimal.TryParse(Convert.ToString(batch), out decimal value);

            if (validate == true)
                return Task.FromResult(true);


            return Task.FromResult(false);
        }
    }   
}

