    using ApplicationCore.AttributesValidatons;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Validation;
using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   

namespace ApplicationCore.DTO
{
    public class AppUserDTO : IdentityUser
    {
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [HarfKontrol]
        [StringLength(30, MinimumLength = 2, ErrorMessage =("Ad alanı en az {2} en fazla {1} karakter olabilir "))]
        public string Name { get; set; } = null!;


        [HarfKontrol]
        [StringLength(30, MinimumLength = 2, ErrorMessage =("İkinci ad alanı en az {2} en fazla {1} karakter olabilir "))]
        public string? MiddleName { get; set; }


        [Required(ErrorMessage = "Soyadı alanı zorunludur")]
        [HarfKontrol]
        [StringLength(30, MinimumLength = 2, ErrorMessage =("Soyad alanı en az {2} en fazla {1} karakter olabilir "))]
        public string Surname { get; set; } = null!;


        [HarfKontrol]
        [StringLength(30, MinimumLength = 2, ErrorMessage =("İkinci soyad alanı en az {2} en fazla {1} karakter olabilir "))]
        public string? SecondSurname { get; set; }


        [Required(ErrorMessage = "Doğum tarihi alanı zorunludur")]
        [TarihKontrol]
        [DogumTarihiKontrol]
        public DateTime? BirthDate { get; set; } 


        [Required(ErrorMessage = "Doğum yeri alanı zorunludur"), HarfKontrol]
        [StringLength(30, MinimumLength = 2, ErrorMessage =("Doğumyeri alanı en az {2} en fazla {1} karakter olabilir "))]
        public string PlaceOfBirth { get; set; } = null!;


        [Required(ErrorMessage = "Tc alanı zorunludur"), RakamKontrol, TcKontrol]
        public string IdentityNumber { get; set; } = null!;


        [Required(ErrorMessage = "İşe başlama tarihi alanı zorunludur"), TarihKontrol]
        public DateTime? HireDate { get; set; }


        [TarihKontrol]
        public DateTime? ReleaseDate { get; set; }


        [Required(ErrorMessage = "Meslek alanı zorunludur")]
        public Job? Job { get; set; } 


        [StringLength(255, MinimumLength = 10, ErrorMessage = "Adres en az {2} en fazla {1} karakter olabilir")]
        [Required(ErrorMessage = "Adres alanı zorunludur")]
        [AdresKontrol(ErrorMessage = "Geçersiz adres formatı")]
        public string Address { get; set; } = null!;


        [Required(ErrorMessage = "Maaş alanı zorunludur")]
        [Range(typeof(decimal), "8500", "200000", ErrorMessage = "Maaş aralığı 8500 ile 200.000 arasında olmalıdır")]
        public decimal? Salary { get; set; }

        [ResimKontrol]
        public IFormFile? Resim { get; set; }

        public string? Picutre { get; set; }

        public AppCompanyDTO? CompanyDTO { get; set; }

        public Guid? CompanyDTOId { get; set; }

        [Required(ErrorMessage = "Departman alanı zorunludur")]
        public Department? Department { get; set; }

        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "Cinsiyet alanı zorunludur")]
        public Gender? Gender { get; set; }


        [Required(ErrorMessage = "Telefon numarası alanı zorunludur")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "Telefon numarası 11 haneden oluşur")]
        public string PhoneNumber { get; set; } 

        public List<AppExpenseDTO>? AppExpenseDTOList { get; set; } = new();
        public List<AppAdvanceDTO>? AppAdvanceDTOList { get; set; } = new();
        public bool IsUpdate { get; set; }
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
