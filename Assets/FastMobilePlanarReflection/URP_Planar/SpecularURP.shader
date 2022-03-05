Shader "SupGames/PlanarReflectionURP/Specular"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_Glossiness("Glossiness", Range(0.01,50)) = 0.5
	    _SpecColor("Specular Color", Color) = (1,1,1,1)
		_MaskTex("Mask Texture", 2D) = "white" {}
		_BlurAmount("Blur Amount", Range(0,100)) = 1
		[Toggle(RECEIVE_SHADOWS)]
		_ReceiveShadows("Recieve Shadows", Float) = 0
	}
	SubShader{
		Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
		LOD 150

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
			TEXTURE2D(_ReflectionTex);
			SAMPLER(sampler_ReflectionTex);
#ifdef VRon
			TEXTURE2D(_ReflectionTexRight);
			SAMPLER(sampler_ReflectionTexRight);
#endif
			TEXTURE2D(_MaskTex);
			SAMPLER(sampler_MaskTex);

			half _BlurAmount;
			half _RefAlpha;
			half _Glossiness;
			half4 _MainTex_ST;
			half4 _Color;
			half4 _SpecColor;
			half4 _ReflectionTex_TexelSize;

			struct Attributes
			{
				half4 pos : POSITION;
				half4 uv : TEXCOORD0;
				half4 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				half4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
				half4 normal : TEXCOORD1;
				half4 viewDir : TEXCOORD2;
#ifdef LIGHTMAP_ON
				half2 lightmapUV : TEXCOORD3;
#else
				half3 vertexSH : TEXCOORD3;
#endif
#if defined(BLUR)
				half4 offset : TEXCOORD4;
#endif
#if defined(_MAIN_LIGHT_SHADOWS)
				half4 shadowCoord : TEXCOORD5;
#endif
				half3 lightData : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes i)
			{
				Varyings o = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_TRANSFER_INSTANCE_ID(i, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.uv.xy = TRANSFORM_TEX(i.uv, _MainTex);
				half4 ws = mul(unity_ObjectToWorld, i.pos);
				o.viewDir.xyz = normalize(_WorldSpaceCameraPos - ws.xyz);
				o.normal.xyz = normalize(mul(i.normal, unity_WorldToObject).xyz);
				o.pos = mul(unity_MatrixVP, ws); 
				o.viewDir.w = ComputeFogFactor(o.pos.z);
				half4 scrPos = ComputeScreenPos(o.pos);
				o.uv.zw = scrPos.xy;
				o.normal.w = scrPos.w;
#if defined(BLUR)
				half2 offset = _ReflectionTex_TexelSize.xy * _BlurAmount;
				o.offset = half4(-offset, offset);
#endif
#if defined(_MAIN_LIGHT_SHADOWS)
				o.shadowCoord = TransformWorldToShadowCoord(ws.xyz);
#endif
#ifdef LIGHTMAP_ON
				o.lightmapUV = i.uv.zw * unity_LightmapST.xy + unity_LightmapST.zw;
#else
				o.vertexSH = SampleSHVertex(i.normal.xyz);
#endif
#ifdef _ADDITIONAL_LIGHTS_VERTEX
				o.lightData = half3(0.0h, 0.0h, 0.0h);
				uint lightsCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex)
				{
					Light light = GetAdditionalLight(lightIndex, ws.xyz);
					o.lightData += light.color * light.distanceAttenuation * saturate(dot(o.normal.xyz, light.direction));
				}
#endif
#ifdef _ADDITIONAL_LIGHTS
				o.lightData = ws.xyz;
#endif
				return o;
			}

			half4 frag(Varyings i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				half3 diffuseReflection = _MainLightColor.rgb * dot(i.normal.xyz, _MainLightPosition.xyz);
				half3 bakedGI = SAMPLE_GI(i.lightmapUV, i.vertexSH, i.normal.xyz);
#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF) && defined(RECEIVE_SHADOWS)
				half3 realtimeShadow = lerp(bakedGI, max(bakedGI - diffuseReflection * (1.0h - MainLightRealtimeShadow(i.shadowCoord)), _SubtractiveShadowColor.xyz), _MainLightShadowData.x);
				bakedGI = min(bakedGI, realtimeShadow);
#endif
				half3 specularReflection = _SpecColor.rgb * _MainLightColor.rgb * pow(saturate(dot(i.normal.xyz, normalize(_MainLightPosition.xyz + i.viewDir.xyz))), _Glossiness);
				half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
				half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv.xy);
				i.uv.zw /= i.normal.w;
				half4 reflection = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.uv.zw);
#if defined(BLUR)
				i.offset /= i.normal.w;
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
					diffuseReflection += light.color * light.distanceAttenuation * light.shadowAttenuation * saturate(dot(i.normal.xyz, light.direction));
				}
#endif
#ifdef _ADDITIONAL_LIGHTS_VERTEX
				diffuseReflection += i.lightData;
#endif
				color = half4(specularReflection * (1.0h - color.a) + (diffuseReflection + bakedGI) * color.rgb, 1.0h);
				color.rgb = MixFog(color.rgb, i.viewDir.w);
				return (lerp(color, reflection, _RefAlpha * mask.r) + lerp(reflection, color, 1 - _RefAlpha * mask.r))*_Color * 0.5h;
			}
			ENDHLSL
		}
	}
}