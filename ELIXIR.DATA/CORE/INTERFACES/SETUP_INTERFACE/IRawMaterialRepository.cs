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
    public interface IRawMaterialRepository : IGenericRepository<RawMaterialDto>
    {
        //---RAW MATERIALS------
        Task<IReadOnlyList<RawMaterialDto>> GetAllActiveRawMaterial();
        Task<IReadOnlyList<RawMaterialDto>> GetAllInActiveRawMaterial();
        Task<bool> AddNewRawMaterial(RawMaterial rawmaterial);
        Task<bool> UpdateRawMaterialInfo(RawMaterial rawmaterial);
        Task<bool> InActiveRawMaterial(RawMaterial rawmaterial);
        Task<bool> ActivateRawMaterial(RawMaterial rawmaterial);
        Task<bool> ValidateItemCategoryId(int id);
        Task<bool> ValidateUOMId(int id);
        Task<bool> ItemCodeExist(string itemcode);


        //---ITEM CATEGORY--------
        Task<IReadOnlyList<ItemCategoryDto>> GetAllItemCategory();
        Task<ItemCategoryDto> GetCategoryById(int id);
        Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory();
        Task<IReadOnlyList<ItemCategoryDto>> GetAllInActiveItemCategory();
        Task<bool> AddNewItemCategory(ItemCategory category);
        Task<bool> UpdateItemCategory(ItemCategory category);
        Task<bool> InActiveItemCategory(ItemCategory category);
        Task<bool> ActivateItemCategory(ItemCategory category);
        Task<bool> ItemCategoryExist(string category);


        Task<PagedList<RawMaterialDto>> GetAllRawMaterialWithPagination(bool status, UserParams userParams);
        Task<PagedList<RawMaterialDto>> GetRawMaterialByStatusWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPagination(bool status, UserParams userParams);
        Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPaginationOrig(UserParams userParams, bool status, string search);



    }
}
