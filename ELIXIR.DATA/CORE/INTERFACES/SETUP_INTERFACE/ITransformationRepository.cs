using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ITransformationRepository   : IGenericRepository<TransformationFormulaDto>
    {
        //-----FORMULA-------
        Task<bool> AddNewTransformationFormula(TransformationFormula formula);
        Task<bool> InactiveTransformationFormula(TransformationFormula formula);
        Task<bool> ActivateTransformationFormula(TransformationFormula formula);
        Task<IReadOnlyList<TransformationFormulaDto>> GetAllActiveFormula();
        Task<IReadOnlyList<TransformationFormulaDto>> GetAllInActiveFormula();
        Task<bool> ValidateFormulaIfActive(int id);

        Task<bool> UpdateFormulaInfo(TransformationFormula formula);


        //-----FORMULA REQUIREMENTS-------


        Task<bool> AddNewRequirementsInFormula(TransformationRequirement requirement);
        Task<IReadOnlyList<TransformationRequirementDto>> GetAllRequirementwithFormula();
        Task<IReadOnlyList<TransformationRequirementDto>> GetAllRequirementsWithFormulaByItemCode(string itemcode);
        Task<bool> DeleteRequirementInFormula(int id);
        Task<bool> ValidateEntryInRequirement(TransformationRequirement requirement);
        Task<bool> ValidateRawMaterial(int id);
        Task<bool> ValidateTagRequirements(TransformationRequirement requirement);
        Task<IReadOnlyList<TransformationRequirementDto>> GetAllFormulaWithRequirementsByFormulaId(int id);


        Task<PagedList<TransformationFormulaDto>> GetAllFormulaWithPagination(bool status, UserParams userParams);
        Task<PagedList<TransformationFormulaDto>> GetFormulaByStatusWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<bool> UpdateFormulaCode(TransformationFormula formula);
        Task<bool> InActiveRequirement(TransformationRequirement requirement);
        Task<bool> ActivateRequirement(TransformationRequirement requirement);


        Task<IReadOnlyList<TransformationRequirementDto>> GetAllInActiveRequirements(int id);
        Task<bool> UpdateQuantity(TransformationRequirement requirement);



    }
}
