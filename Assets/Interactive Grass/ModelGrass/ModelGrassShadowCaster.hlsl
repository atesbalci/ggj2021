#ifndef MODEL_GRASS_SHADOW_CASTER
#define MODEL_GRASS_SHADOW_CASTER

struct ShadowPassAttributes
{
	float4 positionOS  : POSITION;
	float2 uv          : TEXCOORD0;
	half4  color       : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct ShadowPassVaryings
{
	float2 uv          : TEXCOORD0;
	float4 positionCS  : SV_POSITION;
};

struct SurfaceData
{
	half3 diffuse;  // diffuse color. should be black for metals.
	half  alpha;    // 0 for transparent materials, 1.0 for opaque.
};

float4 VertexFunction (ShadowPassAttributes IN);
void SurfaceFunction (ShadowPassVaryings IN, out SurfaceData surfaceData);

ShadowPassVaryings ShadowPassVertex (ShadowPassAttributes IN)
{
	float4 positionOS = VertexFunction(IN);
	VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz);

	ShadowPassVaryings OUT;
	OUT.uv = IN.uv;
	OUT.positionCS = vertexInput.positionCS;
	return OUT;
}

half4 ShadowPassFragment (ShadowPassVaryings IN) : SV_Target
{
	SurfaceData surfaceData;
	SurfaceFunction(IN, surfaceData);
	return half4(surfaceData.diffuse, surfaceData.alpha);
}

#endif