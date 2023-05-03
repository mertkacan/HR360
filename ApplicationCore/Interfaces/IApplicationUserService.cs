using ApplicationCore.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Interfaces
{
    public interface IApplicationUserService
    {
        Task UpdateApplicationUserAsync(AppUserDTO appUserDTO);
        Task CreatePersonelAsync(AppUserDTO appUserDTO);
        Task CreateYoneticiAsync(AppUserDTO appUserDTO);
        Task<List<ApplicationUser>> GetAllPersonelAsync();
        Task<List<ApplicationUser>> GetAllYoneticiAsync();

    }
}
