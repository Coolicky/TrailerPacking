using Coolicky.ContainerPacking.Api.Dtos;
using Coolicky.ContainerPacking.Algorithms;

namespace Coolicky.ContainerPacking.Api.Services;

public interface IPackingService
{
    PackingResultDto Pack(PackingRequestDto request);
}
public class PackingService : IPackingService
{
    private readonly IPackingAlgorithm _algorithm;

    public PackingService(
        IPackingAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    public PackingResultDto Pack(PackingRequestDto request)
    {
        var result = new PackingResultDto();
        var itemsLeft = request.Items;

        while (itemsLeft.Count > 0)
        {
            var packingResult = _algorithm.Run(request.Container, request.Items);
            if (packingResult.PackedItems.Count > 0)
                break;

            result.Containers.Add(new ContainerPackDto(packingResult));
            itemsLeft = packingResult.UnpackedItems;
        }
        return result;
    }
}