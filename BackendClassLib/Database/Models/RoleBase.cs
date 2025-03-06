using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.Models;

public abstract class RoleBase : BaseV2
{
    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = null!;
}
