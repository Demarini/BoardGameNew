

Shader "Tazo/DiscoGrid"
{
	Properties
	{

		[NoScaleOffset] _RainBowTex("RainBowTex (RGB)", 2D) = "white" {}
		rainbow_go("RainBowSpeed", Range(-20.0, 20.0)) = 1
		[NoScaleOffset]_BaseTex3("Grid (RGBA)", 2D) = "white" {}
		[Toggle] _Invert("Grid Use Object UV", Float) = 0
		_BA("Use Black As Alpha", Range(0,1)) = 1
		_tile_u_0("Tile U 0", Range(0.0, 20.0)) = 1
		_tile_v_0("Tile V 0", Range(0.0, 20.0)) = 1
		_tileOffsetX0("Offset U 0", Range(0, 1)) = 0
		_tileOffsetY0("Offsee V 0", Range(0, 1)) = 0
		_flow_u_0("flow U 0", Range(-10, 10)) = 0.5
		_flow_v_0("flow V 0", Range(-10, 10)) = 0.5
	

		_MColor("Mat Color", Color) = (0.5,0.5,0.5,1)
		//[NoScaleOffset]_MatCap ("MatCap (RGB)", 2D) = "white" {}
	

		[HDR]_RimColor("Rim Color", Color) = (1,1,1,1)
		rimWidth("rimWidth", Range(0.0, 10.0)) = 0.75
	
			/*
		[Project]
		[NoScaleOffset]_Project1("Matrix 1(RGB)", 2D) = "black" {}


		
		_tile_u_1("Tile U 1", Range(0.0, 20.0)) = 1
		_tile_v_1("Tile V 1", Range(0.0, 20.0)) = 1
		_tileOffsetX1("Offset U 1", Range(0, 1)) = 0
		_tileOffsetY1("Offsee V 1", Range(0, 1)) = 0
		_flow_u_1("flow U 1", Range(-10, 10)) = 0.5
		_flow_v_1("flow V 1", Range(-10, 10)) = 0.5

		[NoScaleOffset]_Project2("Matrix 2(RGB)", 2D) = "black" {}
		_tile_u_2("Tile U 2", Range(0.0, 20.0)) = 1
		_tile_v_2("Tile V 2", Range(0.0, 20.0)) = 1
		_tileOffsetX2("Offset U 2", Range(0, 1)) = 0
		_tileOffsetY2("Offsee V 2", Range(0, 1)) = 0
		_flow_u_2("flow U 2", Range(-10, 10)) = 0.5
		_flow_v_2("flow V 2", Range(-10, 10)) = 0.5
	*/
		_power("HDR Power", Range(0, 10)) = 2.3
		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0

	}
	
	Subshader
	{
		Tags { "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		



//shell
		Pass
		{
			//Cull Back
			ZWrite [_ZWrite]
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _INVERT_ON
			//#pragma shader_feature _INVERTT_ON
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
				float4 cap	: TEXCOORD3;
				float2 uv	: TEXCOORD2;
				float4 scrPos : TEXCOORD1;
				float4 uv_view : TEXCOORD0;
				fixed4 color : COLOR;
			};
			uniform float rimWidth;

			uniform sampler2D _RainBowTex;
			uniform sampler2D _BaseTex3;
			float4 _BaseTex3_ST;
			fixed _BA;
			
			float _tileOffsetX0;
			float _tileOffsetY0;
			float _tile_u_0;
			float _tile_v_0;
			float _flow_u_0;
			float _flow_v_0;
	
		
			fixed rainbow_go;
			/*
			float _tileOffsetX1;
			float _tileOffsetY1;
			float _tile_u_1;
			float _tile_v_1;
			float _flow_u_1;
			float _flow_v_1;

			float _tileOffsetX2;
			float _tileOffsetY2;
			float _tile_u_2;
			float _tile_v_2;
			float _flow_u_2;
			float _flow_v_2;
			*/
			float _power;

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
				o.cap.z = 1 - abs(v.normal.y);
				o.cap.w = v.color.a;
				o.uv = TRANSFORM_TEX(v.texcoord, _BaseTex3);
				o.uv_view = mul(UNITY_MATRIX_MV, v.vertex)*0.1 ;
				o.scrPos = ComputeScreenPos(o.pos);
				return o;
			}

			float4 _RimColor;
			uniform float4 _MColor;
			//uniform sampler2D _MatCap;
			/*
			uniform sampler2D _Project1;
			uniform sampler2D _Project2;
			uniform sampler2D _ProjectUV;
			*/
			float4 frag(v2f i) : SV_Target
			{
				
				float2 project_uv1 =i.uv_view;
				/*
				float2 offset1   = float2(_tileOffsetX1, _tileOffsetY1);
				float2 scale1 = float2(_tile_u_1, _tile_v_1);
				float2 flow1 = float2(_flow_u_1, _flow_v_1);
				fixed4 p1 = tex2D(_Project1, offset1 + project_uv1 * scale1 + flow1 * _Time.y);
				
				float2 project_uv2 = i.scrPos.xy/ i.scrPos.w;
				float2 offset2 = float2(_tileOffsetX2, _tileOffsetY2);
				float2 scale2 = float2(_tile_u_2, _tile_v_2);
				float2 flow2 = float2(_flow_u_2, _flow_v_2);
				fixed4 p2 = tex2D(_Project2, offset2 + project_uv1 * scale2 + flow2 * _Time.y);
				*/
		
			

				//float4 mc1 = tex2D(_MatCap, i.cap.xy);
				//float4 mc = saturate(mc1);
				
	
				float2 offset0 = float2(_tileOffsetX0, _tileOffsetY0);
				float2 scale0 = float2(_tile_u_0, _tile_v_0);
				float2 flow0 = float2(_flow_u_0, _flow_v_0);
#ifdef _INVERT_ON
				fixed4 mt = tex2D(_BaseTex3, offset0 +i.uv * scale0 + flow0 * _Time.y);

#else

				fixed4 mt = tex2D(_BaseTex3, offset0 + project_uv1 * scale0 + flow0 * _Time.y);
#endif
				fixed3 k = float3(0.57735, 0.57735, 0.57735);
				fixed no_color = dot( mt.rgb, k);
				fixed3 rr = tex2D(_RainBowTex, no_color + frac(_Time.y * rainbow_go)).rgb;

				fixed4 cc = 1;
				
				
				
				cc.rgb = (rr* _power + _RimColor.rgb * _RimColor.a * i.color.r)* _MColor.rgb;

				cc.a =lerp(_MColor.a, (pow(mt.a, 0.5) * step(rr.r, 0.5) * i.cap.w + _RimColor.r * _RimColor.a * i.color.r) * _MColor.a, _BA);


				return cc;
			}
			ENDCG
		}

		
	}
	
	Fallback "VertexLit"
}
