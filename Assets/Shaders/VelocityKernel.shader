Shader "Custom/VelocityKernel" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_ParticlePositions ("-", 2D) = "" {}
		_ForceIndices ("-", 2D) = "" {}
		_ForceValues ("-", 2D) = "" {}
		_MaxSpeed ("Max Speed", float) = 10
		_Config ("-", Vector) = (1000, 2000, 0, 0) // width, height, life, dt)
	}

	CGINCLUDE

    #include "UnityCG.cginc"
    #include "ClassicNoise3D.cginc"

	sampler2D _MainTex;
	sampler2D _ParticlePositions;
	sampler2D _ForceIndices;
	sampler2D _ForceValues;

	float4 _Config;
	float _MaxSpeed;


	float4 find_force_for(float4 particle) {

		float2 pUV;
		int w = _Config.r;
		int h = _Config.g;
		pUV.x = particle.x / w;
		pUV.y = particle.y / h;

		float4 forceUVEncoded = tex2D(_ForceIndices, pUV);
		float2 forceUV = forceUVEncoded.rg;
		return tex2D(_ForceValues, forceUV);	
	}

	float4 frag(v2f_img i) : SV_Target {
		float4 pv = tex2D(_MainTex, i.uv);
		float4 p = tex2D(_ParticlePositions, i.uv);
		float4 f = find_force_for(p);

		if (p.w > 0) {
			pv += f; 
			pv = normalize(pv) * _MaxSpeed;
			return pv;
		} else {
			// return no speed?
			return float4(0, 0, 0, 0);
		}
	}

	float4 frag_init(v2f_img i) : SV_Target {
		return float4(0, 0, 0, 0);
	}

	ENDCG

	SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment frag_init
            ENDCG
        }
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
