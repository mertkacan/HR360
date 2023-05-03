using ApplicationCore.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationCore.Validation
{
    public class TcKontrolAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("Tc alanı zorunludur");

            string? tc = value!.ToString();


            if (tc!.Length != 11) { return new ValidationResult("TC hane sayısı 11 olmalı"); }

            var list = tc.Select(x => (int)Char.GetNumericValue(x)).ToList(); // string ifadeyi önce char'a ayırıp int'e çevirip listeye attık

            if (list[0] == 0) { return new ValidationResult("TC numarası 0 ile başlayamaz"); }

            // çift indexlerin (0'dan başladğı için) son elemanı hariç toplamları
            var ciftler = list.Where((item, index) => index % 2 == 0)
                .Take(5).Sum();

            // tek indexlerin son elemanı hariç toplamları
            var tekler = list.Where((item, index) => index % 2 == 1)
                .Take(4).Sum();

            if (((7 * ciftler) - tekler) % 10 != list[list.Count - 2]) { return new ValidationResult("Yanlış tc denemesi"); }

            var toplam = list.Sum() - list[list.Count - 1];

            if (toplam % 10 != list[list.Count - 1]) { return new ValidationResult("Yanlış tc denemesi"); }

            return ValidationResult.Success;
        }

    }
}
