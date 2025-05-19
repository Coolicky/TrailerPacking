namespace Coolicky.ContainerPacking.Api.Dtos;

public record PackingResultDto
{
    public List<ContainerPackDto> Containers { get; set; } = [];
}