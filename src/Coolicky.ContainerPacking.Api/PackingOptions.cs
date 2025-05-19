namespace Coolicky.ContainerPacking.Api;

public class PackingOptions
{
    /// <summary>
    /// Specifies the maximum allowed number of items that can be included in a packing operation.
    /// </summary>
    public int MaxItems { get; set; } = 1000;

    /// <summary>
    /// Defines the maximum allowable volume for a container.
    /// </summary>
    public decimal MaxContainerVolume { get; set; } = 100;

    /// <summary>
    /// Specifies the timeout duration, in seconds, for the packing operation to complete.
    /// If the operation exceeds this duration, it will be cancelled to prevent prolonged execution.
    /// </summary>
    public int TimeoutInSeconds { get; set; } = 30;
}