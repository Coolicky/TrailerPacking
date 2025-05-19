namespace Coolicky.ContainerPacking.Entities;

/// <summary>
/// Represents a container with defined dimensions and weight capacity.
/// Used in packing operations to store items and evaluate spatial and weight constraints.
/// </summary>
public record Container
{
	public Container(int id, decimal length, decimal width, decimal height, decimal maxWeight)
	{
		Id = id;
		Length = length;
		Width = width;
		Height = height;
		MaxWeight = maxWeight;
	}

	public int Id { get; set; }
	/// <summary>
	/// Length of the container in millimeters. (Default: 5870 | Standard: 20ft container)
	/// </summary>
	public decimal Length { get; set; } = 5870;
	/// <summary>
	/// Width of the container in millimeters. (Default: 2330 | Standard: 20ft container)
	/// </summary>
	public decimal Width { get; set; } = 2330;
	/// <summary>
	/// Height of the container in millimeters. (Default: 2350 | Standard: 20ft container)
	/// </summary>
	public decimal Height { get; set; } = 2350;
	public decimal Volume => Length * Width * Height;
	/// <summary>
	/// Maximum weight capacity of the container, measured in kilograms.
	/// Defines the total allowable weight of the items to be packed. (Default: 19300kg)
	/// </summary>
	public decimal MaxWeight { get; set; } = 19300;
}