using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class AdvanceUserIncludeSpecification : Specification<Advance>
    {
        public AdvanceUserIncludeSpecification(Guid? companyId) 
        {
            Query.Include(x => x.ApplicationUser)
                .Where(x => x.ApplicationUser.CompanyId == companyId);
        }
    }
}
