using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class TransformationRepository : GenericRepository<TransformationFormulaDto>, ITransformationRepository
    {
        private new readonly StoreContext _context;
        public TransformationRepository(StoreContext context) : base(context)
        {
            _context = context;
        }

        //------TRANSFORMATION FORMULA--------

        public override async Task<IReadOnlyList<TransformationFormulaDto>> GetAll()
        {
            return await _context.Formulas
                                            .Select(formula => new TransformationFormulaDto
                                            {
                                                Id = formula.Id,
                                                ItemCode = formula.ItemCode,
                                                ItemDescription = formula.ItemDescription,
                                                Version = formula.Version,
                                                Quantity = formula.Quantity,
                                                DateAdded = (formula.DateAdded).ToString("MM/dd/yyyy"),
                                                AddedBy = formula.AddedBy,
                                                IsActive = formula.IsActive,
                                                Reason = formula.Reason
                                            }).ToListAsync();
        }

        public override async Task<TransformationFormulaDto> GetById(int id)
        {
            return await _context.Formulas
                                           .Select(formula => new TransformationFormulaDto
                                           {
                                               Id = formula.Id,
                                               ItemCode = formula.ItemCode,
                                               ItemDescription = formula.ItemDescription,
                                               Version = formula.Version,
                                               Quantity = formula.Quantity,
                                               DateAdded = (formula.DateAdded).ToString("MM/dd/yyyy"),
                                               AddedBy = formula.AddedBy,
                                               IsActive = formula.IsActive,
                                               Reason = formula.Reason
                                           }).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<IReadOnlyList<TransformationFormulaDto>> GetAllActiveFormula()
        {
            return await _context.Formulas
                                           .Select(formula => new TransformationFormulaDto
                                           {
                                               Id = formula.Id,
                                               ItemCode = formula.ItemCode,
                                               ItemDescription = formula.ItemDescription,
                                               Version = formula.Version,
                                               Quantity = formula.Quantity,
                                               DateAdded = (formula.DateAdded).ToString("MM/dd/yyyy"),
                                               AddedBy = formula.AddedBy,
                                               IsActive = formula.IsActive,
                                               Reason = formula.Reason
                                           }).Where(x => x.IsActive == true)
                                             .ToListAsync();
        }

        public async Task<IReadOnlyList<TransformationFormulaDto>> GetAllInActiveFormula()
        {
            return await _context.Formulas
                                          .Select(formula => new TransformationFormulaDto
                                          {
                                              Id = formula.Id,
                                              ItemCode = formula.ItemCode,
                                              ItemDescription = formula.ItemDescription,
                                              Version = formula.Version,
                                              Quantity = formula.Quantity,
                                              DateAdded = (formula.DateAdded).ToString("MM/dd/yyyy"),
                                              AddedBy = formula.AddedBy,
                                              IsActive = formula.IsActive,
                                              Reason = formula.Reason
                                          }).Where(x => x.IsActive == false)
                                            .ToListAsync();
        }

        public async Task<bool> AddNewTransformationFormula(TransformationFormula formula)
        {
            var countFormula = await _context.Formulas.Where(x => x.ItemCode == formula.ItemCode)
                                                      .ToListAsync();

            if (countFormula.Count == 0)
                formula.Version = 1;

            if (countFormula != null)
                formula.Version = countFormula.Count + 1;

            if (formula.AddedBy == null)
                formula.AddedBy = "Admin";

            formula.DateAdded = DateTime.Now;
            formula.IsActive = true;

            await _context.Formulas.AddAsync(formula);

            return true;
        }

        public async Task<bool> InactiveTransformationFormula(TransformationFormula formula)
        {
            var existingFormula = await _context.Formulas.Where(x => x.Id == formula.Id)
                                                         .FirstOrDefaultAsync();

            existingFormula.ItemCode = existingFormula.ItemCode;
            existingFormula.ItemDescription = existingFormula.ItemDescription;
            existingFormula.Version = existingFormula.Version;
            existingFormula.Reason = formula.Reason;
            existingFormula.IsActive = false;

            if (formula.Reason == null)
                existingFormula.Reason = "Change Data";


            return true;
        }

        public async Task<bool> ActivateTransformationFormula(TransformationFormula formula)
        {
            var existingFormula = await _context.Formulas.Where(x => x.Id == formula.Id)
                                                         .FirstOrDefaultAsync();

            existingFormula.ItemCode = existingFormula.ItemCode;
            existingFormula.ItemDescription = existingFormula.ItemDescription;
            existingFormula.Version = existingFormula.Version;
            existingFormula.Reason = formula.Reason;
            existingFormula.IsActive = true;

            if (formula.Reason == null)
                existingFormula.Reason = "Reopened Formula";


            return true;
        }


        //----TRANSFORMATION REQUIREMENT---------

        public async Task<bool> AddNewRequirementsInFormula(TransformationRequirement requirement)
        {

            var getitemDescription = await _context.RawMaterials.FindAsync(requirement.RawMaterialId);



            requirement.ItemDescription = getitemDescription.ItemDescription;

            if (requirement.AddedBy == null)
                requirement.AddedBy = "Admin";

            requirement.IsActive = true;

            await _context.FormulaRequirements.AddAsync(requirement);

            return true;

        }
        public async Task<IReadOnlyList<TransformationRequirementDto>> GetAllRequirementwithFormula()
        {
            var transform = (from requirement in _context.FormulaRequirements
                             join formula in _context.Formulas on requirement.TransformationFormulaId equals formula.Id
                             join rawmaterial in _context.RawMaterials on requirement.RawMaterialId equals rawmaterial.Id

                             select new TransformationRequirementDto
                             {
                                 FormulaCode = formula.ItemCode,
                                 FormulaDescription = formula.ItemDescription,
                                 FormulaVersion = formula.Version,
                                 FormulaQuantity = formula.Quantity,
                                 RequirementCode = rawmaterial.ItemCode,
                                 RequirementDescription = rawmaterial.ItemDescription,
                                 RequirementQuantity = requirement.Quantity,
                                 AddedBy = requirement.AddedBy
                             });

            return await transform.ToListAsync();
        }

        public async Task<IReadOnlyList<TransformationRequirementDto>> GetAllRequirementsWithFormulaByItemCode(string itemcode)
        {
            var transform = (from requirement in _context.FormulaRequirements
                             join formula in _context.Formulas on requirement.TransformationFormulaId equals formula.Id
                             join rawmaterial in _context.RawMaterials on requirement.RawMaterialId equals rawmaterial.Id

                             select new TransformationRequirementDto
                             {
                                 FormulaCode = formula.ItemCode,
                                 FormulaDescription = formula.ItemDescription,
                                 FormulaVersion = formula.Version,
                                 FormulaQuantity = formula.Quantity,
                                 RequirementCode = rawmaterial.ItemCode,
                                 RequirementDescription = rawmaterial.ItemDescription,
                                 RequirementQuantity = requirement.Quantity,
                                 AddedBy = requirement.AddedBy
                             });

            return await transform.Where(x => x.FormulaCode == itemcode)
                                  .ToListAsync();
        }

        public async Task<bool> DeleteRequirementInFormula(int id)
        {
            var existingRawmaterial = await _context.FormulaRequirements.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRawmaterial == null)
                return false;

            _context.FormulaRequirements.Remove(existingRawmaterial);

            return true;

        }

        public async Task<bool> ValidateEntryInRequirement(TransformationRequirement requirement)
        {
            var validate = await _context.FormulaRequirements.Where(x => x.TransformationFormulaId == requirement.TransformationFormulaId)
                                                             .Where(x => x.RawMaterialId == requirement.RawMaterialId)
                                                             .Where(x => x.ItemDescription == requirement.ItemDescription)
                                                             .ToListAsync();
            if (validate.Count == 0)
                return false;

            return true;
        }

        public async Task<bool> ValidateRawMaterial(int id)
        {
            var validateExisting = await _context.RawMaterials.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateTagRequirements(TransformationRequirement requirement)
        {
            var existingformula = await _context.FormulaRequirements.Where(x => x.TransformationFormulaId == requirement.TransformationFormulaId)
                                                                    .Where(x => x.RawMaterialId == requirement.RawMaterialId)
                                                                    .FirstOrDefaultAsync();
            if (existingformula == null)
                return true;


            return false;
        }

        public async Task<IReadOnlyList<TransformationRequirementDto>> GetAllFormulaWithRequirementsByFormulaId(int id)
        {
            var transform = (from requirement in _context.FormulaRequirements
                             join formula in _context.Formulas on requirement.TransformationFormulaId equals formula.Id
                             join rawmaterial in _context.RawMaterials on requirement.RawMaterialId equals rawmaterial.Id

                             select new TransformationRequirementDto
                             {
                                 Id = requirement.TransformationFormulaId,
                                 FormulaCode = formula.ItemCode,
                                 FormulaDescription = formula.ItemDescription,
                                 FormulaVersion = formula.Version,
                                 FormulaQuantity = formula.Quantity,
                                 RequirementId = requirement.Id,
                                 RequirementCode = rawmaterial.ItemCode,
                                 RequirementDescription = rawmaterial.ItemDescription,
                                 RequirementQuantity = requirement.Quantity,
                                 AddedBy = requirement.AddedBy,
                                 IsActive = requirement.IsActive,
                                 uom = formula.Uom
                             });

            return await transform.Where(x => x.Id == id)
                                  .Where(x => x.IsActive == true)
                                  .ToListAsync();
        }

        public async Task<bool> ValidateFormulaIfActive(int id)
        {
            var validate = await _context.Formulas.Where(x => x.Id == id)
                                                  .Where(x => x.IsActive == true)
                                                  .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<PagedList<TransformationFormulaDto>> GetAllFormulaWithPagination(bool status, UserParams userParams)
        {

            var formula = (from formulacode in _context.Formulas
                           where formulacode.IsActive == status
                           join requirements in _context.FormulaRequirements
                           on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                           from requirements in leftJ.DefaultIfEmpty()
                           where requirements.IsActive == true || requirements == null

                           group requirements by new
                           {
                               formulacode.Id,
                               formulacode.ItemCode,
                               formulacode.ItemDescription,
                               formulacode.Version,
                               formulacode.Quantity,
                               formulacode.DateAdded,
                               formulacode.AddedBy,
                               formulacode.IsActive,
                               formulacode.Reason,
                               formulacode.Uom,
                               Testing = requirements != null ? true : false

                           } into total
                           orderby total.Key.DateAdded descending

                           select new TransformationFormulaDto
                           {
                               Id = total.Key.Id,
                               ItemCode = total.Key.ItemCode,
                               ItemDescription = total.Key.ItemDescription,
                               Version = total.Key.Version,
                               Quantity = total.Key.Quantity,
                               DateAdded = total.Key.DateAdded.ToString("MM/dd/yyyy"),
                               AddedBy = total.Key.AddedBy,
                               IsActive = total.Key.IsActive,
                               Reason = total.Key.Reason,
                               CountFormula = total.Key.Testing,
                               Uom = total.Key.Uom,
                               CountQuantity = total.Sum(x => x.Quantity)
                           });



            return await PagedList<TransformationFormulaDto>.CreateAsync(formula, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<TransformationFormulaDto>> GetFormulaByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {

            var formula = (from formulacode in _context.Formulas
                           where formulacode.IsActive == status && formulacode.ItemCode.ToLower().Contains(search.Trim().ToLower())
                           join requirements in _context.FormulaRequirements
                           on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                           from requirements in leftJ.DefaultIfEmpty()
                           where requirements.IsActive == true || requirements == null


                           group requirements by new
                           {
                               formulacode.Id,
                               formulacode.ItemCode,
                               formulacode.ItemDescription,
                               formulacode.Version,
                               formulacode.Quantity,
                               formulacode.DateAdded,
                               formulacode.AddedBy,
                               formulacode.IsActive,
                               formulacode.Reason,
                               formulacode.Uom,
                               Testing = requirements != null ? true : false

                           } into total

                           orderby total.Key.DateAdded descending
                           select new TransformationFormulaDto
                           {
                               Id = total.Key.Id,
                               ItemCode = total.Key.ItemCode,
                               ItemDescription = total.Key.ItemDescription,
                               Version = total.Key.Version,
                               Quantity = total.Key.Quantity,
                               DateAdded = total.Key.DateAdded.ToString("MM/dd/yyyy"),
                               AddedBy = total.Key.AddedBy,
                               IsActive = total.Key.IsActive,
                               Reason = total.Key.Reason,
                               CountFormula = total.Key.Testing,
                               Uom = total.Key.Uom,
                               CountQuantity = total.Sum(x => x.Quantity)
                           });

            return await PagedList<TransformationFormulaDto>.CreateAsync(formula, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UpdateFormulaInfo(TransformationFormula formula)
        {
            var existingFormula = await _context.Formulas.Where(x => x.Id == formula.Id)
                                                        .FirstOrDefaultAsync();

            if (existingFormula == null)
                return false;

            existingFormula.ItemCode = formula.ItemCode;
            existingFormula.ItemDescription = formula.ItemDescription;
            existingFormula.Quantity = formula.Quantity;

            return true;
        }

        public async Task<bool> UpdateFormulaCode(TransformationFormula formula)
        {
            var existingInfo = await _context.Formulas.Where(x => x.Id == formula.Id)
                                                      .FirstOrDefaultAsync();

            var validateItemcode = await _context.Formulas.Where(x => x.ItemCode == formula.ItemCode)
                                                          .ToListAsync();


            if (validateItemcode.Count == 0 || validateItemcode.Count != 0)
            {
                existingInfo.ItemCode = formula.ItemCode;
                existingInfo.ItemDescription = formula.ItemDescription;
                existingInfo.Quantity = formula.Quantity;
                existingInfo.Uom = formula.Uom;
                existingInfo.Version = validateItemcode.Count + 1;
            }


            existingInfo.ItemCode = formula.ItemCode;
            existingInfo.ItemDescription = formula.ItemDescription;
            existingInfo.Quantity = formula.Quantity;
            existingInfo.Uom = formula.Uom;

            return true;

        }

        public async Task<bool> InActiveRequirement(TransformationRequirement requirement)
        {
            var existingInfo = await _context.FormulaRequirements.Where(x => x.Id == requirement.Id)
                                                                 .FirstOrDefaultAsync();

            existingInfo.IsActive = false;

            return true;

        }

        public async Task<IReadOnlyList<TransformationRequirementDto>> GetAllInActiveRequirements(int id)
        {
            var validateInactive = _context.FormulaRequirements.Select(requirements => new TransformationRequirementDto
            {
                Id = requirements.TransformationFormulaId,
                RequirementCode = requirements.RawMaterial.ItemCode,
                RequirementDescription = requirements.ItemDescription,
                RequirementQuantity = requirements.Quantity,
                RequirementId = requirements.Id,
                IsActive = requirements.IsActive,

            }).Where(x => x.Id == id)
              .Where(x => x.IsActive == false);


            return await validateInactive.ToListAsync();

        }

        public async Task<bool> ActivateRequirement(TransformationRequirement requirement)
        {
            var existingInfo = await _context.FormulaRequirements.Where(x => x.Id == requirement.Id)
                                                                 .FirstOrDefaultAsync();

            existingInfo.IsActive = true;

            return true;
        }

        public async Task<bool> UpdateQuantity(TransformationRequirement requirement)
        {
            var existingInfo = await _context.FormulaRequirements.Where(x => x.Id == requirement.Id)
                                                                 .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;


            existingInfo.Quantity = requirement.Quantity;

            return true;

        }


    }
}
