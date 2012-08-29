Shader "Planet/Oceano2"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_Ilum("textura de luz", 2D) = "black" {}
_ColorFar("agua profunda", Color) = (0,0.2666667,0.1647059,1)
_ColorNear("agua cercana a la superficie", Color) = (0.3411765,0.7215686,0.1764706,1)
_ColorPlayaAgua("agua superficial", Color) = (0.7568628,0.8078431,0.3294118,1)
_ColorPlaya("playa", Color) = (0.8039216,0.7450981,0.6156863,1)
_nivelMar("Nivel del Agua", Float) = 0
_tamPlaya("Tamaño Playa", Float) = 0
_Mar("Textura Olas", 2D) = "black" {}
_velVer("velocidad", Float) = 0
_costa("Textura de la costa", 2D) = "black" {}
_Brillo("Brillo General. [0,5 - 1]", Float) = 0
_Olas("_Olas", Range(0.01,0.05) ) = 0.01
_VelocidadOlas("_VelocidadOlas", Float) = 0
_OlasTex("_OlasTex", 2D) = "black" {}
_ColorOlas("_ColorOlas", Color) = (1,1,1,1)

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
Color (0.00751838,0.0346301,0.06716418,1)
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _MainTex;
sampler2D _Ilum;
float4 _ColorFar;
float4 _ColorNear;
float4 _ColorPlayaAgua;
float4 _ColorPlaya;
float _nivelMar;
float _tamPlaya;
sampler2D _Mar;
float _velVer;
sampler2D _costa;
float _Brillo;
float _Olas;
float _VelocidadOlas;
sampler2D _OlasTex;
float4 _ColorOlas;

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
				float4 color : COLOR;

			};

			void vert (inout appdata_full v, out Input o) {
float4 Add3_1_NoInput = float4(0,0,0,0);
float4 Add3=_nivelMar.xxxx + Add3_1_NoInput;
float4 Clamp0_0_NoInput = float4(0,0,0,0);
float4 Clamp0=clamp(Clamp0_0_NoInput,_nivelMar.xxxx,Add3);
float4 Step0=step(Add3,Clamp0);
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Step0;
float4 Subtract2=_nivelMar.xxxx - _tamPlaya.xxxx;
float4 Clamp1_0_NoInput = float4(0,0,0,0);
float4 Clamp1=clamp(Clamp1_0_NoInput,Subtract2,_nivelMar.xxxx);
float4 Step1=step(_nivelMar.xxxx,Clamp1);
float4 Invert1= float4(1.0, 1.0, 1.0, 1.0) - Step1;
float4 Subtract0=Invert0 - Invert1;
float4 Clamp2_0_NoInput = float4(0,0,0,0);
float4 Clamp2=clamp(Clamp2_0_NoInput,float4( 0.0, 0.0, 0.0, 0.0 ),Subtract2);
float4 Step2=step(Subtract2,Clamp2);
float4 Invert2= float4(1.0, 1.0, 1.0, 1.0) - Step2;
float4 Subtract1=Invert1 - Invert2;
float4 Add2=Subtract0 + Subtract1;
float4 Add1=Add2 + Invert2;
float4 Multiply0=float4( v.normal.x, v.normal.y, v.normal.z, 1.0 ) * Add1;
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
				
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}