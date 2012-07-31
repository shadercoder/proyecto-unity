Shader "Planet/PlantasconFiltro"
{
	Properties 
	{
_Ilum("_Ilum", 2D) = "black" {}
_Emission("_Emission", Float) = 0
_colorPlanta1("_colorPlanta1", Color) = (1,1,1,1)
_planta1("_planta1", 2D) = "black" {}
_colorPlanta2("_colorPlanta2", Color) = (1,1,1,1)
_planta2("_planta2", 2D) = "black" {}
_colorPlanta3("_colorPlanta3", Color) = (1,1,1,1)
_planta3("_planta3", 2D) = "black" {}
_colorPlanta4("_colorPlanta4", Color) = (1,1,1,1)
_planta4("_planta4", 2D) = "black" {}
_MainTex("Textura Plantas", 2D) = "black" {}
_valorBlend("_valorBlend", Range(0.01,0.9) ) = 0.01

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
ColorMask RGB
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _Ilum;
float _Emission;
float4 _colorPlanta1;
sampler2D _planta1;
float4 _colorPlanta2;
sampler2D _planta2;
float4 _colorPlanta3;
sampler2D _planta3;
float4 _colorPlanta4;
sampler2D _planta4;
sampler2D _MainTex;
float _valorBlend;

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
float4 Luminance0= Luminance( light.xyz ).xxxx;
float4 Assemble0=float4(Luminance0.x, float4( 0.0, 0.0, 0.0, 0.0 ).y, float4( 0.0, 0.0, 0.0, 0.0 ).z, float4( 0.0, 0.0, 0.0, 0.0 ).w);
float4 Tex2D0=tex2D(_Ilum,Assemble0.xy);
float4 Multiply0=float4( s.Albedo.x, s.Albedo.y, s.Albedo.z, 1.0 ) * Tex2D0;
return Multiply0;

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
float2 uv_MainTex;
float2 uv_planta2;
float2 uv_planta3;
float2 uv_planta4;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


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
float4 Multiply3=Sampled2D2 * _colorPlanta1;
float4 Sampled2D6=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Splat0=Sampled2D6.w;
float4 Lerp4_0_NoInput = float4(0,0,0,0);
float4 Lerp4=lerp(Lerp4_0_NoInput,Multiply3,Splat0);
float4 Sampled2D5=tex2D(_planta2,IN.uv_planta2.xy);
float4 Multiply4=Sampled2D5 * _colorPlanta2;
float4 Splat1=Sampled2D6.z;
float4 Lerp0=lerp(Lerp4,Multiply4,Splat1);
float4 Sampled2D3=tex2D(_planta3,IN.uv_planta3.xy);
float4 Multiply1=Sampled2D3 * _colorPlanta3;
float4 Splat3=Sampled2D6.y;
float4 Lerp1=lerp(Lerp0,Multiply1,Splat3);
float4 Sampled2D1=tex2D(_planta4,IN.uv_planta4.xy);
float4 Multiply0=Sampled2D1 * _colorPlanta4;
float4 Splat2=Sampled2D6.x;
float4 Lerp2=lerp(Lerp1,Multiply0,Splat2);
float4 Multiply5=Lerp2 * _Emission.xxxx;
float4 Add4=Splat0 + Splat1;
float4 Add5=Splat3 + Splat2;
float4 Add6=Add4 + Add5;
float4 Subtract0=Add6 - _valorBlend.xxxx;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
clip( Subtract0 );
o.Albedo = Lerp2;
o.Emission = Multiply5;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Transparent/Diffuse"
}