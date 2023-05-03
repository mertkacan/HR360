using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Interfaces
{
    public interface IMailService
    {
        void SendEmailAsync(ApplicationUser user, string password);
    }
}
