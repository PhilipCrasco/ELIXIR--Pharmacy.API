using ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.TRANSFORMATION_REPOSITORY
{
    public class TransformationPreparationRepository : ITransformationPreparation

    {
        private readonly StoreContext _context;
        public TransformationPreparationRepository(StoreContext context)
        {
            _context = context;

        }

        public async Task<IReadOnlyList<TransformationPreparationDto>> GetAllListOfTransformationByTransformationId(TransformationPlanning planning)
        {
            var prep = (from plan in _context.Transformation_Planning
                        join request in _context.Transformation_Request
                        on plan.Id equals request.TransformId into leftJ
                        from request in leftJ.DefaultIfEmpty()

                        select new TransformationPreparationDto
                        {

                            TranformationId = plan.Id,
                            FormulaCode = plan.ItemCode,
                            FormulaDescription = plan.ItemDescription,
                            FormulaQuantity = Math.Round(Convert.ToDecimal(planning.Quantity),2),
                            RawmaterialCode = request.ItemCode,
                            RawmaterialDescription = request.ItemDescription,
                            Uom = plan.Uom,
                            Batch = request.Batch,
                            RawmaterialQuantity = Math.Round(Convert.ToDecimal(request.Quantity), 2),
                            IsRequirementActive = request.IsActive,
                            IsPrepared = request.IsPrepared


                        }).Where(x => x.TranformationId == planning.Id)
                          .Where(x => x.IsPrepared == false)
                          .Where(x => x.IsRequirementActive == true);

            return await prep.ToListAsync();

        }

        public async Task<bool> AddPreparationMaterials(TransformationPreparation preparation)
        {
            await _context.Transformation_Preparation.AddAsync(preparation);
       
            return true;

        }

        public async Task<bool> PrepareTransformationMaterials(TransformationPreparation preparation)
        {

            await AddPreparationMaterials(preparation);
         
            return true;
        } 

        public async Task<bool> UpdatePrepareStatusInRequest(int id)
        {
            var validate = await _context.Transformation_Request
                                                          .Where(x => x.TransformId == id)
                                                          .Where(x => x.IsPrepared == false)
                                                          .Where(x => x.IsActive == true)
                                                          .ToListAsync();

            var getRequest = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                   .FirstOrDefaultAsync();
            if (validate.Count == 0)
                getRequest.IsPrepared = true;

            return true;
        }

        public async Task<bool> ValidatePreparedMaterials(int id, string code)
        {
            var validate = await _context.Transformation_Request.Where(x => x.TransformId == id)
                                                          .Where(x => x.ItemCode == code)
                                                          .Where(x => x.IsPrepared == false)
                                                          .Where(x => x.IsActive == true)
                                                          .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllListOfTransformationRequestForMixing()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                Version = planning.Version,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Batch = planning.Batch,
                Quantity = Math.Round(Convert.ToDecimal(planning.Quantity),2),
                AddedBy = planning.AddedBy,
                Status = planning.Status,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                IsPrepared = planning.IsPrepared,
                IsApproved = planning.Is_Approved != null,
                StatusRemarks = planning.StatusRequest,
                IsMixed = planning.IsMixed != null


            }).Where(x => x.Status == true)
              .Where(x => x.IsApproved == true)
              .Where(x => x.IsPrepared == true)
              .Where(x => x.IsMixed == false)
              .Where(x => x.StatusRemarks == "Approved")
              .ToListAsync();


        }

        public async Task<bool> ValidateIfApproved(int id)
        {
            var validate = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                 .Where(x => x.Status == true)
                                                                 .Where(x => x.Is_Approved == true)
                                                                 .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> AddMixingTransformation(WarehouseReceiving warehouse)
        {
            await _context.WarehouseReceived.AddAsync(warehouse);

            return true;
        }

        public async Task<bool> FinishedMixedMaterialsForWarehouse(WarehouseReceiving warehouse, int id)
        {
            DateTime dateNow = DateTime.Now;

            var mixing = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                               .FirstOrDefaultAsync();

            var countBatch = await _context.WarehouseReceived.Where(x => x.TransformId == warehouse.TransformId)
                                                             .ToListAsync();

            if (mixing.Batch <= countBatch.Count)
                return false;


            warehouse.ManufacturingDate = DateTime.Now;
            warehouse.ItemCode = mixing.ItemCode;
            warehouse.ItemDescription = mixing.ItemDescription;
            warehouse.Uom = mixing.Uom;
            warehouse.Supplier = "RDF";
            warehouse.ReceivingDate = DateTime.Now;
            warehouse.TransactionType = "Transformation";
            warehouse.ActualGood = mixing.Quantity;
            warehouse.IsWarehouseReceive = true;
            warehouse.IsActive = true;
            warehouse.Expiration = warehouse.Expiration;
            warehouse.ExpirationDays = warehouse.Expiration.Subtract(dateNow).Days;
            warehouse.TransformId = id;
            warehouse.BatchCount = countBatch.Count + 1;

            await AddMixingTransformation(warehouse);

            return true;

        }

        public async Task<RawmaterialDetailsFromWarehouseDto> GetReceivingDetailsForRawmaterials(int id, string code)
        {

            var totalPreparation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });


            var totalMoveOrder = _context.MoveOrders.Where(x => x.IsActive == true)
                                                    .Where(x => x.IsPrepared == true)
              .GroupBy(x => new
              {
                  x.ItemCode,
                  x.WarehouseId,

              }).Select(x => new ItemStocks
              {
                  ItemCode = x.Key.ItemCode,
                  Out = x.Sum(x => x.QuantityOrdered),
                  WarehouseId = x.Key.WarehouseId
              });

            var issueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsTransact == true)
              .GroupBy(x => new
              {
                  x.ItemCode,
                  x.WarehouseId,

              }).Select(x => new ItemStocks
              {
                  ItemCode = x.Key.ItemCode,
                  Out = x.Sum(x => x.Quantity),
                  WarehouseId = x.Key.WarehouseId
              });

            var warehouseStock = (from warehouse in _context.WarehouseReceived
                                  where warehouse.IsActive == true
                                  join req in totalPreparation
                                  on warehouse.Id equals req.WarehouseId into leftJ
                                  from req in leftJ.DefaultIfEmpty()

                                  join totalMoveOut in totalMoveOrder
                                  on warehouse.Id equals totalMoveOut.WarehouseId
                                  into leftJ2
                                  from totalMoveOut in leftJ2.DefaultIfEmpty()

                                  join issue in issueOut
                                  on warehouse.Id equals issue.WarehouseId
                                  into leftJ3
                                  from issue in leftJ3.DefaultIfEmpty()

                                  group new
                                  {
                                      warehouse,
                                      req,
                                      totalMoveOut,
                                      issue
                                  }

                                  by new
                                  {

                                      warehouse.Id,
                                      warehouse.Supplier,
                                      warehouse.ItemCode,
                                      warehouse.ItemDescription,
                                      warehouse.ManufacturingDate,
                                      warehouse.Expiration,
                                      warehouse.ExpirationDays,
                                      warehouse.ActualGood,
                                      PreparationOut = req.Out != null ? req.Out : 0,
                                      MoveOrderOut = totalMoveOut.Out != null ? totalMoveOut.Out : 0,
                                      IssueOut = issue.Out != null ? issue.Out : 0


                                  } into total

                                  select new
                                  {

                                      total.Key.Id,
                                      total.Key.Supplier,
                                      total.Key.ItemCode,
                                      total.Key.ItemDescription,
                                      total.Key.ManufacturingDate,
                                      total.Key.Expiration,
                                      total.Key.ExpirationDays,
                                      Reserve = total.Key.PreparationOut + total.Key.MoveOrderOut + total.Key.IssueOut,
                                      total.Key.ActualGood,
                                      RemainingStocks = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut - total.Key.IssueOut

                                  });

            var warehousereceived = (from request in _context.Transformation_Request
                                     where request.IsActive == true && request.IsPrepared == false && request.TransformId == id
                                     join warehouse in warehouseStock
                                     on request.ItemCode equals warehouse.ItemCode

                                     group warehouse by new
                                     {

                                         warehouse.Id,
                                         warehouse.Supplier,
                                         warehouse.ItemCode,
                                         warehouse.ItemDescription,
                                         warehouse.ManufacturingDate,
                                         warehouse.Expiration,
                                         warehouse.ExpirationDays,
                                         request.Quantity,
                                         request.Batch,
                                         request.IsPrepared,
                                         request.TransformId,
                                         warehouse.RemainingStocks

                                     } into total

                                     orderby total.Key.ExpirationDays ascending

                                     select new RawmaterialDetailsFromWarehouseDto
                                     {
                                         TransformId = total.Key.TransformId,
                                         WarehouseReceivedId = total.Key.Id,
                                         Supplier = total.Key.Supplier,
                                         ItemCode = total.Key.ItemCode,
                                         ItemDescription = total.Key.ItemDescription,
                                         ManufacturingDate = total.Key.ManufacturingDate,
                                         ExpirationDate = total.Key.Expiration,
                                         ExpirationDays = total.Key.ExpirationDays,
                                         QuantityNeeded = Math.Round(Convert.ToDecimal(total.Key.Quantity), 2),
                                         Batch = total.Key.Batch,
                                         IsPrepared = total.Key.IsPrepared,
                                         Balance = total.Key.RemainingStocks

                                     });

            return await warehousereceived.Where(x => x.ItemCode == code)
                                          .Where(x => x.Balance != 0)
                                          .FirstOrDefaultAsync();

        }

        public async Task<bool> UpdatedWarehouseStock(string code)
        {
   
            var warehouseStock = await _context.WarehouseReceived.Where(x => x.ItemCode == code)
                                                                 .OrderBy(x => x.ExpirationDays)                                                           
                                                                 .FirstOrDefaultAsync();

            var computePrepared = await _context.Transformation_Preparation.Where(x => x.ItemCode == code)
                                                                           .Where(x => x.IsActive == true)
                                                                           .SumAsync(x => x.WeighingScale);

            var warehousereceived = await (from request in _context.Transformation_Request
                                     where request.IsActive == true
                                     join warehouse in _context.WarehouseReceived
                                     on request.ItemCode equals warehouse.ItemCode into leftJ
                                     from warehouse in leftJ.DefaultIfEmpty()
                                     where warehouse.ItemCode == code
                                     orderby warehouse.ExpirationDays ascending

                                     select new RawmaterialDetailsFromWarehouseDto
                                     {
                                         WarehouseReceivedId = warehouse.Id,
                                         Supplier = warehouse.Supplier,
                                         ItemCode = warehouse.ItemCode,
                                         ItemDescription = warehouse.ItemDescription,
                                         ManufacturingDate = warehouse.ManufacturingDate,
                                         ExpirationDate = warehouse.Expiration,
                                         ExpirationDays = warehouse.ExpirationDays,
                                         Balance = warehouse.ActualGood - computePrepared,
                                         QuantityNeeded = request.Quantity,
                                         Batch = request.Batch

                                     }).Where(x => x.WarehouseItemStatus == true)
                                       .FirstOrDefaultAsync();

            return false;

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllTransformationFormulaInformation()
        {

            var transformplanning =  (from planning in _context.Transformation_Planning
                                     where planning.Is_Approved == true && planning.StatusRequest == "Approved" && planning.IsPrepared == false
                                     join warehouse in _context.WarehouseReceived
                                     on planning.ItemCode equals warehouse.ItemCode into leftJ
                                     from warehouse in leftJ.DefaultIfEmpty()

                                     group warehouse by new
                                     {
                                     
                                         planning.Id,
                                         planning.ItemCode,
                                         planning.ItemDescription,
                                         planning.Quantity,
                                         planning.Batch,

                                     } into total

    
                                      select new TransformationPlanningDto
                                 {
                                     Id = total.Key.Id,
                                     ItemCode = total.Key.ItemCode,
                                     ItemDescription = total.Key.ItemDescription,
                                     Quantity = Math.Round(Convert.ToDecimal(total.Key.Quantity * total.Key.Batch), 2),
                                     Batch = total.Key.Batch,
                                     WarehouseStock = total.Sum(x => x.ActualGood),
                         
                                 });

            return await transformplanning.ToListAsync();


        }

        public async Task<decimal> ValidatePreparedItems(TransformationPreparation preparation)
        {
            var validate =   _context.Transformation_Preparation.Where(x => x.TransformId == preparation.TransformId)
                                                                .Where(x => x.ItemCode == preparation.ItemCode)
                                                                .SumAsync(x => x.WeighingScale);

            var total = validate;

            return await total;

        }

        public async Task<IReadOnlyList<ForTesting>> GetAllAvailableStocks()
        {

            var totalOut = (from warehouse in _context.WarehouseReceived
                       join req in _context.Transformation_Preparation
                       on warehouse.Id equals req.WarehouseId into leftJ

                       from req in leftJ.DefaultIfEmpty()

                       group req by new
                       {

                           warehouse.Id,
                           warehouse.Supplier,
                           warehouse.ItemCode,
                           warehouse.ItemDescription,
                           warehouse.ManufacturingDate,
                           warehouse.Expiration,
                           warehouse.ExpirationDays,
                           warehouse.ActualGood

                       } into total

                       select new
                       {

                           total.Key.Id,
                           total.Key.Supplier,
                           total.Key.ItemCode,
                           total.Key.ItemDescription,
                           total.Key.ManufacturingDate,
                           total.Key.Expiration,
                           total.Key.ExpirationDays,
                           Reserve = total.Sum(x => x.WeighingScale),
                           total.Key.ActualGood

                       });

            var warehousestock = _context.WarehouseReceived.OrderBy(x => x.ExpirationDays)                                                    
                                                           .Select(x => new ForTesting
                                                            {
                                                                ReceivedId = x.Id,
                                                                ItemCode = x.ItemCode,
                                                                ExpirationDays = x.ExpirationDays,
                                                                In = x.ActualGood,

                                                            });


            return await warehousestock.ToListAsync();

                                                              
        }

        public async Task<IReadOnlyList<ItemStocks>> GetAllRemainingStocksPerReceivingId(string itemcode)
        {
            var totalout = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                .GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var totalMoveOrder = _context.MoveOrders.Where(x => x.IsActive == true)
                                                    .Where(x => x.IsPrepared == true)
               .GroupBy(x => new
               {
                   x.ItemCode,
                   x.WarehouseId,

               }).Select(x => new ItemStocks
               {
                   ItemCode = x.Key.ItemCode,
                   Out = x.Sum(x => x.QuantityOrdered),
                   WarehouseId = x.Key.WarehouseId
               });

            var issueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                        .Where(x => x.IsTransact == true)
         .GroupBy(x => new
         {
             x.ItemCode,
             x.WarehouseId,

         }).Select(x => new ItemStocks
         {
             ItemCode = x.Key.ItemCode,
             Out = x.Sum(x => x.Quantity),
             WarehouseId = x.Key.WarehouseId
         });

            var totalRemaining = (from totalIn in _context.WarehouseReceived
                                  where totalIn.ItemCode == itemcode && totalIn.IsActive == true
                                  join totalOut in totalout
                                  on totalIn.Id equals totalOut.WarehouseId
                                  into leftJ
                                  from totalOut in leftJ.DefaultIfEmpty()

                                  join totalMoveOut in totalMoveOrder
                                  on totalIn.Id equals totalMoveOut.WarehouseId
                                  into leftJ2
                                  from totalMoveOut in leftJ2.DefaultIfEmpty()

                                  join totalIssue in issueOut
                                  on totalIn.Id equals totalIssue.WarehouseId
                                  into leftJ3
                                  from totalIssue in leftJ3.DefaultIfEmpty()

                                  group new
                                  {
                                      totalIn,
                                      totalOut,
                                      totalMoveOut,
                                      totalIssue
                                  }

                                  by new
                                  {
                                      totalIn.Id,
                                      totalIn.ItemCode,
                                      totalIn.ItemDescription,
                                      totalIn.ManufacturingDate,
                                      totalIn.Expiration,
                                      totalIn.ActualGood,
                                      totalIn.ExpirationDays,
                                      PreparationOut = totalOut.Out != null ? totalOut.Out : 0,
                                      MoveOrderOut = totalMoveOut.Out != null ? totalMoveOut.Out : 0,
                                      IssueOut = totalIssue.Out != null ? totalIssue.Out : 0

                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.Id,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      ManufacturingDate = total.Key.ManufacturingDate,
                                      ExpirationDate = total.Key.Expiration,
                                      ExpirationDays = total.Key.ExpirationDays,
                                      In = total.Key.ActualGood,
                                      Out = total.Key.PreparationOut,
                                      Remaining = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut - total.Key.IssueOut

                                  });

            return await totalRemaining.Where(x => x.Remaining != 0)
                                       .ToListAsync();

        }

        public async Task<RawmaterialDetailsFromWarehouseDto> GetQuantityAndBatch(int id, string code)
        {

            var getInfo = _context.Transformation_Request.Select(x => new RawmaterialDetailsFromWarehouseDto
            {

                TransformId = x.TransformId,
                ItemCode = x.ItemCode,
                QuantityNeeded = x.Quantity,
                Batch = x.Batch,

            });

            return await getInfo.Where(x => x.TransformId == id)
                                .Where(x => x.ItemCode == code)
                                .FirstOrDefaultAsync();

        }

        public async Task<bool> UpdateRequestStatus(TransformationPreparation preparation)
        {
            var existingList = await _context.Transformation_Request.Where(x => x.ItemCode == preparation.ItemCode)
                                                                    .Where(x => x.TransformId == preparation.TransformId)
                                                                    .Where(x => x.IsActive == true)
                                                                    .FirstOrDefaultAsync();

            var existingPlan = await _context.Transformation_Planning.Where(x => x.Id == preparation.TransformId)
                                                                     .FirstOrDefaultAsync();

            if (existingList == null)
            {
                return false;
            }
            else
            {
                existingList.IsPrepared = true;
                existingList.Quantity = preparation.WeighingScale;
            }

           
            return true;
        }

        public async Task<bool> ValidateRequestAndPreparation(int id)
        {
            var request = await _context.Transformation_Request.Where(x => x.TransformId == id)
                                                               .Where(x => x.IsPrepared == false)
                                                               .Where(x => x.IsActive == true)
                                                               .ToListAsync();
            if (request.Count != 0)
                return false;


            return true;
       
        }

        public async Task<PagedList<TransformationPlanningDto>> GetAllTransformationFormulaInformationPagination(UserParams userParams)
        {


            var transformplanning = (from planning in _context.Transformation_Planning
                                     where planning.Is_Approved == true && planning.StatusRequest == "Approved" && planning.IsPrepared == false
                                     join warehouse in _context.WarehouseReceived
                                     on planning.ItemCode equals warehouse.ItemCode into leftJ
                                     from warehouse in leftJ.DefaultIfEmpty()

                                     group warehouse by new
                                     {

                                         planning.Id,
                                         planning.ProdPlan,
                                         planning.ItemCode,
                                         planning.ItemDescription,
                                         planning.Quantity,
                                         planning.Batch,
                                         planning.Is_Approved 

                                     } into total

                                     orderby total.Key.ProdPlan ascending

                                     select new TransformationPlanningDto
                                     {
                                         Id = total.Key.Id,
                                         ItemCode = total.Key.ItemCode,
                                         ItemDescription = total.Key.ItemDescription,
                                         ProdPlan = total.Key.ProdPlan.ToString("MM/dd/yyyy"),
                                         Quantity = Math.Round(Convert.ToDecimal(total.Key.Quantity * total.Key.Batch), 2),
                                         Batch = total.Key.Batch,
                                         WarehouseStock = total.Sum(x => x.ActualGood),
                                         IsApproved = total.Key.Is_Approved != null
                                     });

            return await PagedList<TransformationPlanningDto>.CreateAsync(transformplanning, userParams.PageNumber, userParams.PageSize);


        }

        public async Task<bool> UpdatePlanningStatus(TransformationPreparation preparation)
        {
            var getRequestList = await _context.Transformation_Request.Where(x => x.TransformId == preparation.TransformId)
                                                                      .Where(x => x.IsPrepared == false)
                                                                      .Where(x => x.IsActive == true)
                                                                      .ToListAsync();

            var existingPlan = await _context.Transformation_Planning.Where(x => x.Id == preparation.TransformId)
                                                                     .FirstOrDefaultAsync();

            if (getRequestList.Count == 0)
            {
                existingPlan.IsPrepared = true;
            }

            return true;

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllTransformationForMixing()
        {


            var mixing =  _context.Transformation_Planning.Select(x => new TransformationPlanningDto
            {

                Id = x.Id,
                ItemCode = x.ItemCode, 
                ItemDescription = x.ItemDescription, 
                Batch = x.Batch,
                Version = x.Version, 
                Quantity = Math.Round(Convert.ToDecimal(x.Quantity * x.Batch), 2),
                ProdPlan = x.ProdPlan.ToString("MM/dd/yyyy"),
                IsApproved = x.Is_Approved != null, 
                IsPrepared = x.IsPrepared,
                Status = x.Status,
                IsMixed = x.IsMixed != null,
                StatusRemarks = x.StatusRequest

            });

            return await mixing.Where(x => x.Status == true)
                               .Where(x => x.IsApproved == true)
                               .Where(x => x.IsPrepared == true)
                               .Where(x => x.IsMixed != true)
                               .Where(x => x.StatusRemarks == "Approved")
                               .ToListAsync();

        }

        public async Task<PagedList<TransformationPlanningDto>> GetAllTransformationForMixingPagination(UserParams userParams)
        {
            var mixing = (from planning in _context.Transformation_Planning
                          where planning.Status == true && planning.Is_Approved == true && planning.IsPrepared == true &&
                          planning.IsMixed != true && planning.StatusRequest == "Approved"
                          join warehouse in _context.WarehouseReceived
                          on planning.Id equals warehouse.TransformId into leftJ

                          from warehouse in leftJ.DefaultIfEmpty()

                          group warehouse by new
                          {
                              planning.Id,
                              planning.ItemCode,
                              planning.ItemDescription,
                              planning.Uom,
                              planning.Batch,
                              planning.Version,
                              planning.Quantity,
                              planning.ProdPlan,
                              planning.IsPrepared,
                              planning.CancelRemarks,
                              planning.Is_Approved,
                              planning.Status,
                              planning.DateAdded,
                              planning.StatusRequest,
                              planning.IsMixed

                          } into total
                          
                          orderby total.Key.ProdPlan ascending

                          select new TransformationPlanningDto
                          {

                              Id = total.Key.Id,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              Batch = total.Key.Batch,
                              Version = total.Key.Version,
                              Quantity = Math.Round(Convert.ToDecimal(total.Key.Quantity * total.Key.Batch), 2),
                              ProdPlan = total.Key.ProdPlan.ToString("MM/dd/yyyy"),
                              IsApproved = total.Key.Is_Approved != null,
                              IsPrepared = total.Key.IsPrepared,
                              Status = total.Key.Status,
                              DateAdded = total.Key.DateAdded.ToString("MM/dd/yyyy"),
                              StatusRemarks = total.Key.StatusRequest,
                              IsMixed = total.Key.IsMixed != null
      
                          });


            return await PagedList<TransformationPlanningDto>.CreateAsync(mixing, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IReadOnlyList<TransformationMixingRequirements>> GetAllRequirementsForMixing(int id)
        {

            var mixing = (from preparation in _context.Transformation_Preparation
                          where preparation.TransformId == id && preparation.IsMixed == false

                          group preparation by new
                          {

                              preparation.TransformId,
                              preparation.ItemCode,
                              preparation.ItemDescription,
                              preparation.Batch,
                              preparation.QuantityNeeded
                              

                          } into total

                          select new TransformationMixingRequirements
                          {

                              TransformId = total.Key.TransformId,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Batch = total.Key.Batch,
                              QuantityBatch = Math.Round(Convert.ToDecimal(total.Key.QuantityNeeded/total.Key.Batch),2),
                              TotalQuantity = total.Key.QuantityNeeded,
                              WeighingScale = total.Sum(x => x.WeighingScale)

                          });
                        
            return await mixing.ToListAsync();

        }

        public async Task<bool> CompareBatchCount(int id)
        {
            var warehouse = await _context.WarehouseReceived.Where(x => x.TransformId == id)
                                                            .ToListAsync();

            var planning = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                 .FirstOrDefaultAsync();

            var preparation = await _context.Transformation_Preparation.Where(x => x.TransformId == id)
                                                                       .ToListAsync();

            if(warehouse.Count == planning.Batch)
            {
                planning.IsMixed = true;

                foreach(var items in preparation)
                {
                    items.IsMixed = true;
                }

            }

            return false;

        }

        public async Task<MixingValue> CountBatch(int id)
        {

            var batch = await _context.WarehouseReceived.Where(x => x.TransformId == id)
                                                        .ToListAsync();

            var planning = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                 .FirstOrDefaultAsync();

            var mixing = await _context.WarehouseReceived.Where(x => x.TransformId == id)
                                                         .Where(x => x.IsActive == true)
                                                         .SumAsync(x => x.ActualGood);                                                          
            var temp = planning.Batch - batch.Count;  

            var remainingResult = new MixingValue
            {
                 RemainingBatch = temp,
                 TotalWeighingScale = mixing
            };

            return remainingResult; 

        }

    }
}
