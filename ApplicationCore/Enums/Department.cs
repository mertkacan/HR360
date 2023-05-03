using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Enums
{
    public enum Department
    {
        [Display(Name ="Mühendis")]
        Engineer = 1,
        IT,
    }
}
