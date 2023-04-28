// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transparent"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Tint("Tint", Color) = (0,0,0,0)
		_Stretch("Stretch", Vector) = (1,0,0,0)
		[ToggleUI]_UseVertexColor("UseVertexColor", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow nometa noforwardadd 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float _UseVertexColor;
		uniform float4 _Tint;
		uniform sampler2D _MainTex;
		uniform float2 _Stretch;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = (( _UseVertexColor )?( i.vertexColor ):( _Tint )).rgb;
			float2 appendResult11 = (float2(_Stretch.x , 1.0));
			float2 appendResult12 = (float2(_Stretch.y , 0.0));
			float2 uv_TexCoord5 = i.uv_texcoord * appendResult11 + appendResult12;
			o.Alpha = ( ( tex2D( _MainTex, uv_TexCoord5 ).r * i.vertexColor.a ) * _Tint.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.ColorNode;4;-526.9813,187.4485;Inherit;False;Property;_Tint;Tint;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-738.1281,123.3784;Inherit;False;Property;_Alpha;Alpha;2;0;Create;True;0;0;0;False;0;False;0;0.431;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;7;-473.9087,-295.1269;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-162.6086,-71.92703;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;30.18408,-62.77979;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-652.0521,-102.0235;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;759818ed10d682c418d63648c16565a6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1055.128,-203.2216;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;12;-1227.564,-93.33917;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;10;-1419.564,-161.3392;Inherit;False;Property;_Stretch;Stretch;3;0;Create;True;0;0;0;False;0;False;1,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;11;-1225.564,-218.3392;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;392,-82;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Transparent;False;False;False;False;False;False;False;False;False;False;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ToggleSwitchNode;13;105.436,-202.3392;Inherit;False;Property;_UseVertexColor;UseVertexColor;4;0;Create;True;0;0;0;False;0;False;0;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;8;0;3;1
WireConnection;8;1;7;4
WireConnection;9;0;8;0
WireConnection;9;1;4;4
WireConnection;3;1;5;0
WireConnection;5;0;11;0
WireConnection;5;1;12;0
WireConnection;12;0;10;2
WireConnection;11;0;10;1
WireConnection;2;2;13;0
WireConnection;2;9;9;0
WireConnection;13;0;4;0
WireConnection;13;1;7;0
ASEEND*/
//CHKSM=E9CF321B628F54ED6E8D08EBF18135BC54AB44C3