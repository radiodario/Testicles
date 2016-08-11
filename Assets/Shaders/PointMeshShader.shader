Shader "Custom/PointMeshShader" {
	Properties
    {
        _ParticlePositions1("-", 2D) = ""{}
        _ParticlePositions2("-", 2D) = ""{}
        _ParticleSide("-", int) = 100
        _ParticleRadius("-", float) = 0.02
        _Resolution("-", Vector) = (1000, 800, 0, 0)
    }

    CGINCLUDE

     #pragma multi_compile_fog

    #include "UnityCG.cginc"

    sampler2D _ParticlePositions;
    sampler2D _ParticleVelocities;

    float4 _ParticleTex1_TexelSize;

    float2 _Resolution;
    float _ParticleRadius;
    int _ParticleSide;

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

	v2f vert(appdata v)
    {
        v2f o;

        float2 uv = v.texcoord.xy + _ParticleTex1_TexelSize.xy / 2;

		float4 p1 = tex2Dlod(_ParticleTex1, float4(uv, 0, 0));
        float4 p2 = tex2Dlod(_ParticleTex2, float4(uv, 0, 0));
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

        o.color = float4(1, 1, 1, 0);
        o.color.a = p.s;

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
