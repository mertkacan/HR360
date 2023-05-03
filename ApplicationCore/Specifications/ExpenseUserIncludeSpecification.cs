using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Specifications
{
    public class ExpenseUserIncludeSpecification : Specification<Expense>
    {
        public ExpenseUserIncludeSpecification(Guid? companyId)
        {
            Query.Include(x => x.ApplicationUser)
                .Where(x => x.ApplicationUser.CompanyId == companyId);
        }
    }
}
