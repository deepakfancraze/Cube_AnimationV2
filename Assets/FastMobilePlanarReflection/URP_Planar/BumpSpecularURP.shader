Shader "SupGames/PlanarReflectionURP/BumpedSpecular"
{
	Properties{
		_Color("Specular Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpTex("Normal Map", 2D) = "bump" {}
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Glossiness("Glossiness", Range(0.01,100)) = 0.03
		_Distort("Distort Amount", Range(0.01,50)) = 1
		_MaskTex("Mask Texture", 2D) = "white" {}
		_BlurAmount("Blur Amount", Range(0,10)) = 1
		[Toggle(RECEIVE_SHADOWS)]
		_ReceiveShadows("Recieve Shadows", Float) = 0
	}
	SubShader
	{
		Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
		LOD 250

		Pass {
			Tags { "LightMode" = "UniversalForward" }
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local BLUR
			#pragma shader_feature_local VRon
			#pragma shader_feature RECEIVE_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_BumpTex);
			SAMPLER(sampler_BumpTex);
			TEXTURE2D(_ReflectionTex);
			SAMPLER(sampler_ReflectionTex);
#ifdef VRon
			TEXTURE2D(_ReflectionTexRight);
			SAMPLER(sampler_ReflectionTexRight);
#endif
			TEXTURE2D(_MaskTex);
			SAMPLER(sampler_MaskTex);

			half4 _MainTex_ST;
			half4 _BumpTex_ST;
			half _Glossiness;
			half4 _SpecColor;
			half4 _Color;
			half _BlurAmount;
			half _RefAlpha;
			half _Distort;
			half4 _ReflectionTex_TexelSize;

			struct appdata
			{
				half4 pos : POSITION;
				half4 uv : TEXCOORD0;
				half4 normal : NORMAL;
				half4 tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				half4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
				half4 normal : TEXCOORD1;
				half4 tangent : TEXCOORD2;
				half4 bitangent : TEXCOORD3;
#ifdef LIGHTMAP_ON
				half3 lightmapUV : TEXCOORD4;
#else
				half4 vertexSH : TEXCOORD4;
#endif
#if defined(_MAIN_LIGHT_SHADOWS)
				half4 shadowCoord : TEXCOORD5;
#endif
#if defined(BLUR)
				half4 offset : TEXCOORD6;
#endif
#if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
				half4 lightData : TEXCOORD7;
#else
				half lightData : TEXCOORD7;
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata i)
			{
				v2f o = (v2f)0;
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_TRANSFER_INSTANCE_ID(i, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.uv.xy = TRANSFORM_TEX(i.uv.xy, _MainTex);
				half4 ws = mul(unity_ObjectToWorld, i.pos);
				half3 viewDirection = _WorldSpaceCameraPos - ws.xyz;
				o.normal = half4(normalize(mul(i.normal, unity_WorldToObject).xyz), viewDirection.x);
				o.tangent = half4(normalize(mul(unity_ObjectToWorld, half4(i.tangent.xyz, 0.0h)).xyz), viewDirection.y);
				o.bitangent = half4(cross(o.normal.xyz, o.tangent.xyz) * i.tangent.w * unity_WorldTransformParams.w, viewDirection.z);
				o.pos = mul(unity_MatrixVP, ws);
				half4 scrPos = ComputeScreenPos(o.pos);
				o.uv.zw = scrPos.xy;
#ifdef BLUR
				half2 offset = _ReflectionTex_TexelSize.xy * _BlurAmount;
				o.offset = half4(-offset, offset);
#endif
#ifdef _MAIN_LIGHT_SHADOWS
				o.shadowCoord = TransformWorldToShadowCoord(ws.xyz);
#endif
#ifdef LIGHTMAP_ON
				o.lightmapUV.xy = i.uv.zw * unity_LightmapST.xy + unity_LightmapST.zw;
				o.lightmapUV.z = scrPos.w;
#else
				o.vertexSH.xyz = SampleSHVertex(i.normal.xyz);
				o.vertexSH.w = scrPos.w;
#endif
#ifdef _ADDITIONAL_LIGHTS_VERTEX
				o.lightData = half4(0.0h, 0.0h, 0.0h, ComputeFogFactor(o.pos.z));
				uint lightsCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex)
				{
					Light light = GetAdditionalLight(lightIndex, ws.xyz);
					o.lightData.rgb += light.color * light.distanceAttenuation * saturate(dot(o.normal.xyz, light.direction));
				}
#endif
#ifdef _ADDITIONAL_LIGHTS
				o.lightData = half4(ws.xyz, ComputeFogFactor(o.pos.z));
#endif
#if !defined(_ADDITIONAL_LIGHTS) && !defined(_ADDITIONAL_LIGHTS_VERTEX)
				o.lightData = ComputeFogFactor(o.pos.z);
#endif
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				half4 encodedNormal = SAMPLE_TEXTURE2D(_BumpTex, sampler_BumpTex, _BumpTex_ST.xy * i.uv.xy + _BumpTex_ST.zw);
				half3 viewDirection = half3(i.normal.w, i.tangent.w, i.bitangent.w);
				half3 normalDirection = normalize(mul(UnpackNormal(encodedNormal), half3x3(i.tangent.xyz, i.bitangent.xyz, i.normal.xyz)));
				half3 diffuseReflection = _MainLightColor.rgb * saturate(dot(normalDirection, _MainLightPosition.xyz));
				half3 bakedGI = SAMPLE_GI(i.lightmapUV.xy, i.vertexSH.xyz, normalDirection);
#if defined(_MAIN_LIGHT_SHADOWS) && defined(RECEIVE_SHADOWS)
				half3 realtimeShadow = lerp(bakedGI, max(bakedGI - diffuseReflection * (1.0h - MainLightRealtimeShadow(i.shadowCoord)), _SubtractiveShadowColor.xyz), _MainLightShadowData.x);
				bakedGI = min(bakedGI, realtimeShadow);
#endif
				half3 specularReflection = _SpecColor.rgb * _MainLightColor.rgb * pow(saturate(dot(normalDirection, normalize(_MainLightPosition.xyz + viewDirection))), _Glossiness);
				half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
				half4 bump = SAMPLE_TEXTURE2D(_BumpTex, sampler_BumpTex, i.uv.xy);
				half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv.xy);
				i.uv.z += (bump.x - 0.5h) * _Distort;
				i.uv.w += (0.5h - bump.y) * _Distort;
				half w = 0.0h;
#ifdef LIGHTMAP_ON
				w = i.lightmapUV.z;
#else
				w = i.vertexSH.w;
#endif
				i.uv.zw /= w;
				half4 reflection = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.uv.zw);
#if defined(BLUR)
				i.offset /= w;
				i.offset = half4(i.uv.zz + i.offset.xz, i.uv.ww + i.offset.yw);
				reflection += SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.offset.xz);
				reflection += SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.offset.xw);
				reflection += SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.offset.yz);
				reflection += SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.offset.yw);
				reflection *= 0.2h;
#endif
#ifdef VRon
				half4 reflectionr = SAMPLE_TEXTURE2D(_ReflectionTexRight, sampler_ReflectionTexRight, i.uv.zw);
#ifdef BLUR
				reflectionr += SAMPLE_TEXTURE2D(_ReflectionTexRight, sampler_ReflectionTexRight, i.offset.xz);
				reflectionr += SAMPLE_TEXTURE2D(_ReflectionTexRight, sampler_ReflectionTexRight, i.offset.xw);
				reflectionr += SAMPLE_TEXTURE2D(_ReflectionTexRight, sampler_ReflectionTexRight, i.offset.yz);
				reflectionr += SAMPLE_TEXTURE2D(_ReflectionTexRight, sampler_ReflectionTexRight, i.offset.yw);
				reflectionr *= 0.2h;
#endif
				reflection = lerp(reflection, reflectionr, unity_StereoEyeIndex);
#endif
#ifdef _ADDITIONAL_LIGHTS
				uint pixelLightCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
				{
					Light light = GetAdditionalLight(lightIndex, i.lightData.xyz);
					diffuseReflection += light.color * light.distanceAttenuation * light.shadowAttenuation * saturate(dot(normalDirection.xyz, light.direction));
				}
#endif
#ifdef _ADDITIONAL_LIGHTS_VERTEX
				diffuseReflection += i.lightData.xyz;
#endif
				color = half4(specularReflection * color.a + (diffuseReflection + bakedGI) * color.rgb, 1.0h);
#if !defined(_ADDITIONAL_LIGHTS) && !defined(_ADDITIONAL_LIGHTS_VERTEX)
				color.rgb = MixFog(color.rgb, i.lightData);
#else
				color.rgb = MixFog(color.rgb, i.lightData.w);
#endif
				return (lerp(color, reflection, _RefAlpha * mask.r) + lerp(reflection, color, 1.0h - _RefAlpha * mask.r)) * _Color * 0.5h;
			}
			ENDHLSL
		}
	}
}