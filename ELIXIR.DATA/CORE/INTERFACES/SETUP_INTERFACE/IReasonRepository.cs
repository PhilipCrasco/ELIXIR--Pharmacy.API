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
    public interface IReasonRepository : IGenericRepository<ReasonDto>
    {
        Task<bool> AddnewReason(Reason reason);
        Task<bool> UpdateReason(Reason reason);
        Task<IReadOnlyList<ReasonDto>> GetAllActiveReason();
        Task<IReadOnlyList<ReasonDto>> GetAllInActiveReason();
        Task<bool> InActiveReason(Reason reason);
        Task<bool> ActivateReason(Reason reason);
        Task<bool> ReasonNameExist(string reason);
        Task<bool> ValidateModuleId(int id);

        Task<PagedList<ReasonDto>> GetAllReasonWithPagination(bool status, UserParams userParams);
        Task<PagedList<ReasonDto>> GetReasonByStatusWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<bool> ValidateReasonEntry(Reason reason);
    }
}
