Shader "Planet/OceanoRamp"
{
	Properties 
	{
_ColorNear("_ColorNear", Color) = (0.3411765,0.7215686,0.1764706,1)
_ColorFar("_ColorFar", Color) = (0,0.2666667,0.1647059,1)
_colorPlayaAgua("_colorPlayaAgua", Color) = (0.7568628,0.8078431,0.3294118,1)
_ColorPlaya("_ColorPlaya", Color) = (0.8039216,0.7450981,0.6156863,1)
_densidadAgua("_densidadAgua", Range(0,0.05) ) = 0.004
_MainTex("_MainTex", 2D) = "black" {}
_Ilum("_Ilum", 2D) = "black" {}

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="True"
"RenderType"="Transparent"

		}

		
Cull Back
ZWrite Off
ZTest LEqual
ColorMask RGB
Blend SrcAlpha OneMinusSrcAlpha
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  addshadow vertex:vert
#pragma target 3.0


float4 _ColorNear;
float4 _ColorFar;
float4 _colorPlayaAgua;
float4 _ColorPlaya;
float _densidadAgua;
sampler2D _MainTex;
sampler2D _Ilum;

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
				float3 viewDir;
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
				
float4 Fresnel0_1_NoInput = float4(0,0,1,1);
float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
float4 Pow0=pow(_densidadAgua.xxxx,Fresnel0);
float4 Lerp1=lerp(_ColorNear,_ColorFar,Pow0);
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
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Add1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}