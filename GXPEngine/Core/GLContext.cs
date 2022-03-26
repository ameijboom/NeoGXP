//#define USE_FMOD_AUDIO
#define STRETCH_ON_RESIZE

using System;
// using OpenTK.Graphics.OpenGL;
// using OpenTK.Windowing.GraphicsLibraryFramework;
// using OpenTK.Windowing.Desktop;

using Arqan;
using System.Text;
using System.Runtime.InteropServices;
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

		public static IntPtr Window;
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

			GLFW.glfwInit();

			// GLFW.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
			GLFW.glfwWindowHint(GLFW.GLFW_SAMPLES, 8);
			Window = GLFW.glfwCreateWindow(realWidth, realHeight, Encoding.ASCII.GetBytes("Game"), (fullScreen ? GLFW.glfwGetPrimaryMonitor() : (IntPtr)null), (IntPtr)null);

			GLFW.glfwMakeContextCurrent(Window);

			GLFW.glfwSwapInterval(vSync ? 1 : 0);
			
			GLFW.glfwSetKeyCallback(Window,
				(System.IntPtr _window,  int _key, int _scanCode, int _action, int _mods) => {
					bool press = (_action == GLFW.GLFW_PRESS);

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
			
			GLFW.glfwSetMouseButtonCallback(Window,
				(System.IntPtr _window, int _button, int _action, int _mods) => {
				bool press = (_action == GLFW.GLFW_PRESS);

				if (press)
					mousehits[((int)_button)] = true;
				else
					mouseup[((int)_button)] = true;

				buttons[((int)_button)] = press;
			});

			GLFW.glfwSetWindowSizeCallback(Window, (System.IntPtr _window, int newWidth, int newHeight) => {
				GL.glViewport(0, 0, newWidth, newHeight);	
				GL.glEnable(GL.GL_MULTISAMPLE);	
				GL.glEnable (GL.GL_TEXTURE_2D);
				GL.glEnable( GL.GL_BLEND );
				GL.glBlendFunc( GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA );
				GL.glHint (GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_FASTEST);
				//Glfw.Enable (Glfw.POLYGON_SMOOTH);
				GL.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);

				// Load the basic projection settings:
				GL.glMatrixMode(GL.GL_PROJECTION);
				GL.glLoadIdentity();

#if STRETCH_ON_RESIZE
				_realToLogicWidthRatio = (double)newWidth / WindowSize.instance.width;
				_realToLogicHeightRatio = (double)newHeight / WindowSize.instance.height;
#endif
				// Here's where the conversion from logical width/height to real width/height happens: 
				GL.glOrtho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);
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
				GLFW.glfwSetInputMode(Window, GLFW.GLFW_CURSOR, 1);
			} else {
				GLFW.glfwSetInputMode(Window, GLFW.GLFW_CURSOR, 0);
			}
		}

		public void SetVSync(bool enableVSync) {
			_vsyncEnabled = enableVSync;
			GLFW.glfwSwapInterval(_vsyncEnabled ? 1 : 0);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetScissor()
		//------------------------------------------------------------------------------------------------------------------------
		public void SetScissor(int x, int y, int width, int height) {
			if ((width == WindowSize.instance.width) && (height == WindowSize.instance.height)) {
				GL.glDisable(GL.GL_SCISSOR_TEST);
			} else {
				GL.glEnable(GL.GL_SCISSOR_TEST);
			}

			GL.glScissor(
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
            GLFW.glfwDestroyWindow(Window);
			GLFW.glfwTerminate();
			System.Environment.Exit(0);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Run()
		//------------------------------------------------------------------------------------------------------------------------
		public void Run() {
			Console.WriteLine("This is GLContext.cs");
            GLFW.glfwSetTime(0.0);
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
					GLFW.glfwPollEvents();
				}
				
				
			} while (GLFW.glfwGetWindowAttrib(Window, GLFW.GLFW_FOCUSED) != 0);
		}
		
		
		//------------------------------------------------------------------------------------------------------------------------
		//														display()
		//------------------------------------------------------------------------------------------------------------------------
		private void Display () {
			GL.glClear(GL.GL_COLOR_BUFFER_BIT);
			
			GL.glMatrixMode(GL.GL_MATRIX_MODE);
			GL.glLoadIdentity();
			
			_owner.Render(this);

			GLFW.glfwSwapBuffers(Window);
			if (GetKey(Key.ESCAPE)) this.Close();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetColor()
		//------------------------------------------------------------------------------------------------------------------------
		public void SetColor (byte r, byte g, byte b, byte a) {
			GL.glColor4ub(r, g, b, a);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														PushMatrix()
		//------------------------------------------------------------------------------------------------------------------------
		public void PushMatrix(float[] matrix) {
			GL.glPushMatrix();
			GL.glMultMatrixf(matrix);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														PopMatrix()
		//------------------------------------------------------------------------------------------------------------------------
		public void PopMatrix() {
			GL.glPopMatrix();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														DrawQuad()
		//------------------------------------------------------------------------------------------------------------------------
		public void DrawQuad(float[] vertices, float[] uv) {
			GL.glEnableClientState( GL.GL_TEXTURE_COORD_ARRAY );
			GL.glEnableClientState( GL.GL_VERTEX_ARRAY );
			GL.glTexCoordPointer( 2, GL.GL_FLOAT, 0, uv.ToIntPtr());
			GL.glVertexPointer( 2, GL.GL_FLOAT, 0, vertices.ToIntPtr());
			GL.glDrawArrays(GL.GL_QUADS, 0, 4);
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisableClientState(GL.GL_TEXTURE_COORD_ARRAY);		
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
			GLFW.glfwGetCursorPos(Window, ref mouseX, ref mouseY);
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