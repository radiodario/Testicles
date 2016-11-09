Shader "Custom/PointMeshShader" {
	Properties
    {
        _ParticlePositions1("-", 2D) = ""{}
        _ParticlePositions2("-", 2D) = ""{}
        _ParticleSide("-", int) = 100
        _ParticleRadius("-", float) = 10.02
        _Hue("-", float) = 0.5
        _Resolution("-", Vector) = (1000, 800, 0, 0)
    }

    CGINCLUDE

     #pragma multi_compile_fog

    #include "UnityCG.cginc"

    sampler2D _ParticlePositions1;
    sampler2D _ParticlePositions2;

    float4 _ParticleTex1_TexelSize;

    float2 _Resolution;
    float _ParticleRadius;
    int _ParticleSide;
    float _Hue;

    struct appdata
    {
        float4 position : POSITION;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        half4 color : COLOR;
        UNITY_FOG_COORDS(0)
    };

    float3 Hue(float H) {
    	float R = abs(H * 6 - 3) - 1;
    	float G = 2 - abs(H * 6 - 2);
    	float B = 2 - abs(H * 6 - 4);
    	return saturate(float3(R,G,B));
    }


	float4 HSVtoRGB(in float3 HSV) {
	    return float4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z,1);
	}

	v2f vert(appdata v)
    {
        v2f o;

        float2 uv = v.texcoord.xy + _ParticleTex1_TexelSize.xy / 2;

		float4 p1 = tex2Dlod(_ParticlePositions1, float4(uv, 0, 0));
        float4 p2 = tex2Dlod(_ParticlePositions2, float4(uv, 0, 0));
        float sw = v.position.x;

      	if (p1.w < 0)
        {
            o.position = mul(UNITY_MATRIX_MVP, float4(p2.xyz, 1));
        }
        else
        {
            float3 p = lerp(p2.xyz, p1.xyz, (1.0 - sw));
            o.position = mul(UNITY_MATRIX_MVP, float4(p, 1));
        }

        o.color = HSVtoRGB(float3(_Hue, 0.01, 1)); 
        o.color.a = 0.4;

        UNITY_TRANSFER_FOG(o, o.position);

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        fixed4 c = i.color;
        UNITY_APPLY_FOG(i.fogCoord, c);
        return c;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
