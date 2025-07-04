

Shader "Tazo/Magic_in_rainbow"
{
	Properties
	{
		[Header(Main)]
		[NoScaleOffset] _RainBowTex("RainBowTex (RGB)", 2D) = "white" {}
		rainbow_go("RainBowSpeed", Range(-20.0, 20.0)) = 1
		_BaseMap("BaseMap(RGB)", 2D) = "white" {}
		_MColor("Mat Color", Color) = (0.5,0.5,0.5,1)
		_power("HDR Power", Range(0, 5)) = 1
		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0

		[Header(Noise)]
		[NoScaleOffset] _Noise("Noise Texture (R)", 2D) = "white" {}
		_NoiseTile("Noise Tiling", Range(0.0, 20)) = 1
		_tileOffsetX("OffsetX", Range(0, 1)) = 0
		_tileOffsetY("OffseeY", Range(0, 1)) = 0
		_Mix("Use Noise Instead Of BaseMap", Range(0, 1)) = 1
		[Header(Rim)]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		rimWidth("rimWidth", Range(0.0, 3)) = 0.75
	

	
		
	}
	
	Subshader
	{
		Tags { "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		



//shell
		Pass
		{
			//Cull Back
			ZWrite[_ZWrite]
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 pos	: SV_POSITION;
				float4 cap	: TEXCOORD0;
				float2 uv	: TEXCOORD1;
				float4 uv_object: TEXCOORD2;
				fixed4 color : COLOR;
			};
			uniform float rimWidth;

			uniform sampler2D _RainBowTex;
			uniform sampler2D _Noise;
			uniform sampler2D _BaseMap;
			float4 _BaseMap_ST;
			fixed _NoiseTile;
			
	
	
		
			fixed rainbow_go;

		
			float _tileOffsetX;
			float _tileOffsetY;
			float _power;
			fixed _Mix;

			v2f vert(appdata v)
			{
				v2f o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
				worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float dotProduct = 1 - dot(v.normal, viewDir);
				o.color.r = smoothstep(1 - rimWidth, 1.0, dotProduct);
				o.color.g = dotProduct;
				o.cap.xy = worldNorm.xy * 0.5 + 0.5;
				o.cap.z = abs(v.normal.z);
				o.cap.w = abs(v.normal.y);
				o.uv_object = v.vertex*0.5+0.5;
				o.uv = TRANSFORM_TEX(v.texcoord, _BaseMap);
				
				return o;
			}

			float4 _RimColor;
			uniform float4 _MColor;
	
		
			uniform sampler2D _ProjectUV;

			float4 frag(v2f i) : SV_Target
			{
				float3 k = float3(0.57735, 0.57735, 0.57735);
				float2 offset = float2(_tileOffsetX, _tileOffsetY);
				fixed4 p1 = tex2D(_Noise, offset + i.uv_object.rg * _NoiseTile );
				fixed4 p2 = tex2D(_Noise, offset + i.uv_object.gb* _NoiseTile*1.07 );
				fixed4 p3 = tex2D(_Noise, offset + i.uv_object.rb * _NoiseTile*1.02);
		
			
				fixed4 ppp = lerp(lerp(p2, p1, i.cap.z), p3, i.cap.w);
				ppp = dot(1- ppp.rgb, k);
				float4 mt = tex2D(_BaseMap, i.uv ).r;
				
				fixed ccc = dot(1 - mt.rgb, k);
				fixed no_color = lerp(ccc,ppp,_Mix);
				fixed3 rr = tex2D(_RainBowTex, no_color + frac(_Time.y * rainbow_go)).rgb;

				float4 cc = 1;
				
				cc.rgb = (rr*_MColor + _RimColor.rgb*_RimColor.a* i.color.r)* _power;

				cc.a =   _MColor.a* no_color;
				
			
				return cc;
			}
			ENDCG
		}

		
	}
	
	Fallback "VertexLit"
}
