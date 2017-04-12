Shader "Unlit/UnlitTexDepth"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		/*_DepthMax("Max Depth", Float) = 5*/
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

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
				float2 uv : TEXCOORD0;
			};
	
			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				float depth : DEPTH;
			};
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _DepthMax;
	
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				float fovdepth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;
				o.depth = fovdepth;
				return o;
			}
	
			fixed4 _Color;
			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				float invert = _DepthMax - i.depth;
				return col * invert;
			}
			ENDCG
		}
	}
}
