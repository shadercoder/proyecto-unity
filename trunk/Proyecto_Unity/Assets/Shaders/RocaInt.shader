Shader "Planet/RocaTextureLerp"
{
	Properties 
	{
_MainTex("Mapa terreno Base", 2D) = "black" {}
_RampaColor("_RampaColor", 2D) = "black" {}
_Normals("Mapa Relieve", 2D) = "bump" {}
_Ilum("_Ilum", 2D) = "black" {}
_BiasTerreno("_BiasTerreno", Range(-3,3) ) = 0.5
_Amount("Extrusion", Range(0,2) ) = 0.5
_texTop("cumbres", 2D) = "black" {}
_texMountain("montañas", 2D) = "black" {}
_texHill("altura alta", 2D) = "black" {}
_texMedium("altura media", 2D) = "black" {}
_texLow("altura baja", 2D) = "black" {}
_texBottom("valles", 2D) = "black" {}
_prueba1("_prueba1", Range(0.01,1) ) = 0.5
_prueba2("_prueba2", Range(0,1) ) = 0.5

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
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _MainTex;
sampler2D _RampaColor;
sampler2D _Normals;
sampler2D _Ilum;
float _BiasTerreno;
sampler2D _texTop;
sampler2D _texMountain;
sampler2D _texHill;
sampler2D _texMedium;
sampler2D _texLow;
sampler2D _texBottom;
float _prueba1;
float _prueba2;
float _Amount;

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
				float2 uv_texBottom;
float2 uv_texLow;
float2 uv_MainTex;
float2 uv_texMedium;
float2 uv_texHill;
float2 uv_texMountain;
float2 uv_texTop;
float2 uv_Normals;
float3 viewDir;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
#if !defined(SHADER_API_OPENGL)
				float4 tex = tex2Dlod (_MainTex, float4(v.texcoord.xyz,0));
				v.vertex.xyz += v.normal * tex.rgb * _Amount;
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
				
float4 Sampled2D6=tex2D(_texBottom,IN.uv_texBottom.xy);
float4 Sampled2D1=tex2D(_texLow,IN.uv_texLow.xy);
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Tex2D1=tex2D(_RampaColor,Sampled2D0.xy);
float4 Splat2=Tex2D1.z;
float4 Lerp2=lerp(Sampled2D6,Sampled2D1,Splat2);
float4 Sampled2D3=tex2D(_texMedium,IN.uv_texMedium.xy);
float4 Splat1=Tex2D1.y;
float4 Add1=Splat2 + Splat1;
float4 Lerp0=lerp(Lerp2,Sampled2D3,Add1);
float4 Sampled2D7=tex2D(_texHill,IN.uv_texHill.xy);
float4 Splat0=Tex2D1.x;
float4 Add2=Splat1 + Splat0;
float4 Lerp3=lerp(Lerp0,Sampled2D7,Add2);
float4 Sampled2D4=tex2D(_texMountain,IN.uv_texMountain.xy);
float4 Lerp4_2_NoInput = float4(0,0,0,0);
float4 Lerp4=lerp(Lerp3,Sampled2D4,Lerp4_2_NoInput);
float4 Sampled2D5=tex2D(_texTop,IN.uv_texTop.xy);
float4 Lerp1=lerp(Lerp4,Sampled2D5,Splat0);
float4 Sampled2D2=tex2D(_Normals,IN.uv_Normals.xy);
float4 UnpackNormal0=float4(UnpackNormal(Sampled2D2).xyz, 1.0);
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Tex2D1;
float4 ParallaxOffset0= ParallaxOffset( Invert0.x, _BiasTerreno.xxxx.x, float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz).xyxy;
float4 Add0=UnpackNormal0 + ParallaxOffset0;
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp1;
o.Normal = Add0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}