using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApplicationCore.Enums
{
    public enum Job
    {
        [Display(Name = "Bilgisayar Mühendisi")]
        BilgisayarMuhendisi,

        [Display(Name = "Yazılım Mühendisi")]
        YazilimMuhendisi,

        [Display(Name = "Sistem Destek Uzmanı")]
        SistemDestekUzmani,

        [Display(Name = "Bulut Mühendisi")]
        BulutMuhendisi,

        [Display(Name = "Veri Tabanı Yöneticisi")]
        VeriTabaniYoneticisi,

        [Display(Name = "İş Zekası Uzmanı")]
        IsZekasiUzmani,

        [Display(Name = "Sistem Yöneticisi")]
        SistemYoneticisi,

        [Display(Name = "İş Analisti")]
        IsAnalisti,

        [Display(Name = "Proje Yöneticisi")]
        ProjeYoneticisi,

        [Display(Name = "Temizlik Personeli")]
        Temizlikci,

        [Display(Name = "Muhasebeci")]
        Muhasebeci,

        [Display(Name = "Sekreter")]
        Sekreter,

        [Display(Name = "Aşçı")]
        Asci,

        [Display(Name = "Güvenlik")]
        Guvenlik,

        [Display(Name = "Genel Müdür")]
        GenelMudur,

        [Display(Name = "Admin")]
        Admin


    }
}
