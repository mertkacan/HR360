using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Enums
{
    public enum CompanyStatus
    {
        [Display(Name = "Ltd.")]
        Ltd,

        [Display(Name = "AŞ.")]
        As,

        [Display(Name = "Diğer")]
        Diger
    }
}