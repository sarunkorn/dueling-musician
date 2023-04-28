// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Character"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Desat("Desat", Float) = 0
		[ToggleUI]_IsUnlit("IsUnlit", Float) = 1
		[ToggleUI]_Usediffuse("Usediffuse", Float) = 1
		_Contrast("Contrast", Float) = 0
		_GroundShadow("GroundShadow", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 vertexColor : COLOR;
			float3 worldPos;
		};

		uniform float _Usediffuse;
		uniform float4 _Color;
		uniform float _Contrast;
		uniform float2 _GroundShadow;
		uniform float _IsUnlit;
		uniform float _Desat;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult23 = clamp( (ase_vertex3Pos.y*_GroundShadow.x + _GroundShadow.y) , 0.0 , 1.0 );
			float4 temp_output_2_0 = ( _Color * CalculateContrast(_Contrast,i.vertexColor) * clampResult23 );
			o.Albedo = (( _Usediffuse )?( float4( 0,0,0,0 ) ):( ( temp_output_2_0 * 0.4 ) )).rgb;
			float3 desaturateInitialColor12 = ( temp_output_2_0 * 0.4 ).rgb;
			float desaturateDot12 = dot( desaturateInitialColor12, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar12 = lerp( desaturateInitialColor12, desaturateDot12.xxx, _Desat );
			o.Emission = (( _IsUnlit )?( temp_output_2_0 ):( float4( desaturateVar12 , 0.0 ) )).rgb;
			o.Metallic = 0.0;
			o.Smoothness = 0.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.SamplerNode;3;-927,-13.89999;Inherit;True;Property;_Texture;Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-820,-234.1;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-90,233.225;Inherit;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-92,140.225;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;5;-433,-132.55;Inherit;False;Property;_UseVertexColor;UseVertexColor;2;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-311,-271.9;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-218,-107.775;Inherit;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-17,7.224976;Inherit;False;Property;_Desat;Desat;3;0;Create;True;0;0;0;False;0;False;0;-0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;22,-322.775;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-49.69999,-107.275;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;12;134.3,-103.075;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;16;386.5904,-353.3951;Inherit;False;Property;_Usediffuse;Usediffuse;5;0;Create;True;0;0;0;False;0;False;1;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;14;467.1897,-198.0353;Inherit;False;Property;_IsUnlit;IsUnlit;4;0;Create;True;0;0;0;False;0;False;1;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1044,-260.3;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Character;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.VertexColorNode;4;-1105.573,-508.0681;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-833.5182,-712.668;Inherit;False;Property;_Contrast;Contrast;6;0;Create;True;0;0;0;False;0;False;0;1.63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-235.2819,-594.2933;Inherit;False;Constant;_Color0;Color 0;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;20;-862.4619,-990.1863;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;22;-856.3617,-844.8862;Inherit;False;Property;_GroundShadow;GroundShadow;7;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;21;-622.5619,-867.6863;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;17;-685.9093,-517.1951;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;23;-389.0044,-734.4055;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
WireConnection;5;1;4;0
WireConnection;2;0;1;0
WireConnection;2;1;17;0
WireConnection;2;2;23;0
WireConnection;9;0;2;0
WireConnection;9;1;10;0
WireConnection;11;0;2;0
WireConnection;11;1;10;0
WireConnection;12;0;11;0
WireConnection;12;1;13;0
WireConnection;16;0;9;0
WireConnection;14;0;12;0
WireConnection;14;1;2;0
WireConnection;0;0;16;0
WireConnection;0;2;14;0
WireConnection;0;3;7;0
WireConnection;0;4;8;0
WireConnection;21;0;20;2
WireConnection;21;1;22;1
WireConnection;21;2;22;2
WireConnection;17;1;4;0
WireConnection;17;0;18;0
WireConnection;23;0;21;0
ASEEND*/
//CHKSM=D6B2423667858043226E05D68390B8A6384FA7C9