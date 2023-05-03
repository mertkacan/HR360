﻿using Ardalis.Specification;
using Infrastructure.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class GetCompanyByIdSpecification : Specification<Company>
    {
        public GetCompanyByIdSpecification(Guid? id)
        {
            if (id != null)
            {
                Query.Where(x => x.Id == id);
            }
        }
    }
}
