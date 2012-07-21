Shader "Planet/SeresFiltro"
{
	Properties 
	{
_Difuso("_Difuso", 2D) = "black" {}
_FiltroOn("_FiltroOn", Range(0,0.5) ) = 0
_Tinte("_Tinte", Color) = (1,1,1,1)
_Bump("_Bump", 2D) = "black" {}
_Luces("_Luces", 2D) = "black" {}
_Emision("_Emision", Float) = 1
_Ramp("_Ramp", 2D) = "black" {}

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


sampler2D _Difuso;
float _FiltroOn;
float4 _Tinte;
sampler2D _Bump;
sampler2D _Luces;
float _Emision;
sampler2D _Ramp;

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
				float2 uv_Difuso;
float2 uv_Bump;
float2 uv_Luces;

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
				
float4 Tex2D0=tex2D(_Difuso,(IN.uv_Difuso.xyxy).xy);
float4 Multiply2=_Tinte * Tex2D0;
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_Bump,(IN.uv_Bump.xyxy).xy)).xyz, 1.0 );
float4 UnpackNormal0=float4(UnpackNormal(Tex2DNormal0).xyz, 1.0);
float4 Tex2D1=tex2D(_Luces,(IN.uv_Luces.xyxy).xy);
float4 Multiply1=Tex2D1 * _Emision.xxxx;
float4 Multiply0=_Tinte * _FiltroOn.xxxx;
float4 Add1=Multiply1 + Multiply0;
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply2;
o.Normal = UnpackNormal0;
o.Emission = Add1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}