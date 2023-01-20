using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly StoreContext _context;

        public TransactionRepository(StoreContext context) 
        {
            _context = context;

        }
        public async Task<IReadOnlyList<TransactionDto>> GetAllTransactionName()
        {

            var transact = _context.Transactions.Select(x => new TransactionDto
            {

                Id = x.Id,
                TransactionName = x.TransactionName,
                AddedBy = x.AddedBy,
                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                IsActive = x.IsActive

            });

                return await transact.Where(x => x.IsActive == true)
                                     .ToListAsync();

        }

        public async Task<bool> AddNewTransactionName(Transaction transact)
        {

            transact.DateAdded = DateTime.Now;
            transact.IsActive = true;

            await _context.Transactions.AddAsync(transact);

            return true;
        }

        public async Task<PagedList<TransactionDto>> GetAllTransactionPagination(bool status, UserParams userParams)
        {
            var transact = _context.Transactions.Select(x => new TransactionDto
            {

                Id = x.Id,
                TransactionName = x.TransactionName,
                AddedBy = x.AddedBy,
                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                IsActive = x.IsActive

            }).Where(x => x.IsActive == status);


            return await PagedList<TransactionDto>.CreateAsync(transact, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<TransactionDto>> GetAllTransactionPaginationOrig(UserParams userParams, bool status, string search)
        {
            var transact = _context.Transactions.Select(x => new TransactionDto
            {

                Id = x.Id,
                TransactionName = x.TransactionName,
                AddedBy = x.AddedBy,
                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                IsActive = x.IsActive

            }).Where(x => x.IsActive == status)
              .Where(x => x.TransactionName.ToLower()
              .Contains(search.Trim().ToLower()));


            return await PagedList<TransactionDto>.CreateAsync(transact, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> InActiveTransactionName(Transaction transact)
        {
            var existing = await _context.Transactions.Where(x => x.Id == transact.Id)
                                                .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = false;

            return true;

        }

        public async Task<bool> ActivateTransactionName(Transaction transact)
        {
            var existing = await _context.Transactions.Where(x => x.Id == transact.Id)
                                              .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = true;

            return true;

        }

        public async Task<bool> TransactionNameExist(string transact)
        {
            return await _context.Transactions.AnyAsync(x => x.TransactionName == transact);
        }

        public async Task<bool> UpdateTransactionName(Transaction transact)
        {
            var existing = await _context.Transactions.Where(x => x.Id == transact.Id)
                                                .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.TransactionName = transact.TransactionName;

            return true;

        }
    }
}
