using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class User
{
    [Required]
    public string DisplayName { get; set; } = null!;
}
