using ApplicationCore.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Web.Areas.Identity.Data;

namespace Infrastructure.Identity.Data
{
    public class Company
    {
        public Guid Id { get; set; }
        public string? CompanyName { get; set; } = null!;   
        public string? CompanyPhoneNumber { get; set; } = null!;
        public string? CompanyAddress { get; set; } = null!;
        public string? CompanyMail { get; set; } = null!;
        public int? PersonnelAmount => ApplicationUsers!.Count();
        public List<ApplicationUser>? ApplicationUsers { get; set; } = new();
        public CompanyStatus? CompanyStatus { get; set; }
        public string? MersisNumarasi { get; set; }
        public string? VergiNumarasi { get; set; }
        public VergiDairesi? VergiDairesi { get; set; }
        public string? Logo { get; set; }
        public DateTime? CompanyEstablishmentDate { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractFinishDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
