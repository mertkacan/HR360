using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Ardalis.Specification;
using AutoMapper;
using Infrastructure.Identity.Data;
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
    public class ExpenseService : IExpenseService
    {
        private readonly IRepository<Expense> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ExpenseService(IRepository<Expense> repository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task CreateExpenseAsync(AppExpenseDTO appExpenseDTO)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var expense = _mapper.Map<AppExpenseDTO, Expense>(appExpenseDTO);
            expense.ApplicationUserID = user.Id;
            expense.Invoce = appExpenseDTO.PictureName!;
            await _repository.AddAsync(expense);
        }

        public async Task DeleteExpenseByIdAsync(int id)
        {
            var expense = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(expense!);
        }

        public async Task<List<Expense>> GetAllExpensesAsync(Specification<Expense> specification)
        {
            var expenses = await _repository.ListAsync(specification);

            return expenses;
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            var expense = await _repository.GetByIdAsync(id);

            return expense!;
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            var expenseUpdate = await _repository.GetByIdAsync(expense.Id);
            await _repository.UpdateAsync(expenseUpdate!);
        }
    }
}
