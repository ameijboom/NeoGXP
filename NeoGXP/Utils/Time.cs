using Arqan;

namespace NeoGXP.NeoGXP.Utils;

/// <summary>
/// Contains various time related functions.
/// </summary>
public static class Time
{
	private static int _previousTime;

	static Time() {
	}

	/// <summary>
	/// Returns the current system time in milliseconds
	/// </summary>
	public static int now => System.Environment.TickCount;

	/// <summary>
	/// Returns this time in milliseconds since the program started
	/// </summary>
	/// <value>
	/// The time.
	/// </value>
	public static int time => (int)(GLFW.glfwGetTime()*1000);

	/// <summary>
	/// Returns the time in milliseconds that has passed since the previous frame
	/// </summary>
	/// <value>
	/// The delta time.
	/// </value>
	public static int deltaMillis { get; private set; }

	/// <summary>
	/// The interval in seconds from the last frame to the current one<br/>
	/// https://docs.unity3d.com/ScriptReference/Time-deltaTime.html
	/// </summary>
	public static float deltaTime => deltaMillis / 1000f;

	internal static void NewFrame() {
		deltaMillis = time - _previousTime;
		_previousTime = time;
	}
}
