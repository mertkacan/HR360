using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Ardalis.Specification;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Services
{
    public class AdvanceService : IAdvanceService
    {
        private readonly IRepository<Advance> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AdvanceService(IRepository<Advance> repository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task CreateAdvanceAsync(AppAdvanceDTO appAdvanceDTO)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var advance = _mapper.Map<AppAdvanceDTO, Advance>(appAdvanceDTO);
            advance.ApplicationUserID = user.Id;
            await _repository.AddAsync(advance);
        }

        public async Task<Advance> GetAdvanceByUserIdAsync(Specification<Advance> getAdvanceById)
        {
            var advance = await _repository.FirstOrDefaultAsync(getAdvanceById);

            return advance!;
        }

        public Task<List<Advance>> GetAdvancesByUserIdAsync(Specification<Advance> advanceFilterSpec)
        {
            var advances = _repository.ListAsync(advanceFilterSpec);
            return advances;
        }

        public async Task DeleteAdvanceByIdAsync(int id)
        {
            var advance = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(advance!);
        }

        public async Task<List<Advance>> GetAllAdvanceAsync()
        {
            return await _repository.ListAsync();
        }

        public async Task<Advance> GetAdvanceByIdAsync(int id)
        {
            var advance = await _repository.GetByIdAsync(id);

            return advance!; 
        }

        public async Task UpdateAdvanceAsync(Advance advance)
        {
            var advanceUpdate = await _repository.GetByIdAsync(advance.Id);
            await _repository.UpdateAsync(advanceUpdate!);
        }
    }
}
