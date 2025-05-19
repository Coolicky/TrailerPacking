using System;

namespace Coolicky.TrailerPacking.Entities;

/// <summary>
/// Represents an item to be packed, containing its dimensions, weight, stackability, and other attributes
/// that are relevant for packing operations.
/// </summary>
public record Item
{
	public Item(Guid id, decimal length, decimal height, decimal depth, int quantity, bool isStackable, decimal weight, int? maxStack)
	{
		Id = id;
		Length = length;
		Height = height;
		Depth = depth;
		Volume = length * height * depth;
		Quantity = quantity;
		IsStackable = isStackable;
		Weight = weight;
		MaxStack = maxStack > 0 ? maxStack : null;
	}

	public Guid Id { get; set; }
	public bool IsPacked { get; set; }

	/// <summary>
	/// First of the item dimensions. Generally the Width.
	/// </summary>
	public decimal Length { get; set; }

	/// <summary>
	/// Second of the item dimensions. Generally the Height.
	/// </summary>
	public decimal Height { get; set; }

	/// <summary>
	/// Third of the item dimensions. Generally the Depth.
	/// </summary>
	public decimal Depth { get; set; }

	/// <summary>
	/// The x coordinate of the location of the packed item within the container.
	/// </summary>
	public decimal CoordX { get; set; }

	/// <summary>
	/// The y (Up) coordinate of the location of the packed item within the container.
	/// </summary>
	public decimal CoordY { get; set; }

	/// <summary>
	/// The z coordinate of the location of the packed item within the container.
	/// </summary>
	public decimal CoordZ { get; set; }

	public int Quantity { get; set; }

	/// <summary>
	/// The x dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimX { get; set; }

	/// <summary>
	/// The y (Up) dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimY { get; set; }

	/// <summary>
	/// The z dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimZ { get; set; }
	public decimal Volume { get; }
	public bool IsStackable { get; set; }
	public decimal Weight { get; set; }
	public int? MaxStack { get; set; }
	public bool? IsCombined { get; set; }
	public Guid? CombinedItemId { get; set; }
}