using FluentValidation;

namespace events.backend.Application.EventsAggregates.CreateEvent;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("EventDate is required.");

        RuleFor(x => x.Zones)
            .NotNull().WithMessage("Zones is required.")
            .Must(z => z.Count > 0).WithMessage("At least one zone is required.");

        RuleForEach(x => x.Zones).ChildRules(z =>
        {
            z.RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Zone name is required.")
                .MaximumLength(100).WithMessage("Zone name must not exceed 100 characters.");

            z.RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Zone price must be >= 0.");

            z.RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Zone capacity must be > 0.");
        });
    }
}
