Shader "SupGames/PlanarReflectionURP/Unlit"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_BlurAmount("Blur Amount", Range(0,7)) = 1
	}
		SubShader{
			Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
			LOD 100
		
			Pass {
				Tags { "LightMode" = "UniversalForward" }
				HLSLPROGRAM
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature_local BLUR
				#pragma shader_feature_local VRon
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
				half4 _MainTex_ST;
				half4 _Color;
				half4 _ReflectionTex_TexelSize;

				struct Attributes
				{
					half4 pos : POSITION;
					half2 uv : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct Varyings
				{
					half4 pos : SV_POSITION;
					half4 uv : TEXCOORD0;
					half2 data : TEXCOORD1;
#if defined(BLUR)
					half4 offset : TEXCOORD2;
#endif
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
					o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, half4(i.pos.xyz, 1.0h)));
					half4 scrPos = ComputeScreenPos(o.pos);
					o.uv.zw = scrPos.xy;
					o.data.x = scrPos.w;
					o.data.y = ComputeFogFactor(o.pos.z);
#if defined(BLUR)
					half2 offset = _ReflectionTex_TexelSize.xy * _BlurAmount;
					o.offset = half4(-offset, offset);
#endif
					return o;
				}

				half4 frag(Varyings i) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID(i);
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
					half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
					half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv.xy);
					i.uv.zw /= i.data.x;
					half4 reflection = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, i.uv.zw);
#if defined(BLUR)
					i.offset /= i.data.x;
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
					color.rgb = MixFog(color.rgb, i.data.y);
					return (lerp(color, reflection, _RefAlpha * mask.r) + lerp(reflection, color, 1 - _RefAlpha * mask.r))*_Color * 0.5h;
				}
				ENDHLSL
			}
		}
}