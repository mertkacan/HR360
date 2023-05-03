using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.DTO
{
    public class AppAdvanceDTO
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Avans türü seçilmelidir!")]
        public TypeOfAdvance? TypeOfAdvance { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [Required(ErrorMessage = "Avans miktarını giriniz")]
        public decimal? AdvanceAmount { get; set; }


        public DateTime? CreationDate { get; set; } = DateTime.Now;


        public Approval Approval { get; set; } = Approval.OnayBekliyor;


        [Required(ErrorMessage = "Para birimi seçilmelidir!")]
        public Currency? Currency { get; set; }


        [Required(ErrorMessage = "Açıklama girilmelidir!")]
        [MaxLength(60)]
        public string? Description { get; set; }


        public string? ManagerDescription { get; set; }

        // talep oluşturan kişiId
        public string? AppUserDTOId { get; set; } = null!;

        // talep oluşturan kişi
        public AppUserDTO? AppUserDTO { get; set; } = null!;
    }
}
