namespace CromulentBisgetti.ContainerPacking.Entities;

public record Container
{
	public Container(int id, decimal length, decimal width, decimal height)
	{
		ID = id;
		Length = length;
		Width = width;
		Height = height;
	}

	public int ID { get; set; }
	public decimal Length { get; set; }
	public decimal Width { get; set; }
	public decimal Height { get; set; }
	public decimal Volume => Length * Width * Height;
}