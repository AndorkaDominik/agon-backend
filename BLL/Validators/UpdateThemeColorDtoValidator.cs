using FluentValidation;
using Transfer.Dtos;

namespace BLL.Validators;

public class UpdateThemeColorDtoValidator : AbstractValidator<UpdateThemeColorDto>
{
    public UpdateThemeColorDtoValidator()
    {
        RuleFor(x => x.ThemeColor)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Theme color is required.")
            .Must(color => color is "grass" or "pink")
            .WithMessage("Theme color must be grass or pink.");
    }
}
