using Ardalis.Specification;
using Infrastructure.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Specifications
{
    public class CompanyFilterSpecification : Specification<Company>
    {
        public CompanyFilterSpecification(ApplicationUser user)
        {
            if (user != null)
            {
                Query.Where(x => x.Id == user.CompanyId);
            }
        }
    }
}
