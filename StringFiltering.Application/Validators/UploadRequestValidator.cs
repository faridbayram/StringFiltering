using FluentValidation;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Dtos;

namespace StringFiltering.Application.Validators;

public class UploadRequestValidator : AbstractValidator<UploadRequestDto>
{
    public UploadRequestValidator()
    {
        RuleFor(x => x.UploadId)
            .NotEmpty().WithMessage(ValidationMessages.UploadIdRequired);

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage(ValidationMessages.DataRequired);

        RuleFor(x => x.ChunkIndex)
            .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.ChunkIndexMustBeNonNegative);
    }
}