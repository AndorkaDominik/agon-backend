namespace Transfer.Dtos;

public class RegisterRequestDto
{
    public string UserType { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirm { get; set; } = string.Empty;

    // Trainer
    public string? Specialization { get; set; }
    public string? Introduction { get; set; }
}