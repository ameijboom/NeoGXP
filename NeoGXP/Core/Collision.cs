namespace NeoGXP.NeoGXP.Core;

/// <summary>
/// A class that contains info about collisions, such as returned by the MoveUntilCollision method.
/// </summary>
public class Collision
{
	public GameObject Self, Other;
	public Vec2 Normal;
	public Vec2 Point;
	public float TimeOfImpact;
	public float PenetrationDepth;

	public Collision(GameObject self, GameObject other, Vec2 normal, Vec2 point, float timeOfImpact, float penetrationDepth)
	{
		Self = self;
		Other = other;
		Normal = normal;
		Point = point;
		TimeOfImpact = timeOfImpact;
		PenetrationDepth = penetrationDepth;
	}

	public Collision(GameObject self, GameObject other, Vec2 normal, float timeOfImpact) :
		this (self, other, normal, new Vec2 (0, 0), timeOfImpact, 0)
	{
	}

	public Collision(GameObject self, GameObject other, Vec2 normal, Vec2 point, float penetrationDepth) :
		this (self, other, normal, point, 0, penetrationDepth)
	{
	}
}
