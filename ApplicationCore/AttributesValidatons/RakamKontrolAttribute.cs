using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.AttributesValidatons
{
    public class RakamKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) 
                return true;

            var metin = value!.ToString();

            var rakamMi = metin!.All(x => char.IsDigit(x));

            if (!rakamMi)
            {
                ErrorMessage = "Lütfen sadece rakam giriniz";
                return false;
            }

            return true;
        }
    }
}
