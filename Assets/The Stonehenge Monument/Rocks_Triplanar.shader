// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Stonehenge/Rocks_Triplanar"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		_MossAlbedo("Moss Albedo", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_MossNoiseMask("Moss Noise Mask", 2D) = "white" {}
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 1
		[Normal]_MossNormalMap("Moss Normal Map", 2D) = "bump" {}
		_MossNormalScale("Moss Normal Scale", Float) = 1
		_Moss_Noise_Scale("Moss_Noise_Scale", Float) = 0.5
		_Moss1Power("Moss 1 Power", Float) = 1
		_MetallicScale("Metallic Scale", Range( 0.1 , 4)) = 1
		_Smoothness("Smoothness", 2D) = "white" {}
		_SmoothnessScale("Smoothness Scale", Range( 0.1 , 4)) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _NormalScale;
		uniform sampler2D _MossNormalMap;
		uniform float4 _MossNormalMap_ST;
		uniform float _MossNormalScale;
		uniform sampler2D _MossNoiseMask;
		uniform float4 _MossNoiseMask_ST;
		uniform float _Moss_Noise_Scale;
		uniform float _Moss1Power;
		uniform float4 _TintColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _MossAlbedo;
		uniform float4 _MossAlbedo_ST;
		uniform float _MetallicScale;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform sampler2D _Smoothness;
		uniform float4 _Smoothness_ST;
		uniform float _SmoothnessScale;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float2 uv_MossNormalMap = i.uv_texcoord * _MossNormalMap_ST.xy + _MossNormalMap_ST.zw;
			float2 uv_MossNoiseMask = i.uv_texcoord * _MossNoiseMask_ST.xy + _MossNoiseMask_ST.zw;
			float4 temp_cast_0 = (_Moss1Power).xxxx;
			float4 temp_output_17_0 = saturate( pow( ( tex2D( _MossNoiseMask, uv_MossNoiseMask ) * _Moss_Noise_Scale ) , temp_cast_0 ) );
			float3 lerpResult20 = lerp( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) , UnpackScaleNormal( tex2D( _MossNormalMap, uv_MossNormalMap ), _MossNormalScale ) , temp_output_17_0.rgb);
			o.Normal = lerpResult20;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 uv_MossAlbedo = i.uv_texcoord * _MossAlbedo_ST.xy + _MossAlbedo_ST.zw;
			float4 lerpResult9 = lerp( tex2D( _Albedo, uv_Albedo ) , tex2D( _MossAlbedo, uv_MossAlbedo ) , temp_output_17_0);
			o.Albedo = ( _TintColor * lerpResult9 ).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			o.Metallic = ( _MetallicScale * tex2D( _Metallic, uv_Metallic ) ).r;
			float2 uv_Smoothness = i.uv_texcoord * _Smoothness_ST.xy + _Smoothness_ST.zw;
			o.Smoothness = ( tex2D( _Smoothness, uv_Smoothness ) * _SmoothnessScale ).r;
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			o.Occlusion = tex2D( _Occlusion, uv_Occlusion ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18934
1205;1055;928;418;868.9734;725.114;2.202343;True;True
Node;AmplifyShaderEditor.SamplerNode;10;-1076.207,-69.82126;Inherit;True;Property;_MossNoiseMask;Moss Noise Mask;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-836.9094,163.3454;Inherit;False;Property;_Moss_Noise_Scale;Moss_Noise_Scale;10;0;Create;True;0;0;0;False;0;False;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-513.9108,194.266;Inherit;False;Property;_Moss1Power;Moss 1 Power;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-590.0261,99.69348;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;15;-376.6875,26.53577;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1281.717,369.2152;Inherit;False;Property;_MossNormalScale;Moss Normal Scale;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-767.877,520.9758;Inherit;False;Property;_NormalScale;Normal Scale;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;17;-135.8085,0.3968408;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;7;-1046.126,-379.8407;Inherit;True;Property;_MossAlbedo;Moss Albedo;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-852.3494,-726.1801;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-552.0468,494.3163;Inherit;True;Property;_NormalMap;Normal Map;6;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;362.2975,474.7952;Inherit;False;Property;_SmoothnessScale;Smoothness Scale;14;0;Create;True;0;0;0;False;0;False;0.1;0;0.1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-367.375,160.5598;Inherit;False;Property;_MetallicScale;Metallic Scale;12;0;Create;True;0;0;0;False;0;False;1;1;0.1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;174.2342,-456.5965;Inherit;False;Property;_TintColor;Tint Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-469.9644,298.1183;Inherit;True;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;9;33.36557,-213.1143;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;25;244.3978,221.7608;Inherit;True;Property;_Smoothness;Smoothness;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;18;-1065.887,342.5557;Inherit;True;Property;_MossNormalMap;Moss Normal Map;8;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;458.4591,-335.3174;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-90.42072,248.6514;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;628.6695,299.588;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-533.6909,741.6;Inherit;True;Property;_Occlusion;Occlusion;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;20;263.8802,-76.74368;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;705.9886,-218.1855;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Stonehenge/Rocks_Triplanar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;10;0
WireConnection;12;1;11;0
WireConnection;15;0;12;0
WireConnection;15;1;16;0
WireConnection;17;0;15;0
WireConnection;3;5;4;0
WireConnection;9;0;1;0
WireConnection;9;1;7;0
WireConnection;9;2;17;0
WireConnection;18;5;19;0
WireConnection;22;0;21;0
WireConnection;22;1;9;0
WireConnection;24;0;23;0
WireConnection;24;1;2;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;20;0;3;0
WireConnection;20;1;18;0
WireConnection;20;2;17;0
WireConnection;0;0;22;0
WireConnection;0;1;20;0
WireConnection;0;3;24;0
WireConnection;0;4;27;0
WireConnection;0;5;6;0
ASEEND*/
//CHKSM=BB374042884B81E7A0106D7F05DD5ED889DF77D6