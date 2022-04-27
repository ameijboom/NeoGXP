using System;
using System.Collections;
using Arqan;
using SkiaSharp;

namespace NeoGXP.NeoGXP.Core;

public class Texture2D
{
	private static readonly Hashtable LoadCache = new();
	private static Texture2D _lastBound = null;

	private const int UNDEFINED_GLTEXTURE 	= 0;

	private uint[] _glTexture;
	private int _count = 0;
	private bool _stayInCache = false;

	//------------------------------------------------------------------------------------------------------------------------
	//														Texture2D()
	//------------------------------------------------------------------------------------------------------------------------
	public Texture2D (int width, int height) {
		if (width == 0) if (height == 0) return;
		SetBitmap (new SKBitmap(width, height));
	}
	public Texture2D (string filename) {
		Load (filename);
	}
	public Texture2D (SKBitmap bitmap) {
		SetBitmap (bitmap);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														GetInstance()
	//------------------------------------------------------------------------------------------------------------------------
	public static Texture2D GetInstance (string filename, bool keepInCache=false) {
		if (LoadCache[filename] is not Texture2D tex2d) {
			tex2d = new Texture2D(filename);
			LoadCache[filename] = tex2d;
		}
		tex2d._stayInCache |= keepInCache; // setting it once to true keeps it in cache
		tex2d._count ++;
		return tex2d;
	}


	//------------------------------------------------------------------------------------------------------------------------
	//														RemoveInstance()
	//------------------------------------------------------------------------------------------------------------------------
	public static void RemoveInstance (string filename)
	{
		if (!LoadCache.ContainsKey(filename)) return;
		if (LoadCache[filename] is not Texture2D tex2D) return;
		tex2D._count--;
		if (tex2D._count == 0 && !tex2D._stayInCache) LoadCache.Remove(filename);
	}

	public void Dispose () {
		if (filename != "") {
			RemoveInstance (filename);
		}
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														bitmap
	//------------------------------------------------------------------------------------------------------------------------
	public SKBitmap bitmap { get; private set; }

	//------------------------------------------------------------------------------------------------------------------------
	//														filename
	//------------------------------------------------------------------------------------------------------------------------
	public string filename { get; private set; } = "";

	//------------------------------------------------------------------------------------------------------------------------
	//														width
	//------------------------------------------------------------------------------------------------------------------------
	public int width => bitmap.Width;

	//------------------------------------------------------------------------------------------------------------------------
	//														height
	//------------------------------------------------------------------------------------------------------------------------
	public int height => bitmap.Height;

	//------------------------------------------------------------------------------------------------------------------------
	//														Bind()
	//------------------------------------------------------------------------------------------------------------------------
	public void Bind() {
		if (_lastBound == this) return;
		_lastBound = this;
		GL.glBindTexture(GL.GL_TEXTURE_2D, _glTexture[0]);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Unbind()
	//------------------------------------------------------------------------------------------------------------------------
	public void Unbind() {
		//GL.BindTexture (GL.TEXTURE_2D, 0);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Load()
	//------------------------------------------------------------------------------------------------------------------------
	private void Load(string filename) {
		this.filename = filename;
		SKBitmap bitmap;
		try {
			bitmap = SKBitmap.Decode(filename);
		} catch {
			throw new Exception("Image " + filename + " cannot be found.");
		}
		SetBitmap(bitmap);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														SetBitmap()
	//------------------------------------------------------------------------------------------------------------------------
	private void SetBitmap(SKBitmap bitmap) {
		this.bitmap = bitmap;
		CreateGLTexture ();
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														CreateGLTexture()
	//------------------------------------------------------------------------------------------------------------------------
	private void CreateGLTexture ()
	{
		if (_glTexture is {Length: > 0})
			if (_glTexture[0] != UNDEFINED_GLTEXTURE)
				DestroyGLTexture ();

		_glTexture = new uint[1];
		bitmap ??= new SKBitmap(64, 64);

		GL.glGenTextures (1, _glTexture);

		GL.glBindTexture (GL.GL_TEXTURE_2D, _glTexture[0]);
		if (Game.main.PixelArt) {
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_NEAREST);
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_NEAREST);
		} else {
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
		}
		GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP_TO_EDGE);
		GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP_TO_EDGE);

		UpdateGLTexture();
		GL.glBindTexture (GL.GL_TEXTURE_2D, 0);
		_lastBound = null;
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														UpdateGLTexture()
	//------------------------------------------------------------------------------------------------------------------------
	public void UpdateGLTexture() {
		GL.glBindTexture (GL.GL_TEXTURE_2D, _glTexture[0]);
		GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, bitmap.Width, bitmap.Height, 0,
			GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, bitmap.GetPixels());

		_lastBound = null;
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														destroyGLTexture()
	//------------------------------------------------------------------------------------------------------------------------
	private void DestroyGLTexture() {
		GL.glDeleteTextures(1, _glTexture);
		_glTexture[0] = UNDEFINED_GLTEXTURE;
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Clone()
	//------------------------------------------------------------------------------------------------------------------------
	public Texture2D Clone (bool deepCopy=false) {
		SKBitmap bitmap;
		if (deepCopy) {
			bitmap = this.bitmap.Copy () as SKBitmap;
		} else {
			bitmap = this.bitmap;
		}
		Texture2D newTexture = new(0, 0);
		newTexture.SetBitmap(bitmap);
		return newTexture;
	}

	public bool wrap {
		set {
			GL.glBindTexture (GL.GL_TEXTURE_2D, _glTexture[0]);
			GL.glTextureParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, value ? (int)GL.GL_REPEAT : (int)GL.GL_CLAMP_TO_EDGE);
			GL.glTextureParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, value ? (int)GL.GL_REPEAT : (int)GL.GL_CLAMP_TO_EDGE);
			GL.glBindTexture (GL.GL_TEXTURE_2D, 0);
			_lastBound = null;
		}
	}

	public static string GetDiagnostics() {
		string output = "";
		output += "Number of textures in cache: " + LoadCache.Keys.Count+'\n';
		return output;
	}
}
