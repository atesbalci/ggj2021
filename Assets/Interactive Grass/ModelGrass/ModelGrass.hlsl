#ifndef MODEL_GRASS
#define MODEL_GRASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/BSDF.hlsl"

struct Attributes
{
	float4 positionOS   : POSITION;
	float3 normalOS     : NORMAL;
	float4 tangentOS    : TANGENT;
	half4  color        : COLOR;
	float2 uv           : TEXCOORD0;
#if LIGHTMAP_ON
	float2 uvLightmap   : TEXCOORD1;
#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float2 uv          : TEXCOORD0;
#if LIGHTMAP_ON
	float2 uvLightmap  : TEXCOORD1;
#endif
	float3 positionWS  : TEXCOORD2;
	half3  normalWS    : TEXCOORD3;
#ifdef _NORMALMAP
	half4 tangentWS    : TEXCOORD4;
#endif
	float4 positionCS  : SV_POSITION;
};

struct SurfaceData
{
	half3 diffuse;    // diffuse color. should be black for metals.
	half3 normalWS;   // normal in world space
	half  ao;         // ambient occlusion
	half3 emission;   // emissive color
	half  alpha;      // 0 for transparent materials, 1.0 for opaque.
};

struct LightingData 
{
	Light light;
	half3 viewDirectionWS;
	half3 normalWS;
};

float4 VertexFunction (Attributes IN);
void SurfaceFunction (Varyings IN, out SurfaceData surfaceData);

half3 GetPerPixelNormalScaled (TEXTURE2D_PARAM(normalMap, sampler_NormalMap), float2 uv, half3 normal, half4 tangent, half scale)
{
	half3 bitangent = cross(normal, tangent.xyz) * tangent.w;
	half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(normalMap, sampler_NormalMap, uv), scale);
	return normalize(mul(normalTS, half3x3(tangent.xyz, bitangent, normal)));
}

// defined in latest URP
#if SHADER_LIBRARY_VERSION_MAJOR < 9
// Computes the world space view direction (pointing towards the viewer).
float3 GetWorldSpaceViewDir (float3 positionWS)
{
	if (unity_OrthoParams.w == 0)
	{
		// Perspective
		return _WorldSpaceCameraPos - positionWS;
	}
	else
	{
		// Orthographic
		float4x4 viewMat = GetWorldToViewMatrix();
		return viewMat[2].xyz;
	}
}
#endif

#ifdef CUSTOM_LIGHTING_FUNCTION
	half4 CUSTOM_LIGHTING_FUNCTION (SurfaceData surfaceData, LightingData lightingData);
#else
	half4 CUSTOM_LIGHTING_FUNCTION (SurfaceData surfaceData, LightingData lightingData)
	{
		float3 V = lightingData.viewDirectionWS;
		float3 N = surfaceData.normalWS;
		float3 L = lightingData.light.direction;

		float ndl = dot(N, L);
		float back = clamp(dot(V, -L), 0.0, 1.0);
		back = lerp(clamp(-ndl, 0.0, 1.0), back, 0.85);
		ndl = max(0.0, ndl * 0.7 + 0.3);

		half4 c;
		c.rgb = (back + ndl) * surfaceData.diffuse * lightingData.light.color + surfaceData.emission;
		c.a = surfaceData.alpha;
		return c;
	}
#endif

Varyings SurfaceVertex (Attributes IN)
{
	float4 positionOS = VertexFunction(IN);
	VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz);
	VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

	Varyings OUT;
	OUT.uv = IN.uv;
#if LIGHTMAP_ON
	OUT.uvLightmap = IN.uvLightmap.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
	OUT.positionWS = vertexInput.positionWS;
	OUT.normalWS = vertexNormalInput.normalWS;
#ifdef _NORMALMAP
	OUT.tangentWS = float4(vertexNormalInput.tangentWS, IN.tangentOS.w * GetOddNegativeScale());
#endif
	OUT.positionCS = vertexInput.positionCS;
	return OUT;
}

half4 SurfaceFragment (Varyings IN) : SV_Target
{
	SurfaceData surfaceData;
	SurfaceFunction(IN, surfaceData);

	half3 viewDirectionWS = normalize(GetWorldSpaceViewDir(IN.positionWS));

	// shadowCoord is position in shadow light space
	float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
	Light light = GetMainLight(shadowCoord);

	LightingData lightingData;
	lightingData.light = light;
	lightingData.viewDirectionWS = viewDirectionWS;
	lightingData.normalWS = surfaceData.normalWS;
	return CUSTOM_LIGHTING_FUNCTION(surfaceData, lightingData);
}

#endif