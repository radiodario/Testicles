
Shader "Custom/ForceKernel" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_NoiseParams ("-", Vector) = (0.1, 0.1, 0.1, 0) // xinc, yinc, zinc, dt
	}

	CGINCLUDE

    #include "UnityCG.cginc"
    #include "ClassicNoise3D.cginc"

	sampler2D _MainTex;

	float4 _NoiseParams;

	float4 frag(v2f_img i) : SV_Target {
		float4 f = tex2D(_MainTex, i.uv);

		float fx = i.uv.x * _NoiseParams.x;
		float fy = i.uv.y * _NoiseParams.y;
		float fz = _Time.y * _NoiseParams.z;

		float pi = 3.14159265358979323846264338327;
		float angle = cnoise(float3(fx, fy, fz)) * 2 * pi;

		float4 newForce = float4(
			cos(angle) - sin(angle),
			sin(angle) - cos(angle),
			0,
			0
		);

		return newForce;
	
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
