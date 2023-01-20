using ELIXIR.DATA.CORE.INTERFACES;
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
    public class ReasonRepository : GenericRepository<ReasonDto>, IReasonRepository
    {
        private new readonly StoreContext _context;
        public ReasonRepository(StoreContext context) : base (context)
        {
            _context = context;
        }

        public override async Task<IReadOnlyList<ReasonDto>> GetAll()
        {
            return await _context.Reasons
                                          .Join(_context.MainMenus,
                                           reason => reason.MenuId,
                                           menu => menu.Id,
                                              (reason, menu) => new ReasonDto
                                              {
                                                  Id = reason.Id,
                                                  Menu = menu.ModuleName,
                                                  MenuId = menu.Id,
                                                  ReasonName = reason.ReasonName,
                                                  DateAdded = reason.DateAdded.ToString("MM/dd/yyyy"),
                                                  AddedBy = reason.AddedBy,
                                                  IsActive = reason.IsActive,
                                              }).ToListAsync();
        }
        public override async Task<ReasonDto> GetById(int id)
        {
            return await _context.Reasons
                                          .Join(_context.MainMenus,
                                           reason => reason.MenuId,
                                           menu => menu.Id,
                                              (reason, menu) => new ReasonDto
                                              {
                                                 Id = reason.Id,
                                                 Menu = menu.ModuleName,
                                                 MenuId = menu.Id,
                                                 ReasonName = reason.ReasonName,
                                                 DateAdded = reason.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = reason.AddedBy,
                                                 IsActive = reason.IsActive,
                                             }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<ReasonDto>> GetAllActiveReason()
        {
            return await _context.Reasons
                                          .Join(_context.MainMenus,
                                           reason => reason.MenuId,
                                           menu => menu.Id,
                                              (reason, menu) => new ReasonDto
                                              {
                                                 Id = reason.Id,
                                                 Menu = menu.ModuleName,
                                                 MenuId = menu.Id,
                                                 ReasonName = reason.ReasonName,
                                                 DateAdded = reason.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = reason.AddedBy,
                                                 IsActive = reason.IsActive,
                                             }).Where(x => x.IsActive == true)
                                               .ToListAsync();
        }

        public async Task<IReadOnlyList<ReasonDto>> GetAllInActiveReason()
        {
            return await _context.Reasons
                                     .Join(_context.MainMenus,
                                           reason => reason.MenuId,
                                           menu => menu.Id,
                                              (reason, menu) => new ReasonDto
                                              {
                                               Id = reason.Id,
                                               Menu = menu.ModuleName,
                                               MenuId = menu.Id,
                                               ReasonName = reason.ReasonName,
                                               DateAdded = reason.DateAdded.ToString("MM/dd/yyyy"),
                                               AddedBy = reason.AddedBy,
                                               IsActive = reason.IsActive,
                                           }).Where(x => x.IsActive == false)
                                             .ToListAsync();
        }

        public async Task<bool> AddnewReason(Reason reason)
        {
            reason.IsActive = true;
            reason.DateAdded = DateTime.Now;

            if (reason.AddedBy == null)
                reason.AddedBy = "Admin";

            await _context.Reasons.AddAsync(reason);

            return true;
        }

        public async Task<bool> UpdateReason(Reason reason)
        {
            var exisitngReason = await _context.Reasons.Where(x => x.Id == reason.Id)
                                                       .FirstOrDefaultAsync();

            if (exisitngReason == null)
                return await AddnewReason(reason);

            exisitngReason.MenuId = reason.MenuId;
            exisitngReason.ReasonName = reason.ReasonName;

            return true;
        }
        public async Task<bool> InActiveReason(Reason reason)
        {
            var existingReason = await _context.Reasons.Where(x => x.Id == reason.Id)
                                                       .FirstOrDefaultAsync();

            existingReason.MenuId = existingReason.MenuId;
            existingReason.ReasonName = existingReason.ReasonName;
            existingReason.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateReason(Reason reason)
        {
            var existingReason = await _context.Reasons.Where(x => x.Id == reason.Id)
                                                       .FirstOrDefaultAsync();

            existingReason.MenuId = existingReason.MenuId;
            existingReason.ReasonName = existingReason.ReasonName;
            existingReason.IsActive = true;

            return true;
        }

        public async Task<bool> ReasonNameExist(string reason)
        {
            return await _context.Reasons.AnyAsync(x => x.ReasonName == reason);
        }

        public async Task<bool> ValidateModuleId(int id)
        {
            var validateExisting = await _context.MainMenus.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<PagedList<ReasonDto>> GetAllReasonWithPagination(bool status, UserParams userParams)
        {
            var reason = _context.Reasons.OrderByDescending(x => x.DateAdded)
                                         .Join(_context.MainMenus,
                                          reason => reason.MenuId,
                                          menu => menu.Id,
                                             (reason, menu) => new ReasonDto
                                             {
                                                 Id = reason.Id,
                                                 Menu = menu.ModuleName,
                                                 MenuId = menu.Id,
                                                 ReasonName = reason.ReasonName,
                                                 DateAdded = reason.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = reason.AddedBy,
                                                 IsActive = reason.IsActive,
                                             }).Where(x => x.IsActive == status);

            return await PagedList<ReasonDto>.CreateAsync(reason, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ReasonDto>> GetReasonByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var reason = _context.Reasons.OrderByDescending(x => x.DateAdded)
                                       .Join(_context.MainMenus,
                                        reason => reason.MenuId,
                                        menu => menu.Id,
                                           (reason, menu) => new ReasonDto
                                           {
                                               Id = reason.Id,
                                               Menu = menu.ModuleName,
                                               MenuId = menu.Id,
                                               ReasonName = reason.ReasonName,
                                               DateAdded = reason.DateAdded.ToString("MM/dd/yyyy"),
                                               AddedBy = reason.AddedBy,
                                               IsActive = reason.IsActive,
                                           }).Where(x => x.IsActive == status)
                                             .Where(x => x.ReasonName.ToLower()
                                             .Contains(search.Trim().ToLower()));

            return await PagedList<ReasonDto>.CreateAsync(reason, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateReasonEntry(Reason reason)
        {
            var validate = await _context.Reasons.Where(x => x.MenuId == reason.MenuId)
                                           .Where(x => x.ReasonName == reason.ReasonName)
                                           .ToListAsync();
            if (validate.Count != 0)
                return false;

            return true;
        }
    }
}
