using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApplicationCore.Enums
{
    public enum Approval
    {
        [Display(Name = "Onaylandı")]
        Onaylandi,

        [Display(Name = "Onay Bekliyor")]
        OnayBekliyor,

        [Display(Name = "Reddedildi")]
        Reddedildi,

        [Display(Name = "İptal edildi")]
        IptalEdildi
    }
}
