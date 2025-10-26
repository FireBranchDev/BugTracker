using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Bug
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}
