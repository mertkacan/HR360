using ApplicationCore.DTO;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUser, AppUserDTO>().ReverseMap();
            CreateMap<Expense, AppExpenseDTO>().ReverseMap();
            CreateMap<Advance, AppAdvanceDTO>().ReverseMap();
            CreateMap<Company, AppCompanyDTO>().ReverseMap();
        }
    }
}
