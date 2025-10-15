using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.Models;

public abstract class RoleBase : Base
{
    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = null!;
}
