// -----------------------------------------------------------------------------
// Original code from SlimDX project.
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

cbuffer CBufferPerFrame : register(b0)
{
	row_major float4x4 viewProj;
}

cbuffer CBufferPerObject : register(b1)
{
	row_major float4x4 world;
	row_major float4x4 boneMatrices[96];
}




/*struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};*/

/*struct VS_IN
{
	float4 pos : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL0;
};*/

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL0;
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

	output.pos = float4(pos, 1.0f);
	output.pos = mul(output.pos, mul(world,viewProj));
	output.texCoord = input.texCoord;
	output.normal = input.normal;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	return float4(input.texCoord.x, input.texCoord.y, 0, 0);
}
