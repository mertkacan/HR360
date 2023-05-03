using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Enums
{
    public enum VergiDairesi
    {
        Sincan,

        Ulus,

        [Display(Name ="Keçiören")]
        Kecioren,

        [Display(Name ="Çankaya")]
        Cankaya,

        Mamak,

        [Display(Name ="Beylikdüzü")]
        Beylikduzu,

        Taksim,

        Tarabya,

        Sultanahmet,

        [Display(Name = "İnegöl")]
        Inegol,

        [Display(Name = "Karaköprü")]
        Karakopru
    }
}
