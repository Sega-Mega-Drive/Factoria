Shader "Hidden/MP41312322312102312302312201" {
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
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;

            struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 texcoord3 : TEXCOORD3;

            };

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR0;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;

            };

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_VP, v.vertex);
				o.color = v.color;
				o.uv0 = v.texcoord0; o.uv0.y = 1.0 - o.uv0.y;
				o.uv1 = v.texcoord1; o.uv1.y = 1.0 - o.uv1.y;
				o.uv2 = v.texcoord2; o.uv2.y = 1.0 - o.uv2.y;
				o.uv3 = v.texcoord3; o.uv3.y = 1.0 - o.uv3.y;

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
				arg1.xyz = i.color.xyz;
				arg1.w = colorTex.w;
				arg2 = colorTex;
				color = arg1 * arg2;
				
				colorTex = tex2D(_Tex1, i.uv1);
				arg1.xyz = i.color.xyz;
				arg1.w = color.w;
				arg2 = colorTex;
				color.xyz = arg1.xyz * arg2.xyz;
				color.w = arg1.w + arg2.w;
				
				colorTex = tex2D(_Tex2, i.uv2);
				arg1.xyz = i.color.xyz;
				arg1.w = color.w;
				arg2 = colorTex;
				color = arg1 * arg2;
				
				colorTex = tex2D(_Tex3, i.uv3);
				arg1.xyz = i.color.xyz;
				arg1.w = color.w;
				arg2.xyz = colorTex.xyz;
				arg2.w = i.color.w;
				color.xyz = arg1.xyz * arg2.xyz;
				color.w = arg1.w - arg2.w;
				
				return color;

            }
			ENDCG
		}
	}
	FallBack "Particles/Additive"
}
