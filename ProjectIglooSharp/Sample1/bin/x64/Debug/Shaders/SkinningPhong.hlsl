Texture2D diffuseTex : register(t0);
SamplerState texSampler : register(s0);

cbuffer CBufferPerFrame : register(b0)
{
	row_major float4x4 viewProj;
	float4 lightDirection;
	float4 eyePos;
}

cbuffer CBufferPerObject : register(b1)
{
	row_major float4x4 world;
	row_major float4x4 boneMatrices[96];
}

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL0;
	float3 worldPos : TEXCOORD1;
};

struct VS_IN
{
	float4 pos : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL0;
	float3 weights : WEIGHTS;
	uint4 boneIndices : BONEINDICES;
};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	float weights[4] = { 0.0f, 0.0f, 0.0f, 0.0f };

	weights[0] = input.weights.x;
	weights[1] = input.weights.y;
	weights[2] = input.weights.z;
	weights[3] = 1 - input.weights.x - input.weights.y - input.weights.z;

	float3 pos = float3(0, 0, 0);
    float3 normal = float3(0, 0, 0);

	for (int i = 0; i < 4; i++)
	{
		pos += weights[i] * mul(float4(input.pos.xyz, 1.0f), boneMatrices[input.boneIndices[i]]).xyz;
	}

	float4 worldPos = mul(float4(pos,1), world);
	output.pos = mul(worldPos, viewProj);
	output.texCoord = input.texCoord;
	output.normal = input.normal;
	output.worldPos = worldPos.xyz;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	float3 lightDir = normalize(lightDirection).xyz;
	float3 normal = normalize(input.normal);
	float NoL = max(dot(normal, lightDir), 0);
	float3 diffuse = NoL * diffuseTex.Sample(texSampler, input.texCoord) * 2;
	
	float3 eyeVector = normalize(eyePos.xyz - input.worldPos);
	float3 reflectionVector = reflect(-lightDir, normal);
	float specDot = max(dot(eyeVector, reflectionVector), 0);
	float3 specular = float3(0, 0, 0);

		specular = 1000* pow(specDot, 20) * float3(1,1,1);


	return float4(diffuse  + specular, 1);
}
