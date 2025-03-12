using CromulentBisgetti.ContainerPacking.Entities;

namespace CromulentBisgetti.ContainerPacking;

public struct BoxCoords
{
    public BoxCoords(Item item)
    {
        MinX = item.CoordX;
        MaxX = item.CoordX + item.PackDimX;
        MinZ = item.CoordZ;
        MaxZ = item.CoordZ + item.PackDimZ;
    }

    public bool OverlapsXZ(Item other)
    {
        return OverlapsX(new BoxCoords(other)) && OverlapsZ(new BoxCoords(other));
    }

    private bool OverlapsX(BoxCoords other)
    {
        return other.MinX >= MinX && other.MaxX <= MaxX ||
               other.MinX < MinX && other.MaxX < MaxX && other.MaxX > MinX ||
               other.MinX < MinX && other.MaxX > MinX ||
               other.MinX > MinX && other.MaxX > MaxX && other.MinX < MaxX ||
               other.MinX < MaxX && other.MaxX > MaxX ||
               other.MinX < MinX && other.MaxX > MaxX;
    }

    private bool OverlapsZ(BoxCoords other)
    {
        return other.MinZ >= MinZ && other.MaxZ <= MaxZ ||
               other.MinZ < MinZ && other.MaxZ < MaxZ && other.MaxZ > MinZ ||
               other.MinZ < MinZ && other.MaxZ > MinZ ||
               other.MinZ > MinZ && other.MaxZ > MaxZ && other.MinZ < MaxZ ||
               other.MinZ < MaxZ && other.MaxZ > MaxZ ||
               other.MinZ < MinZ && other.MaxZ > MaxZ;
    }

    private decimal MinX { get; }
    private decimal MaxX { get; }
    private decimal MinZ { get; }
    private decimal MaxZ { get; }
}