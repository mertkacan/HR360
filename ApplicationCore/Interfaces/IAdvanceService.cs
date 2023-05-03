using ApplicationCore.DTO;
using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAdvanceService
    {
        Task CreateAdvanceAsync(AppAdvanceDTO appAdvanceDTO);
        Task<List<Advance>> GetAdvancesByUserIdAsync(Specification<Advance> advanceFilterSpec);

        Task<Advance> GetAdvanceByUserIdAsync(Specification<Advance> advanceFilterSpec);

        Task DeleteAdvanceByIdAsync(int id);

        Task<List<Advance>> GetAllAdvanceAsync();
        Task<Advance> GetAdvanceByIdAsync(int id);

        Task UpdateAdvanceAsync(Advance advance);

    }
}
