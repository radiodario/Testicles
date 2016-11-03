Shader "Custom/PositionKernel" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_ParticleVelocities ("-", 2D) = "" {}
		_Config ("-", Vector) = (1000, 2000, 0, 0) // width, height, life, dt)
		_Decay ("-", Float) = 0.0
		_EmitterPos ("-", Vector) = (0, 0, 0, 0)
	}

	CGINCLUDE

    #include "UnityCG.cginc"
    #include "ClassicNoise3D.cginc"

	sampler2D _MainTex;
	sampler2D _ParticleVelocities;

	float4 _Config;
	float3 _EmitterPos;
	float _Decay;

	// PRNG function.
    float nrand(float2 uv, float salt)
    {
        uv += float2(salt, _Config.z);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

	 // Get a new particle.
    float4 new_particle(float2 uv)
    {
        float t = _Time.x;

        // Random position.
//        float3 p = float3(nrand(uv, t + 1), nrand(uv, t + 2), 0);
		float3 p = float3(_EmitterPos.x, _EmitterPos.y, 0);
        p.x *= _Config.x;
        p.y *= _Config.y;

        // Life duration.
        float l = _Config.z * (nrand(uv, t + 4) * 2) - 1;

        return float4(p, l);
    }

	float4 frag(v2f_img i) : SV_Target {
		float4 p = tex2D(_MainTex, i.uv);
		float4 pv = tex2D(_ParticleVelocities, i.uv);

		if (p.w > 0) {
			float dt = _Config.w;
			p.xy += pv.xy * dt;
			p.z = 0;
			p.w -= _Decay;
			return p;
		} else {
			
			return new_particle(i.uv);
		}
	
	}

	float4 frag_init(v2f_img i) : SV_Target {
		return new_particle(i.uv);
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
