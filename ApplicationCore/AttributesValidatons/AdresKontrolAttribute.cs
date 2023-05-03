using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.AttributesValidatons
{
    public class AdresKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            char[] yasaklilar = new char[]
        {
                    '@','^', '+','~', '`' , '*'
        };

            if (value == null) 
                return true;

            var adres = value!.ToString();

            var yasakVarMi = adres!.Any(x => yasaklilar.Contains(x));

            if (yasakVarMi)
                return false;

            return true;
        }
    }
}
