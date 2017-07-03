// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CreativePudding/VertexColor"
{ 
	Properties {
	}
 
	SubShader {
		Tags { 
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite On
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex	: POSITION;
				fixed4 color 	: COLOR;
			};

			struct v2f
			{
				float4 vertex	: SV_POSITION;
				fixed4 color 	: COLOR;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
		
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.color = IN.color;

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				return IN.color;
			}
			ENDCG
		}
	}
}	