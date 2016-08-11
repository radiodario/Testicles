using UnityEngine;
using System.Collections;

public class HexagonParticleSystem : MonoBehaviour {

	public int MaxParticles = 32768;

	public int Width = 1000;
	public int Height = 800;

	[SerializeField]
	Vector3 _emitterPosition = Vector3.forward * Width;

	[SerializeField]
	Vector3 _emitterSize = Vector3.one * Height;

	public int Scale = 10;
	public int CurrentLife = 10;

	public float xINC = 0.1f;
	public float yINC = 0.1f;
	public float zINC = 0.1f;

	public float MaxSpeed = 20;

	[SerializeField]
	bool _debug;

	private bool _needsReset = true;


	// wow! awesome.
    static float deltaTime {
        get {
            return Application.isPlaying && Time.frameCount > 1 ? Time.deltaTime : 1.0f / 10;
        }
    }

	RenderTexture _forceValuesBuffer;
	RenderTexture _forceLookupBuffer;

	RenderTexture _positionBuffer1;
	RenderTexture _positionBuffer2;

	RenderTexture _velocityBuffer1;
	RenderTexture _velocityBuffer2;

	RenderTexture _displayBuffer1;
	RenderTexture _displayBuffer2;


	// this updates the force values
	Material _forceKernelMaterial;

	// this updates the velocity values for the particles
	Material _velocityKernelMaterial;

	// this updates the positions of the particles, and renders
	Material _positionKernelMaterial;

	Material _drawTextureMaterial;

	Material _debugMaterial;

	Mesh _mesh;

	Mesh CreateMesh(int BufferWidth, int BufferHeight)
	{
		var Nx = BufferWidth;
		var Ny = BufferHeight;

		// Create vertex arrays.
		var VA = new Vector3[Nx * Ny * 2];
		var TA = new Vector2[Nx * Ny * 2];

		var Ai = 0;
		for (var x = 0; x < Nx; x++)
		{
			for (var y = 0; y < Ny; y++)
			{
				VA[Ai + 0] = new Vector3(1, 0, 0);
				VA[Ai + 1] = new Vector3(0, 0, 0);

				var u = (float)x / Nx;
				var v = (float)y / Ny;
				TA[Ai] = TA[Ai + 1] = new Vector2(u, v);

				Ai += 2;
			}
		}

		// Index array.
		var IA = new int[VA.Length];
		for (Ai = 0; Ai < VA.Length; Ai++) IA[Ai] = Ai;

		// Create a mesh object.
		var mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.vertices = VA;
		mesh.uv = TA;
		mesh.SetIndices(IA, MeshTopology.Lines, 0);
		mesh.Optimize();

		// Avoid being culled.
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

		return mesh;
	}

	Material CreateMaterial(Shader shader) {
		var material = new Material (shader);
		material.hideFlags = HideFlags.DontSave;
		return material;
	}

	RenderTexture CreateBuffer (int BufferWidth) {
		return CreateBuffer (BufferWidth, BufferWidth);
	}

	RenderTexture CreateBuffer(int BufferWidth, int BufferHeight) {
        var buffer = new RenderTexture(BufferWidth, BufferHeight, 0, RenderTextureFormat.ARGBFloat);
        buffer.hideFlags = HideFlags.DontSave;
        buffer.filterMode = FilterMode.Point;
        buffer.wrapMode = TextureWrapMode.Repeat;
        return buffer;
    }

	void InitializeAndPrewarmBuffers() {
		// initialize the position and velocity kernels
		Graphics.Blit(null, _forceValuesBuffer, _forceKernelMaterial, 0);
		Graphics.Blit(null, _positionBuffer2, _positionKernelMaterial, 0);
		Graphics.Blit(null, _velocityBuffer2, _velocityKernelMaterial, 0);

		// execute the kernels a few times
		for (var i = 0; i < 8; i++) {
			Graphics.Blit (_velocityBuffer2, _velocityBuffer1, _velocityKernelMaterial, 1);
			Graphics.Blit (_positionBuffer2, _positionBuffer1, _positionKernelMaterial, 1);
			Graphics.Blit (_velocityBuffer1, _velocityBuffer2, _velocityKernelMaterial, 1);
			Graphics.Blit (_positionBuffer1, _positionBuffer2, _positionKernelMaterial, 1);
		}
	}

	void UpdateForceKernelShader() {
		var m = _forceKernelMaterial;
		m.SetVector ("_Config", new Vector4 (xINC, yINC, zINC, deltaTime));
	}

	void UpdateVelocityKernelShader() {
		var m = _velocityKernelMaterial;
		m.SetTexture ("_ParticlePositions", _positionBuffer2);
		m.SetTexture ("_ForceIndices", _forceLookupBuffer);
		m.SetTexture ("_ForceValues", _forceValuesBuffer);
		m.SetFloat ("_MaxSpeed", MaxSpeed);
		m.SetVector("_Config", new Vector4(Width, Height, CurrentLife, deltaTime));
	}

	void UpdatePositionKernelShader() {
		var m = _positionKernelMaterial;
		m.SetTexture ("_ParticleVelocities", _velocityBuffer2);
		m.SetVector("_Config", new Vector4(Width, Height, CurrentLife, deltaTime));
	}

	void GenerateForceLookupTexture() {
		_forceLookupBuffer = HexagonDrawer.DrawHexagonSurface (Width, Height, Scale);
	}

	void UpdateKernelShaders() {
		UpdateForceKernelShader ();
		UpdateVelocityKernelShader ();
		UpdatePositionKernelShader ();
	}

	void ResetResources() {
		if (_forceLookupBuffer)
			DestroyImmediate (_forceLookupBuffer);
		if (_forceValuesBuffer)
			DestroyImmediate (_forceValuesBuffer);

		if (_positionBuffer1)
			DestroyImmediate (_positionBuffer1);
		if (_positionBuffer2)
			DestroyImmediate (_positionBuffer2);

		if (_velocityBuffer1)
			DestroyImmediate (_velocityBuffer1);
		if (_velocityBuffer2)
			DestroyImmediate (_velocityBuffer2);

		if (_displayBuffer1)
			DestroyImmediate (_displayBuffer1);
		if (_displayBuffer2)
			DestroyImmediate (_displayBuffer2);

		GenerateForceLookupTexture ();

		int BufferWidth = (int)Mathf.Sqrt (MaxParticles);

		// Mesh object.
		if (_mesh == null) _mesh = CreateMesh(BufferWidth, BufferWidth);

		_positionBuffer1 = CreateBuffer (BufferWidth);
		_positionBuffer2 = CreateBuffer (BufferWidth);

		_velocityBuffer1 = CreateBuffer (BufferWidth);
		_velocityBuffer2 = CreateBuffer (BufferWidth);

		Vector2 forceDims = HexagonDrawer.NumberOfForces (Width, Height, Scale); 
		_forceValuesBuffer = CreateBuffer ((int) forceDims.x, (int) forceDims.y);

		_displayBuffer1 = CreateBuffer (Width, Height);

		if (!_forceKernelMaterial)
			_forceKernelMaterial = CreateMaterial (Shader.Find("Custom/ForceKernel"));

		if (!_velocityKernelMaterial)
			_velocityKernelMaterial = CreateMaterial (Shader.Find ("Custom/VelocityKernel"));

		if (!_positionKernelMaterial)
			_positionKernelMaterial = CreateMaterial (Shader.Find ("Custom/PositionKernel"));

		if (!_debugMaterial)
			_debugMaterial = CreateMaterial (Shader.Find ("Custom/Debug"));

		if (!_drawTextureMaterial)
			_drawTextureMaterial = CreateMaterial (Shader.Find ("Custom/PointMeshShader"));

		// warm up
		UpdateKernelShaders();
		InitializeAndPrewarmBuffers ();

		_needsReset = false;

	}

	void Reset() {
         _needsReset = true;
    }

    void OnDestroy() {
        if (_velocityBuffer1) DestroyImmediate(_velocityBuffer1);
        if (_velocityBuffer2) DestroyImmediate(_velocityBuffer2);
        if (_positionBuffer1) DestroyImmediate(_positionBuffer1);
        if (_positionBuffer2) DestroyImmediate(_positionBuffer2);
        if (_forceKernelMaterial)  DestroyImmediate(_forceKernelMaterial);
        if (_forceKernelMaterial)  DestroyImmediate(_forceKernelMaterial);
        if (_velocityKernelMaterial)  DestroyImmediate(_velocityKernelMaterial);
        if (_positionKernelMaterial)  DestroyImmediate(_positionKernelMaterial);
    }

	// Update is called once per frame
	void Update () {
		if (_needsReset) ResetResources();

		UpdateKernelShaders ();

		if (Application.isPlaying) {
			// update the forces
			Graphics.Blit(null, _forceValuesBuffer, _forceKernelMaterial, 0);

			// swap the particle velocities
			var tempV = _velocityBuffer1;
			_velocityBuffer1 = _velocityBuffer2;
			_velocityBuffer2 = tempV;
			// apply the velocity kernel
			Graphics.Blit(_velocityBuffer1, _velocityBuffer2, _velocityKernelMaterial, 1);

			// swap the particle positions
			var tempP = _positionBuffer1;
			_positionBuffer1 = _positionBuffer2;
			_positionBuffer2 = tempP;

			// apply the position kernel
			Graphics.Blit(_positionBuffer1, _positionBuffer2, _positionKernelMaterial, 1);

		} else {
			InitializeAndPrewarmBuffers();
		}

		// draw the particles to a texture;
		_drawTextureMaterial.SetTexture("_ParticlePositions1", _positionBuffer1);
		_drawTextureMaterial.SetTexture ("_ParticlePositions2", _positionBuffer2);
		_drawTextureMaterial.SetInt ("_ParticleSide", (int)Mathf.Sqrt (MaxParticles));
		_drawTextureMaterial.SetFloat ("_ParticleRadius", 0.02f);
		_drawTextureMaterial.SetVector ("_Resolution", new Vector2 (Height, Width));
		Graphics.DrawMesh(_mesh, transform.position, transform.rotation, _drawTextureMaterial, 0);
	}

	void OnGUI() {
	    if (_debug && Event.current.type.Equals(EventType.Repaint))
	    {
	        if (_debugMaterial && _positionBuffer2)
	        {
	            var rect = new Rect(0, 0, 512, 128);
	            Graphics.DrawTexture(rect, _positionBuffer2, _debugMaterial);
	        }

			if (_debugMaterial && _velocityBuffer2)
			{
				var rect = new Rect(0, 128 + 10, 512, 128);
				Graphics.DrawTexture(rect, _velocityBuffer2, _debugMaterial);
			}

			if (_debugMaterial && _forceValuesBuffer)
			{
				var rect = new Rect(0, (128 + 10) * 2, 512, 128);
				Graphics.DrawTexture(rect, _forceValuesBuffer, _debugMaterial);
			}

			if (_debugMaterial && _forceLookupBuffer)
			{
				var rect = new Rect(0, (128 + 10) * 3, 512, 128);
				Graphics.DrawTexture(rect, _forceLookupBuffer, _debugMaterial);
			}
	    }
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(_emitterPosition, _emitterSize);
	}
}
