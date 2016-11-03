
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

		//angle = ((float)((int) (angle / (2*pi/3)))) * (2*pi/3);

		//angle += (orientation * (pi));


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
