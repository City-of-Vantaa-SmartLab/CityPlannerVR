// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/HologramShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_NoiseMap ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,0,0,1)
		_TopColor ("TopColor", Color) = (1,0,0,1)
		_TopAlpha ("TopAlpha", Range(0.0, 1.0)) = 1.0
		_Brightness("Brightness", Range(0.1, 6.0)) = 3.0
		_Alpha ("Alpha", Range (0.0, 1.0)) = 1.0
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.1, 10)) = 5.0
		_HolographAmount ("Holograph Amount", Range(0.1, 10)) = 1
		_HolagraphSpeed ("Holograph Speed", Range(-10, 10)) = 1
		_Bias ("Bias", Range(-1, 2)) = 0
		_GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.5
		_GlitchSpeed("Glitch Speed", Range(0, 10)) = 2.5
		_GlowTiling ("Glow Tiling", Range(0.01, 1.0)) = 0.05
		_GlowSpeed ("Glow Speed", Range(-10.0, 10.0)) = 1.0

	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Back

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(3)
				float4 vertex : SV_POSITION;
				float4 objVertex : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 worldNormal : NORMAL;

			};

			fixed4 _Color;
			fixed4 _TopColor;
			float _HolographAmount, _HolagraphSpeed;
			sampler2D _MainTex;
			sampler2D _NoiseMap;
			float4 _MainTex_ST;
			float _Bias;
			float4 _RimColor;
			float _RimPower;
			float _Brightness;
			float _Alpha, _GlitchIntensity,_GlitchSpeed;
			float _TopAlpha;
			float _GlowTiling;
			float _GlowSpeed;
			
			v2f vert (appdata v)
			{
				v2f o;

				v.vertex.x += _GlitchIntensity * (step(0.5, sin(_Time.y * 2.0 + v.vertex.y * 1.0)) * step(0.99, sin(_Time.y*_GlitchSpeed)));

				o.objVertex = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = normalize(UnityWorldSpaceViewDir(o.objVertex.xyz));
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				fixed4 tex;

				if(i.worldNormal.y >=0.1)
				{
					col = _TopColor;
					col.a = _TopAlpha;
				}

				else
				{
					// sample the texture
					col = tex2D(_MainTex, i.uv);
					tex = tex2D(_MainTex, i.uv);

					//TEST
					half rim = 1.0-saturate(dot(i.viewDir, i.worldNormal));
					fixed4 rimColor = _RimColor * pow (rim, _RimPower);

					//i.objVertex sign determines the direction of fill 
					float glow = frac(-i.objVertex.y * _GlowTiling - _Time.x * _GlowSpeed);

					col = tex * _Color + rimColor;

					col.a = _Alpha * rim * glow;

					col.rgb *= _Brightness;

					col *= max(0, cos(i.objVertex.y * _HolographAmount * 100 + _Time.x * _HolagraphSpeed * 100) + _Bias);;
				}
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				

				return col;
			}
			ENDCG
		}
	}
}
