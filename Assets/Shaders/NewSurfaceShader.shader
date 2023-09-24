Shader "Custom/WavyEdgeShader" {
	Properties{
	_MainTex("Texture", 2D) = "white" {}
	_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
	_WaveSpeed("Wave Speed", Range(0, 10)) = 1
	_WaveAmplitude("Wave Amplitude", Range(0, 1)) = 0.1
	}

		SubShader{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata {
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		};

		struct v2f {
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _EdgeColor;
		float _WaveSpeed;
		float _WaveAmplitude;

		v2f vert(appdata v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
		}

		fixed4 frag(v2f i) : SV_Target {
		float2 uv = i.uv;
		float2 waveOffset = float2(sin(_Time.y * _WaveSpeed), cos(_Time.y * _WaveSpeed));
		float2 distortedUV = uv + waveOffset * _WaveAmplitude;
		fixed4 col = tex2D(_MainTex, distortedUV) * _EdgeColor;
		return col;
		}
		ENDCG
		}
	}
}