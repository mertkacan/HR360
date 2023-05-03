using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class GetAdvanceByIdSpecification : Specification<Advance>
    {
        public GetAdvanceByIdSpecification(int id)
        {
            Query.Where(x => x.Id == id);
        }
    }
}
