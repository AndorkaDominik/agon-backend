namespace Transfer.Dtos;

public class AuthResponseDto
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string UserType { get; set; }
    public string? ThemeColor { get; set; }
}
