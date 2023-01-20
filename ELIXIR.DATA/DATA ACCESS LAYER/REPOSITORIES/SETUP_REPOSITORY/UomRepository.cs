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
    public class UomRepository : GenericRepository<UomDto>, IUomRepository
    {
        private new readonly StoreContext _context;
        public UomRepository(StoreContext context) : base (context)
        {
            _context = context;
        }

        public override async Task<IReadOnlyList<UomDto>> GetAll()
        {
            return await _context.UOMS
                                .Select(uom => new UomDto
                                {
                                    Id = uom.Id,
                                    UOM_Code = uom.UOM_Code,
                                    UOM_Description = uom.UOM_Description,
                                    DateAdded = (uom.DateAdded).ToString("MM/dd/yyyy"),
                                    AddedBy = uom.AddedBy,
                                    IsActive = uom.IsActive,
                                    Reason = uom.Reason
                                }).ToListAsync();             
        }

        public override async Task<UomDto> GetById(int id)
        {
            return await _context.UOMS
                                       .Select(uom => new UomDto
                                       {
                                           Id = uom.Id,
                                           UOM_Code = uom.UOM_Code,
                                           UOM_Description = uom.UOM_Description,
                                           DateAdded = (uom.DateAdded).ToString("MM/dd/yyyy"),
                                           AddedBy = uom.AddedBy,
                                           IsActive = uom.IsActive,
                                           Reason = uom.Reason
                                       }).FirstOrDefaultAsync(x => x.Id == id);
                                       
        }

        public async Task<bool> AddNewUom(UOM uom)
        {

            uom.DateAdded = DateTime.Now;
            uom.IsActive = true;

            if(uom.AddedBy == null)
               uom.AddedBy = "Admin";

            await _context.UOMS.AddAsync(uom);
            return true;
        }
        public async Task<bool> UomCodeExist(string uom)
        {
            return await _context.UOMS.AnyAsync(x => x.UOM_Code == uom);
        }

        public async Task<bool> UomDescription(string uom)
        {
            return await _context.UOMS.AnyAsync(x => x.UOM_Description == uom);
        }

        public async Task<bool> UpdateUom(UOM uom)
        {
            var existingUom = await _context.UOMS.Where(x => x.Id == uom.Id)
                                                 .FirstOrDefaultAsync();

            if (existingUom == null)
                return await AddNewUom(uom);

            existingUom.UOM_Code = uom.UOM_Code;
            existingUom.UOM_Description = uom.UOM_Description;

            return true;

        }

        public async Task<IReadOnlyList<UomDto>> GetAllActiveUOM()
        {
            return await _context.UOMS
                            .Select(uom => new UomDto
                            {
                                Id = uom.Id,
                                UOM_Code = uom.UOM_Code,
                                UOM_Description = uom.UOM_Description,
                                DateAdded = (uom.DateAdded).ToString("MM/dd/yyyy"),
                                AddedBy = uom.AddedBy,
                                IsActive = uom.IsActive,
                                Reason = uom.Reason
                            }).Where(x => x.IsActive == true)
                              .ToListAsync();
        }

        public async Task<IReadOnlyList<UomDto>> GetAllInActiveUOM()
        {
            return await _context.UOMS
                           .Select(uom => new UomDto
                           {
                               Id = uom.Id,
                               UOM_Code = uom.UOM_Code,
                               UOM_Description = uom.UOM_Description,
                               DateAdded = (uom.DateAdded).ToString("MM/dd/yyyy"),
                               AddedBy = uom.AddedBy,
                               IsActive = uom.IsActive,
                               Reason = uom.Reason
                           }).Where(x => x.IsActive == false)
                             .ToListAsync();
        }

        public async Task<bool> InActiveUom(UOM uom)
        {
            var existingUom = await _context.UOMS.Where(x => x.Id == uom.Id)
                                                 .FirstOrDefaultAsync();

            existingUom.UOM_Code = existingUom.UOM_Code;
            existingUom.UOM_Description = existingUom.UOM_Description;
            existingUom.Reason = uom.Reason;
            existingUom.IsActive = false;
         

            if (uom.Reason == null)
                existingUom.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateUom(UOM uom)
        {
            var existingUom = await _context.UOMS.Where(x => x.Id == uom.Id)
                                                 .FirstOrDefaultAsync();

            existingUom.UOM_Code = existingUom.UOM_Code;
            existingUom.UOM_Description = existingUom.UOM_Description;
            existingUom.Reason = uom.Reason;
            existingUom.IsActive = true;

            if (uom.Reason == null)
                existingUom.Reason = "Reopened UOM";

            return true;
        }

        public async Task<PagedList<UomDto>> GetAllUomWithPagination(bool status, UserParams userParams)
        {
            var uom = _context.UOMS.OrderByDescending(x => x.DateAdded)
                                  .Select(uom => new UomDto
                                  {
                                      Id = uom.Id,
                                      UOM_Code = uom.UOM_Code,
                                      UOM_Description = uom.UOM_Description,
                                      DateAdded = (uom.DateAdded).ToString("MM/dd/yyyy"),
                                      AddedBy = uom.AddedBy,
                                      IsActive = uom.IsActive,
                                      Reason = uom.Reason
                                  }).Where(x => x.IsActive == status);

            return await PagedList<UomDto>.CreateAsync(uom, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<UomDto>> GetUomByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var uom = _context.UOMS.OrderByDescending(x => x.DateAdded)
                                  .Select(uom => new UomDto
                                  {
                                      Id = uom.Id,
                                      UOM_Code = uom.UOM_Code,
                                      UOM_Description = uom.UOM_Description,
                                      DateAdded = (uom.DateAdded).ToString("MM/dd/yyyy"),
                                      AddedBy = uom.AddedBy,
                                      IsActive = uom.IsActive,
                                      Reason = uom.Reason
                                  }).Where(x => x.IsActive == status)
                                    .Where(x => x.UOM_Description.ToLower()
                                    .Contains(search.Trim().ToLower()));

            return await PagedList<UomDto>.CreateAsync(uom, userParams.PageNumber, userParams.PageSize);
        }
    }
}
