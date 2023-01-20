using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class SupplierRepository : GenericRepository<SupplierDto>, ISupplierRepository
    {
        private new readonly StoreContext _context;
        public SupplierRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<IReadOnlyList<SupplierDto>> GetAll()
        {
            return await  _context.Suppliers
                                            .Select(supplier => new SupplierDto
                                            {
                                                Id = supplier.Id,
                                                SupplierName = supplier.SupplierName,
                                                SupplierAddress = supplier.SupplierAddress,
                                                DateAdded = (supplier.DateAdded).ToString("MM/dd/yyyy"),
                                                AddedBy = supplier.AddedBy,
                                                IsActive = supplier.IsActive,
                                                Reason = supplier.Reason
                                            }).ToListAsync();

        }
        public override async Task<SupplierDto> GetById(int id)
        {
              return await _context.Suppliers
                                             .Select(supplier => new SupplierDto
                                             {
                                                 Id = supplier.Id,
                                                 SupplierName = supplier.SupplierName,
                                                 SupplierAddress = supplier.SupplierAddress,
                                                 DateAdded = (supplier.DateAdded).ToString("MM/dd/yyyy"),
                                                 AddedBy = supplier.AddedBy,
                                                 IsActive = supplier.IsActive,
                                                 Reason = supplier.Reason
                                             }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddnewSupplier(Supplier supplier)
        {
            supplier.DateAdded = DateTime.Now;
            supplier.IsActive = true;

            if(supplier.AddedBy == null)
            supplier.AddedBy = "Admin";

            await _context.Suppliers.AddAsync(supplier);
            return true;
        }

        public async Task<bool> UpdateSupplierInfo(Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.Where(x => x.Id == supplier.Id)
                                                           .FirstOrDefaultAsync();

            if (existingSupplier == null)
                return await AddnewSupplier(supplier);

            existingSupplier.SupplierCode = supplier.SupplierCode;
            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierAddress = supplier.SupplierAddress;

            return true;
        }

        public async Task<IReadOnlyList<SupplierDto>> GetAllActiveSupplier()
        {
            return await _context.Suppliers
                            .Select(supplier => new SupplierDto
                            {
                                Id = supplier.Id,
                                SupplierName = supplier.SupplierName,
                                SupplierCode = supplier.SupplierCode,
                                SupplierAddress = supplier.SupplierAddress,
                                DateAdded = (supplier.DateAdded).ToString("MM/dd/yyyy"),
                                AddedBy = supplier.AddedBy,
                                IsActive = supplier.IsActive,
                                Reason = supplier.Reason,
                            }).Where(x => x.IsActive == true)
                              .ToListAsync();
        }

        public async Task<IReadOnlyList<SupplierDto>> GetAllInActiveSupplier()
        {
            return await _context.Suppliers
                           .Select(supplier => new SupplierDto
                           {
                               Id = supplier.Id,
                               SupplierName = supplier.SupplierName,
                               SupplierAddress = supplier.SupplierAddress,
                               DateAdded = (supplier.DateAdded).ToString("MM/dd/yyyy"),
                               AddedBy = supplier.AddedBy,
                               IsActive = supplier.IsActive,
                               Reason = supplier.Reason
                           }).Where(x => x.IsActive == false)
                             .ToListAsync();
        }

        public async Task<bool> InActiveSupplier(Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.Where(x => x.Id == supplier.Id)
                                                           .FirstOrDefaultAsync();

            existingSupplier.SupplierName = existingSupplier.SupplierName;
            existingSupplier.SupplierAddress = existingSupplier.SupplierAddress;
            existingSupplier.Reason = supplier.Reason;
            existingSupplier.IsActive = false;

            if (supplier.Reason == null)
                existingSupplier.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateSupplier(Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.Where(x => x.Id == supplier.Id)
                                                           .FirstOrDefaultAsync();

            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierAddress = supplier.SupplierAddress;
            existingSupplier.Reason = supplier.Reason;
            existingSupplier.IsActive = true;

            if (supplier.Reason == null)
                existingSupplier.Reason = "Reopened Supplier";

            return true;
        }

        public async Task<bool> SupplierCodeExist(string supplier)
        {
            return await _context.Suppliers.AnyAsync(x => x.SupplierCode == supplier);
        }

        public async Task<PagedList<SupplierDto>> GetAllSupplierWithPagination(bool status, UserParams userParams)
        {
            var suppliers = _context.Suppliers.OrderByDescending(x => x.DateAdded)
                                         .Select(supplier => new SupplierDto
                                         {
                                             Id = supplier.Id,
                                             SupplierCode = supplier.SupplierCode,
                                             SupplierName = supplier.SupplierName,
                                             SupplierAddress = supplier.SupplierAddress,
                                             DateAdded = (supplier.DateAdded).ToString("MM/dd/yyyy"),
                                             AddedBy = supplier.AddedBy,
                                             IsActive = supplier.IsActive,
                                             Reason = supplier.Reason

                                         }).Where(x => x.IsActive == status);
                                           

            return await PagedList<SupplierDto>.CreateAsync(suppliers, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<SupplierDto>> GetSupplierByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var suppliers = _context.Suppliers.OrderByDescending(x => x.DateAdded)
                                       .Select(supplier => new SupplierDto
                                       {
                                           Id = supplier.Id,
                                           SupplierCode = supplier.SupplierCode,
                                           SupplierName = supplier.SupplierName,
                                           SupplierAddress = supplier.SupplierAddress,
                                           DateAdded = (supplier.DateAdded).ToString("MM/dd/yyyy"),
                                           AddedBy = supplier.AddedBy,
                                           IsActive = supplier.IsActive,
                                           Reason = supplier.Reason

                                       }).Where(x => x.IsActive == status)
                                         .Where(x => x.SupplierName.ToLower()
                                         .Contains(search.Trim().ToLower()));

            return await PagedList<SupplierDto>.CreateAsync(suppliers, userParams.PageNumber, userParams.PageSize);
        }
    }
}
