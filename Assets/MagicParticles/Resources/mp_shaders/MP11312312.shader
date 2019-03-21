Shader "Hidden/MP11312312" {
	SubShader {		

		Tags { 
			"Queue"="Transparent+1" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}				

		Pass {
            Blend [_srcBlend] [_dstBlend]
            ZTest [_ZTest] // LEqual default
			AlphaTest Greater .01
			ZWrite Off

            ColorMask ARGB						 			
			Cull Off 
			Lighting Off 		
			Fog {Mode Off}

			CGPROGRAM								

			#pragma vertex vert
			#pragma fragment frag			

			sampler2D _Tex0;

            struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord0 : TEXCOORD0;

            };

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR0;
				float2 uv0 : TEXCOORD0;

            };

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_VP, v.vertex);
				o.color = v.color;
				o.uv0 = v.texcoord0; o.uv0.y = 1.0 - o.uv0.y;

                return o;
            }            

			fixed4 frag (v2f i) : SV_Target
			{
                //_Tex0 = _MainTex;

                fixed4 color;
                fixed4 arg1;
                fixed4 arg2;
                fixed4 colorTex;

                colorTex = tex2D(_Tex0, i.uv0);
				arg1 = i.color;
				arg2 = colorTex;
				color = arg1 * arg2;
				
				return color;

            }
			ENDCG
		}
	}
	FallBack "Particles/Additive"
}
