// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Davis3D/Backrooms/Carpet"
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
		_Roughness_A("Roughness_A", Float) = 0
		_Roughness_B("Roughness_B", Float) = 1
		_Macro_Roughness("Macro_Roughness", Float) = 0
		[Toggle(_PARALLAX_ON)] _Parallax("Parallax", Float) = 0
		_Displacement("Displacement", 2D) = "black" {}
		_Parallax_Amount("Parallax_Amount", Float) = 1
		_ReferencePlane("ReferencePlane", Float) = 0.5
		[Toggle(_FUZZYSHADING_ON)] _FuzzyShading("FuzzyShading", Float) = 0
		_Fuzzy_Brightness("Fuzzy_Brightness", Float) = 0.8
		_Fuzzy_Darkness("Fuzzy_Darkness", Float) = 0.8
		_Fuzzy_EdgeDesat("Fuzzy_EdgeDesat", Float) = 0.5
		_Fuzzy_Power("Fuzzy_Power", Float) = 0
		_Tiling("Tiling", Float) = 100
		_Macro_Tiling("Macro_Tiling", Float) = 300
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
		#pragma shader_feature_local _PARALLAX_ON
		#pragma shader_feature_local _FUZZYSHADING_ON
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
		uniform float _Tiling;
		uniform float _NormIntensity;
		uniform sampler2D _Displacement;
		uniform float _Parallax_Amount;
		uniform float _ReferencePlane;
		uniform float4 _Displacement_ST;
		uniform float4 _Color;
		uniform float _Brightness;
		uniform float _Contrast;
		uniform sampler2D _Diffuse;
		uniform float _Desaturation;
		uniform float _Fuzzy_Darkness;
		uniform float _Fuzzy_EdgeDesat;
		uniform float _Fuzzy_Power;
		uniform float _Fuzzy_Brightness;
		uniform sampler2D _Macro;
		uniform float _Macro_Tiling;
		uniform float _Macro_Dif;
		uniform float _Roughness_A;
		uniform float _Roughness_B;
		uniform float _Macro_Roughness;


		inline float3 TriplanarSampling58( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			xNorm.xyz  = half3( UnpackScaleNormal( xNorm, normalScale.y ).xy * float2(  nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz  = half3( UnpackScaleNormal( yNorm, normalScale.x ).xy * float2(  nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz  = half3( UnpackScaleNormal( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		float3 WorldToAbsoluteWorld3_g5( float3 In )
		{
			#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
			    In += _WorldSpaceCameraPos.xyz;
			#endif
			return In;
		}


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs.xy += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
			 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).g;
			 	if ( currHeight > currRayZ )
			 	{
			 	 	stepIndex = numSteps + 1;
			 	}
			 	else
			 	{
			 	 	stepIndex++;
			 	 	prevTexOffset = currTexOffset;
			 	 	prevRayZ = currRayZ;
			 	 	prevHeight = currHeight;
			 	 	currTexOffset += deltaTex;
			 	 	currRayZ -= layerHeight;
			 	}
			}
			int sectionSteps = 2;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
			 	intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
			 	finalTexOffset = prevTexOffset + intersection * deltaTex;
			 	newZ = prevRayZ - intersection * layerHeight;
			 	newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).g;
			 	if ( newHeight > newZ )
			 	{
			 	 	currTexOffset = finalTexOffset;
			 	 	currHeight = newHeight;
			 	 	currRayZ = newZ;
			 	 	deltaTex = intersection * deltaTex;
			 	 	layerHeight = intersection * layerHeight;
			 	}
			 	else
			 	{
			 	 	prevTexOffset = finalTexOffset;
			 	 	prevHeight = newHeight;
			 	 	prevRayZ = newZ;
			 	 	deltaTex = ( 1 - intersection ) * deltaTex;
			 	 	layerHeight = ( 1 - intersection ) * layerHeight;
			 	}
			 	sectionIndex++;
			}
			return uvs.xy + finalTexOffset;
		}


		inline float4 TriplanarSampling53( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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

		inline float4 TriplanarSampling48( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling10( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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
			float temp_output_64_0 = ( _Tiling / 100.0 );
			float2 temp_cast_0 = (temp_output_64_0).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar58 = TriplanarSampling58( _Normal, ase_worldPos, ase_worldNormal, 1.0, temp_cast_0, _NormIntensity, 0 );
			float3 tanTriplanarNormal58 = mul( ase_worldToTangent, triplanar58 );
			float3 In3_g5 = ase_worldPos;
			float3 localWorldToAbsoluteWorld3_g5 = WorldToAbsoluteWorld3_g5( In3_g5 );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_tanViewDir = mul( ase_worldToTangent, ase_worldViewDir );
			float2 OffsetPOM37 = POM( _Displacement, ( (localWorldToAbsoluteWorld3_g5).xz / temp_output_64_0 ), ddx(( (localWorldToAbsoluteWorld3_g5).xz / temp_output_64_0 )), ddy(( (localWorldToAbsoluteWorld3_g5).xz / temp_output_64_0 )), ase_worldNormal, ase_worldViewDir, ase_tanViewDir, 8, 8, ( _Parallax_Amount * 0.01 ), _ReferencePlane, _Displacement_ST.xy, float2(100,100), 0 );
			float3 lerpResult60 = lerp( float3( 0,0,1 ) , UnpackNormal( tex2D( _Normal, OffsetPOM37 ) ) , _NormIntensity);
			#ifdef _PARALLAX_ON
				float3 staticSwitch62 = lerpResult60;
			#else
				float3 staticSwitch62 = tanTriplanarNormal58;
			#endif
			o.Normal = staticSwitch62;
			float2 temp_cast_1 = (temp_output_64_0).xx;
			float4 triplanar53 = TriplanarSampling53( _Diffuse, ase_worldPos, ase_worldNormal, 1.0, temp_cast_1, 1.0, 0 );
			#ifdef _PARALLAX_ON
				float4 staticSwitch56 = tex2D( _Diffuse, OffsetPOM37 );
			#else
				float4 staticSwitch56 = triplanar53;
			#endif
			float3 desaturateInitialColor18 = CalculateContrast(_Contrast,staticSwitch56).rgb;
			float desaturateDot18 = dot( desaturateInitialColor18, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar18 = lerp( desaturateInitialColor18, desaturateDot18.xxx, _Desaturation );
			float4 temp_output_28_0 = ( _Color * float4( ( _Brightness * desaturateVar18 ) , 0.0 ) );
			float3 appendResult2_g6 = (float3(1.0 , 1.0 , i.ASEIsFrontFacing));
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentToWorldDir10_g6 = mul( ase_tangentToWorldFast, ( float3( 0,0,1 ) * appendResult2_g6 ) );
			float dotResult12_g6 = dot( ase_worldViewDir , tangentToWorldDir10_g6 );
			float clampResult13_g6 = clamp( dotResult12_g6 , 0.0 , 1.0 );
			float3 temp_output_26_0_g6 = temp_output_28_0.rgb;
			float3 desaturateInitialColor27_g6 = temp_output_26_0_g6;
			float desaturateDot27_g6 = dot( desaturateInitialColor27_g6, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar27_g6 = lerp( desaturateInitialColor27_g6, desaturateDot27_g6.xxx, _Fuzzy_EdgeDesat );
			float clampResult21_g6 = clamp( ( pow( ( 1.0 - clampResult13_g6 ) , _Fuzzy_Power ) * _Fuzzy_Brightness ) , 0.0 , 1.0 );
			float3 lerpResult23_g6 = lerp( ( ( 1.0 - ( clampResult13_g6 * _Fuzzy_Darkness ) ) * temp_output_26_0_g6 ) , ( desaturateVar27_g6 * float3( 1.5,1.5,1.5 ) ) , clampResult21_g6);
			#ifdef _FUZZYSHADING_ON
				float4 staticSwitch32 = float4( lerpResult23_g6 , 0.0 );
			#else
				float4 staticSwitch32 = temp_output_28_0;
			#endif
			float2 temp_cast_9 = (( _Macro_Tiling / 100.0 )).xx;
			float4 triplanar48 = TriplanarSampling48( _Macro, ase_worldPos, ase_worldNormal, 1.0, temp_cast_9, 1.0, 0 );
			float4 temp_output_49_0 = ( triplanar48 + float4( 0.5,0.5,0.5,0.5 ) );
			float4 lerpResult35 = lerp( staticSwitch32 , ( staticSwitch32 * temp_output_49_0 ) , _Macro_Dif);
			o.Albedo = lerpResult35.rgb;
			float2 temp_cast_12 = (temp_output_64_0).xx;
			float4 triplanar10 = TriplanarSampling10( _Displacement, ase_worldPos, ase_worldNormal, 1.0, temp_cast_12, 1.0, 0 );
			#ifdef _PARALLAX_ON
				float staticSwitch43 = tex2D( _Displacement, OffsetPOM37 ).g;
			#else
				float staticSwitch43 = triplanar10.y;
			#endif
			float lerpResult30 = lerp( _Roughness_A , _Roughness_B , staticSwitch43);
			float4 temp_cast_13 = (lerpResult30).xxxx;
			float4 lerpResult52 = lerp( temp_cast_13 , ( lerpResult30 * temp_output_49_0 ) , _Macro_Roughness);
			o.Smoothness = ( 1.0 - lerpResult52 ).x;
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
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-4024.049,504.5823;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;3;-3752.103,878.2327;Inherit;False;Property;_Tiling;Tiling;21;0;Create;True;0;0;0;False;0;False;100;512;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;5;-3845.049,507.5823;Inherit;False;WorldToAbsoluteWorld;-1;;5;6d428b11c6ab4974e8382ebacb8c51f1;0;1;5;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;64;-3567.557,895.8148;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;70;-3567.89,505.501;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-3297.671,728.2562;Inherit;False;Property;_Parallax_Amount;Parallax_Amount;14;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-3057.671,764.2562;Inherit;False;Property;_ReferencePlane;ReferencePlane;15;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-3057.017,627.2725;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;36;-3220.039,507.2169;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;42;-3547.444,669.1934;Inherit;False;Tangent;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;38;-3185.446,266.651;Inherit;True;Property;_Displacement;Displacement;13;0;Create;True;0;0;0;False;0;False;abc00000000002230093908177291143;abc00000000002230093908177291143;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;54;-2677.075,835.8482;Inherit;True;Property;_Diffuse;Diffuse;4;0;Create;True;0;0;0;False;0;False;abc00000000002001208967323050580;abc00000000002001208967323050580;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;37;-2836.375,497.4662;Inherit;False;1;8;False;;32;False;;2;0.02;0;False;1,1;False;100,100;8;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;7;SAMPLERSTATE;;False;2;FLOAT;0.02;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT2;0,0;False;6;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;55;-2372.025,605.4852;Inherit;True;Property;_TextureSample1;Texture Sample 1;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1846.004,96.53168;Inherit;False;Property;_Contrast;Contrast;1;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;56;-2005.301,771.099;Inherit;False;Property;_Parallax;Parallax;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;43;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;15;-1702.004,20.53162;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1672.004,118.5317;Inherit;False;Property;_Desaturation;Desaturation;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1371.204,698.3644;Inherit;False;Property;_Macro_Tiling;Macro_Tiling;22;0;Create;True;0;0;0;False;0;False;300;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1482.004,-71.46835;Inherit;False;Property;_Brightness;Brightness;0;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;18;-1512.004,24.53162;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;22;-1397.74,-240.3262;Inherit;False;Property;_Color;Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1323.565,-36.68323;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;39;-2491.872,296.0503;Inherit;True;Property;_TextureSample0;Texture Sample 0;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;68;-1159.376,670.6756;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;10;-2550.44,97.44315;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;46;-1278.512,456.6616;Inherit;True;Property;_Macro;Macro;5;0;Create;True;0;0;0;False;0;False;abc00000000004444428409645600615;abc00000000004444428409645600615;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;26;-2056.079,299.9158;Inherit;False;Property;_Roughness_A;Roughness_A;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1241.729,237.3819;Inherit;False;Property;_Fuzzy_Brightness;Fuzzy_Brightness;17;0;Create;True;0;0;0;False;0;False;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1230.729,93.3819;Inherit;False;Property;_Fuzzy_Darkness;Fuzzy_Darkness;18;0;Create;True;0;0;0;False;0;False;0.8;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;43;-2058.108,190.7136;Inherit;False;Property;_Parallax;Parallax;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-1224.259,27.55475;Inherit;False;Property;_Fuzzy_EdgeDesat;Fuzzy_EdgeDesat;19;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1152.288,-73.09616;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;1,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-1210.729,166.3819;Inherit;False;Property;_Fuzzy_Power;Fuzzy_Power;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;48;-1010.911,455.5972;Inherit;True;Spherical;World;False;Top Texture 3;_TopTexture3;white;-1;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-2057.079,377.9158;Inherit;False;Property;_Roughness_B;Roughness_B;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;30;-1853.121,347.7542;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;31;-974.7283,22.38184;Inherit;False;FuzzyShadingGrass;-1;;6;8d9ecc9cff62bbe4aad0d715e0768ba6;0;7;28;FLOAT;0.5;False;30;FLOAT3;1.5,1.5,1.5;False;26;FLOAT3;0.5,0.5,0.5;False;5;FLOAT3;0,0,1;False;16;FLOAT;0.8;False;19;FLOAT;6;False;22;FLOAT;0.8;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-619.1504,455.7289;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.5,0.5,0.5,0.5;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-490.5235,426.42;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;32;-698.1255,-79.62836;Inherit;False;Property;_FuzzyShading;FuzzyShading;16;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-620.0564,561.4993;Inherit;False;Property;_Macro_Roughness;Macro_Roughness;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;52;-343.714,401.9846;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TriplanarNode;58;-2438.318,1399.934;Inherit;True;Spherical;World;True;Top Texture 5;_TopTexture5;white;1;None;Mid Texture 5;_MidTexture5;white;-1;None;Bot Texture 5;_BotTexture5;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-468.0103,95.36963;Inherit;False;Property;_Macro_Dif;Macro_Dif;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-454.2308,5.803866;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;35;-275.0995,-31.40048;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;62;-1874.775,1316.748;Inherit;False;Property;_Parallax;Parallax;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;43;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;44;-191.1269,404.8961;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Davis3D/Backrooms/Carpet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TexturePropertyNode;57;-2690.124,1396.906;Inherit;True;Property;_Normal;Normal;7;0;Create;True;0;0;0;False;0;False;abc00000000010675937737126691446;abc00000000010675937737126691446;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;61;-2367.321,1170.962;Inherit;True;Property;_TextureSample2;Texture Sample 2;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;60;-2090.048,1233.378;Inherit;False;3;0;FLOAT3;0,0,1;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-2672.75,1144.07;Inherit;False;Property;_NormIntensity;NormIntensity;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;53;-2419.119,820.4494;Inherit;True;Spherical;World;False;Top Texture 4;_TopTexture4;white;0;None;Mid Texture 4;_MidTexture4;white;-1;None;Bot Texture 4;_BotTexture4;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;5;5;1;0
WireConnection;64;0;3;0
WireConnection;70;0;5;0
WireConnection;71;0;40;0
WireConnection;36;0;70;0
WireConnection;36;1;64;0
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;37;7;38;1
WireConnection;37;2;71;0
WireConnection;37;3;42;0
WireConnection;37;4;41;0
WireConnection;55;0;54;0
WireConnection;55;1;37;0
WireConnection;55;7;54;1
WireConnection;56;1;53;0
WireConnection;56;0;55;0
WireConnection;15;1;56;0
WireConnection;15;0;11;0
WireConnection;18;0;15;0
WireConnection;18;1;13;0
WireConnection;21;0;17;0
WireConnection;21;1;18;0
WireConnection;39;0;38;0
WireConnection;39;1;37;0
WireConnection;39;7;38;1
WireConnection;68;0;67;0
WireConnection;10;0;38;0
WireConnection;10;3;64;0
WireConnection;43;1;10;2
WireConnection;43;0;39;2
WireConnection;28;0;22;0
WireConnection;28;1;21;0
WireConnection;48;0;46;0
WireConnection;48;3;68;0
WireConnection;30;0;26;0
WireConnection;30;1;23;0
WireConnection;30;2;43;0
WireConnection;31;28;66;0
WireConnection;31;26;28;0
WireConnection;31;16;24;0
WireConnection;31;19;25;0
WireConnection;31;22;27;0
WireConnection;49;0;48;0
WireConnection;50;0;30;0
WireConnection;50;1;49;0
WireConnection;32;1;28;0
WireConnection;32;0;31;0
WireConnection;52;0;30;0
WireConnection;52;1;50;0
WireConnection;52;2;51;0
WireConnection;58;0;57;0
WireConnection;58;8;59;0
WireConnection;58;3;64;0
WireConnection;34;0;32;0
WireConnection;34;1;49;0
WireConnection;35;0;32;0
WireConnection;35;1;34;0
WireConnection;35;2;33;0
WireConnection;62;1;58;0
WireConnection;62;0;60;0
WireConnection;44;0;52;0
WireConnection;0;0;35;0
WireConnection;0;1;62;0
WireConnection;0;4;44;0
WireConnection;61;0;57;0
WireConnection;61;1;37;0
WireConnection;61;7;57;1
WireConnection;60;1;61;0
WireConnection;60;2;59;0
WireConnection;53;0;54;0
WireConnection;53;3;64;0
ASEEND*/
//CHKSM=EB1FFD87044B4174B76F4CDE3A43F1B929267C5C