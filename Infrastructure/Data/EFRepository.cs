using ApplicationCore.Interfaces;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace Infrastructure.Data
{
    public class EFRepository<T> : RepositoryBase<T>, IReadRepositoryBase<T>, IRepository<T> where T : class
    {
        public EFRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
