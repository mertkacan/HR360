using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.AttributesValidatons
{
    public class MersisNoKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true;
            var mersisNo = value!.ToString();

            if (mersisNo!.Length != 16)
                return false;

            return true;
        }
    }
}