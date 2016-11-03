Shader "Custom/PointShader" {
	Properties
    {
        _ParticlePositions("-", 2D) = ""{}
        _ParticleVelocities("-", 2D) = ""{}
        _ParticleSide("-", int) = 100
        _ParticleRadius("-", float) = 0.02
        _Resolution("-", Vector) = (1000, 800, 0, 0)
    }

    CGINCLUDE


    #include "UnityCG.cginc"

    sampler2D _ParticlePositions;
    sampler2D _ParticleVelocities;

    float2 _Resolution;
    float _ParticleRadius;
    int _ParticleSide;


    float4 frag(v2f_img i) : SV_Target 
    {
    	float4 outColor = float4(0, 0, 0, 0);
    	float2 uv = i.uv;

    	float diag = sqrt(pow(_Resolution.x,2) + pow(_Resolution.y,2));

    	float iu, jv;
    	for (int i = 0; i < _ParticleSide; i++) {
    		for (int j = 0; j < _ParticleSide; j++) {
				iu = i/_ParticleSide;
				jv = j/_ParticleSide;
				float2 pUV = float2(iu, jv);
				float4 particle = tex2D(_ParticlePositions, pUV);
				float dist = distance(uv, particle.xy) / diag;
				outColor.rgb += smoothstep(dist, _ParticleRadius, _ParticleRadius + 0.001);
    		}
    	}

    	return outColor;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
