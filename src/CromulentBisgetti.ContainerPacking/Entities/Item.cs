using System;

namespace CromulentBisgetti.ContainerPacking.Entities;

public record Item
{
	public Item(Guid id, decimal length, decimal height, decimal depth, int quantity, bool isStackable)
	{
		ID = id;
		Length = length;
		Height = height;
		Depth = depth;
		Volume = length * height * depth;
		Quantity = quantity;
		IsStackable = isStackable;
	}

	public Guid ID { get; set; }
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
	/// The y coordinate of the location of the packed item within the container.
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
	/// The y dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimY { get; set; }

	/// <summary>
	/// The z dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimZ { get; set; }
	public decimal Volume { get; }
	public bool IsStackable { get; set; } = false;
	public bool CanBePlacedOnSide { get; set; } = false;
	public bool CanBeStackedOnTop { get; set; } = true;
}
public class RequestItem
{
	public RequestItem(int id, decimal dim1, decimal dim2, decimal dim3, int quantity, bool isStackable)
	{
		ID = id;
		Dim1 = dim1;
		Dim2 = dim2;
		Dim3 = dim3;
		Volume = dim1 * dim2 * dim3;
		Quantity = quantity;
		IsStackable = isStackable;
	}

	public int ID { get; set; }
	public bool IsPacked { get; set; }

	/// <summary>
	/// First of the item dimensions. Generally the Width.
	/// </summary>
	public decimal Dim1 { get; set; }

	/// <summary>
	/// Second of the item dimensions. Generally the Height.
	/// </summary>
	public decimal Dim2 { get; set; }

	/// <summary>
	/// Third of the item dimensions. Generally the Depth.
	/// </summary>
	public decimal Dim3 { get; set; }

	/// <summary>
	/// The x coordinate of the location of the packed item within the container.
	/// </summary>
	public decimal CoordX { get; set; }

	/// <summary>
	/// The y coordinate of the location of the packed item within the container.
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
	/// The y dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimY { get; set; }

	/// <summary>
	/// The z dimension of the orientation of the item as it has been packed.
	/// </summary>
	public decimal PackDimZ { get; set; }
	public decimal Volume { get; }
	public bool IsStackable { get; set; } = false;
	public bool CanBePlacedOnSide { get; set; } = false;
	public bool CanBeStackedOnTop { get; set; } = true;
}