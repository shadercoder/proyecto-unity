Shader "Planet/planetaconramp"
{
	Properties 
	{
		_MainTex("Mapa terreno Base", 2D) = "black" {}
		_RampaColor("_RampaColor", 2D) = "black" {}
		_Normals("Mapa Relieve", 2D) = "bump" {}
		_Ilum("_Ilum", 2D) = "black" {}
		_BiasTerreno("_BiasTerreno", Range(-3,3) ) = 0.5
		_Amount("Extrusion", Range(-3,3) ) = 0.5
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
			sampler2D _RampaColor;
			sampler2D _Normals;
			sampler2D _Ilum;
			float _BiasTerreno;
			float _Amount;

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
				float2 uv_MainTex;
				float2 uv_Normals;
				float3 viewDir;

			};

			void vert (inout appdata_full v, out Input o) {
				float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
				#if !defined(SHADER_API_OPENGL)
				float4 tex = tex2Dlod (_MainTex, float4(v.texcoord.xyz,0));
				v.vertex.xyz += v.normal * tex.rgb * _Amount;
				#endif
			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Tex2D1=tex2D(_RampaColor,Sampled2D0.xy);
float4 Sampled2D2=tex2D(_Normals,IN.uv_Normals.xy);
float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - Sampled2D0;
float4 ParallaxOffset0= ParallaxOffset( Invert0.x, _BiasTerreno.xxxx.x, float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz).xyxy;
float4 Add0=Sampled2D2 + ParallaxOffset0;
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Tex2D1;
o.Normal = Add0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}