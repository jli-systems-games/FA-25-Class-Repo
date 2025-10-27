// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Davis3D/Backrooms/Walls"
{
	Properties
	{
		_Brightness("Brightness", Float) = 1
		_Contrast("Contrast", Float) = 1
		_Desaturation("Desaturation", Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_Diffuse("Diffuse", 2D) = "white" {}
		_Macro("Macro", 2D) = "white" {}
		_Macro_Dif("Macro_Dif", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		_NormIntensity("NormIntensity", Float) = 1
		_Roughness("Roughness", 2D) = "gray" {}
		[Toggle(_ROUGHNESSFROMDIFFUSE_ON)] _RoughnessFromDiffuse("RoughnessFromDiffuse", Float) = 0
		_Roughness_A("Roughness_A", Float) = 0
		_Roughness_B("Roughness_B", Float) = 1
		_Macro_Roughness("Macro_Roughness", Float) = 0
		[Toggle(_MACROROUGHNESSOFF_ON)] _MacroRoughnessOFF("Macro Roughness OFF?", Float) = 1
		[Toggle(_FUZZYSHADING_ON)] _FuzzyShading("FuzzyShading", Float) = 0
		_Fuzzy_Brightness("Fuzzy_Brightness", Float) = 0.8
		_Fuzzy_Darkness("Fuzzy_Darkness", Float) = 0.8
		_Fuzzy_Power("Fuzzy_Power", Float) = 0
		_TilingMaster("TilingMaster", Float) = 512
		_Dif_Tiling("Dif_Tiling", Float) = 1
		_Macro_Tiling("Macro_Tiling", Float) = 300
		_Norm_Tiling("Norm_Tiling", Float) = 1
		_Roughness_Tiling("Roughness_Tiling", Float) = 1
		_Offset("Offset", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _FUZZYSHADING_ON
		#pragma shader_feature_local _MACROROUGHNESSOFF_ON
		#pragma shader_feature_local _ROUGHNESSFROMDIFFUSE_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			half ASEIsFrontFacing : VFACE;
		};

		uniform sampler2D _Normal;
		uniform float _TilingMaster;
		uniform float _Norm_Tiling;
		uniform float3 _Offset;
		uniform float _NormIntensity;
		uniform float4 _Color;
		uniform float _Brightness;
		uniform float _Contrast;
		uniform sampler2D _Diffuse;
		uniform float _Dif_Tiling;
		uniform float _Desaturation;
		uniform float _Fuzzy_Darkness;
		uniform float _Fuzzy_Power;
		uniform float _Fuzzy_Brightness;
		uniform sampler2D _Macro;
		uniform float _Macro_Tiling;
		uniform float _Macro_Dif;
		uniform float _Roughness_A;
		uniform float _Roughness_B;
		uniform sampler2D _Roughness;
		uniform float _Roughness_Tiling;
		uniform float _Macro_Roughness;


		float3 WorldToAbsoluteWorld3_g5( float3 In )
		{
			#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
			    In += _WorldSpaceCameraPos.xyz;
			#endif
			return In;
		}


		inline float3 TriplanarSampling47( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			xNorm.xyz  = half3( UnpackNormal( xNorm ).xy * float2(  nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz  = half3( UnpackNormal( yNorm ).xy * float2(  nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz  = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSampling19( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline float4 TriplanarSampling41( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling29( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (( ( _TilingMaster * _Norm_Tiling ) / 1000.0 )).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 In3_g5 = ase_worldPos;
			float3 localWorldToAbsoluteWorld3_g5 = WorldToAbsoluteWorld3_g5( In3_g5 );
			float3 temp_output_23_0 = ( _Offset + localWorldToAbsoluteWorld3_g5 );
			float3 triplanar47 = TriplanarSampling47( _Normal, temp_output_23_0, ase_worldNormal, 1.0, temp_cast_0, 1.0, 0 );
			float3 tanTriplanarNormal47 = mul( ase_worldToTangent, triplanar47 );
			float3 lerpResult48 = lerp( float3( 0,0,1 ) , tanTriplanarNormal47 , _NormIntensity);
			o.Normal = lerpResult48;
			float2 temp_cast_1 = (( ( _Dif_Tiling * _TilingMaster ) / 1000.0 )).xx;
			float4 triplanar19 = TriplanarSampling19( _Diffuse, temp_output_23_0, ase_worldNormal, 1.0, temp_cast_1, 1.0, 0 );
			float3 desaturateInitialColor14 = CalculateContrast(_Contrast,triplanar19).rgb;
			float desaturateDot14 = dot( desaturateInitialColor14, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar14 = lerp( desaturateInitialColor14, desaturateDot14.xxx, _Desaturation );
			float4 temp_output_10_0 = ( _Color * float4( ( _Brightness * desaturateVar14 ) , 0.0 ) );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 appendResult2_g6 = (float3(1.0 , 1.0 , i.ASEIsFrontFacing));
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentToWorldDir10_g6 = mul( ase_tangentToWorldFast, ( float3( 0,0,1 ) * appendResult2_g6 ) );
			float dotResult12_g6 = dot( ase_worldViewDir , tangentToWorldDir10_g6 );
			float clampResult13_g6 = clamp( dotResult12_g6 , 0.0 , 1.0 );
			float3 temp_output_26_0_g6 = temp_output_10_0.rgb;
			float3 desaturateInitialColor27_g6 = temp_output_26_0_g6;
			float desaturateDot27_g6 = dot( desaturateInitialColor27_g6, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar27_g6 = lerp( desaturateInitialColor27_g6, desaturateDot27_g6.xxx, 0.5 );
			float clampResult21_g6 = clamp( ( pow( ( 1.0 - clampResult13_g6 ) , _Fuzzy_Power ) * _Fuzzy_Brightness ) , 0.0 , 1.0 );
			float3 lerpResult23_g6 = lerp( ( ( 1.0 - ( clampResult13_g6 * _Fuzzy_Darkness ) ) * temp_output_26_0_g6 ) , ( desaturateVar27_g6 * float3( 1.5,1.5,1.5 ) ) , clampResult21_g6);
			#ifdef _FUZZYSHADING_ON
				float4 staticSwitch4 = float4( lerpResult23_g6 , 0.0 );
			#else
				float4 staticSwitch4 = temp_output_10_0;
			#endif
			float2 temp_cast_8 = (( _Macro_Tiling / 1000.0 )).xx;
			float4 triplanar41 = TriplanarSampling41( _Macro, ase_worldPos, ase_worldNormal, 1.0, temp_cast_8, 1.0, 0 );
			float4 temp_output_42_0 = ( triplanar41 + float4( 0.5,0.5,0.5,0.5 ) );
			float4 lerpResult1 = lerp( staticSwitch4 , ( staticSwitch4 * temp_output_42_0 ) , _Macro_Dif);
			o.Albedo = lerpResult1.rgb;
			float2 temp_cast_11 = (( ( _TilingMaster * _Roughness_Tiling ) / 1000.0 )).xx;
			float4 triplanar29 = TriplanarSampling29( _Roughness, temp_output_23_0, ase_worldNormal, 1.0, temp_cast_11, 1.0, 0 );
			#ifdef _ROUGHNESSFROMDIFFUSE_ON
				float staticSwitch34 = triplanar19.g;
			#else
				float staticSwitch34 = triplanar29.y;
			#endif
			float lerpResult35 = lerp( _Roughness_A , _Roughness_B , staticSwitch34);
			float4 temp_cast_12 = (lerpResult35).xxxx;
			float4 lerpResult44 = lerp( temp_cast_12 , ( lerpResult35 * temp_output_42_0 ) , _Macro_Roughness);
			float4 temp_cast_13 = (lerpResult35).xxxx;
			#ifdef _MACROROUGHNESSOFF_ON
				float4 staticSwitch57 = temp_cast_13;
			#else
				float4 staticSwitch57 = ( 1.0 - lerpResult44 );
			#endif
			o.Smoothness = staticSwitch57.x;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-3279.354,486.2396;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;-2938.566,257.1871;Inherit;False;Property;_TilingMaster;TilingMaster;19;0;Create;True;0;0;0;False;0;False;512;512;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2838.717,107.6469;Inherit;False;Property;_Dif_Tiling;Dif_Tiling;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2686.863,126.6964;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;21;-3102.354,485.2396;Inherit;False;WorldToAbsoluteWorld;-1;;5;6d428b11c6ab4974e8382ebacb8c51f1;0;1;5;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-2531.708,48.99323;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;20;-2451.029,-186.8596;Inherit;True;Property;_Diffuse;Diffuse;4;0;Create;True;0;0;0;False;0;False;abc00000000006089472395180396765;abc00000000006089472395180396765;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-2860.354,427.2396;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1768.029,5.140411;Inherit;False;Property;_Contrast;Contrast;1;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-2655.75,507.4238;Inherit;False;Property;_Roughness_Tiling;Roughness_Tiling;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;19;-2189.029,-179.8596;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-1594.029,27.14041;Inherit;False;Property;_Desaturation;Desaturation;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2469.727,453.1216;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;17;-1624.029,-70.85959;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;32;-2309.75,437.4238;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1404.029,-162.8596;Inherit;False;Property;_Brightness;Brightness;0;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;30;-2452.629,156.5048;Inherit;True;Property;_Roughness;Roughness;9;0;Create;True;0;0;0;False;0;False;abc00000000017441329500566613936;abc00000000017441329500566613936;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DesaturateOpNode;14;-1434.029,-66.85959;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1396.257,698.3315;Inherit;False;Property;_Macro_Tiling;Macro_Tiling;21;0;Create;True;0;0;0;False;0;False;300;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;39;-1370.608,821.0087;Inherit;True;Property;_Macro;Macro;5;0;Create;True;0;0;0;False;0;False;abc00000000004444428409645600615;abc00000000004444428409645600615;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleDivideOpNode;40;-1205.137,698.3453;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;29;-2173.786,325.3948;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;0;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1245.59,-128.0744;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;11;-1319.765,-331.7175;Inherit;False;Property;_Color;Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1163.754,145.9906;Inherit;False;Property;_Fuzzy_Brightness;Fuzzy_Brightness;16;0;Create;True;0;0;0;False;0;False;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1074.313,-164.4874;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;1,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;34;-1708.933,393.1798;Inherit;False;Property;_RoughnessFromDiffuse;RoughnessFromDiffuse;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1631.523,233.2803;Inherit;False;Property;_Roughness_A;Roughness_A;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;41;-1044.778,640.8899;Inherit;True;Spherical;World;False;Top Texture 2;_TopTexture2;white;-1;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1132.754,74.9906;Inherit;False;Property;_Fuzzy_Power;Fuzzy_Power;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2675.834,920.0839;Inherit;False;Property;_Norm_Tiling;Norm_Tiling;22;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1632.523,311.2803;Inherit;False;Property;_Roughness_B;Roughness_B;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1152.754,1.990601;Inherit;False;Property;_Fuzzy_Darkness;Fuzzy_Darkness;17;0;Create;True;0;0;0;False;0;False;0.8;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-653.0173,641.0216;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.5,0.5,0.5,0.5;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-2503.287,849.4827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;35;-1428.564,281.1187;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;6;-896.7535,-69.0094;Inherit;False;FuzzyShadingGrass;-1;;6;8d9ecc9cff62bbe4aad0d715e0768ba6;0;7;28;FLOAT;0.5;False;30;FLOAT3;1.5,1.5,1.5;False;26;FLOAT3;0.5,0.5,0.5;False;5;FLOAT3;0,0,1;False;16;FLOAT;0.8;False;19;FLOAT;6;False;22;FLOAT;0.8;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-653.9233,746.792;Inherit;False;Property;_Macro_Roughness;Macro_Roughness;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-524.3905,611.7127;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;54;-2343.21,910.1287;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;51;-2357.136,651.5496;Inherit;True;Property;_Normal;Normal;7;0;Create;True;0;0;0;False;0;False;abc00000000010675937737126691446;abc00000000010675937737126691446;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TriplanarNode;47;-2048.237,664.6529;Inherit;True;Spherical;World;True;Top Texture 3;_TopTexture3;white;1;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-376.256,-85.58742;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-390.0355,3.978354;Inherit;False;Property;_Macro_Dif;Macro_Dif;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1804.198,856.7673;Inherit;False;Property;_NormIntensity;NormIntensity;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;-377.581,587.2772;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;1;-197.1247,-122.7917;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;48;-1640.967,635.0969;Inherit;False;3;0;FLOAT3;0,0,1;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Davis3D/Backrooms/Walls;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.StaticSwitch;4;-620.1506,-171.0196;Inherit;False;Property;_FuzzyShading;FuzzyShading;15;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;24;-3108.354,310.2396;Inherit;False;Property;_Offset;Offset;24;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;57;-294.2927,305.7996;Inherit;False;Property;_MacroRoughnessOFF;Macro Roughness OFF?;14;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;46;-224.994,590.1888;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
WireConnection;26;0;27;0
WireConnection;26;1;25;0
WireConnection;21;5;22;0
WireConnection;31;0;26;0
WireConnection;23;0;24;0
WireConnection;23;1;21;0
WireConnection;19;0;20;0
WireConnection;19;9;23;0
WireConnection;19;3;31;0
WireConnection;28;0;25;0
WireConnection;28;1;33;0
WireConnection;17;1;19;0
WireConnection;17;0;18;0
WireConnection;32;0;28;0
WireConnection;14;0;17;0
WireConnection;14;1;16;0
WireConnection;40;0;38;0
WireConnection;29;0;30;0
WireConnection;29;9;23;0
WireConnection;29;3;32;0
WireConnection;12;0;13;0
WireConnection;12;1;14;0
WireConnection;10;0;11;0
WireConnection;10;1;12;0
WireConnection;34;1;29;2
WireConnection;34;0;19;2
WireConnection;41;0;39;0
WireConnection;41;3;40;0
WireConnection;42;0;41;0
WireConnection;53;0;25;0
WireConnection;53;1;52;0
WireConnection;35;0;36;0
WireConnection;35;1;37;0
WireConnection;35;2;34;0
WireConnection;6;26;10;0
WireConnection;6;16;7;0
WireConnection;6;19;8;0
WireConnection;6;22;9;0
WireConnection;43;0;35;0
WireConnection;43;1;42;0
WireConnection;54;0;53;0
WireConnection;47;0;51;0
WireConnection;47;9;23;0
WireConnection;47;3;54;0
WireConnection;3;0;4;0
WireConnection;3;1;42;0
WireConnection;44;0;35;0
WireConnection;44;1;43;0
WireConnection;44;2;45;0
WireConnection;1;0;4;0
WireConnection;1;1;3;0
WireConnection;1;2;2;0
WireConnection;48;1;47;0
WireConnection;48;2;49;0
WireConnection;0;0;1;0
WireConnection;0;1;48;0
WireConnection;0;4;57;0
WireConnection;4;1;10;0
WireConnection;4;0;6;0
WireConnection;57;1;46;0
WireConnection;57;0;35;0
WireConnection;46;0;44;0
ASEEND*/
//CHKSM=E84F10143AF227CF9AF339629873D74CA7F53431