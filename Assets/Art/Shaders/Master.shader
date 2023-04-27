// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Master"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_Texture("Texture", 2D) = "white" {}
		[Toggle]_UseVertexColor("UseVertexColor", Float) = 1
		[Toggle]_MultOverlay("MultOverlay", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Saturation("Saturation", Float) = 0
		_ScaleOffset("ScaleOffset", Vector) = (0,0,0,0)
		_ScaleOffset2("ScaleOffset2", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float4 vertexColor : COLOR;
		};

		uniform float _UseVertexColor;
		uniform float _MultOverlay;
		uniform float4 _Color;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform sampler2D _TextureSample0;
		uniform float2 _ScaleOffset;
		uniform float2 _ScaleOffset2;
		uniform float _Saturation;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 temp_output_2_0 = ( _Color * tex2D( _Texture, uv_Texture ) );
			float4 tex2DNode9 = tex2D( _TextureSample0, i.uv2_texcoord2 );
			float3 desaturateInitialColor25 = ( temp_output_2_0 * (tex2DNode9*_ScaleOffset.x + _ScaleOffset.y) ).rgb;
			float desaturateDot25 = dot( desaturateInitialColor25, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar25 = lerp( desaturateInitialColor25, desaturateDot25.xxx, (tex2DNode9*_ScaleOffset2.x + _ScaleOffset2.y).r );
			float3 desaturateInitialColor14 = (( _UseVertexColor )?( ( i.vertexColor * _Color ) ):( (( _MultOverlay )?( float4( desaturateVar25 , 0.0 ) ):( temp_output_2_0 )) )).rgb;
			float desaturateDot14 = dot( desaturateInitialColor14, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar14 = lerp( desaturateInitialColor14, desaturateDot14.xxx, ( _Saturation + 0.0 ) );
			o.Albedo = desaturateVar14;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.VertexColorNode;4;-871,-582.55;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-933,-404.1;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-646,-117.9;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-1172,-142.9;Inherit;True;Property;_Texture;Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-1416.863,150.7943;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-1160.928,145.2937;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;691.5833,-116.0646;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Master;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.DesaturateOpNode;14;401.328,-218.7554;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;272.536,-127.3602;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-397.6993,540.7748;Inherit;False;Property;_Mult;Mult;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;16;-739.6875,142.0379;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;17;-1021.733,368.7762;Inherit;False;Property;_ScaleOffset;ScaleOffset;7;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;23;-512.038,238.3094;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;24;-794.0834,465.0477;Inherit;False;Property;_ScaleOffset2;ScaleOffset2;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;15;305.2723,-382.3916;Inherit;False;Property;_Saturation;Saturation;6;0;Create;True;0;0;0;False;0;False;0;-0.19;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;5;55.63801,-334.1808;Inherit;False;Property;_UseVertexColor;UseVertexColor;2;0;Create;True;0;0;0;False;0;False;1;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;7;-132.8793,-150.3603;Inherit;False;Property;_MultOverlay;MultOverlay;3;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-479.5708,29.5206;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;25;-237.6229,44.49866;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-489.9197,-544.1738;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;9;1;13;0
WireConnection;0;0;14;0
WireConnection;14;0;5;0
WireConnection;14;1;19;0
WireConnection;19;0;15;0
WireConnection;16;0;9;0
WireConnection;16;1;17;1
WireConnection;16;2;17;2
WireConnection;23;0;9;0
WireConnection;23;1;24;1
WireConnection;23;2;24;2
WireConnection;5;0;7;0
WireConnection;5;1;26;0
WireConnection;7;0;2;0
WireConnection;7;1;25;0
WireConnection;6;0;2;0
WireConnection;6;1;16;0
WireConnection;25;0;6;0
WireConnection;25;1;23;0
WireConnection;26;0;4;0
WireConnection;26;1;1;0
ASEEND*/
//CHKSM=48E7325FE39ADF469AC3FCA65A7918AF46630EA9