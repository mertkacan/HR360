using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Entities
{
    public class Advance
    {
        public int Id { get; set; }

        public TypeOfAdvance? TypeOfAdvance { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public decimal? AdvanceAmount { get; set; }

        public DateTime? CreationDate { get; set; } = DateTime.Now;

        public Approval Approval { get; set; }

        public Currency? Currency { get; set; }

        public string? Description { get; set; }
        public string? ManagerDescription { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;
        public string ApplicationUserID { get; set; } = null!;

    }
}
