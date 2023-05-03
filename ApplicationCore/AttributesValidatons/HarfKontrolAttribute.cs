using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.AttributesValidatons
{
    public class HarfKontrolAttribute : ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
            char[] harfler = new char[]
                 {
                    'A', 'a', 'B', 'b', 'C', 'c', 'Ç', 'ç', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g',
                    'Ğ', 'ğ', 'H', 'h', 'I', 'ı', 'İ', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm',
                    'N', 'n', 'O', 'o', 'Ö', 'ö', 'P', 'p', 'R', 'r', 'S', 's', 'Ş', 'ş', 'T', 't',
                    'U', 'u', 'Ü', 'ü', 'V', 'v', 'Y', 'y', 'Z', 'z', ' '
                 };

            if (value == null)
                return true;

            var metin = value!.ToString();

            var harfMi = metin!.All(x => harfler.Contains(x));

            if (!harfMi)
            {
                ErrorMessage = "Lütfen sadece harf giriniz";
                return false;
            }

            return true;
        }
    }
}
