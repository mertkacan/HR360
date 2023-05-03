using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.AttributesValidatons
{
    public class TarihKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true; // null değer geçerli kabul edilir
            }
            int year = 0;

            if (value is DateTime dateTimeValue)
                year = dateTimeValue.Year;

            if (year < 1900 || year > 2100)
            {
                ErrorMessage = "Geçersiz tarih aralğı";

                return false;
            }

            var date = value as DateTime?;

            var fark = date!.Value - DateTime.Now;

            if (fark.TotalDays > 90)
            {
                ErrorMessage = "Tarih en fazla 3 ay sonrası olabilir";

                return false;
            }

            return true;
        }
    }
}
