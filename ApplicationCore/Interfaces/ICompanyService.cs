using ApplicationCore.DTO;
using Ardalis.Specification;
using Infrastructure.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ICompanyService
    {
        Task CreateCompanyAsync(AppCompanyDTO appCompanyDTO);
        Task UpdateCompanyAsync(AppCompanyDTO appCompanyDTO);
        Task<List<Company>> GetAllCompanyAsync();
        Task<Company> GetCompanyByEmailAsync(string sirketEmail);
        Task<Company> GetCompanyByIdAsync(Guid? id);

        Task<Company> FirstOrDefaultAsync(Specification<Company> specification);
    }
}
