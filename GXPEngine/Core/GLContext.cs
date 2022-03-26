//#define USE_FMOD_AUDIO
#define STRETCH_ON_RESIZE

using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace GXPEngine.Core {

	class WindowSize {
		public static WindowSize instance = new WindowSize();
		public int width, height;
	}
	
	public class GLContext {
		
		const int MAXKEYS = 65535;
		const int MAXBUTTONS = 255;

		private static bool[] keys = new bool[MAXKEYS+1];
		private static bool[] keydown = new bool[MAXKEYS+1];
		private static bool[] keyup = new bool[MAXKEYS+1];
		private static bool[] buttons = new bool[MAXBUTTONS+1];
		private static bool[] mousehits = new bool[MAXBUTTONS+1];
		private static bool[] mouseup = new bool[MAXBUTTONS+1]; //mouseup kindly donated by LeonB
		private static int keyPressedCount = 0;
		private static bool anyKeyDown = false;

		public static double mouseX = 0;
		public static double mouseY = 0;
		
		private Game _owner;
        private static SoundSystem _soundSystem;
		
		private int _targetFrameRate = 60;
		private long _lastFrameTime = 0;
		private long _lastFPSTime = 0;
		private int _frameCount = 0;
		private int _lastFPS = 0;
		private bool _vsyncEnabled = false;

		private static double _realToLogicWidthRatio;
		private static double _realToLogicHeightRatio;

		public static Window CurrentWindow;
		//------------------------------------------------------------------------------------------------------------------------
		//														RenderWindow()
		//------------------------------------------------------------------------------------------------------------------------
		public GLContext (Game owner) {
			_owner = owner;
			_lastFPS = _targetFrameRate;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Width
		//------------------------------------------------------------------------------------------------------------------------
		public int width {
			get { return WindowSize.instance.width; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Height
		//------------------------------------------------------------------------------------------------------------------------
		public int height {
			get { return WindowSize.instance.height; }
		}

        //------------------------------------------------------------------------------------------------------------------------
        //														SoundSystem
        //------------------------------------------------------------------------------------------------------------------------
        public static SoundSystem soundSystem
        {
            get
            {
				if (_soundSystem == null) {
					InitializeSoundSystem ();
				}
                return _soundSystem;
            }
        }
		
		//------------------------------------------------------------------------------------------------------------------------
		//														setupWindow()
		//------------------------------------------------------------------------------------------------------------------------
		public void CreateWindow(int width, int height, bool fullScreen, bool vSync, int realWidth, int realHeight) {
			// This stores the "logical" width, used by all the game logic:
			WindowSize.instance.width = width;
			WindowSize.instance.height = height;
			_realToLogicWidthRatio = (double)realWidth / width;
			_realToLogicHeightRatio = (double)realHeight / height;
			_vsyncEnabled = vSync;

			GLFW.Init();

			// GLFW.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
			GLFW.WindowHint(WindowHintInt.Samples, 8);
			GLFW.CreateWindow(realWidth, realHeight, "Game", (fullScreen ? GLFW.GetPrimaryMonitor() : null), null);

			GLFW.SwapInterval(vSync ? 1 : 0);
			
			GLFW.SetKeyCallback(Window,
				(System.IntPtr _window, GLFW.Keys _key, int _scanCode, GLFW.InputState _action, GLFW.ModifierKeys _mods) => {
					bool press = (_action == GLFW.InputState.Press);

					if (press) 
					{ 
						keydown[((int)_key)] = true; 
						anyKeyDown = true; 
						keyPressedCount++; 
					} else 
					{ 
						keyup[((int)_key)] = true; 
						keyPressedCount--; 
					}

					keys[((int)_key)] = press;
				});
			
			Glfw.SetMouseButtonCallback(Window,
				(System.IntPtr _window, GLFW.MouseButton _button, GLFW.InputState _action, GLFW.ModifierKeys _mods) => {
				bool press = (_action == GLFW.InputState.Press);

				if (press)
					mousehits[((int)_button)] = true;
				else
					mouseup[((int)_button)] = true;

				buttons[((int)_button)] = press;
			});

			Glfw.SetWindowSizeCallback(Window, (System.IntPtr _window, int newWidth, int newHeight) => {
				GL.Viewport(0, 0, newWidth, newHeight);	
				GL.Enable(EnableCap.Multisample);	
				GL.Enable (EnableCap.Texture2D);
				GL.Enable( EnableCap.Blend );
				GL.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
				GL.Hint (HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);
				//Glfw.Enable (Glfw.POLYGON_SMOOTH);
				GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

				// Load the basic projection settings:
				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();

#if STRETCH_ON_RESIZE
				_realToLogicWidthRatio = (double)newWidth / WindowSize.instance.width;
				_realToLogicHeightRatio = (double)newHeight / WindowSize.instance.height;
#endif
				// Here's where the conversion from logical width/height to real width/height happens: 
				GL.Ortho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);
#if !STRETCH_ON_RESIZE
				lock (WindowSize.instance) {
					WindowSize.instance.width = (int)(newWidth/_realToLogicWidthRatio);
					WindowSize.instance.height = (int)(newHeight/_realToLogicHeightRatio);
				}
#endif

				if (Game.main!=null) {
					Game.main.RenderRange=new Rectangle(0,0,WindowSize.instance.width,WindowSize.instance.height);
				}
			});
			InitializeSoundSystem ();
		}

		private static void InitializeSoundSystem() {
#if USE_FMOD_AUDIO
			_soundSystem = new FMODSoundSystem();
#else
			_soundSystem = new SoloudSoundSystem();
#endif
			_soundSystem.Init();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														ShowCursor()
		//------------------------------------------------------------------------------------------------------------------------
		public void ShowCursor (bool enable)
		{
			if (enable) {
				Glfw.SetInputMode(Window, InputMode.Cursor, 1);
			} else {
				Glfw.SetInputMode(Window, InputMode.Cursor, 0);
			}
		}

		public void SetVSync(bool enableVSync) {
			_vsyncEnabled = enableVSync;
			Glfw.SwapInterval(_vsyncEnabled ? 1 : 0);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetScissor()
		//------------------------------------------------------------------------------------------------------------------------
		public void SetScissor(int x, int y, int width, int height) {
			if ((width == WindowSize.instance.width) && (height == WindowSize.instance.height)) {
				GL.Disable(EnableCap.ScissorTest);
			} else {
				GL.Enable(EnableCap.ScissorTest);
			}

			GL.Scissor(
				(int)(x*_realToLogicWidthRatio), 
				(int)(y*_realToLogicHeightRatio), 
				(int)(width*_realToLogicWidthRatio), 
				(int)(height*_realToLogicHeightRatio)
			);
			//Glfw.Scissor(x, y, width, height);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Close()
		//------------------------------------------------------------------------------------------------------------------------
		public void Close() {
            _soundSystem.Deinit();
            Glfw.DestroyWindow(Window);
			Glfw.Terminate();
			System.Environment.Exit(0);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Run()
		//------------------------------------------------------------------------------------------------------------------------
		public void Run() {
            Glfw.Time = 0.0;
			do {
				if (_vsyncEnabled || (Time.time - _lastFrameTime > (1000 / _targetFrameRate))) {
					_lastFrameTime = Time.time;
					
					//actual fps count tracker
					_frameCount++;
					if (Time.time - _lastFPSTime > 1000) {
						_lastFPS = (int)(_frameCount / ((Time.time -_lastFPSTime) / 1000.0f));
						_lastFPSTime = Time.time;
						_frameCount = 0;
					}
					
					UpdateMouseInput();
					_owner.Step();
                    _soundSystem.Step();
					
					ResetHitCounters();
					Display();
					
					Time.newFrame ();
					Glfw.PollEvents();
				}
				
				
			} while (Glfw.GetWindowAttribute(Window, WindowAttribute.Focused));
		}
		
		
		//------------------------------------------------------------------------------------------------------------------------
		//														display()
		//------------------------------------------------------------------------------------------------------------------------
		private void Display () {
			GL.Clear(ClearBufferMask.ColorBufferBit);
			
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			
			_owner.Render(this);

			Glfw.SwapBuffers(Window);
			if (GetKey(Key.ESCAPE)) this.Close();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetColor()
		//------------------------------------------------------------------------------------------------------------------------
		public void SetColor (byte r, byte g, byte b, byte a) {
			GL.Color4(r, g, b, a);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														PushMatrix()
		//------------------------------------------------------------------------------------------------------------------------
		public void PushMatrix(float[] matrix) {
			GL.PushMatrix();
			GL.MultMatrix(matrix);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														PopMatrix()
		//------------------------------------------------------------------------------------------------------------------------
		public void PopMatrix() {
			GL.PopMatrix();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														DrawQuad()
		//------------------------------------------------------------------------------------------------------------------------
		public void DrawQuad(float[] vertices, float[] uv) {
			GL.EnableClientState( ArrayCap.TextureCoordArray );
			GL.EnableClientState( ArrayCap.VertexArray );
			GL.TexCoordPointer( 2, TexCoordPointerType.Float, 0, uv);
			GL.VertexPointer( 2, VertexPointerType.Float, 0, vertices);
			GL.DrawArrays(BeginMode.Quads, 0, 4);
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.TextureCoordArray);			
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetKey()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetKey(int key) {
			return keys[key];
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetKeyDown()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetKeyDown(int key) {
			return keydown[key];
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetKeyUp()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetKeyUp(int key) {
			return keyup[key];
		}
		
		public static bool AnyKey() {
			return keyPressedCount > 0;
		}

		public static bool AnyKeyDown() {
			return anyKeyDown;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetMouseButton()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetMouseButton(int button) {
			return buttons[button];
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetMouseButtonDown()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetMouseButtonDown(int button) {
			return mousehits[button];
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetMouseButtonUp()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetMouseButtonUp(int button) {
			return mouseup[button];
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														ResetHitCounters()
		//------------------------------------------------------------------------------------------------------------------------
		public static void ResetHitCounters() {
			Array.Clear (keydown, 0, MAXKEYS);
			Array.Clear (keyup, 0, MAXKEYS);
			Array.Clear (mousehits, 0, MAXBUTTONS);
			Array.Clear (mouseup, 0, MAXBUTTONS);
			anyKeyDown = false;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														UpdateMouseInput()
		//------------------------------------------------------------------------------------------------------------------------
		public static void UpdateMouseInput() {
			Glfw.GetCursorPosition(Window, out mouseX, out mouseY);
			mouseX = (int)(mouseX / _realToLogicWidthRatio);
			mouseY = (int)(mouseY / _realToLogicHeightRatio);
		}
		
		public int currentFps {
			get { return _lastFPS; }
		}
		
		public int targetFps {
			get { return _targetFrameRate; }
			set {
				if (value < 1) {
					_targetFrameRate = 1;
				} else {
					_targetFrameRate = value;
				}
			}
		}
		
	}	
	
}