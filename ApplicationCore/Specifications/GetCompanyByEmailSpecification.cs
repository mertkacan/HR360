using Ardalis.Specification;
using Infrastructure.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class GetCompanyByEmailSpecification : Specification<Company>
    {
        public GetCompanyByEmailSpecification(string sirketMail)
        {
            if (sirketMail != null)
            {
                Query.Where(x => x.CompanyMail == sirketMail);
            }
        }
    }
}
