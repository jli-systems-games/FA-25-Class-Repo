// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Davis3D/Backrooms/Light"
{
	Properties
	{
		_Color("Color", Color) = (0.8352941,0.8313726,0.7568628,1)
		[Toggle(_FLICKERING_ON)] _Flickering("Flickering", Float) = 0
		_Brightness("Brightness", Float) = 6
		_Flicker("_Flicker", Float) = 0
		_MinIntensity("MinIntensity", Float) = 0.01
		_MaxIntensity("MaxIntensity", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _FLICKERING_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			half filler;
		};

		uniform float4 _Color;
		uniform float _Brightness;
		uniform float _Flicker;
		uniform float _MinIntensity;
		uniform float _MaxIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Color.rgb;
			#ifdef _FLICKERING_ON
				float staticSwitch3 = (0.01 + (_Flicker - _MinIntensity) * (_Brightness - 0.01) / (_MaxIntensity - _MinIntensity));
			#else
				float staticSwitch3 = _Brightness;
			#endif
			float3 temp_cast_1 = (staticSwitch3).xxx;
			o.Emission = temp_cast_1;
			o.Smoothness = 0.7;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
748;172;1055;698;1357.684;411.3266;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-971.6046,-104.8752;Inherit;False;Property;_Brightness;Brightness;2;0;Create;True;0;0;0;False;0;False;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-982.9564,-353.9892;Inherit;False;Property;_Flicker;_Flicker;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-992.684,-220.3266;Inherit;False;Property;_MaxIntensity;MaxIntensity;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-996.6841,-290.3266;Inherit;False;Property;_MinIntensity;MinIntensity;4;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;29;-795.684,-262.3266;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.01;False;2;FLOAT;1;False;3;FLOAT;0.01;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-976.2656,437.7652;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;17;-1109.978,61.59573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-957.0869,292.957;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-963.0869,160.957;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;13;-1784.488,269.0572;Inherit;False;1;0;FLOAT;6.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1401.878,60.59573;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.514;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1432.637,423.9959;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.354;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1334.737,541.996;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.557;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;22;-1283.737,424.9959;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1303.978,178.5957;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2.12;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;14;-1105.087,293.957;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;18;-1252.978,61.59573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;7;-833.0869,290.957;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;19;-1152.978,176.5957;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;16;-847.2656,438.7652;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;6;-683.0869,193.957;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;-519.6046,97.12482;Inherit;False;3;0;FLOAT;0.01;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-402.6046,-266.8752;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;0.8352941,0.8313726,0.7568628,1;0.8352941,0.8313726,0.7568628,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1;-162.6046,135.1248;Inherit;False;Constant;_07;0.7;0;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;23;-1140.737,424.9959;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3;-362.6046,31.12482;Inherit;False;Property;_Flickering;Flickering;1;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;24;-1183.737,539.996;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;8;-834.0869,161.957;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Davis3D/Backrooms/Light;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;27;0
WireConnection;29;1;30;0
WireConnection;29;2;31;0
WireConnection;29;4;4;0
WireConnection;15;0;23;0
WireConnection;15;1;24;0
WireConnection;17;0;18;0
WireConnection;9;0;14;0
WireConnection;10;0;17;0
WireConnection;10;1;19;0
WireConnection;20;0;13;0
WireConnection;25;0;13;0
WireConnection;26;0;13;0
WireConnection;22;0;25;0
WireConnection;21;0;13;0
WireConnection;14;0;13;0
WireConnection;18;0;20;0
WireConnection;7;0;9;0
WireConnection;19;0;21;0
WireConnection;16;0;15;0
WireConnection;6;0;8;0
WireConnection;6;1;16;0
WireConnection;6;2;7;0
WireConnection;5;1;4;0
WireConnection;5;2;6;0
WireConnection;23;0;22;0
WireConnection;3;1;4;0
WireConnection;3;0;29;0
WireConnection;24;0;26;0
WireConnection;8;0;10;0
WireConnection;0;0;2;0
WireConnection;0;2;3;0
WireConnection;0;4;1;0
ASEEND*/
//CHKSM=C299C2EBA651EF47E44EFA4898EFA5F03A054710