Shader "Planet/OceanoRamp"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_Ilum("textura de luz", 2D) = "black" {}
_ColorNear("agua cercana a la superficie", Color) = (0.3411765,0.7215686,0.1764706,1)
_ColorFar("agua profunda", Color) = (0,0.2666667,0.1647059,1)
_ColorPlayaAgua("agua superficial", Color) = (0.7568628,0.8078431,0.3294118,1)
_ColorPlaya("playa", Color) = (0.8039216,0.7450981,0.6156863,1)
_densidadAgua("cantidad de refraccion", Range(0,0.05) ) = 0.004
_nivelMar("Nivel del Agua", Float) = 0
_tamPlaya("Tamaño Playa", Float) = 0
_Mar("Textura Olas", 2D) = "black" {}
_velVer("velocidad", Float) = 0
_costa("Textura de la costa", 2D) = "black" {}
_Brillo("Brillo General. [0,5 - 1]", Float) = 0

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
sampler2D _Ilum;
float4 _ColorNear;
float4 _ColorFar;
float4 _ColorPlayaAgua;
float4 _ColorPlaya;
float _densidadAgua;
float _nivelMar;
float _tamPlaya;
sampler2D _Mar;
float _velVer;
sampler2D _costa;
float _Brillo;

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
float4 Multiply1=float4( s.Albedo.x, s.Albedo.y, s.Albedo.z, 1.0 ) * Tex2D0;
return Multiply1;

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
				float2 uv_Mar;
float3 viewDir;
float2 uv_MainTex;
float2 uv_costa;

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
				
float4 Tex2D1=tex2D(_Mar,(IN.uv_Mar.xyxy).xy);
float4 Multiply6=_velVer.xxxx * _Time;
float4 UV_Pan2=float4(Tex2D1.x + Multiply6.x,Tex2D1.y + Multiply6.x,Tex2D1.z,Tex2D1.w);
float4 Tex2D0=tex2D(_Mar,UV_Pan2.xy);
float4 Multiply0=_ColorNear * Tex2D0;
float4 Multiply1=Tex2D0 * _ColorFar;
float4 Fresnel0_1_NoInput = float4(0,0,1,1);
float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
float4 Pow0=pow(_densidadAgua.xxxx,Fresnel0);
float4 Lerp1=lerp(Multiply0,Multiply1,Pow0);
float4 Add2=_nivelMar.xxxx + _tamPlaya.xxxx;
float4 Sampled2D1=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Clamp2=clamp(Sampled2D1,_nivelMar.xxxx,Add2);
float4 Step2=step(Add2,Clamp2);
float4 Invert2= float4(1.0, 1.0, 1.0, 1.0) - Step2;
float4 Subtract0=_nivelMar.xxxx - _tamPlaya.xxxx;
float4 Clamp1=clamp(Sampled2D1,Subtract0,_nivelMar.xxxx);
float4 Step1=step(_nivelMar.xxxx,Clamp1);
float4 Invert1= float4(1.0, 1.0, 1.0, 1.0) - Step1;
float4 Subtract2=Invert2 - Invert1;
float4 Clamp0=clamp(Sampled2D1,float4( 0.0, 0.0, 0.0, 0.0 ),Subtract0);
float4 Step0=step(Subtract0,Clamp0);
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Step0;
float4 Subtract1=Invert1 - Invert0;
float4 Assemble0_3_NoInput = float4(0,0,0,0);
float4 Assemble0=float4(Subtract2.x, Subtract1.y, Invert0.z, Assemble0_3_NoInput.w);
float4 Add3=_Brillo.xxxx + Sampled2D1;
float4 Multiply4=Assemble0 * Add3;
float4 Split0=Multiply4;
float4 Lerp3_0_NoInput = float4(0,0,0,0);
float4 Lerp3=lerp(Lerp3_0_NoInput,Lerp1,float4( Split0.z, Split0.z, Split0.z, Split0.z));
float4 Multiply2=Tex2D0 * _ColorPlayaAgua;
float4 Lerp7_0_NoInput = float4(0,0,0,0);
float4 Lerp7=lerp(Lerp7_0_NoInput,Multiply2,float4( Split0.y, Split0.y, Split0.y, Split0.y));
float4 Add0=Lerp3 + Lerp7;
float4 Sampled2D0=tex2D(_costa,IN.uv_costa.xy);
float4 Multiply3=Sampled2D0 * _ColorPlaya;
float4 Lerp0_0_NoInput = float4(0,0,0,0);
float4 Lerp0=lerp(Lerp0_0_NoInput,Multiply3,float4( Split0.x, Split0.x, Split0.x, Split0.x));
float4 Add1=Add0 + Lerp0;
float4 Multiply7=Multiply3 * Step2;
float4 Add4=Add1 + Multiply7;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Add4;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}