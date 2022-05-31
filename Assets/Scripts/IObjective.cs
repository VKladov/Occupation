using UnityEngine;

public abstract class Objective
{
	public abstract Vector3 Position { get; }
}

public class BuildingObjective : Objective
{
	public override Vector3 Position => Target.Enter;

	public readonly Building Target;

	public BuildingObjective(Building target)
	{
		Target = target;
	}
}

public class PositionObjective : Objective
{
	public override Vector3 Position { get; }

	public PositionObjective(Vector3 position)
	{
		Position = position;
	}
}