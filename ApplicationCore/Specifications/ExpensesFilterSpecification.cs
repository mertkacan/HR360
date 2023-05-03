using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class ExpensesFilterSpecification : Specification<Expense>   
    {
        public ExpensesFilterSpecification(string? userId)
        {
            if (userId != null)
            {
                Query.Where(x => x.ApplicationUserID == userId);
            }
        }
    }
}
