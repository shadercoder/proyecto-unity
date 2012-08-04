Shader "Planet/Habitats"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_Ilum("_Ilum", 2D) = "black" {}
_volcanico("_volcanico", 2D) = "black" {}
_volcanicoEmision("_volcanicoEmision", 2D) = "black" {}
_Emision("_Emision", Range(0.05,1) ) = 0.5
_tundra("_tundra", 2D) = "black" {}
_desierto("_desierto", 2D) = "black" {}
_costa("_costa", 2D) = "black" {}
_crop("Suavizado del recorte", Range(0,1) ) = 0.5
_FiltroTex("_FiltroTex", 2D) = "black" {}
_FiltroOn("_FiltroOn", Float) = 0

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
sampler2D _Ilum;
sampler2D _volcanico;
sampler2D _volcanicoEmision;
float _Emision;
sampler2D _tundra;
sampler2D _desierto;
sampler2D _costa;
float _crop;
sampler2D _FiltroTex;
float _FiltroOn;

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
				float2 uv_volcanico;
float2 uv_MainTex;
float2 uv_tundra;
float2 uv_desierto;
float2 uv_costa;
float2 uv_FiltroTex;
float2 uv_volcanicoEmision;

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
				
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - _FiltroOn.xxxx;
float4 Sampled2D4=tex2D(_volcanico,IN.uv_volcanico.xy);
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Splat3=Sampled2D0.x;
float4 Lerp3_0_NoInput = float4(0,0,0,0);
float4 Lerp3=lerp(Lerp3_0_NoInput,Sampled2D4,Splat3);
float4 Sampled2D3=tex2D(_tundra,IN.uv_tundra.xy);
float4 Splat0=Sampled2D0.y;
float4 Lerp2=lerp(Lerp3,Sampled2D3,Splat0);
float4 Sampled2D2=tex2D(_desierto,IN.uv_desierto.xy);
float4 Splat2=Sampled2D0.z;
float4 Lerp1=lerp(Lerp2,Sampled2D2,Splat2);
float4 Sampled2D1=tex2D(_costa,IN.uv_costa.xy);
float4 Splat1=Sampled2D0.w;
float4 Lerp0=lerp(Lerp1,Sampled2D1,Splat1);
float4 Multiply1=Invert0 * Lerp0;
float4 Tex2D0=tex2D(_FiltroTex,(IN.uv_FiltroTex.xyxy).xy);
float4 Multiply2=Tex2D0 * _FiltroOn.xxxx;
float4 Add3=Multiply1 + Multiply2;
float4 Sampled2D5=tex2D(_volcanicoEmision,IN.uv_volcanicoEmision.xy);
float4 Splat8=Sampled2D0.x;
float4 Lerp4_0_NoInput = float4(0,0,0,0);
float4 Lerp4=lerp(Lerp4_0_NoInput,Sampled2D5,Splat8);
float4 Multiply0=Lerp4 * _Emision.xxxx;
float4 Multiply3=Multiply0 * Invert0;
float4 Multiply5=float4( 0.2,0.2,0.2,0.2 ) * Multiply2;
float4 Add4=Multiply3 + Multiply5;
float4 Splat4=Sampled2D0.x;
float4 Splat5=Sampled2D0.y;
float4 Add1=Splat4 + Splat5;
float4 Splat6=Sampled2D0.z;
float4 Splat7=Sampled2D0.w;
float4 Add2=Splat6 + Splat7;
float4 Add0=Add1 + Add2;
float4 Subtract0=Add0 - _crop.xxxx;
float4 Multiply4=Invert0 * Subtract0;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
clip( Multiply4 );
o.Albedo = Add3;
o.Emission = Add4;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}