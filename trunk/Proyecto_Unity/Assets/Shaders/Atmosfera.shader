Shader "Planet/Atmosfera2"
{
	Properties 
	{
_BaseNubes("_BaseNubes", 2D) = "black" {}
_atmLejos("_atmLejos", Color) = (0,0.5481684,0.7761194,1)
_atmCerca("_atmCerca", Color) = (1,1,1,1)
_Espesor("_Espesor", Range(10,0) ) = 7.507936
_fogColor("_fogColor", Color) = (0.06983738,0.1717742,0.4253731,1)
_densidadNiebla("_densidadNiebla", Range(0,5) ) = 1.5
_baseNubes("_baseNubes", 2D) = "black" {}
_velocidadNubes("_velocidadNubes", Float) = 0.02
_Transparencia("_Transparencia", Float) = 0.4

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="True"
"RenderType"="Transparent"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGB
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  noforwardadd alpha decal:add vertex:vert
#pragma target 3.0


sampler2D _BaseNubes;
float4 _atmLejos;
float4 _atmCerca;
float _Espesor;
float4 _fogColor;
float _densidadNiebla;
sampler2D _baseNubes;
float _velocidadNubes;
float _Transparencia;

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
float2 uv_baseNubes;

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
float4 Pow0=pow(Fresnel0,_Espesor.xxxx);
float4 Saturate0=saturate(Pow0);
float4 Lerp0=lerp(_atmCerca,_atmLejos,Saturate0);
float4 Multiply0=Lerp0 * Saturate0;
float4 Multiply4=Pow0 * _fogColor;
float4 Lerp1_0_NoInput = float4(0,0,0,0);
float4 Lerp1=lerp(Lerp1_0_NoInput,Multiply4,_densidadNiebla.xxxx);
float4 Add0=Multiply0 + Lerp1;
float4 Multiply1=_Time * _velocidadNubes.xxxx;
float4 UV_Pan0=float4((IN.uv_baseNubes.xyxy).x + Multiply1.y,(IN.uv_baseNubes.xyxy).y,(IN.uv_baseNubes.xyxy).z,(IN.uv_baseNubes.xyxy).w);
float4 Tex2D0=tex2D(_baseNubes,UV_Pan0.xy);
float4 Multiply2=_Transparencia.xxxx * Tex2D0;
float4 Add1=Add0 + Multiply2;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Add1;
o.Alpha = Tex2D0.aaaa;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}