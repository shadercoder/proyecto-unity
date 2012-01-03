Shader "Planeta"
{
	Properties 
	{
_MainTex("Textura Base", 2D) = "black" {}
_Pallete("_Pallete", 2D) = "black" {}
_Lights("Luces ciudades", 2D) = "black" {}
_Lightscale("Intensidad luces ciudades", Float) = 1
_Normals("Relieve", 2D) = "black" {}
_ColorSpec("Color reflejado", Color) = (0.1383381,0.4698198,0.8059701,1)
_Roughness("Dureza del Reflejo", Float) = 0.1
_Mask("Mascara de regflejo del agua", 2D) = "black" {}
_AtmosFalloff("Espesura atmosfera", Float) = 3
_AtmosFar("Color atmosfera Lejana", Color) = (0.5447761,0.942699,1,1)
_AtmosNear("Color atmosfera cercana", Color) = (0.2241591,0.6719844,0.858209,1)
_Ramp("Degradado Iluminacion", 2D) = "black" {}
_Nubes("textura Nubes", 2D) = "black" {}
_TimeScale("Escala de tiempo rot nubes", Float) = 0.01
_CloudHeight("Altura de las Nubes", Range(0,-0.1) ) = -0.05024631
_AnisoCloudPower("Aniso Nubes", Range(0,0.1) ) = 0.05763547

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
sampler2D _Pallete;
sampler2D _Lights;
float _Lightscale;
sampler2D _Normals;
float4 _ColorSpec;
float _Roughness;
sampler2D _Mask;
float _AtmosFalloff;
float4 _AtmosFar;
float4 _AtmosNear;
sampler2D _Ramp;
sampler2D _Nubes;
float _TimeScale;
float _CloudHeight;
float _AnisoCloudPower;

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
float4 Tex2D0=tex2D(_Ramp,Assemble0.xy);
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
				float3 viewDir;
float2 uv_MainTex;
float2 uv_Nubes;
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
float4 Multiply3=Lerp0 * Saturate0;
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Tex2D1=tex2D(_Pallete,Sampled2D0.xy);
float4 Add0=Multiply3 + Tex2D1;
float4 Multiply4=_TimeScale.xxxx * _Time;
float4 UV_Pan0=float4((IN.uv_Nubes.xyxy).x + Multiply4.y,(IN.uv_Nubes.xyxy).y,(IN.uv_Nubes.xyxy).z,(IN.uv_Nubes.xyxy).w);
float4 ParallaxOffset0_2_NoInput = float4(0,0,0,0);
float4 ParallaxOffset0= ParallaxOffset( _CloudHeight.xxxx.x, ParallaxOffset0_2_NoInput.x, float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz).xyxy;
float4 Add1=UV_Pan0 + ParallaxOffset0;
float4 Tex2D0=tex2D(_Nubes,Add1.xy);
float4 Lerp1=lerp(Add0,Tex2D0,Tex2D0);
float4 Sampled2D1=tex2D(_Normals,IN.uv_Normals.xy);
float4 UnpackNormal0=float4(UnpackNormal(Sampled2D1).xyz, 1.0);
float4 Normalize0=normalize(float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ));
float4 Negative0= -Normalize0; 
 float4 Invert1= float4(1.0, 1.0, 1.0, 1.0) - _AnisoCloudPower.xxxx;
float4 Lerp4=lerp(Negative0,float4( 0,0,1,0),Invert1);
float4 Lerp3=lerp(UnpackNormal0,Lerp4,Tex2D0);
float4 Sampled2D2=tex2D(_Lights,IN.uv_Lights.xy);
float4 Multiply0=Sampled2D2 * _Lightscale.xxxx;
float4 Lerp2_1_NoInput = float4(0,0,0,0);
float4 Lerp2=lerp(Multiply0,Lerp2_1_NoInput,Tex2D0);
float4 Sampled2D3=tex2D(_Mask,IN.uv_Mask.xy);
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Tex2D0;
float4 Multiply2=Sampled2D3 * Invert0;
float4 Multiply1=Multiply2 * _ColorSpec;
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp1;
o.Normal = Lerp3;
o.Emission = Lerp2;
o.Specular = Multiply1;
o.Gloss = _Roughness.xxxx;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}