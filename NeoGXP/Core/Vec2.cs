namespace NeoGXP.NeoGXP.Core;

public struct Vec2
{
	// ReSharper disable InconsistentNaming
	public float x;
	public float y;
	// ReSharper restore InconsistentNaming

	public Vec2 (float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public override string ToString() {
		return "[Vector2 " + x + ", " + y + "]";
	}
}
