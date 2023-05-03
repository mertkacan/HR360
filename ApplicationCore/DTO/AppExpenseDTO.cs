using ApplicationCore.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.DTO
{
    public class AppExpenseDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Gider miktarını giriniz")]
        [Range(typeof(decimal), "10", "50000", ErrorMessage = "Harcama tutarı 10 ile 50.000 arasında olmalıdır")]
        public decimal? ExpenseAmount { get; set; }

        // sadece pdf jpg png türünde dosya yükleyebilsin
        [Required(ErrorMessage = "Fatura ekleyiniz!")]
        public IFormFile? Invoce { get; set; } = null!;

        public string? PictureName { get; set; } = null!;


        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = false)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public Approval Approval { get; set; } = Approval.OnayBekliyor;

        [Required(ErrorMessage = "Para birimi seçilmelidir!")]
        public Currency? Currency { get; set; }

        [Required(ErrorMessage = "Gider türü seçilmelidir!")]
        public TypeOfExpenses? TypeOfExpenses { get; set; }

        [Required(ErrorMessage = "Açıklama girilmelidir!")]
        [MaxLength(60)]
        public string? Description { get; set; } = null!;
        public string? ManagerDescription { get; set; } = null!;


        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = false)]
        public DateTime? ApprovalDate { get; set; }


        // talep oluşturan kişiId
        public string? AppUserDTOId { get; set; } = null!;

        // talep oluşturan kişi
        public AppUserDTO? AppUserDTO { get; set; } = null!;
    }
}
