using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.AttributesValidatons
{
    public class VergiNoKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true;

            var vergiNo = value.ToString();


            if (vergiNo!.Length != 10)
                return false;

            return true;
        }
    }
}