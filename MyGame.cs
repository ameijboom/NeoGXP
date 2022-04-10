using System;					// System contains a lot of default C# libraries 
using GXPEngine;				// GXPEngine contains the engine

public class MyGame : Game
{
	// Declare the Sprite variables:
	Sprite spaceShip;
	AnimationSprite character;
	EasyDraw background;
	EasyDraw button;
	EasyDraw information;

	// Declare other variables:

	float turnSpeedShip = 3;
	float moveSpeedShip = 5;
	float characterSpeed = 3;

	SoundChannel soundTrack;

	public MyGame() : base(800, 600, false)		// Create a window that's 800x600 and NOT fullscreen
	{
		// Create a full screen canvas (EasyDraw):
		// (in MyGame, width and height refer to game.width and game.height: the window size)
		background = new EasyDraw(width, height);

		FillBackground();

		// Create a sprite from an image file (png, in bin/Debug):
		spaceShip = new Sprite("hellogxp/playerShip1_blue.png");
		// Set the (rotation) origin of the space ship to its center:
		// (spaceShip.width and spaceShip.height refer to the width and height of the sprite, in pixels)
		spaceShip.SetOrigin(spaceShip.width / 2, spaceShip.height / 2);

		// Create an animation sprite from a sprite sheet (png file, in bin/Debug) with four columns and two rows:
		character = new AnimationSprite("hellogxp/platformerPack_character.png", 4, 2);
		// Set the origin of the character sprite in the top middle:
		character.SetOrigin(character.width / 2, 0);

		SetStartPositions();

		// Create a small button canvas (EasyDraw):
		button = new EasyDraw(100, 30);
		// Clear the canvas with a grayscale value, semi-transparent:
		// (The values are red, green, blue, alpha, all between 0 and 255)
		button.Clear(100,100,100,128);
		// Write the text "RESET" on it (centered):
		button.TextAlign(CenterMode.Center, CenterMode.Center);
		button.Text("RESET");
		button.SetOrigin(button.width / 2, button.height / 2);
		// Set the position of the button to the bottom middle of the screen:
		button.SetXY(width / 2, height - button.height);

		// Create a larger canvas with some information:
		information = new EasyDraw(400, 100);
		// Set the text color to a blue-ish tint (red, green, blue values between 0 and 255):
		information.Fill(150, 150, 255);
		information.TextAlign(CenterMode.Min, CenterMode.Center);
		information.Text(
			"Use WASD to steer the space ship\n"+
			"and arrows to control the character\n" +
			"and sound track volume"
		);
		information.SetOrigin(0, information.height);
		information.SetXY(0, 0.75f*height);

		// Add all sprites to the engine, so that they will be displayed every frame:
		// (The order that we add them is the order that they will be drawn.)
		AddChild(background);

		AddChild(character);
		AddChild(spaceShip);

		AddChild(button);
		AddChild(information);

		// Play a sound track, looping and streaming, and keep a reference to it such that
		// we can change the volume:
		// (The .ogg file is in bin/Debug. ogg, mp3 and wav files are supported)
		soundTrack = new Sound("hellogxp/The_Endless_Journey.ogg",true,true).Play();

		// Print some information to the console (behind the game window):
		Console.WriteLine("Scene successfully initialized");
	}

	void FillBackground() {		
		for (int i=0; i<100; i++) {
			// Set the fill color of the canvas to a random color:
			background.Fill((byte)Utils.Random(100, 255), (byte)Utils.Random(100, 255), (byte)Utils.Random(100, 255));
			// Don't draw an outline for shapes:
			background.NoStroke();
			// Choose a random position and size:
			float px = Utils.Random(0, width);
			float py = Utils.Random(0, height);
			float size = Utils.Random(2, 5);
			// Draw a small circle shape on the canvas:
			background.Ellipse(px, py, size, size);
		}
	}

	void SetStartPositions() {
		// Set both sprites to the center of the screen (top/middle):
		character.SetXY(width / 2, 0);
		spaceShip.SetXY(width / 2, height / 2);		
		// Reset the rotation:
		spaceShip.rotation = 0;
	}

	void MoveSpaceShip() {
		// Check if the A and D keys are currently pressed.
		// If so, change the rotation in degrees of the space ship:
		if (Input.GetKey(Key.A)) {
			spaceShip.rotation += turnSpeedShip;
		}
		if (Input.GetKey(Key.D)) {
			spaceShip.rotation -= turnSpeedShip;
		}
		// Check if the W and S keys are currently pressed.
		// If so, move the space ship relative to its current orientation:
		if (Input.GetKey(Key.W)) {
			spaceShip.Move(0, moveSpeedShip); // move forward (=up)
		}
		if (Input.GetKey(Key.S)) {
			spaceShip.Move(0, -moveSpeedShip); // move backward (=down)
		}
	}

	void MoveCharacter() {
		bool moving = false;
		// Check if the left and right arrows are currently pressed.
		// If so, move the character to the left or right, and mirror the sprite accordingly:
		if (Input.GetKey(Key.LEFT)) {
			character.x -= characterSpeed;
			moving = true;
			character.Mirror(true, false);
		}
		if (Input.GetKey(Key.RIGHT)) {
			character.x += characterSpeed;
			moving = true;
			character.Mirror(false, false);
		}
		// Check whether the character and spaceShip sprites are currently overlapping:
		if (character.HitTest(spaceShip)) {
			// Set the animation cycle to start at frame 6, and have length 1 (=duck frame):
			character.SetCycle(6, 1);
		} else if (moving) {
			// Set the animation cycle to start at frame 2, and have length 2 (=walk animation):
			character.SetCycle(2, 2);
		} else {
			// Set the animation cycle to start at frame 0, and have length 1 (=idle frame):
			character.SetCycle(0, 1);
		}
		// animate the character within the current animation cycle.
		// 0.1 means that one animation frame will be shown for 10 game frames, so 
		// if the game frame rate is 60Hz, the animation will be 6 frames per second:
		character.Animate(0.1f);
	}

	void CheckButton() {
		// Input.mouseX/Y give the mouse position, in screen coordinates.
		// With HitTestPoint we test whether the point hits the button sprite:
		if (button.HitTestPoint((float)Input.mouseX, (float)Input.mouseY)) {
			// if so, we scale the button sprite up, and change its color to a red-like tint:
			button.scale = 1.2f;
			button.SetColor(1, 0.5f, 0.5f);

			// Check whether the left mouse button is pressed down this frame:
			if (Input.GetMouseButtonDown(0)) {
				// If so, reset the positions and play a sound:
				SetStartPositions();
				new Sound("hellogxp/transmit.wav").Play();

				Console.WriteLine("Positions reset");
			}

		} else {
			button.scale = 1;
			button.SetColor(1, 1, 1);
		}
	}

	void ChangeVolume() {
		// If the UP or DOWN keys are pressed, we change the sound track volume (between 0 and 1):
		if (Input.GetKey(Key.UP)) {
			soundTrack.Volume += 0.01f;
		}
		if (Input.GetKey(Key.DOWN)) {
			soundTrack.Volume -= 0.01f;
		}
		// Mathf contains various useful math functions.
		// Clamp makes sure that a value stays within a given range: 0-1 here.
		soundTrack.Volume = Mathf.Clamp(soundTrack.Volume, 0, 1);
	}

	// Update is called once per frame, by the engine, for each game object in the hierarchy
	// (including the Game itself)
	void Update()
	{
		MoveSpaceShip();
		MoveCharacter();
		CheckButton();
		ChangeVolume();
	}

	// Main is the first method that's called when the program is run
	static void Main()							
	{
		// Create a "MyGame" and start it:
		new MyGame().Start();				
	}
}