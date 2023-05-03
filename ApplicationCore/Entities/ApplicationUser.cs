using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.Linq;
using Infrastructure.Identity.Data;
using ApplicationCore.Enums;
using ApplicationCore.Entities;

namespace Web.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string Surname { get; set; } = null!;
        public string? SecondSurname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PlaceOfBirth { get; set; } = null!;
        public string IdentityNumber { get; set; } = null!;
        public DateTime? HireDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public Job? Job { get; set; }
        public string Address { get; set; } = null!;
        public decimal? Salary { get; set; }
        public string Picture { get; set; } = null!;
        public Company? Company { get; set; }
        public Guid? CompanyId { get; set; }
        public Department? Department { get; set; }
        public Guid DepartmentId { get; set; }
        public Gender? Gender { get; set; }

        //Navigation property
        public List<Expense>? Expenses { get; set; }
        public bool IsUpdate { get; set; }
        public List<Advance> Advances { get; set; } = new();
        public decimal AdvanceSpent { get; set; }

        public DateTime? FirstAdvanceDate { get; set; }
        public bool IsActive
        {
            get
            {
                DateTime? iseGiris = HireDate;
                DateTime? istenCikis = ReleaseDate;
                DateTime suan = DateTime.Now;

                if (iseGiris.HasValue && istenCikis.HasValue)
                {
                    bool iseBasladiMi = iseGiris.Value.CompareTo(suan) < 0;
                    bool iseDevamEdiyorMu = suan.CompareTo(istenCikis.Value) < 0;

                    return iseBasladiMi && iseDevamEdiyorMu;
                }
                else if (iseGiris.HasValue && !istenCikis.HasValue)
                {
                    bool iseBasladiMi = iseGiris.Value.CompareTo(suan) < 0;

                    return iseBasladiMi;
                }
                else
                {
                    return false;
                }

            }
        }
    }
}
