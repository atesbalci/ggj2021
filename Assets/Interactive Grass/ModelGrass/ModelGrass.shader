Shader "Interactive Grass/Model Grass" {
	Properties {
		[Header(Surface)]
		_BaseColor                            ("Base Color", Color) = (1, 1, 1, 1)
		_BaseMap                              ("Base Map", 2D) = "white" {}
		[Toggle(_NORMALMAP)] _EnableNormalMap ("Enable Normal Map", Float) = 0
		[Normal][NoScaleOffset]_NormalMap     ("Normal Map", 2D) = "bump" {}
		_NormalMapScale                       ("Normal Map Scale", Float) = 1
		[HDR]_Emission                        ("Emission Color", Color) = (0,0,0,1)
		_Cutoff                               ("Cutoff", Range(0, 1)) = 0.5
		[Header(Animation)]
		_Amplitude     ("Amplitude", Float) = 0.2
		_WaveX         ("Waves X", Float) = 1
		_WaveY         ("Waves Y", Float) = 1
		_TimeScale     ("Time Scale", Float) = 1
		_MoveVec       ("Move Vec", Vector) = (0, 0, 0, 0)
		[Header(Burn)]
		_BurnAmount    ("Amount", Range(0.0, 1.0)) = 0.0
		_BurnLineWidth ("Line Width", Range(0.0, 0.2)) = 0.1
		_BurnColor1    ("First Color", Color) = (1, 0, 0, 1)
		_BurnColor2    ("Second Color", Color) = (1, 0, 0, 1)
		_BurnMap       ("Burn", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True" }
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST, _BurnMap_ST;
			half4 _BaseColor, _BurnColor1, _BurnColor2, _Emission;
			half _NormalMapScale, _Cutoff;
			half _BurnAmount, _BurnLineWidth;

			// vertex animation
			float _Amplitude, _WaveX, _WaveY, _TimeScale;
			float3 _MoveVec;

			TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
			TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
			TEXTURE2D(_BurnMap); SAMPLER(sampler_BurnMap);
		CBUFFER_END
		ENDHLSL

		Pass {
			Tags { "LightMode" = "UniversalForward" }
			HLSLPROGRAM
			#pragma vertex SurfaceVertex
			#pragma fragment SurfaceFragment
			#include "ModelGrass.hlsl"

			#pragma shader_feature _NORMALMAP
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			float4 VertexFunction (Attributes IN)
			{
				float4 p = IN.positionOS;
				p.x += _Amplitude * sin(p.x * _WaveX + _Time.y * _TimeScale + p.z * _WaveY) * IN.color.r + _MoveVec.x * IN.color.r;
				p.z += _Amplitude * sin(p.x * _WaveX + _Time.y * _TimeScale + p.z * _WaveY) * IN.color.r + _MoveVec.z * IN.color.r;
				return p;
			}
			void SurfaceFunction (Varyings IN, out SurfaceData surfaceData)
			{
				float2 uv1 = TRANSFORM_TEX(IN.uv, _BaseMap);
				half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv1) * _BaseColor;
				clip(baseColor.a - _Cutoff);

				float2 uv2 = TRANSFORM_TEX(IN.uv, _BurnMap);
				half4 burn = SAMPLE_TEXTURE2D(_BurnMap, sampler_BurnMap, uv2);
				clip(burn.r - _BurnAmount);

				half t = 1.0 - smoothstep(0.0, _BurnLineWidth, burn.r - _BurnAmount);
				half3 burnColor = lerp(_BurnColor1.rgb, _BurnColor2.rgb, t);
				baseColor.rgb = lerp(baseColor.rgb, burnColor, t * step(0.0001, _BurnAmount));

				surfaceData = (SurfaceData)0;
				surfaceData.diffuse = baseColor.rgb;
				surfaceData.ao = 1.0;
#ifdef _NORMALMAP
				surfaceData.normalWS = GetPerPixelNormalScaled(TEXTURE2D_ARGS(_NormalMap, sampler_NormalMap), uv1, IN.normalWS, IN.tangentWS, _NormalMapScale);
#else
				surfaceData.normalWS = normalize(IN.normalWS);
#endif
				surfaceData.emission = _Emission.rgb;
				surfaceData.alpha = 1.0;
			}
			ENDHLSL
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster" }
			ZWrite On ZTest LEqual Cull [_Cull]

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma multi_compile_instancing

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment
			#include "ModelGrassShadowCaster.hlsl"

			float4 VertexFunction (ShadowPassAttributes IN)
			{
				float4 p = IN.positionOS;
				p.x += _Amplitude * sin(p.x * _WaveX + _Time.y * _TimeScale + p.z * _WaveY) * IN.color.r + _MoveVec.x * IN.color.r;
				p.z += _Amplitude * sin(p.x * _WaveX + _Time.y * _TimeScale + p.z * _WaveY) * IN.color.r + _MoveVec.z * IN.color.r;
				return p;
			}
			void SurfaceFunction (ShadowPassVaryings IN, out SurfaceData surfaceData)
			{
				float2 uv1 = TRANSFORM_TEX(IN.uv, _BaseMap);
				half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv1) * _BaseColor;
				clip(baseColor.a - _Cutoff);

				float2 uv2 = TRANSFORM_TEX(IN.uv, _BurnMap);
				half4 burn = SAMPLE_TEXTURE2D(_BurnMap, sampler_BurnMap, uv2);
				clip(burn.r - _BurnAmount);

				surfaceData = (SurfaceData)0;
				surfaceData.diffuse = baseColor.rgb;
				surfaceData.alpha = 1.0;
			}
			ENDHLSL
		}
		UsePass "Universal Render Pipeline/Lit/DepthOnly"
		UsePass "Universal Render Pipeline/Lit/Meta"
	}
	FallBack Off
}
