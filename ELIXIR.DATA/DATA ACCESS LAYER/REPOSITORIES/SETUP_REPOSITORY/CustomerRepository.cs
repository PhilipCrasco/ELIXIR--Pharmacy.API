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
    public class CustomerRepository : GenericRepository<CustomerDto>, ICustomerRepository
    {
        private new readonly StoreContext _context;
        public CustomerRepository(StoreContext context) : base (context)
        {
            _context = context;  
        }

        //-----CUSTOMER-------
        public override async Task<IReadOnlyList<CustomerDto>> GetAll()
        {
            return await _context.Customers.OrderByDescending(x => x.DateAdded)
                                         .Join(_context.Farms,
                                          customer => customer.FarmTypeId,
                                          farm => farm.Id,
                                         (customer, farm) => new CustomerDto
                                         {
                                             Id = customer.Id,
                                             CustomerName = customer.CustomerName,
                                             FarmType = farm.FarmName,
                                             FarmTypeId = farm.Id,
                                             CompanyName = customer.CompanyName,
                                             MobileNumber = customer.MobileNumber,
                                             LeadMan = customer.LeadMan,
                                             Address = customer.Address,
                                             DateAdded = customer.DateAdded.ToString("MM/dd/yyyy"),
                                             AddedBy = customer.AddedBy,
                                             IsActive = customer.IsActive
                                         }).ToListAsync();
        }
        public override async Task<CustomerDto> GetById(int id)
        {
            return await _context.Customers
                                           .Join(_context.Farms,
                                            customer => customer.FarmTypeId,
                                            farm => farm.Id,
                                           (customer, farm) => new CustomerDto
                                           {
                                               Id = customer.Id,
                                               CustomerName = customer.CustomerName,
                                               FarmType = farm.FarmName,
                                               FarmTypeId = farm.Id,
                                               CompanyName = customer.CompanyName,
                                               MobileNumber = customer.MobileNumber,
                                               LeadMan = customer.LeadMan,
                                               Address = customer.Address,
                                               DateAdded = customer.DateAdded.ToString("MM/dd/yyyy"),
                                               AddedBy = customer.AddedBy,
                                               IsActive = customer.IsActive
                                           }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAllActiveCustomer()
        {
            return await _context.Customers
                                          .Join(_context.Farms,
                                           customer => customer.FarmTypeId,
                                           farm => farm.Id,
                                          (customer, farm) => new CustomerDto
                                          {
                                              Id = customer.Id,
                                              CustomerCode = customer.CustomerCode,
                                              CustomerName = customer.CustomerName,
                                              FarmType = farm.FarmName,
                                              CompanyName = customer.CompanyName,
                                              MobileNumber = customer.MobileNumber,
                                              LeadMan = customer.LeadMan,
                                              Address = customer.Address,
                                              DateAdded = customer.DateAdded.ToString("MM/dd/yyyy"),
                                              AddedBy = customer.AddedBy,
                                              IsActive = customer.IsActive
                                          }).Where(x => x.IsActive == true)
                                            .ToListAsync();
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAllInActiveCustomer()
        {
            return await _context.Customers
                                             .Join(_context.Farms,
                                              customer => customer.FarmTypeId,
                                              farm => farm.Id,
                                             (customer, farm) => new CustomerDto
                                             {
                                                 Id = customer.Id,
                                                 CustomerCode = customer.CustomerCode,
                                                 CustomerName = customer.CustomerName,
                                                 FarmType = farm.FarmName,
                                                 CompanyName = customer.CompanyName,
                                                 MobileNumber = customer.MobileNumber,
                                                 LeadMan = customer.LeadMan,
                                                 Address = customer.Address,
                                                 DateAdded = customer.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = customer.AddedBy,
                                                 IsActive = customer.IsActive
                                             }).Where(x => x.IsActive == false)
                                               .ToListAsync();
        }


        public async Task<bool> AddNewCustomer(Customer customer)
        {
            customer.DateAdded = DateTime.Now;
            customer.IsActive = true;

            if (customer.AddedBy == null)
                customer.AddedBy = "Admin";

            await _context.Customers.AddAsync(customer);
            return true;
        }


        public async Task<bool> UpdateCustomerInfo(Customer customer)
        {
            var exisitngCustomer = await _context.Customers.Where(x => x.Id == customer.Id)
                                                           .FirstOrDefaultAsync();

            exisitngCustomer.CustomerName = customer.CustomerName;
            exisitngCustomer.FarmTypeId = customer.FarmTypeId;
            exisitngCustomer.CompanyName = customer.CompanyName;
            exisitngCustomer.MobileNumber = customer.MobileNumber;
            exisitngCustomer.LeadMan = customer.LeadMan;
            exisitngCustomer.Address = customer.Address;
            exisitngCustomer.CustomerCode = customer.CustomerCode;

            return true;    
        }


        public async Task<bool> InActiveCustomer(Customer customer)
        {
            var exisitingCustomer = await _context.Customers.Where(x => x.Id == customer.Id)
                                                            .FirstOrDefaultAsync();

            exisitingCustomer.CustomerName = exisitingCustomer.CustomerName;
            exisitingCustomer.FarmTypeId = exisitingCustomer.FarmTypeId;
            exisitingCustomer.CompanyName = exisitingCustomer.CompanyName;
            exisitingCustomer.MobileNumber = exisitingCustomer.MobileNumber;
            exisitingCustomer.LeadMan = exisitingCustomer.LeadMan;
            exisitingCustomer.Address = exisitingCustomer.Address;
            exisitingCustomer.IsActive = false;
            exisitingCustomer.Reason = customer.Reason;

            if (customer.Reason == null)
                exisitingCustomer.Reason = "Change Data";


            return true;
        }

        public async Task<bool> ActivateCustomer(Customer customer)
        {
            var exisitingCustomer = await _context.Customers.Where(x => x.Id == customer.Id)
                                                            .FirstOrDefaultAsync();

            exisitingCustomer.CustomerName = exisitingCustomer.CustomerName;
            exisitingCustomer.FarmTypeId = exisitingCustomer.FarmTypeId;
            exisitingCustomer.CompanyName = exisitingCustomer.CompanyName;
            exisitingCustomer.MobileNumber = exisitingCustomer.MobileNumber;
            exisitingCustomer.LeadMan = exisitingCustomer.LeadMan;
            exisitingCustomer.Address = exisitingCustomer.Address;
            exisitingCustomer.IsActive = true;
            exisitingCustomer.Reason = customer.Reason;

            if (customer.Reason == null)
                exisitingCustomer.Reason = "Reopened Customer";

            return true;
        }

        public async Task<bool> CustomerCodeExist(string customer)
        {
            return await _context.Customers.AnyAsync(x => x.CustomerCode == customer);
        }

        //----FARM--------

        public async Task<IReadOnlyList<FarmDto>> GetAllFarm()
        {
                 return await _context.Farms
                                            .Select(farm => new FarmDto
                                            {
                                                Id = farm.Id,
                                                FarmCode = farm.FarmCode,
                                                FarmName = farm.FarmName,
                                                DateAdded = farm.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = farm.AddedBy,
                                                IsActive = farm.IsActive
                                            }).ToListAsync();
        }

        public async Task<FarmDto> GetFarmById(int id)
        {
            return await _context.Farms
                                          .Select(farm => new FarmDto
                                          {
                                              Id = farm.Id,
                                              FarmCode = farm.FarmCode,
                                              FarmName = farm.FarmName,
                                              DateAdded = farm.DateAdded.ToString("MM/dd/yyyy"),
                                              AddedBy = farm.AddedBy,
                                              IsActive = farm.IsActive
                                          }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddnewFarm(FarmType farm)
        {

            farm.DateAdded = DateTime.Now;
            farm.IsActive = true;

            if (farm.AddedBy == null)
                farm.AddedBy = "Admin";

            await _context.Farms.AddAsync(farm);
            return true;
        }

        public async Task<bool> UpdateFarmType(FarmType farm)
        {
            var existingFarm = await _context.Farms.Where(x => x.Id == farm.Id)
                                                   .FirstOrDefaultAsync();

            existingFarm.FarmName = farm.FarmName;
            existingFarm.FarmCode = farm.FarmCode;

            return true;
        }

        public async Task<bool> InActiveFarm(FarmType farm)
        {
            var exisitngFarm = await _context.Farms.Where(x => x.Id == farm.Id)
                                                   .FirstOrDefaultAsync();

            exisitngFarm.FarmName = exisitngFarm.FarmName;
            exisitngFarm.IsActive = false;
            exisitngFarm.Reason = farm.Reason;

            if (farm.Reason == null)
                exisitngFarm.Reason = "Change Data";

            return true;
        }

        public async Task<bool> ActivateFarm(FarmType farm)
        {
            var exisitngFarm = await _context.Farms.Where(x => x.Id == farm.Id)
                                                   .FirstOrDefaultAsync();

            exisitngFarm.FarmName = farm.FarmName;
            exisitngFarm.IsActive = true;
            exisitngFarm.Reason = farm.Reason;

            if (farm.Reason == null)
                exisitngFarm.Reason = "Reopened Farm";

            return true;
        }

        public async Task<IReadOnlyList<FarmDto>> GetAllActiveFarm()
        {
                 return await _context.Farms
                                            .Select(farm => new FarmDto
                                            {
                                                Id = farm.Id,
                                                FarmCode = farm.FarmCode,
                                                FarmName = farm.FarmName,
                                                DateAdded = farm.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = farm.AddedBy,
                                                IsActive = farm.IsActive
                                            }).Where(x => x.IsActive == true)
                                              .ToListAsync();
        }

        public async Task<IReadOnlyList<FarmDto>> GetAllInActiveFarm()
        {
            return await _context.Farms
                                         .Select(farm => new FarmDto
                                         {
                                             Id = farm.Id,
                                             FarmCode = farm.FarmCode,
                                             FarmName = farm.FarmName,
                                             DateAdded = farm.DateAdded.ToString("MM/dd/yyyy"),
                                             AddedBy = farm.AddedBy,
                                             IsActive = farm.IsActive
                                         }).Where(x => x.IsActive == false)
                                           .ToListAsync();
        }

        public async Task<bool> ValidateFarmId(int id)
        {
            var validateExisting = await _context.Farms.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> FarmCodeExist(string farm)
        {
            return await _context.Farms.AnyAsync(x => x.FarmCode == farm);
        }

        public async Task<PagedList<CustomerDto>> GetAllCustomerWithPagination(bool status, UserParams userParams)
        {
            var customer =  _context.Customers.OrderByDescending(x => x.DateAdded)
                                               .Join(_context.Farms,
                                                customer => customer.FarmTypeId,
                                                farm => farm.Id,
                                               (customer, farm) => new CustomerDto
                                               {
                                                   Id = customer.Id,
                                                   CustomerCode = customer.CustomerCode,
                                                   CustomerName = customer.CustomerName,
                                                   FarmType = farm.FarmName,
                                                   FarmTypeId = farm.Id,
                                                   CompanyName = customer.CompanyName,
                                                   MobileNumber = customer.MobileNumber,
                                                   LeadMan = customer.LeadMan,
                                                   Address = customer.Address,
                                                   DateAdded = customer.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = customer.AddedBy,
                                                   IsActive = customer.IsActive
                                               }).Where(x => x.IsActive == status);

            return await PagedList<CustomerDto>.CreateAsync(customer, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CustomerDto>> GetCustomerByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var customer = _context.Customers.OrderByDescending(x => x.DateAdded)
                                               .Join(_context.Farms,
                                                customer => customer.FarmTypeId,
                                                farm => farm.Id,
                                               (customer, farm) => new CustomerDto
                                               {
                                                   Id = customer.Id,
                                                   CustomerCode = customer.CustomerCode,
                                                   CustomerName = customer.CustomerName,
                                                   FarmType = farm.FarmName,
                                                   FarmTypeId = farm.Id,
                                                   CompanyName = customer.CompanyName,
                                                   MobileNumber = customer.MobileNumber,
                                                   LeadMan = customer.LeadMan,
                                                   Address = customer.Address,
                                                   DateAdded = customer.DateAdded.ToString("MM/dd/yyyy"),
                                                   AddedBy = customer.AddedBy,
                                                   IsActive = customer.IsActive
                                               }).Where(x => x.IsActive == status)
                                                 .Where(x => x.CustomerName.ToLower()
                                                 .Contains(search.Trim().ToLower()));

            return await PagedList<CustomerDto>.CreateAsync(customer, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<FarmDto>> GetAllFarmWithPagination(bool status, UserParams userParams)
        {
            var farm = _context.Farms.OrderByDescending(x => x.DateAdded)
                             .Select(farm => new FarmDto
                             {
                                 Id = farm.Id,
                                 FarmCode = farm.FarmCode,
                                 FarmName = farm.FarmName,
                                 DateAdded = farm.DateAdded.ToString("MM/dd/yyyy"),
                                 AddedBy = farm.AddedBy,
                                 IsActive = farm.IsActive

                             }).Where(x => x.IsActive == status);

            return await PagedList<FarmDto>.CreateAsync(farm, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<FarmDto>> GetAllFarmWithPaginationOrig(UserParams userParams, bool status, string search)
        {

            var farm = _context.Farms.OrderByDescending(x => x.DateAdded)
                             .Select(farm => new FarmDto
                             {
                                 Id = farm.Id,
                                 FarmCode = farm.FarmCode,
                                 FarmName = farm.FarmName,
                                 DateAdded = farm.DateAdded.ToString("MM/dd/yyyy"),
                                 AddedBy = farm.AddedBy,
                                 IsActive = farm.IsActive

                             }).Where(x => x.IsActive == status)
                               .Where(x => x.FarmCode.ToLower()
                               .Contains(search.Trim().ToLower()));

            return await PagedList<FarmDto>.CreateAsync(farm, userParams.PageNumber, userParams.PageSize);
        }
    }
}
