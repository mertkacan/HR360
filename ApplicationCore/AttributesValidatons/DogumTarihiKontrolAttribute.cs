using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.AttributesValidatons
{
    public class DogumTarihiKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            var dogumTarihi = (DateTime)value;

            var fark = DateTime.Now - dogumTarihi;

            if (fark.TotalDays < 6574)
            {
                ErrorMessage = "18 yaşından küçük çalışan kaydı yapılamaz";
                return false;
            }
            return true;
        }
    }
}