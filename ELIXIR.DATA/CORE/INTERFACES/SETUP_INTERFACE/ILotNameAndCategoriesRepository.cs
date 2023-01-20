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
    public interface ILotNameAndCategoriesRepository : IGenericRepository<LotNameDto>
    {

        //-----LOT NAME --------
        Task<bool> AddNewLot(LotName lotname);
        Task<bool> UpdateLotName(LotName lotname);
        Task<bool> InActiveLotName(LotName lotname);
        Task<bool> ActivateLotName(LotName lotname);
        Task<IReadOnlyList<LotNameDto>> GetAllActiveLotName();
        Task<IReadOnlyList<LotNameDto>> GetAllInActiveLotName();
        Task<bool> ValidateLotCategoryId(int id);
        Task<bool> SectionNameExist(string section);



        //-----LOT CATEGORY---------
        Task<IReadOnlyList<LotCategoryDto>> GetAllLotCategory();
        Task<LotCategoryDto> GetCategoryById(int id);
        Task<bool> AddNewLotCategory(LotCategory category);
        Task<bool> UpdateLotCategory(LotCategory category);
        Task<bool> InActiveLotCategory(LotCategory category);
        Task<bool> ActivateLotCategory(LotCategory category);
        Task<IReadOnlyList<LotCategoryDto>> GetAllActiveLotCategory();
        Task<IReadOnlyList<LotCategoryDto>> GetAllInActiveLotCategory();
        Task<bool> LotCategoryNameExist(string name);



        Task<PagedList<LotNameDto>> GetAllLotNameWithPagination(bool status, UserParams userParams);
        Task<PagedList<LotNameDto>> GetLotNameByStatusWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPagination(bool status, UserParams userParams);
        Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> ValidateLotNameAndSection(LotName lot);

    }
}
