Shader "Planet/Magma"
{
	Properties 
	{
_Diffuse("_Diffuse", 2D) = "black" {}
_Emission("_Emission", 2D) = "black" {}
_EmisionColor("_EmisionColor", Color) = (0.6716418,0.2506126,0.2506126,1)
_EmissionStrength("_EmissionStrength", Float) = 1
_Bump("_Bump", 2D) = "black" {}
_Normals("_Normals", 2D) = "black" {}
_Bias("_Bias", Range(-3,3) ) = 2
_Palpito("_Palpito", Float) = 0.01

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


sampler2D _Diffuse;
sampler2D _Emission;
float4 _EmisionColor;
float _EmissionStrength;
sampler2D _Bump;
sampler2D _Normals;
float _Bias;
float _Palpito;

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
				float2 uv_Diffuse;
float2 uv_Normals;
float3 viewDir;
float2 uv_Bump;
float2 uv_Emission;

			};

			void vert (inout appdata_full v, out Input o) {
float4 Splat0=_SinTime.w;
float4 Abs0=abs(Splat0);
float4 Multiply1=_Palpito.xxxx * Abs0;
float4 Multiply0=Multiply1 * float4( v.normal.x, v.normal.y, v.normal.z, 1.0 );
float4 Add0=Multiply0 + v.vertex;
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
				
float4 Sampled2D0=tex2D(_Diffuse,IN.uv_Diffuse.xy);
float4 Sampled2D3=tex2D(_Normals,IN.uv_Normals.xy);
float4 UnpackNormal0=float4(UnpackNormal(Sampled2D3).xyz, 1.0);
float4 Sampled2D2=tex2D(_Bump,IN.uv_Bump.xy);
float4 ParallaxOffset0= ParallaxOffset( Sampled2D2.x, _Bias.xxxx.x, float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz).xyxy;
float4 Add1=UnpackNormal0 + ParallaxOffset0;
float4 Sampled2D1=tex2D(_Emission,IN.uv_Emission.xy);
float4 Multiply1=_EmisionColor * Sampled2D1;
float4 Splat0=_SinTime.w;
float4 Abs0=abs(Splat0);
float4 Multiply2=_EmissionStrength.xxxx * Abs0;
float4 Multiply3=Multiply1 * Multiply2;
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Sampled2D0;
o.Normal = Add1;
o.Emission = Multiply3;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}