using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Enums
{
    public enum TypeOfExpenses
    {

        Yol,
        Yemek,
        Konaklama,

        [Display(Name ="Ulaşım")]
        Ulasim,

        [Display(Name = "Eğitim")]
        Egitim,

        Temizlik,

        [Display(Name = "Kırtasiye")]
        Kirtasiye,
        Otopark,

        [Display(Name = "Yakıt")]
        Yakit
    }
}
