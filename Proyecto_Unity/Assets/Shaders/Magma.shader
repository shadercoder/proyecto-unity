Shader "VertexMod"
{
	Properties 
	{
_MainTex("Textura Principal", 2D) = "black" {}
_TexColor("Tinte textura principal", Color) = (1,1,1,1)
_ColorStrength("Intensidad del color", Range(0,1) ) = 1
_LightMap("Textura de luces", 2D) = "black" {}
_EmisionColor("Color Luz", Color) = (1,0.5034965,0,1)
_LightStrength("Rango de luz", Range(0,3) ) = 0.6859149
_FactorMov("palpito", Float) = 0.02

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
#pragma target 2.0


sampler2D _MainTex;
float4 _TexColor;
float _ColorStrength;
sampler2D _LightMap;
float4 _EmisionColor;
float _LightStrength;
float _FactorMov;

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
float2 uv_LightMap;

			};

			void vert (inout appdata_full v, out Input o) {
float4 Splat0=_SinTime.w;
float4 Abs0=abs(Splat0);
float4 Multiply1=_FactorMov.xxxx * Abs0;
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
				
float4 Tex2D0=tex2D(_MainTex,(IN.uv_MainTex.xyxy).xy);
float4 Multiply2=_TexColor * _ColorStrength.xxxx;
float4 Multiply0=Tex2D0 * Multiply2;
float4 Tex2D1=tex2D(_LightMap,(IN.uv_LightMap.xyxy).xy);
float4 Add0=_SinTime + float4( 1.0, 1.0, 1.0, 1.0 );
float4 Abs0=abs(Add0);
float4 Multiply4=Abs0 * _LightStrength.xxxx;
float4 Multiply3=_EmisionColor * Multiply4;
float4 Multiply1=Tex2D1 * Multiply3;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply0;
o.Emission = Multiply1;
o.Gloss = Multiply1;
o.Alpha = Tex2D0.aaaa;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}