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
    public class LotNameAndCategoriesRepository : GenericRepository<LotNameDto>, ILotNameAndCategoriesRepository
    {
        private new readonly StoreContext _context;
        public LotNameAndCategoriesRepository(StoreContext context) : base (context)
        {
            _context = context;
        }

        //-----LOT NAME--------
        public override async Task<IReadOnlyList<LotNameDto>> GetAll()
        {
            return await _context.LotNames.OrderByDescending(x => x.DateAdded)
                                           .Join(_context.LotCategories,
                                            lotname => lotname.LotCategoryId,
                                            categories => categories.Id,
                                               (lotname, categories) => new LotNameDto
                                               {
                                                   Id = lotname.Id,
                                                   LotCategory = categories.LotCategoryName,
                                                   LotCategoryId = categories.Id,
                                                   SectionName = lotname.SectionName,
                                                   DateAdded = lotname.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = lotname.AddedBy,
                                                   IsActive = lotname.IsActive,
                                                   Reason = lotname.Reason
                                               }).ToListAsync();
        }
        public override async Task<LotNameDto> GetById(int id)
        {
            return await _context.LotNames.OrderByDescending(x => x.DateAdded)
                                          .Join(_context.LotCategories,
                                             lotname => lotname.LotCategoryId,
                                             categories => categories.Id,
                                              (lotname, categories) => new LotNameDto
                                              {
                                                  Id = lotname.Id,
                                                  LotCategory = categories.LotCategoryName,
                                                  LotCategoryId = categories.Id,
                                                  SectionName = lotname.SectionName,
                                                  DateAdded = lotname.DateAdded.ToString("MM/dd/yyyy"),
                                                  AddedBy = lotname.AddedBy,
                                                  IsActive = lotname.IsActive,
                                                  Reason = lotname.Reason
                                              }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddNewLot(LotName lotname)
        {
            lotname.DateAdded = DateTime.Now;
            lotname.IsActive = true;

            if(lotname.AddedBy == null)
               lotname.AddedBy = "Admin";

            await _context.LotNames.AddAsync(lotname);
            return true;
        }

        public async Task<bool> UpdateLotName(LotName lotname)
        {
            var existinglotname = await _context.LotNames.Where(x => x.Id == lotname.Id)
                                                         .FirstOrDefaultAsync();

            existinglotname.LotCategoryId = lotname.LotCategoryId;
            existinglotname.SectionName = lotname.SectionName;
            existinglotname.LotNameCode = lotname.LotNameCode;
       
            return true;
        }

        public async Task<bool> InActiveLotName(LotName lotname)
        {
            var existinglot = await _context.LotNames.Where(x => x.Id == lotname.Id)
                                                     .FirstOrDefaultAsync();

            existinglot.LotCategoryId = existinglot.LotCategoryId;
            existinglot.SectionName = existinglot.SectionName;
            existinglot.Reason = lotname.Reason;
            existinglot.IsActive = false;

            if (lotname.Reason == null)
                existinglot.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateLotName(LotName lotname)
        {
            var existinglot = await _context.LotNames.Where(x => x.Id == lotname.Id)
                                                     .FirstOrDefaultAsync();

            existinglot.LotCategoryId = existinglot.LotCategoryId;
            existinglot.SectionName = existinglot.SectionName;
            existinglot.Reason = lotname.Reason;
            existinglot.IsActive = true;

            if (lotname.Reason == null)
                existinglot.Reason = "Reopened Lot Name";

            return true;
        }

        public async Task<IReadOnlyList<LotNameDto>> GetAllActiveLotName()
        {
            return await _context.LotNames
                                           .Join(_context.LotCategories,
                                            lotname => lotname.LotCategoryId,
                                            categories => categories.Id,
                                               (lotname, categories) => new LotNameDto
                                               {
                                                   Id = lotname.Id,
                                                   LotNameCode = lotname.LotNameCode,
                                                   LotCategory = categories.LotCategoryName,
                                                   SectionName = lotname.SectionName,
                                                   DateAdded = lotname.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = lotname.AddedBy,
                                                   IsActive = lotname.IsActive,
                                                   Reason = lotname.Reason
                                               }).Where(x => x.IsActive == true)
                                                 .ToListAsync();
        }

        public async Task<IReadOnlyList<LotNameDto>> GetAllInActiveLotName()
        {
            return await _context.LotNames
                                            .Join(_context.LotCategories,
                                             lotname => lotname.LotCategoryId,
                                             categories => categories.Id,
                                                (lotname, categories) => new LotNameDto
                                                {
                                                    Id = lotname.Id,
                                                    LotNameCode = lotname.LotNameCode,
                                                    LotCategory = categories.LotCategoryName,
                                                    SectionName = lotname.SectionName,
                                                    DateAdded = lotname.DateAdded.ToString("MM/dd/yyyy"),
                                                    AddedBy = lotname.AddedBy,
                                                    IsActive = lotname.IsActive,
                                                    Reason = lotname.Reason
                                                }).Where(x => x.IsActive == false)
                                                  .ToListAsync();
        }

        public async Task<bool> ValidateLotCategoryId(int id)
        {
            var validateExisting = await _context.LotCategories.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> SectionNameExist(string section)
        {
            return await _context.LotNames.AnyAsync(x => x.SectionName == section);
        }

        //----LOT CATEGORY--------

        public async Task<IReadOnlyList<LotCategoryDto>> GetAllLotCategory()
        {
            return await _context.LotCategories
                                               .Select(categories => new LotCategoryDto
                                               {
                                                   Id = categories.Id,
                                                   CategoryName = categories.LotCategoryName,
                                                   DateAdded = categories.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = categories.AddedBy,
                                                   IsActive = categories.IsActive,
                                                   Reason = categories.Reason
                                               }).ToListAsync();
        }
        public async Task<LotCategoryDto> GetCategoryById(int id)
        {
            return await _context.LotCategories
                                                .Select(categories => new LotCategoryDto
                                                {
                                                    Id = categories.Id,
                                                    CategoryName = categories.LotCategoryName,
                                                    DateAdded = categories.DateAdded.ToString("MM/dd/yyyy"),
                                                    AddedBy = categories.AddedBy,
                                                    IsActive = categories.IsActive,
                                                    Reason = categories.Reason
                                                }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddNewLotCategory(LotCategory category)
        {
            category.DateAdded = DateTime.Now;
            category.IsActive = true;

            if(category.AddedBy == null)
               category.AddedBy = "Admin";

            await _context.LotCategories.AddAsync(category);

            return true;
        }
        public async Task<bool> UpdateLotCategory(LotCategory category)
        {
            var existingCategory = await _context.LotCategories.Where(x => x.Id == category.Id)
                                                               .FirstOrDefaultAsync();

            existingCategory.LotCategoryName = category.LotCategoryName;
            existingCategory.LotCategoryCode = category.LotCategoryCode;

            return true;
        }

        public async Task<bool> InActiveLotCategory(LotCategory category)
        {
            var existingCategory = await _context.LotCategories.Where(x => x.Id == category.Id)
                                                               .FirstOrDefaultAsync();

            existingCategory.LotCategoryName = existingCategory.LotCategoryName;
            existingCategory.Reason = category.Reason;
            existingCategory.IsActive = false;

            if (category.Reason == null)
                existingCategory.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateLotCategory(LotCategory category)
        {
            var existingCategory = await _context.LotCategories.Where(x => x.Id == category.Id)
                                                               .FirstOrDefaultAsync();

            existingCategory.LotCategoryName = existingCategory.LotCategoryName;
            existingCategory.Reason = category.Reason;
            existingCategory.IsActive = true;

            if (category.Reason == null)
                existingCategory.Reason = "Reopened Categories";

            return true;
        }

        public async Task<IReadOnlyList<LotCategoryDto>> GetAllActiveLotCategory()
        {
            return await _context.LotCategories
                                         .Select(categories => new LotCategoryDto
                                         {
                                             Id = categories.Id,
                                             LotCategoryCode = categories.LotCategoryCode,
                                             CategoryName = categories.LotCategoryName,
                                             DateAdded = categories.DateAdded.ToString("MM/dd/yyyy"),
                                             AddedBy = categories.AddedBy,
                                             IsActive = categories.IsActive,
                                             Reason = categories.Reason
                                         }).Where(x => x.IsActive == true)
                                           .ToListAsync();
        }

        public async Task<IReadOnlyList<LotCategoryDto>> GetAllInActiveLotCategory()
        {
            return await _context.LotCategories
                                        .Select(categories => new LotCategoryDto
                                        {
                                            Id = categories.Id,
                                            LotCategoryCode = categories.LotCategoryCode,
                                            CategoryName = categories.LotCategoryName,
                                            DateAdded = categories.DateAdded.ToString("MM/dd/yyyy"),
                                            AddedBy = categories.AddedBy,
                                            IsActive = categories.IsActive,
                                            Reason = categories.Reason
                                        }).Where(x => x.IsActive == false)
                                          .ToListAsync();
        }

        public async Task<bool> LotCategoryNameExist(string name)
        {
            return await _context.LotCategories.AnyAsync(x => x.LotCategoryName == name);
        }

        public async Task<PagedList<LotNameDto>> GetAllLotNameWithPagination(bool status, UserParams userParams)
        {
            var lotname = _context.LotNames.OrderByDescending(x => x.DateAdded)
                                           .Join(_context.LotCategories,
                                            lotname => lotname.LotCategoryId,
                                            categories => categories.Id,
                                               (lotname, categories) => new LotNameDto
                                               {
                                                   Id = lotname.Id,
                                                   LotNameCode = lotname.LotNameCode,
                                                   LotCategory = categories.LotCategoryName,
                                                   LotCategoryId = categories.Id,
                                                   SectionName = lotname.SectionName,
                                                   DateAdded = lotname.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = lotname.AddedBy,
                                                   IsActive = lotname.IsActive,
                                                   Reason = lotname.Reason
                                               }).Where(x => x.IsActive == status);

            return await PagedList<LotNameDto>.CreateAsync(lotname, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<LotNameDto>> GetLotNameByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var lotname = _context.LotNames.OrderByDescending(x => x.DateAdded)
                                         .Join(_context.LotCategories,
                                          lotname => lotname.LotCategoryId,
                                          categories => categories.Id,
                                             (lotname, categories) => new LotNameDto
                                             {
                                                 Id = lotname.Id,
                                                 LotNameCode = lotname.LotNameCode,
                                                 LotCategory = categories.LotCategoryName,
                                                 LotCategoryId = categories.Id,
                                                 SectionName = lotname.SectionName,
                                                 DateAdded = lotname.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = lotname.AddedBy,
                                                 IsActive = lotname.IsActive,
                                                 Reason = lotname.Reason
                                             }).Where(x => x.IsActive == status)
                                               .Where(x => x.LotCategory.ToLower()
                                               .Contains(search.Trim().ToLower()));

            return await PagedList<LotNameDto>.CreateAsync(lotname, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateLotNameAndSection(LotName lot)
        {
            var validate = await _context.LotNames.Where(x => x.LotCategoryId == lot.LotCategoryId)
                                                  .Where(x => x.SectionName == lot.SectionName)
                                                  .Where(x => x.IsActive == true)
                                                  .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPagination(bool status, UserParams userParams)
        {
            var category = _context.LotCategories.OrderByDescending(x => x.DateAdded)
                                               .Select(categories => new LotCategoryDto
                                               {
                                                   Id = categories.Id,
                                                   CategoryName = categories.LotCategoryName,
                                                   DateAdded = categories.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = categories.AddedBy,
                                                   IsActive = categories.IsActive,
                                                   Reason = categories.Reason

                                               }).Where(x => x.IsActive == status);

            return await PagedList<LotCategoryDto>.CreateAsync(category, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var category = _context.LotCategories.OrderByDescending(x => x.DateAdded)
                                               .Select(categories => new LotCategoryDto
                                               {
                                                   Id = categories.Id,
                                                   CategoryName = categories.LotCategoryName,
                                                   DateAdded = categories.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = categories.AddedBy,
                                                   IsActive = categories.IsActive,
                                                   Reason = categories.Reason

                                               }).Where(x => x.IsActive == status)
                                                 .Where(x => x.CategoryName.ToLower()
                                                 .Contains(search.Trim().ToLower()));

            return await PagedList<LotCategoryDto>.CreateAsync(category, userParams.PageNumber, userParams.PageSize);
        }
    }

}
