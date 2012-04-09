Shader "Planet/Plantas"
{
	Properties 
	{
_planta1("_planta1", 2D) = "black" {}
_planta2("_planta2", 2D) = "black" {}
_planta3("_planta3", 2D) = "black" {}
_PlantTex("_PlantTex", 2D) = "black" {}
_MainTex("_MainTex", 2D) = "black" {}
_alturas("_alturas", 2D) = "black" {}
_valorBlend("_valorBlend", Range(0,10) ) = 7
_valorColorido("_valorColorido", Float) = 2.5
_Amount("Extrusion", float)=0.5
_Amount2("Altura Plantas", float)=0.1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"RenderType"="Transparent"
"IgnoreProjector"="False"
		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Blend SrcAlpha OneMinusSrcAlpha
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _planta1;
sampler2D _planta2;
sampler2D _planta3;
sampler2D _PlantTex;
sampler2D _MainTex;
sampler2D _alturas;
float _valorBlend;
float _valorColorido;
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
float2 uv_PlantTex;
float2 uv_MainTex;
float2 uv_planta2;
float2 uv_planta3;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
#if !defined(SHADER_API_OPENGL)
				float4 tex = tex2Dlod (_MainTex, float4(v.texcoord.xyz,0));
				float4 texP = tex2Dlod (_PlantTex, float4(v.texcoord.xyz,0));
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
float4 Sampled2D6=tex2D(_PlantTex,IN.uv_PlantTex.xy);
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 UV_Pan0=float4(Sampled2D0.x + _Time.x,Sampled2D0.y,Sampled2D0.z,Sampled2D0.w);
float4 Tex2D0=tex2D(_alturas,UV_Pan0.xy);
float4 Multiply0=Sampled2D6 * Tex2D0;
float4 Splat0=Multiply0.z;
float4 Lerp4_0_NoInput = float4(0,0,0,0);
float4 Lerp4=lerp(Lerp4_0_NoInput,Sampled2D2,Splat0);
float4 Sampled2D5=tex2D(_planta2,IN.uv_planta2.xy);
float4 Splat1=Multiply0.y;
float4 Lerp0=lerp(Lerp4,Sampled2D5,Splat1);
float4 Sampled2D3=tex2D(_planta3,IN.uv_planta3.xy);
float4 Splat2=Multiply0.x;
float4 Lerp1=lerp(Lerp0,Sampled2D3,Splat2);
float4 Multiply2=Lerp1 * _valorColorido.xxxx;
float4 Multiply1=Lerp1 * _valorBlend.xxxx;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply2;
o.Alpha = Multiply1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "RocaInt"
}