Shader "Custom/VelocityKernel" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_ParticlePositions ("-", 2D) = "" {}
		_ForceIndices ("-", 2D) = "" {}
		_ForceValues ("-", 2D) = "" {}
		_MaxSpeed ("Max Speed", float) = 10
		_ForceMultiplier ("Force Multiplier", float) = 0
		_Config ("-", Vector) = (1000, 800, 0, 0) // width, height, life, dt)
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
	float _ForceMultiplier;
	// PRNG function.
    float nrand(float2 uv, float salt)
    {
        uv += float2(salt, _Config.z);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

	float4 find_force_for(float4 particle) {

		float2 pUV;
		pUV.x = particle.x / _Config.x;
		pUV.y = particle.y / _Config.y;
		

		float4 forceUVEncoded = tex2D(_ForceIndices, pUV);
		float2 forceUV = forceUVEncoded.rg;
		return tex2D(_ForceValues, forceUV);	
	}

	float4 frag(v2f_img i) : SV_Target {
		float4 pv = tex2D(_MainTex, i.uv);
		float4 p = tex2D(_ParticlePositions, i.uv);
		float4 f = find_force_for(p) * (1 + _ForceMultiplier);

		float spd = sqrt(_MaxSpeed);
		if (p.w > 0) {
			pv += f;
			float l = length(pv);
			pv *= _MaxSpeed/l * (float)(l/_MaxSpeed > 1.0) + 1.0 * (float)(l/_MaxSpeed <= 1.0);//clamp(f, -_ForceMultiplier, _ForceMultiplier);
			//pv *= _MaxSpeed;
			//pv = clamp(pv, -spd, spd); 
			return pv;
		} else {
			// return no speed?
			float4 f = float4(nrand(i.uv, _Time.x+1) * 2 - 1, nrand(i.uv, _Time.y+1) * 2 - 1, 0, 0);
			//return float4(0, 0, 0, 0);
			return f;
		}
	}

	float4 frag_init(v2f_img i) : SV_Target {
			return float4(nrand(i.uv, _Time.x+1) * 2 - 1, nrand(i.uv, _Time.y+1) * 2 - 1, 0, 0);
			//return float4(0, 0, 0, 0);
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
