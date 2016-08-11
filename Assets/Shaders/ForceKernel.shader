
Shader "Custom/ForceKernel" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_ForceSide ("-", int) = 10
		_NoiseParams ("-", Vector) = (0.1, 0.1, 0.1, 0) // xinc, yinc, zinc, dt
	}

	CGINCLUDE

    #include "UnityCG.cginc"
    #include "ClassicNoise3D.cginc"

	sampler2D _MainTex;

	float4 _NoiseParams;
	int _ForceSide;


	float4 frag(v2f_img i) : SV_Target {
		float4 f = tex2D(_MainTex, i.uv);

		int orientation = ((_ForceSide * i.uv.x)) % 2;

		float fx = i.uv.x / _NoiseParams.x;
		float fy = i.uv.y / _NoiseParams.y;
		float fz = _Time.x / _NoiseParams.z;

		float pi = 3.14159265358979323846264338327;

		float noise = cnoise(float3(fx, fy, fz));
		float angle = noise * 2 * pi;

//		float WEDGE = 2*pi / 6;
//		float NORTH = 0;
//		float NORTH_EAST = WEDGE;
//		float SOUTH_EAST = 2 * WEDGE;
//		float SOUTH = 3 * WEDGE;
//		float SOUTH_WEST = 4 * WEDGE;
//		float NORTH_WEST = 5 * WEDGE;
//
//		float north = (float) (angle < NORTH_EAST);
//		float north_east = (float) (angle >= NORTH_EAST && angle < SOUTH_EAST);
//		float south_east = (float) (angle >= SOUTH_EAST && angle < SOUTH);
//		float south = (float) (angle >= SOUTH && angle < SOUTH_WEST);
//		float south_west = (float) (angle >= SOUTH_WEST && angle < NORTH_WEST);
//		float north_west = (float) (angle >= NORTH_WEST);
//
//		float topPointing = (float) (orientation == 1);
//		float southPointing = (float) (orientation == 0);
//
//		angle = (north * NORTH) * topPointing;
//		angle += (north_east * NORTH_EAST) * southPointing;
//		angle += (south_east * SOUTH_EAST) * topPointing;
//		angle += (south * SOUTH) * southPointing;
//		angle += (south_west * SOUTH_WEST) * topPointing;
//		angle += (north_west * NORTH_WEST) * southPointing;


		angle = ((float)((int) (angle / (2*pi/3)))) * (2*pi/3);

		angle += (orientation * (pi));


		float4 newForce = float4(
			sin(angle),
			cos(angle),
			0,
			0
		);




		return normalize(newForce);
	
	}

	ENDCG

	SubShader
    {
    	// Pass 1: update the positions
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }

}
