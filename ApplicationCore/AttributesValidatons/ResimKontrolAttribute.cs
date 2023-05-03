using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.AttributesValidatons
{
    public class ResimKontrolAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            IFormFile? formFile = value as IFormFile;

            if (formFile == null)
                return true;

            string fileExtension = Path.GetExtension(formFile.FileName);

            if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg")
            {
                ErrorMessage = "Geçersiz resim formatı";
                return false;
            }

            return true; 
        }
    }
}
