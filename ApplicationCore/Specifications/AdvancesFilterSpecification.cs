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
    public class AdvancesFilterSpecification : Specification<Advance>
    {
        public AdvancesFilterSpecification(ApplicationUser user)    
        {
            if (user != null)
            {
                Query.Where(x => x.ApplicationUserID == user.Id);
            }
        }
    }
}
