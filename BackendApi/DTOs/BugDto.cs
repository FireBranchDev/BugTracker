﻿namespace BackendApi.DTOs;

public class BugDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}
