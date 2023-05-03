using ApplicationCore.DTO;

namespace Web.Models
{
    public class AdminViewModel
    {
        public AppUserDTO? AppUserDTO { get; set; } 
        public List<AppUserDTO> AppUsersDTOListe { get; set; } = new();
        public AppExpenseDTO? AppExpenseDTO { get; set; }
        public List<AppExpenseDTO> AppExpensesDTOListe { get; set; } = new();

        public AppCompanyDTO? AppCompanyDTO { get; set; }
        public Guid? AppCompanyDTOId { get; set; }
        public List<AppCompanyDTO> AppCompanyDTOListe { get; set; } = new();

    }
}
