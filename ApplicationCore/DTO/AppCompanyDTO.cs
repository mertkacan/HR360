using ApplicationCore.AttributesValidatons;
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
    public class AppCompanyDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = ("Ad alanı en az {2} en fazla {1} karakter olabilir "))]
        public string? CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Şirket telefon numarası alanı zorunludur")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "Telefon numarası 11 haneden oluşur")]
        public string? CompanyPhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Şirket adresi alanı zorunludur")]
        public string? CompanyAddress { get; set; } = null!;
        public string? CompanyMail { get; set; } = null!;
        public int? PersonnelAmount => AppUserDTOs!.Count();
        public List<AppUserDTO>? AppUserDTOs { get; set; } = new();

        [Required(ErrorMessage = "Şirket ünvanı alanı zorunludur")]
        public CompanyStatus? CompanyStatus { get; set; }

        [MersisNoKontrol(ErrorMessage ="Geçersiz Mersis No girişi")]
        [RakamKontrol]
        [Required(ErrorMessage = "Mersis numarası alanı zorunludur")]
        public string? MersisNumarasi { get; set; }

        [Required(ErrorMessage = "Vergi numarası alanı zorunludur")]
        [VergiNoKontrol(ErrorMessage = "Vergi numarasını yanlış girdiniz")]
        [RakamKontrol]
        public string? VergiNumarasi { get; set; }

        [Required(ErrorMessage = "Vergi dairesi alanı zorunludur")]
        public VergiDairesi? VergiDairesi { get; set; }
        public string? Logo { get; set; }

        [ResimKontrol]
        public IFormFile? LogoFile { get; set; }

        [Required(ErrorMessage = "Şirket kuruluş tarihi alanı zorunludur")]
        [TarihKontrol]
        public DateTime? CompanyEstablishmentDate { get; set; }

        [Required(ErrorMessage = "Sözleşme başlangıç tarihi alanı zorunludur")]
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractFinishDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
