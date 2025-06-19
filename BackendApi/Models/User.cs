using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string DisplayName { get; set; } = null!;
}
