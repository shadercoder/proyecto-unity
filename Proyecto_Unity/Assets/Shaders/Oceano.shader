Shader "Planet/Oceano"
{
	Properties 
	{
_ColorNear("_ColorNear", Color) = (1,0,0,1)
_ColorFar("_ColorFar", Color) = (0.2910448,0,0,1)
_colorPlayaAgua("_colorPlayaAgua", Color) = (1,0.9551196,0.6791044,1)
_ColorPlaya("_ColorPlaya", Color) = (1,0.7238806,0.8107713,1)
_densidadAgua("_densidadAgua", Range(0,0.05) ) = 0.004
_MainTex("_MainTex", 2D) = "black" {}
_RefStrGloss("_RefStrGloss", 2D) = "gray" {}
_bumpAnimControl("_bumpAnimControl", 2D) = "black" {}
_ReflectionCubemap("_ReflectionCubemap", Cube) = "black" {}
_ReflectionColor("_ReflectionColor", Color) = (0.2021609,0.2123139,0.4925373,1)
_SpecularColor("_SpecularColor", Color) = (1,0.8071182,0.5074627,1)
_Shininess("_Shininess", Range(0.1,4) ) = 1.814286
_SeaSpd("_SeaSpd", Float) = 0.25

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
LOD 100
Blend SrcAlpha OneMinusSrcAlpha
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


float4 _ColorNear;
float4 _ColorFar;
float4 _colorPlayaAgua;
float4 _ColorPlaya;
float _densidadAgua;
sampler2D _MainTex;
sampler2D _RefStrGloss;
sampler2D _bumpAnimControl;
samplerCUBE _ReflectionCubemap;
float4 _ReflectionColor;
float4 _SpecularColor;
float _Shininess;
float _SeaSpd;

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
float3 simpleWorldRefl;
float2 uv_bumpAnimControl;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);

o.simpleWorldRefl = -reflect( normalize(WorldSpaceViewDir(v.vertex)), normalize(mul((float3x3)_Object2World, SCALED_NORMAL)));

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
float4 Pow0=pow(_densidadAgua.xxxx,Fresnel0);
float4 Lerp1=lerp(_ColorFar,_ColorNear,Pow0);
float4 Tex2D2=tex2D(_MainTex,(IN.uv_MainTex.xyxy).xy);
float4 Split0=Tex2D2;
float4 Lerp3_0_NoInput = float4(0,0,0,0);
float4 Lerp3=lerp(Lerp3_0_NoInput,Lerp1,float4( Split0.y, Split0.y, Split0.y, Split0.y));
float4 Lerp7_0_NoInput = float4(0,0,0,0);
float4 Lerp7=lerp(Lerp7_0_NoInput,_colorPlayaAgua,float4( Split0.x, Split0.x, Split0.x, Split0.x));
float4 Add0=Lerp3 + Lerp7;
float4 Lerp0_0_NoInput = float4(0,0,0,0);
float4 Lerp0=lerp(Lerp0_0_NoInput,_ColorPlaya,float4( Split0.z, Split0.z, Split0.z, Split0.z));
float4 Add1=Add0 + Lerp0;
float4 TexCUBE0=texCUBE(_ReflectionCubemap,float4( IN.simpleWorldRefl.x, IN.simpleWorldRefl.y,IN.simpleWorldRefl.z,1.0 ));
float4 Multiply0=_ReflectionColor * TexCUBE0;
float4 Split1=Tex2D2;
float4 Add2=float4( Split1.x, Split1.x, Split1.x, Split1.x) + float4( Split1.y, Split1.y, Split1.y, Split1.y);
float4 Lerp5_0_NoInput = float4(0,0,0,0);
float4 Lerp5=lerp(Lerp5_0_NoInput,Multiply0,Add2);
float4 Tex2D4=tex2D(_RefStrGloss,(IN.uv_bumpAnimControl.xyxy).xy);
float4 Multiply2=_SeaSpd.xxxx * _Time;
float4 UV_Pan1=float4(Tex2D4.x + Multiply2.x,Tex2D4.y + Multiply2.x,Tex2D4.z,Tex2D4.w);
float4 Tex2D1=tex2D(_RefStrGloss,UV_Pan1.xy);
float4 Lerp4_0_NoInput = float4(0,0,0,0);
float4 Lerp4=lerp(Lerp4_0_NoInput,Tex2D1,Add2);
float4 Multiply1=_Shininess.xxxx * _SpecularColor;
float4 Lerp2_0_NoInput = float4(0,0,0,0);
float4 Lerp2=lerp(Lerp2_0_NoInput,Multiply1,Add2);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Add1;
o.Emission = Lerp5;
o.Specular = Lerp4;
o.Gloss = Lerp2;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}