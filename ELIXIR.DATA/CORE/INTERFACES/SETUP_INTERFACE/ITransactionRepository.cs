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
    public interface ITransactionRepository
    {


        Task<IReadOnlyList<TransactionDto>> GetAllTransactionName();
        Task<bool> AddNewTransactionName(Transaction transact);
        Task<PagedList<TransactionDto>> GetAllTransactionPagination(bool status, UserParams userParams);
        Task<PagedList<TransactionDto>> GetAllTransactionPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> InActiveTransactionName(Transaction transact);
        Task<bool> ActivateTransactionName(Transaction transact);
        Task<bool> TransactionNameExist(string transact);


        Task<bool> UpdateTransactionName(Transaction transact);



    }
}
