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
    public class RawMaterialRepository :  GenericRepository<RawMaterialDto>, IRawMaterialRepository
    {
        private new readonly StoreContext _context;

       
        public RawMaterialRepository(StoreContext context) : base (context)
        {
            _context = context;
        }

        //----RAW MATERIAL-------

        public override async Task<IReadOnlyList<RawMaterialDto>> GetAll()
        {
                var rawmaterials = (from rawmaterial in _context.RawMaterials
                                    join category in _context.ItemCategories on rawmaterial.ItemCategoryId equals category.Id
                                    join uom in _context.UOMS on rawmaterial.UomId equals uom.Id
                                    select new RawMaterialDto
                                    {
                                        Id = rawmaterial.Id,
                                        ItemCode = rawmaterial.ItemCode,
                                        ItemDescription = rawmaterial.ItemDescription,
                                        ItemCategory = category.ItemCategoryName,
                                        ItemCategoryId = category.Id,
                                        Uom = uom.UOM_Code,
                                        UomId = uom.Id,
                                        BufferLevel = rawmaterial.BufferLevel,
                                        DateAdded = (rawmaterial.DateAdded).ToString("MM/dd/yyyy"),
                                        AddedBy = rawmaterial.AddedBy,
                                        IsActive = rawmaterial.IsActive,
                                        Reason = rawmaterial.Reason
                                    });
                return await rawmaterials.ToListAsync();
        }

        public override async Task<RawMaterialDto> GetById(int id)
        {
            var rawmaterials = (from rawmaterial in _context.RawMaterials
                                join category in _context.ItemCategories on rawmaterial.ItemCategoryId equals category.Id
                                join uom in _context.UOMS on rawmaterial.UomId equals uom.Id
                                select new RawMaterialDto
                                {
                                    Id = rawmaterial.Id,
                                    ItemCode = rawmaterial.ItemCode,
                                    ItemDescription = rawmaterial.ItemDescription,
                                    ItemCategory = category.ItemCategoryName,
                                    ItemCategoryId = category.Id,
                                    Uom = uom.UOM_Code,
                                    UomId = uom.Id,
                                    BufferLevel = rawmaterial.BufferLevel,
                                    DateAdded = (rawmaterial.DateAdded).ToString("MM/dd/yyyy"),
                                    AddedBy = rawmaterial.AddedBy,
                                    IsActive = rawmaterial.IsActive,
                                    Reason = rawmaterial.Reason
                                });
            return await rawmaterials.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<RawMaterialDto>> GetAllActiveRawMaterial()
        {
            var rawmaterials = (from rawmaterial in _context.RawMaterials
                                join category in _context.ItemCategories on rawmaterial.ItemCategoryId equals category.Id
                                join uom in _context.UOMS on rawmaterial.UomId equals uom.Id
                                select new RawMaterialDto
                                {
                                    Id = rawmaterial.Id,
                                    ItemCode = rawmaterial.ItemCode,
                                    ItemDescription = rawmaterial.ItemDescription,
                                    ItemCategory = category.ItemCategoryName,
                                    Uom = uom.UOM_Code,
                                    BufferLevel = rawmaterial.BufferLevel,
                                    DateAdded = (rawmaterial.DateAdded).ToString("MM/dd/yyyy"),
                                    AddedBy = rawmaterial.AddedBy,
                                    IsActive = rawmaterial.IsActive,
                                    Reason = rawmaterial.Reason
                                });

            return await rawmaterials.OrderBy(x => x.ItemCode)
                                     .Where(x => x.IsActive == true)
                                     .ToListAsync();
        }

        public async Task<IReadOnlyList<RawMaterialDto>> GetAllInActiveRawMaterial()
        {
            var rawmaterials = (from rawmaterial in _context.RawMaterials
                                join category in _context.ItemCategories on rawmaterial.ItemCategoryId equals category.Id
                                join uom in _context.UOMS on rawmaterial.UomId equals uom.Id
                                select new RawMaterialDto
                                {
                                    Id = rawmaterial.Id,
                                    ItemCode = rawmaterial.ItemCode,
                                    ItemDescription = rawmaterial.ItemDescription,
                                    ItemCategory = category.ItemCategoryName,
                                    Uom = uom.UOM_Code,
                                    BufferLevel = rawmaterial.BufferLevel,
                                    DateAdded = (rawmaterial.DateAdded).ToString("MM/dd/yyyy"),
                                    AddedBy = rawmaterial.AddedBy,
                                    IsActive = rawmaterial.IsActive,
                                    Reason = rawmaterial.Reason
                                });

            return await rawmaterials
                                     .Where(x => x.IsActive == false)
                                     .ToListAsync();
        }
        public async Task<bool> AddNewRawMaterial(RawMaterial rawmaterial)
        {
            rawmaterial.DateAdded = DateTime.Now;
            rawmaterial.IsActive = true;

            if (rawmaterial.AddedBy == null)
                rawmaterial.AddedBy = "Admin";

            await _context.RawMaterials.AddAsync(rawmaterial);
            return true;
        }

        public async Task<bool> UpdateRawMaterialInfo(RawMaterial rawmaterial)
        {
            var existingRawmaterial = await _context.RawMaterials.Where(x => x.Id == rawmaterial.Id)
                                                    .FirstOrDefaultAsync();

            if (existingRawmaterial == null)
                return await AddNewRawMaterial(rawmaterial);

            existingRawmaterial.ItemCode = rawmaterial.ItemCode;
            existingRawmaterial.ItemDescription = rawmaterial.ItemDescription;
            existingRawmaterial.ItemCategoryId = rawmaterial.ItemCategoryId;
            existingRawmaterial.UomId = rawmaterial.UomId;
            existingRawmaterial.BufferLevel = rawmaterial.BufferLevel;

            return true;
        }
        public async Task<bool> InActiveRawMaterial(RawMaterial rawmaterial)
        {
            var existingRawMaterial = await _context.RawMaterials.Where(x => x.Id == rawmaterial.Id)
                                                                 .FirstOrDefaultAsync();

            existingRawMaterial.ItemCode = existingRawMaterial.ItemCode;
            existingRawMaterial.ItemDescription = existingRawMaterial.ItemDescription;
            existingRawMaterial.ItemCategoryId = existingRawMaterial.ItemCategoryId;
            existingRawMaterial.UomId = existingRawMaterial.UomId;
            existingRawMaterial.BufferLevel = existingRawMaterial.BufferLevel;
            existingRawMaterial.Reason = rawmaterial.Reason;
            existingRawMaterial.IsActive = false;

            if (rawmaterial.Reason == null)
                existingRawMaterial.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateRawMaterial(RawMaterial rawmaterial)
        {
            var existingRawMaterial = await _context.RawMaterials.Where(x => x.Id == rawmaterial.Id)
                                                                 .FirstOrDefaultAsync();

            existingRawMaterial.ItemCode = existingRawMaterial.ItemCode;
            existingRawMaterial.ItemDescription = existingRawMaterial.ItemDescription;
            existingRawMaterial.ItemCategoryId = existingRawMaterial.ItemCategoryId;
            existingRawMaterial.UomId = existingRawMaterial.UomId;
            existingRawMaterial.BufferLevel = existingRawMaterial.BufferLevel;
            existingRawMaterial.Reason = rawmaterial.Reason;
            existingRawMaterial.IsActive = true;

            if (rawmaterial.Reason == null)
                existingRawMaterial.Reason = "Reopened Raw Materrial";

            return true;
        }

        public async Task<bool> ValidateItemCategoryId(int id)
        {
            var validateExisting = await _context.ItemCategories.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUOMId(int id)
        {
            var validateExisting = await _context.UOMS.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ItemCodeExist(string itemcode)
        {
            return await _context.RawMaterials.AnyAsync(x => x.ItemCode == itemcode);
        }



        //-----ITEM CATEGORY-----

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllItemCategory()
        {
            return await _context.ItemCategories
                                                .Select(item => new ItemCategoryDto
                                                {
                                                    Id = item.Id,
                                                    ItemCategoryName = item.ItemCategoryName,
                                                    DateAdded = item.DateAdded.ToString("MM/dd/yyyy"),
                                                    AddedBy = item.AddedBy,
                                                    IsActive = item.IsActive,
                                                    Reason = item.Reason
                                                }).ToListAsync();
        }

        public async Task<ItemCategoryDto> GetCategoryById(int id)
        {
            return await _context.ItemCategories
                                                 .Select(item => new ItemCategoryDto
                                                 {
                                                     Id = item.Id,
                                                     ItemCategoryName = item.ItemCategoryName,
                                                     DateAdded = item.DateAdded.ToString("MM/dd/yyyy"),
                                                     AddedBy = item.AddedBy,
                                                     IsActive = item.IsActive,
                                                     Reason = item.Reason
                                                 }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory()
        {
            return await _context.ItemCategories
                                            .Select(item => new ItemCategoryDto
                                            {
                                                Id = item.Id,
                                                ItemCategoryName = item.ItemCategoryName,
                                                DateAdded = item.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = item.AddedBy,
                                                IsActive = item.IsActive,
                                                Reason = item.Reason
                                            })
                                                .Where(x => x.IsActive == true)
                                                .ToListAsync();
        }

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllInActiveItemCategory()
        {
            return await _context.ItemCategories
                                          .Select(item => new ItemCategoryDto
                                          {
                                              Id = item.Id,
                                              ItemCategoryName = item.ItemCategoryName,
                                              DateAdded = item.DateAdded.ToString("MM/dd/yyyy"),
                                              AddedBy = item.AddedBy,
                                              IsActive = item.IsActive,
                                              Reason = item.Reason
                                          })
                                              .Where(x => x.IsActive == false)
                                              .ToListAsync();
        }

        public async Task<bool> AddNewItemCategory(ItemCategory category)
        {
            category.DateAdded = DateTime.Now;
            category.IsActive = true;

            if (category.AddedBy == null)
                category.AddedBy = "Admin";

            await _context.ItemCategories.AddAsync(category);
            return true;
        }

        public async Task<bool> UpdateItemCategory(ItemCategory category)
        {
            var existingCategory= await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                               .FirstOrDefaultAsync();        
            if (existingCategory == null)
                return await AddNewItemCategory(category);

            existingCategory.ItemCategoryName = category.ItemCategoryName;
         
            return true;
        }

        public async Task<bool> InActiveItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                                .FirstOrDefaultAsync();

            existingCategory.ItemCategoryName = existingCategory.ItemCategoryName;
            existingCategory.Reason = category.Reason;
            existingCategory.IsActive = false;

            if (category.Reason == null)
                existingCategory.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                                .FirstOrDefaultAsync();
             
            existingCategory.ItemCategoryName = existingCategory.ItemCategoryName;
            existingCategory.Reason = category.Reason;
            existingCategory.IsActive = true;

            if (category.Reason == null)
                existingCategory.Reason = "Reopened Item Category";

            return true;
        }

        public async Task<bool> ItemCategoryExist(string category)
        {
            return await _context.ItemCategories.AnyAsync(x => x.ItemCategoryName == category);
        }

        public async Task<PagedList<RawMaterialDto>> GetAllRawMaterialWithPagination(bool status, UserParams userParams)
        {
            var rawmaterials = (from rawmaterial in _context.RawMaterials
                                join category in _context.ItemCategories on rawmaterial.ItemCategoryId equals category.Id
                                join uom in _context.UOMS on rawmaterial.UomId equals uom.Id
                                orderby rawmaterial.DateAdded descending
                                select new RawMaterialDto
                                {
                                    Id = rawmaterial.Id,
                                    ItemCode = rawmaterial.ItemCode,
                                    ItemDescription = rawmaterial.ItemDescription,
                                    ItemCategory = category.ItemCategoryName,
                                    ItemCategoryId = category.Id,
                                    Uom = uom.UOM_Code,
                                    UomId = uom.Id,
                                    BufferLevel = rawmaterial.BufferLevel,
                                    DateAdded = (rawmaterial.DateAdded).ToString("MM/dd/yyyy"),
                                    AddedBy = rawmaterial.AddedBy,
                                    IsActive = rawmaterial.IsActive,
                                    Reason = rawmaterial.Reason
                                }).OrderBy(x => x.ItemCode)
                                  .Where(x => x.IsActive == status);


            return await PagedList<RawMaterialDto>.CreateAsync(rawmaterials, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<RawMaterialDto>> GetRawMaterialByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var rawmaterials = (from rawmaterial in _context.RawMaterials
                                join category in _context.ItemCategories on rawmaterial.ItemCategoryId equals category.Id
                                join uom in _context.UOMS on rawmaterial.UomId equals uom.Id
                                orderby rawmaterial.DateAdded descending
                                select new RawMaterialDto
                                {
                                    Id = rawmaterial.Id,
                                    ItemCode = rawmaterial.ItemCode,
                                    ItemDescription = rawmaterial.ItemDescription,
                                    ItemCategory = category.ItemCategoryName,
                                    ItemCategoryId = category.Id,
                                    Uom = uom.UOM_Code,
                                    UomId = uom.Id,
                                    BufferLevel = rawmaterial.BufferLevel,
                                    DateAdded = (rawmaterial.DateAdded).ToString("MM/dd/yyyy"),
                                    AddedBy = rawmaterial.AddedBy,
                                    IsActive = rawmaterial.IsActive,
                                    Reason = rawmaterial.Reason
                                }).OrderBy(x => x.ItemCode)
                                  .Where(x => x.IsActive == status)
                                  .Where(x => x.ItemCode.ToLower()
                                  .Contains(search.Trim().ToLower()));
            
            return await PagedList<RawMaterialDto>.CreateAsync(rawmaterials, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPagination(bool status, UserParams userParams)
        {
            var category = _context.ItemCategories.OrderByDescending(x => x.DateAdded)
                                                .Select(item => new ItemCategoryDto
                                                {
                                                    Id = item.Id,
                                                    ItemCategoryName = item.ItemCategoryName,
                                                    DateAdded = item.DateAdded.ToString("MM/dd/yyyy"),
                                                    AddedBy = item.AddedBy,
                                                    IsActive = item.IsActive,
                                                    Reason = item.Reason

                                                })
                                                  .Where(x => x.IsActive == status);

            return await PagedList<ItemCategoryDto>.CreateAsync(category, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var category = _context.ItemCategories.OrderByDescending(x => x.DateAdded)
                                               .Select(item => new ItemCategoryDto
                                               {
                                                   Id = item.Id,
                                                   ItemCategoryName = item.ItemCategoryName,
                                                   DateAdded = item.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = item.AddedBy,
                                                   IsActive = item.IsActive,
                                                   Reason = item.Reason

                                               })
                                                 .Where(x => x.IsActive == status)
                                                 .Where(x => x.ItemCategoryName.ToLower()
                                                 .Contains(search.Trim().ToLower()));

            return await PagedList<ItemCategoryDto>.CreateAsync(category, userParams.PageNumber, userParams.PageSize);
        }
    }

}
