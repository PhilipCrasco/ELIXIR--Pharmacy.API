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
    public interface ICustomerRepository : IGenericRepository<CustomerDto>
    {
        //-----CUSTOMER----------
        Task<bool> AddNewCustomer(Customer customer);
        Task<bool> UpdateCustomerInfo(Customer customer);
        Task<bool> InActiveCustomer(Customer customer);
        Task<bool> ActivateCustomer(Customer customer);
        Task<IReadOnlyList<CustomerDto>> GetAllActiveCustomer();
        Task<IReadOnlyList<CustomerDto>> GetAllInActiveCustomer();
        Task<bool> ValidateFarmId(int id);
        Task<bool> CustomerCodeExist(string customer);

        //-----FARM TYPE----------
        Task<IReadOnlyList<FarmDto>> GetAllFarm();
        Task<FarmDto> GetFarmById(int id);
        Task<bool> AddnewFarm(FarmType farm);
        Task<bool> UpdateFarmType(FarmType farm);
        Task<bool> InActiveFarm(FarmType farm);
        Task<bool> ActivateFarm(FarmType farm);
        Task<IReadOnlyList<FarmDto>> GetAllActiveFarm();
        Task<IReadOnlyList<FarmDto>> GetAllInActiveFarm();
        Task<bool> FarmCodeExist(string farm);

        Task<PagedList<CustomerDto>> GetAllCustomerWithPagination(bool status, UserParams userParams);
        Task<PagedList<CustomerDto>> GetCustomerByStatusWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<PagedList<FarmDto>> GetAllFarmWithPagination(bool status, UserParams userParams);
        Task<PagedList<FarmDto>> GetAllFarmWithPaginationOrig(UserParams userParams, bool status, string search);


    }
}
