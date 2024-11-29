// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Depth"
{
	Properties
	{
		_RGBMAP("RGBMAP", 2D) = "white" {}
		_DepthMap("DepthMap", 2D) = "white" {}
		_Power("Power", Float) = 0
		[HDR]_Color1("Color 1", Color) = (0.490566,0.490566,0.490566,0)
		_Texture0("Texture 0", 2D) = "white" {}
		_Float0("Float 0", Float) = 10
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _DepthMap;
		uniform float4 _DepthMap_ST;
		uniform float _Power;
		uniform sampler2D _Texture0;
		uniform float4 _Texture0_ST;
		uniform float _Float0;
		uniform sampler2D _RGBMAP;
		uniform float4 _RGBMAP_ST;
		uniform float4 _Color1;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_DepthMap = v.texcoord * _DepthMap_ST.xy + _DepthMap_ST.zw;
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( tex2Dlod( _DepthMap, float4( uv_DepthMap, 0, 0.0) ) * _Power ) * float4( ase_vertexNormal , 0.0 ) ).rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture0 = i.uv_texcoord * _Texture0_ST.xy + _Texture0_ST.zw;
			float2 temp_output_2_0_g1 = uv_Texture0;
			float2 break6_g1 = temp_output_2_0_g1;
			float temp_output_25_0_g1 = ( pow( 0.5 , 3.0 ) * 0.1 );
			float2 appendResult8_g1 = (float2(( break6_g1.x + temp_output_25_0_g1 ) , break6_g1.y));
			float4 tex2DNode14_g1 = tex2D( _Texture0, temp_output_2_0_g1 );
			float temp_output_4_0_g1 = _Float0;
			float3 appendResult13_g1 = (float3(1.0 , 0.0 , ( ( tex2D( _Texture0, appendResult8_g1 ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float2 appendResult9_g1 = (float2(break6_g1.x , ( break6_g1.y + temp_output_25_0_g1 )));
			float3 appendResult16_g1 = (float3(0.0 , 1.0 , ( ( tex2D( _Texture0, appendResult9_g1 ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float3 normalizeResult22_g1 = normalize( cross( appendResult13_g1 , appendResult16_g1 ) );
			o.Normal = normalizeResult22_g1;
			float2 uv_RGBMAP = i.uv_texcoord * _RGBMAP_ST.xy + _RGBMAP_ST.zw;
			o.Albedo = tex2D( _RGBMAP, uv_RGBMAP ).rgb;
			o.Emission = _Color1.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.RangedFloatNode;8;-576,288;Inherit;False;Property;_Power;Power;4;0;Create;True;0;0;0;False;0;False;0;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-735.3997,-409.0999;Inherit;True;Property;_DepthMap;DepthMap;3;0;Create;True;0;0;0;False;0;False;-1;None;4533ac975413f964cb3232452d8b1cea;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-192,0;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-195.498,-763.0573;Inherit;True;Property;_RGBMAP;RGBMAP;0;0;Create;True;0;0;0;False;0;False;None;c0e344f0170969c4b89441262bc6d968;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-416.9204,436.193;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;26;-96,400;Inherit;True;Property;_Texture0;Texture 0;6;0;Create;True;0;0;0;False;0;False;None;4533ac975413f964cb3232452d8b1cea;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;27;236.9691,462.8764;Inherit;False;Property;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;64,144;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;15.10005,-300.2;Inherit;True;Property;_ColorMap;ColorMap;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1165.167,-468.0742;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.91,1.11;False;1;FLOAT2;1,0.75;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;240,-32;Inherit;False;Property;_Color1;Color 1;5;1;[HDR];Create;True;0;0;0;False;0;False;0.490566,0.490566,0.490566,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.FunctionNode;25;464,368;Inherit;True;NormalCreate;1;;1;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;23;563.9,-35.1;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Depth;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;7;0
WireConnection;5;1;8;0
WireConnection;9;0;5;0
WireConnection;9;1;2;0
WireConnection;1;0;4;0
WireConnection;25;1;26;0
WireConnection;25;4;27;0
WireConnection;23;0;1;0
WireConnection;23;1;25;0
WireConnection;23;2;24;0
WireConnection;23;11;9;0
ASEEND*/
//CHKSM=CBB7227C813ACC4B03FFE9AC4EE892FE8F5735A7