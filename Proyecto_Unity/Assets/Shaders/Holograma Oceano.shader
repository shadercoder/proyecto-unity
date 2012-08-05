Shader "Planet/HologramaOceano"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_nivelMar("Nivel del Agua", Float) = 0
_tamPlaya("Tamaño Playa", Float) = 0
_Escala("Escala extru:tamPlaya", Float) = 0

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
float _nivelMar;
float _tamPlaya;
float _Escala;

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
				float2 uv_MainTex;

			};

			void vert (inout appdata_full v, out Input o) {
float4 Multiply2=_nivelMar.xxxx * _Escala.xxxx;
float4 Multiply1=float4( 0.0005,0.0005,0.0005,0.0005 ) * Multiply2;
float4 Multiply0=float4( v.normal.x, v.normal.y, v.normal.z, 1.0 ) * Multiply1;
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
float4 Multiply4=Assemble0 * float4( 0.5,0.5,0.5,0.5 );
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Assemble0;
float4 atenuacion = float4(0.5,0.5,0.5,0.5);
o.Emission = Multiply4 * atenuacion;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}