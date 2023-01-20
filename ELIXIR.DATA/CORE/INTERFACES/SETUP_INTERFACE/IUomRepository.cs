using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IUomRepository : IGenericRepository<UomDto>
    {
        Task<bool> AddNewUom(UOM uom);
        Task<bool> UpdateUom(UOM uom);
        Task<bool> UomCodeExist(string uom);
        Task<bool> UomDescription(string uom);
        Task<IReadOnlyList<UomDto>> GetAllActiveUOM();
        Task<IReadOnlyList<UomDto>> GetAllInActiveUOM();
        Task<bool> InActiveUom(UOM uom);
        Task<bool> ActivateUom(UOM uom);



        Task<PagedList<UomDto>> GetAllUomWithPagination(bool status, UserParams userParams);
        Task<PagedList<UomDto>> GetUomByStatusWithPaginationOrig(UserParams userParams, bool status, string search);


    }
}
