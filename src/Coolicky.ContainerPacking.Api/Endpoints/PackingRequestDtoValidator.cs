using Coolicky.ContainerPacking.Api.Dtos;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Coolicky.ContainerPacking.Api.Endpoints;

public class PackingRequestDtoValidator : Validator<PackingRequestDto>
{
    public PackingRequestDtoValidator(IOptions<PackingOptions> options)
    {
        RuleFor(r => r.Container)
            .NotNull()
            .WithMessage("Container is required.")
            .Must(c => c.MaxWeight > 0)
            .WithMessage("Container max volume must be greater than 0.")
            .Must(c => c.Volume > options.Value.MaxContainerVolume)
            .WithMessage($"Container volume cannot exceed {options.Value.MaxContainerVolume}.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Items are required.")
            .Must(items => items.Select(r => r.Quantity).Sum() <= options.Value.MaxItems)
            .WithMessage($"Items count must be less than or equal to {options.Value.MaxItems}.");
    }
}
