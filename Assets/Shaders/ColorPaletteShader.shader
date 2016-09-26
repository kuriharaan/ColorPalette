Shader "Sprites/ColorPaletteShader"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" }
		ZWrite Off 
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

		Pass
		{
			CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				half2 uv        : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _Colors[16];

			v2f vert(appdata_base IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, IN.texcoord.xy);
				return OUT;
			}

			float4 frag(v2f i) : COLOR
			{
				return _Colors[(int)(tex2D(_MainTex, i.uv).a * 256)];
			}
			ENDCG
		}
	}

	Fallback "Sprites/Default"
}