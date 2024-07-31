using System.ComponentModel.DataAnnotations;

namespace Caro.ViewModels
{
    public class RoleFormViewModel
    {
        [Required,StringLength(256)]
        public string Name { get; set; }
    }
}
