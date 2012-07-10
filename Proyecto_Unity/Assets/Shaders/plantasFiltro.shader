Shader "Planet/PlantasFiltro"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_Emission("_Emission", Float) = 0.5
_colorPlanta1("_colorPlanta1", Color) = (1,0,0,1)
_colorPlanta2("_colorPlanta2", Color) = (0.090909,0,1,1)
_colorPlanta3("_colorPlanta3", Color) = (0.2167833,1,0,1)
_colorPlanta4("_colorPlanta4", Color) = (1,0.986014,0,1)
_valorBlend("_valorBlend", Range(0.01,0.9) ) = 0.01
_Relieve("_Relieve", 2D) = "black" {}

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
float _Emission;
float4 _colorPlanta1;
float4 _colorPlanta2;
float4 _colorPlanta3;
float4 _colorPlanta4;
float _valorBlend;
sampler2D _Relieve;

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
				
float4 Sampled2D6=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Split0=Sampled2D6;
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - float4( Split0.w, Split0.w, Split0.w, Split0.w);
float4 Lerp4_1_NoInput = float4(0,0,0,0);
float4 Lerp4=lerp(_colorPlanta1,Lerp4_1_NoInput,Invert0);
float4 Lerp0=lerp(Lerp4,_colorPlanta2,float4( Split0.x, Split0.x, Split0.x, Split0.x));
float4 Lerp1=lerp(Lerp0,_colorPlanta3,float4( Split0.y, Split0.y, Split0.y, Split0.y));
float4 Lerp2=lerp(Lerp1,_colorPlanta4,float4( Split0.z, Split0.z, Split0.z, Split0.z));
float4 Multiply0=Lerp2 * _Emission.xxxx;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp2;
o.Emission = Multiply0;
o.Alpha = Lerp2;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Transparent/Diffuse"
}