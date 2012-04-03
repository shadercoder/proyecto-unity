Shader "Planet/Plantas"
{
	Properties 
	{
_planta1("_planta1", 2D) = "black" {}
_planta2("_planta2", 2D) = "black" {}
_planta3("_planta3", 2D) = "black" {}
_planta4("_planta4", 2D) = "black" {}
_MainTex("textura plantas", 2D) = "black" {}
_texturaPlaneta("textura planeta", 2D) = "black" {}
_Amount("Extrusion", float)=0.5
_Amount2("Altura Plantas", float)=0.5

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Blend One One
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _planta1;
sampler2D _planta2;
sampler2D _planta3;
sampler2D _planta4;
sampler2D _MainTex;
sampler2D _texturaPlaneta;
float _Amount;
float _Amount2;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_planta1;
float2 uv_planta3;
float2 uv_texturaPlaneta;
float2 uv_planta4;
float2 uv_planta2;
float2 uv_MainTex;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
#if !defined(SHADER_API_OPENGL)
				float4 tex = tex2Dlod (_texturaPlaneta, float4(v.texcoord.xyz,0));
				float4 texP = tex2Dlod (_MainTex, float4(v.texcoord.xyz,0));
				v.vertex.xyz += v.normal * (tex.rgb * _Amount + _Amount2*texP.rgb);
				#endif

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Sampled2D2=tex2D(_planta1,IN.uv_planta1.xy);
float4 Sampled2D3=tex2D(_planta3,IN.uv_planta3.xy);
float4 Sampled2D0=tex2D(_texturaPlaneta,IN.uv_texturaPlaneta.xy);
float4 Add0_1_NoInput = float4(0,0,0,0);
float4 Add0=Sampled2D0 + Add0_1_NoInput;
float4 Lerp3=lerp(Sampled2D2,Sampled2D3,Add0);
float4 Sampled2D4=tex2D(_planta4,IN.uv_planta4.xy);
float4 Lerp1=lerp(Lerp3,Sampled2D4,Add0);
float4 Sampled2D5=tex2D(_planta2,IN.uv_planta2.xy);
float4 Lerp0=lerp(Lerp1,Sampled2D5,Add0);
float4 Sampled2D6=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Lerp2_0_NoInput = float4(0,0,0,0);
float4 Lerp2=lerp(Lerp2_0_NoInput,Lerp0,Sampled2D6);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp2;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}