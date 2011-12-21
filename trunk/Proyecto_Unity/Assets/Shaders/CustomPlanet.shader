Shader "Tutorial/planet01"
{
	Properties 
	{
_MainTex("Mapa terreno Base", 2D) = "black" {}
_Lights("Mapa Luces", 2D) = "black" {}
_Normals("Mapa Relieve", 2D) = "bump" {}
_Lightscale("Intensidad de las luces", Float) = 1
_AtmosFalloff("Espesura? de la atmosfera", Float) = 3
_AtmosNear("Color interior de la atmosfera", Color) = (0.2202798,0.7206971,1,1)
_AtmosFar("Color borde de la atmosfera", Color) = (0.1137255,1,0.9206322,1)
_Clouds("Nubes", 2D) = "gray" {}
_timeScale("Factor de tiempo", Float) = 0.007
_Roughness("Dureza del reflejo", Float) = 0.08148026
_Mask("Mascara de liquidos (emision)", 2D) = "white" {}
_ColorSpec("Color reflejado", Color) = (0.6142626,0.7215345,0.9925373,1)
_AnisocloudPowder("Aceptacion de Luz Nubes", Range(0,0.1) ) = 0
_CloudHeight("Altura Nubes", Range(0,-0.1) ) = -0.1

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
sampler2D _Lights;
sampler2D _Normals;
float _Lightscale;
float _AtmosFalloff;
float4 _AtmosNear;
float4 _AtmosFar;
sampler2D _Clouds;
float _timeScale;
float _Roughness;
sampler2D _Mask;
float4 _ColorSpec;
float _AnisocloudPowder;
float _CloudHeight;

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
				float3 viewDir;
float2 uv_MainTex;
float2 uv_Clouds;
float2 uv_Normals;
float2 uv_Lights;
float2 uv_Mask;

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
				
float4 Fresnel0_1_NoInput = float4(0,0,1,1);
float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
float4 Pow0=pow(Fresnel0,_AtmosFalloff.xxxx);
float4 Saturate0=saturate(Pow0);
float4 Lerp0=lerp(_AtmosNear,_AtmosFar,Saturate0);
float4 Multiply1=Lerp0 * Saturate0;
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Add0=Multiply1 + Sampled2D0;
float4 Multiply2=_Time * _timeScale.xxxx;
float4 UV_Pan0=float4((IN.uv_Clouds.xyxy).x + Multiply2.y,(IN.uv_Clouds.xyxy).y,(IN.uv_Clouds.xyxy).z,(IN.uv_Clouds.xyxy).w);
float4 ParallaxOffset0_2_NoInput = float4(0,0,0,0);
float4 ParallaxOffset0= ParallaxOffset( _CloudHeight.xxxx.x, ParallaxOffset0_2_NoInput.x, float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz).xyxy;
float4 Add1=UV_Pan0 + ParallaxOffset0;
float4 Tex2D0=tex2D(_Clouds,Add1.xy);
float4 Lerp1=lerp(Add0,Tex2D0,Tex2D0);
float4 Sampled2D2=tex2D(_Normals,IN.uv_Normals.xy);
float4 UnpackNormal0=float4(UnpackNormal(Sampled2D2).xyz, 1.0);
float4 Normalize0=normalize(float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ));
float4 Negative0= -Normalize0; 
 float4 Invert1= float4(1.0, 1.0, 1.0, 1.0) - _AnisocloudPowder.xxxx;
float4 Lerp3=lerp(Negative0,float4( 0,0,1,0),Invert1);
float4 Lerp4=lerp(UnpackNormal0,Lerp3,Tex2D0);
float4 Sampled2D1=tex2D(_Lights,IN.uv_Lights.xy);
float4 Multiply0=Sampled2D1 * _Lightscale.xxxx;
float4 Lerp2_1_NoInput = float4(0,0,0,0);
float4 Lerp2=lerp(Multiply0,Lerp2_1_NoInput,Tex2D0);
float4 Sampled2D3=tex2D(_Mask,IN.uv_Mask.xy);
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Tex2D0.aaaa;
float4 Multiply3=Sampled2D3 * Invert0;
float4 Multiply4=Multiply3 * _ColorSpec;
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp1;
o.Normal = Lerp4;
o.Emission = Lerp2;
o.Specular = Multiply4;
o.Gloss = _Roughness.xxxx;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}