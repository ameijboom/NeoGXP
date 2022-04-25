using System;
using System.Collections.Generic;
using System.Linq;
using Arqan;
using NeoGXP.NeoGXP.Core;
using NeoGXP.NeoGXP.Utils;

namespace NeoGXP.NeoGXP.AddOns;

/// <summary>
/// A SpriteBatch is a GameObject that can be used to render many sprites efficiently, and change the color, alpha and blend mode of
/// all of those sprites simultaneously.
/// Usage: Add any number of sprites as child to a sprite batch, and then call the Freeze method.
/// Note that this will destroy the individual sprites, so there won't be colliders for them anymore, and
/// the position and rotation of individual sprites cannot be changed anymore.
/// </summary>
public class SpriteBatch : GameObject {
	private readonly Dictionary<Texture2D, BufferRenderer> _renderers;
	protected uint Color = 0xFFFFFF;
	protected float Alpha = 1.0f;

	public BlendMode blendMode = null;
	private bool _initialized = false;

	private Rectangle _bounds;

	/// <summary>
	/// Create a new SpriteBatch game object.
	/// After adding sprites as child to this game object, call the Freeze method to started batched drawing.
	/// </summary>
	public SpriteBatch() : base(false) {
		_renderers = new Dictionary<Texture2D, BufferRenderer>();
	}

	/// <summary>
	/// Call this method after adding sprites as child to this game object, and positioning and rotating them correctly.
	/// This will destroy the individual sprites and their colliders.
	/// Note that the individual color, alpha and blend mode of those sprites is forgotten:
	/// use the color, alpha and blend mode of the sprite batch instead.
	/// </summary>
	public void Freeze() {
		float boundsMinX = float.MaxValue;
		float boundsMaxX = float.MinValue;
		float boundsMinY = float.MaxValue;
		float boundsMaxY = float.MinValue;

		List<GameObject> children = GetChildren(true); // intentional clone!
		foreach (Sprite tile in children.OfType<Sprite>())
		{
			tile.parent = null; // To get the proper Extents

			if (!_renderers.ContainsKey(tile.texture)) {
				_renderers[tile.texture] = new BufferRenderer(tile.texture);
			}

			BufferRenderer rend = _renderers[tile.texture];

			Vec2[] bounds = tile.GetExtents();
			float[] uvs = tile.GetUVs(false);
			for (int corner = 0; corner < 4; corner++) {
				rend.AddVert(bounds[corner].x, bounds[corner].y);
				rend.AddUv(uvs[corner * 2], uvs[corner * 2 + 1]);

				if (bounds[corner].x < boundsMinX) boundsMinX = bounds[corner].x;
				if (bounds[corner].x > boundsMaxX) boundsMaxX = bounds[corner].x;
				if (bounds[corner].y < boundsMinY) boundsMinY = bounds[corner].y;
				if (bounds[corner].y > boundsMaxY) boundsMaxY = bounds[corner].y;
			}
			tile.Destroy();
		}
		_bounds = new Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY);

		// Create buffers:
		foreach (Texture2D texture in _renderers.Keys) {
			_renderers[texture].CreateBuffers();
		}

		_initialized = true;
	}

	/// <summary>
	/// Gets the four corners of this object as a set of 4 Vector2s.
	/// </summary>
	/// <returns>
	/// The extents.
	/// </returns>
	public Vec2[] GetExtents() {
		Vec2[] ret = new Vec2[4];
		ret[0] = TransformPoint(_bounds.left, _bounds.top);
		ret[1] = TransformPoint(_bounds.right, _bounds.top);
		ret[2] = TransformPoint(_bounds.right, _bounds.bottom);
		ret[3] = TransformPoint(_bounds.left, _bounds.bottom);
		return ret;
	}

	protected override void OnDestroy() {
		foreach (BufferRenderer rend in _renderers.Values) {
			rend.Dispose();
		}
		_renderers.Clear();
		_initialized = false;
	}


	protected override void RenderSelf(GLContext glContext) {
		if (!_initialized) return;

		bool test = false;

		Vec2[] bounds = GetExtents();
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		for (int i = 0; i < 4; i++) {
			if (bounds[i].x > maxX) maxX = bounds[i].x;
			if (bounds[i].x < minX) minX = bounds[i].x;
			if (bounds[i].y > maxY) maxY = bounds[i].y;
			if (bounds[i].y < minY) minY = bounds[i].y;
		}
		test = maxX < game.RenderRange.left || maxY < game.RenderRange.top || minX >= game.RenderRange.right || minY >= game.RenderRange.bottom;

		if (test == false) {
			blendMode?.enable();
			glContext.SetColor((byte)((Color >> 16) & 0xFF),
				(byte)((Color >> 8) & 0xFF),
				(byte)(Color & 0xFF),
				(byte)(Alpha * 0xFF));

			foreach (BufferRenderer rend in _renderers.Values) {
				rend.DrawBuffers(glContext);
			}

			glContext.SetColor(255, 255, 255, 255);

			if (blendMode != null) BlendMode.NORMAL.enable();
		} else {
			//Console.WriteLine("Not rendering sprite batch");
		}
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														color
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Gets or sets the color filter for this sprite.
	/// This can be any value between 0x000000 and 0xFFFFFF.
	/// </summary>
	public uint color {
		get => Color;
		set => Color = value & 0xFFFFFF;
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														color
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Sets the color of the sprite batch.
	/// </summary>
	/// <param name='r'>
	/// The red component, range 0..1
	/// </param>
	/// <param name='g'>
	/// The green component, range 0..1
	/// </param>
	/// <param name='b'>
	/// The blue component, range 0..1
	/// </param>
	public void SetColor(float r, float g, float b) {
		r = Mathf.Clamp(r, 0, 1);
		g = Mathf.Clamp(g, 0, 1);
		b = Mathf.Clamp(b, 0, 1);
		byte rr = (byte)Math.Floor(r * 255);
		byte rg = (byte)Math.Floor(g * 255);
		byte rb = (byte)Math.Floor(b * 255);
		color = rb + (uint)(rg << 8) + (uint)(rr << 16);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														alpha
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Gets or sets the alpha value of the sprite batch.
	/// Setting this value allows you to make the sprite batch (semi-)transparent.
	/// The alpha value should be in the range 0...1, where 0 is fully transparent and 1 is fully opaque.
	/// </summary>
	public float alpha {
		get => Alpha;
		set => Alpha = value;
	}
}

/// <summary>
/// A helper class for SpriteBatches, and possibly other complex objects or collections with larger vertex and uv lists.
/// </summary>
public class BufferRenderer {
	protected float[] Verts;
	protected float[] UVs;
	protected int NumberOfVertices; // The number of rendered quads is numberOfVertices/4

	private readonly Texture2D _texture;

	private readonly List<float> _vertList = new();
	private readonly List<float> _uvList = new();

	public BufferRenderer(Texture2D texture) {
		_texture = texture;
	}

	public void AddVert(float x, float y) {
		_vertList.Add(x);
		_vertList.Add(y);
	}
	public void AddUv(float u, float v) {
		_uvList.Add(u);
		_uvList.Add(v);
	}

	public void CreateBuffers() {
		Verts = _vertList.ToArray();
		UVs = _uvList.ToArray();
		NumberOfVertices = Verts.Length / 2;
	}

	public void DrawBuffers(GLContext glContext) {
		_texture.Bind();

		GL.glEnableClientState(GL.GL_TEXTURE_COORD_ARRAY);
		GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
		GL.glTexCoordPointer(2, GL.GL_VERTEX_ARRAY, 0, UVs.ToIntPtr());
		GL.glVertexPointer(2, GL.GL_VERTEX_ARRAY, 0, Verts.ToIntPtr());
		GL.glDrawArrays(GL.GL_QUADS, 0, NumberOfVertices);
		GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
		GL.glDisableClientState(GL.GL_TEXTURE_COORD_ARRAY);

		_texture.Unbind();
	}

	public void Dispose()
	{
		// For this backend: nothing needed	}
	}
}
