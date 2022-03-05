Shader "VertexColor"
{
	Properties 
	{

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry+0"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
			};
			
			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
				#endif
				viewDir = normalize(viewDir);
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot (s.Normal, lightDir));
				
				float nh = max (0, dot (s.Normal, h));
				float3 spec = pow (nh, s.Specular*128.0) * s.Gloss;
				
				half4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
				c.a = s.Alpha + _LightColor0.a * Luminance(spec) * atten;
				return c;
			}
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
				half3 spec = light.a * s.Gloss;
				
				half4 c;
				c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
				c.a = s.Alpha + Luminance(spec);
				return c;
			}
			
			struct Input {
				float4 color : COLOR;

			};
			
			void vert (inout appdata_full v, out Input o) {

			}
			



			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Albedo = 0.0;
				o.Normal = float3(0.0,0.0,1.0);
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Alpha = 1.0;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = IN.color;
o.Normal = float3( 0.0, 0.0, 1.0);
o.Alpha = 1.0;

			}
		ENDCG
	}
	Fallback "Diffuse"
}