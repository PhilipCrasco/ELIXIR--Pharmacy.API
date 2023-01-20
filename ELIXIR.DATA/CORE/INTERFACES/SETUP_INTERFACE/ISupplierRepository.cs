using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ISupplierRepository : IGenericRepository<SupplierDto>
    {
        Task<bool> AddnewSupplier(Supplier supplier);
        Task<bool> UpdateSupplierInfo(Supplier supplier);
        Task<IReadOnlyList<SupplierDto>> GetAllActiveSupplier();
        Task<IReadOnlyList<SupplierDto>> GetAllInActiveSupplier();
        Task<bool> InActiveSupplier(Supplier supplier);
        Task<bool> ActivateSupplier(Supplier supplier);
        Task<bool> SupplierCodeExist(string supplier);


        Task<PagedList<SupplierDto>> GetAllSupplierWithPagination(bool status, UserParams userParams);
        Task<PagedList<SupplierDto>> GetSupplierByStatusWithPaginationOrig(UserParams userParams, bool status, string search);
    }
}
