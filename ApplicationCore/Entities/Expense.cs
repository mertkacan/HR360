using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public decimal ExpenseAmount { get; set; }

        public string Invoce { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = false)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public Approval Approval { get; set; } = Approval.OnayBekliyor;


        public Currency Currency { get; set; }

        public TypeOfExpenses TypeOfExpenses { get; set; }

        public string Description { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = false)]
        public DateTime? ApprovalDate { get; set; }

        public string? ManagerDescription { get; set; } = null!;


        //Navigation property
        public string ApplicationUserID { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
