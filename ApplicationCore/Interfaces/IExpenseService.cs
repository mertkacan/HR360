using ApplicationCore.DTO;
using ApplicationCore.Entities;
using Ardalis.Specification;
using Infrastructure.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IExpenseService
    {
        Task CreateExpenseAsync(AppExpenseDTO appExpenseDTO);

        Task<Expense> GetExpenseByIdAsync(int id);

        Task DeleteExpenseByIdAsync(int id);

        Task<List<Expense>> GetAllExpensesAsync(Specification<Expense> specification);

        Task UpdateExpenseAsync(Expense expense);
    }
}
