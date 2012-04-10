Shader "Planet/Oceano"
{
	Properties 
	{
_ColorNear("_ColorNear", Color) = (0,0.8741257,1,1)
_ColorFar("_ColorFar", Color) = (0,0.1811397,0.2910448,1)
_densidadAgua("_densidadAgua", Range(0,0.05) ) = 0.0002645503
_RefStrGloss("_RefStrGloss", 2D) = "gray" {}
_Normals("_Normals", 2D) = "bump" {}
_ReflectionCubemap("_ReflectionCubemap", Cube) = "black" {}
_ReflectionColor("_ReflectionColor", Color) = (0,0.6223779,1,1)
_SpecularColor("_SpecularColor", Color) = (1,0.8071182,0.5074627,1)
_Shininess("_Shininess", Range(0.1,4) ) = 1.814286
_SeaSpd("_SeaSpd", Float) = 0.9

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="False"
"RenderType"="Transparent"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Blend SrcColor DstAlpha
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


float4 _ColorNear;
float4 _ColorFar;
float _densidadAgua;
sampler2D _RefStrGloss;
sampler2D _Normals;
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
float2 uv_Normals;
float3 simpleWorldRefl;
float2 uv_RefStrGloss;

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
float4 Lerp0=lerp(_ColorFar,_ColorNear,Pow0);
float4 Multiply2=_SeaSpd.xxxx * _Time;
float4 UV_Pan0=float4((IN.uv_Normals.xyxy).x + Multiply2.x,(IN.uv_Normals.xyxy).y + Multiply2.x,(IN.uv_Normals.xyxy).z,(IN.uv_Normals.xyxy).w);
float4 Tex2D0=tex2D(_Normals,UV_Pan0.xy);
float4 UnpackNormal0=float4(UnpackNormal(Tex2D0).xyz, 1.0);
float4 TexCUBE0=texCUBE(_ReflectionCubemap,float4( IN.simpleWorldRefl.x, IN.simpleWorldRefl.y,IN.simpleWorldRefl.z,1.0 ));
float4 Multiply0=_ReflectionColor * TexCUBE0;
float4 Tex2D1=tex2D(_RefStrGloss,(IN.uv_RefStrGloss.xyxy).xy);
float4 Multiply1=_Shininess.xxxx * _SpecularColor;
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Lerp0;
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp0;
o.Normal = UnpackNormal0;
o.Emission = Multiply0;
o.Specular = Tex2D1.aaaa;
o.Gloss = Multiply1;
o.Alpha = Invert0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}