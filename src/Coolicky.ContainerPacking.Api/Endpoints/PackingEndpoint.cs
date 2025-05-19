using Coolicky.ContainerPacking.Api.Dtos;
using Coolicky.ContainerPacking.Api.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// ReSharper disable PossiblyMistakenUseOfCancellationToken
namespace Coolicky.ContainerPacking.Api.Endpoints;

[FastEndpoints.HttpPost("pack")]
[ProducesResponseType(typeof(PackingResultDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class PackingEndpoint : Endpoint<PackingRequestDto, PackingResultDto>
{
    private readonly PackingOptions _options;
    private readonly IPackingService _packingService;

    public PackingEndpoint(
        IOptions<PackingOptions> options,
        IPackingService packingService)
    {
        _options = options.Value;
        _packingService = packingService;
    }

    public override async Task HandleAsync(PackingRequestDto req, CancellationToken ct)
    {
        var timeout = TimeSpan.FromSeconds(_options.TimeoutInSeconds);
        var cts = new CancellationTokenSource(timeout);
        var task = Task.Run(() => _packingService.Pack(req), cts.Token);

        if (task.Wait(timeout, ct))
        {
            var result = task.Result;
            await SendAsync(result, cancellation: ct);
        }
        else
        {
            throw new TimeoutException("Packing operation timed out.");
        }
    }
}