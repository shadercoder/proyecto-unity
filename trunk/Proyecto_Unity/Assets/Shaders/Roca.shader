Shader "Planet/Roca"
{
	Properties 
	{
_MainTex("Mapa terreno Base", 2D) = "black" {}
_RampaColor("Canales para cada Altura", 2D) = "black" {}
_Ilum("Rampa de iluminación", 2D) = "black" {}
_Amount("Extrusion por Shader", Float) = 0
_tinteTop("_tinteTop", Color) = (1,1,1,1)
_texTop("Cumbre", 2D) = "black" {}
_tinteMount("_tinteMount", Color) = (1,1,1,1)
_texMount("Montañas", 2D) = "black" {}
_tinteMedium("_tinteMedium", Color) = (1,1,1,1)
_texMedium("Altura Media", 2D) = "black" {}
_tinteLow("_tinteLow", Color) = (1,1,1,1)
_texLow("Baja Altura", 2D) = "black" {}
_tinteBot("_tinteBot", Color) = (1,1,1,1)
_texBot("Valles", 2D) = "black" {}
_Emision("_Emision", Range(0,0.4) ) = 0

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
sampler2D _Ilum;
float _Amount;
float4 _tinteTop;
sampler2D _texTop;
float4 _tinteMount;
sampler2D _texMount;
float4 _tinteMedium;
sampler2D _texMedium;
float4 _tinteLow;
sampler2D _texLow;
float4 _tinteBot;
sampler2D _texBot;
float _Emision;

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
				float2 uv_texBot;
float2 uv_texLow;
float2 uv_MainTex;
float2 uv_texMedium;
float2 uv_texMount;
float2 uv_texTop;

			};

			void vert (inout appdata_full v, out Input o) {
float4 Multiply1_0_NoInput = float4(0,0,0,0);
float4 Multiply1=Multiply1_0_NoInput * float4( v.normal.x, v.normal.y, v.normal.z, 1.0 );
float4 Multiply0=Multiply1 * _Amount.xxxx;
float4 Add0=v.vertex + Multiply0;
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
v.vertex = Add0;


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Sampled2D6=tex2D(_texBot,IN.uv_texBot.xy);
float4 Multiply0=Sampled2D6 * _tinteBot;
float4 Sampled2D1=tex2D(_texLow,IN.uv_texLow.xy);
float4 Multiply1=Sampled2D1 * _tinteLow;
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Tex2D1=tex2D(_RampaColor,Sampled2D0.xy);
float4 Split0=Tex2D1;
float4 Lerp2=lerp(Multiply0,Multiply1,float4( Split0.x, Split0.x, Split0.x, Split0.x));
float4 Sampled2D3=tex2D(_texMedium,IN.uv_texMedium.xy);
float4 Multiply2=Sampled2D3 * _tinteMedium;
float4 Lerp0=lerp(Lerp2,Multiply2,float4( Split0.y, Split0.y, Split0.y, Split0.y));
float4 Sampled2D4=tex2D(_texMount,IN.uv_texMount.xy);
float4 Multiply3=Sampled2D4 * _tinteMount;
float4 Lerp3=lerp(Lerp0,Multiply3,float4( Split0.z, Split0.z, Split0.z, Split0.z));
float4 Sampled2D5=tex2D(_texTop,IN.uv_texTop.xy);
float4 Multiply4=Sampled2D5 * _tinteTop;
float4 Invert1= float4(1.0, 1.0, 1.0, 1.0) - float4( Split0.w, Split0.w, Split0.w, Split0.w);
float4 Lerp4=lerp(Lerp3,Multiply4,Invert1);
float4 Multiply5=Lerp4 * _Emision.xxxx;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp4;
o.Emission = Multiply5;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}