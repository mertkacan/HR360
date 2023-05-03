using ApplicationCore.DTO;

namespace Web.Models
{
    public class YoneticiViewModel
    {
        public AppUserDTO AppUserDTO { get; set; } = null!;
        public List<AppUserDTO> AppUsersDTOListe { get; set; } = new();
        public AppExpenseDTO? AppExpenseDTO { get; set; } 
        public List<AppExpenseDTO> AppExpensesDTOListe { get; set; } = new();

        public AppAdvanceDTO? AppAdvanceDTO { get; set; }
        public List<AppAdvanceDTO>? AppAdvanceDTOListe { get; set; } = new();
    }
}
