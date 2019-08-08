cbuffer CBufferPerFrame : register(b0)
{
	row_major float4x4 viewProj;
	float4 lightDirection;
	float4 eyePos;
	float4 ColorObject;
	float roughnessOrenNayar;
	int DiffuseModels;
	int SpecularModels;
}

cbuffer CBufferPerObject : register(b1)
{
	row_major float4x4 world;
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
};

static const float PI = 3.1416;

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	float4 worldPos = mul(float4(input.pos.xyz, 1), world);
	output.pos = mul(worldPos, viewProj);
	output.texCoord = input.texCoord;
	float3x3 normalRot = (float3x3)world;
	output.normal = mul(input.normal, normalRot);
	output.worldPos = worldPos.xyz;

	return output;
}

float3 LambertDiffuse(float3 lightDir, float3 normal, float2 texCoord) // textCoord - texture coordanate (future)
{
	lightDir = normalize(lightDir).xyz; //Normalize the vector Light Direction
	normal = normalize(normal); //Normalize the Normal Vector
	float NoL = max(dot(normal, -lightDir), 0); //Dot is the product vector between N (normal) vector and L (Light direction)
	float lightIntensity = 1.0f;

	//Pass Diffuse intensity
	return NoL * (ColorObject.rgb * 1/PI) *lightIntensity;
}

float3 OrenNayarDiffuse(float3 eyePosition, float3 worldPos, float3 lightDir, float3 normal, float2 texCoord)
{
	float3 eyeVector = normalize(eyePosition.xyz - worldPos);
	lightDir = normalize(lightDir).xyz; 
	normal = normalize(normal); 
	float NoL = max(dot(normal, -lightDir), 0); 
	float NoV = max(dot(normal, eyeVector), 0);
	float LoV = max(dot(lightDir, eyeVector), 0);
	float lightIntensity = 1.0f;
	float S = LoV - (NoL*NoV);
	float T;

	if (S <= 0) 
	{
		T = 1;
	}
	else
	{
		T = max(NoL, NoV);
	}
	float PI = 3.1416;
	float Ratio = roughnessOrenNayar; 

	float A = 1 - (0.5*(Ratio*Ratio)/((Ratio*Ratio)+0.33));
	float B = (0.45) *((Ratio*Ratio)/((Ratio*Ratio)+0.09));	
	float DistFacetSlope = (A + B*S / T);

	//Pass Diffuse intensity
	return  NoL *  (ColorObject.rgb * 1 / PI) * lightIntensity  * DistFacetSlope;
}


//float A = 1 / (PI + ((PI / 2) - (2 / 3))*Ratio);
//float B = Ratio / (PI + ((PI / 2) - (2 / 3))*Ratio);

float3 PhongSpecular(float3 eyePosition, float3 worldPos, float3 normal, float3 lightDir)
{
	float3 eyeVector = normalize(eyePosition.xyz - worldPos);
	float3 reflectionVector = reflect(normalize(lightDir), normalize(normal));
	float lightIntensity = 1.0f;

	float specDot = max(dot(eyeVector, reflectionVector), 0);
	return pow(specDot, 128) * float3(1,1,1) * lightIntensity;
}

float3 BlinnPhongSpecular(float3 eyePosition, float3 worldPos, float3 normal1, float3 lightDir1)
{
	float3 eyeVector = normalize(eyePosition.xyz - worldPos);
	float3 lightDir = -lightDir1;
	float3 halfVector = normalize(lightDir + eyeVector);
	float3 normal = normalize(normal1);
	float lightIntensity = 1.0f;

	float specDot = max(dot(halfVector, normal), 0);
	return pow(specDot, 128) * float3(1, 1, 1) * lightIntensity;
}

// Cook-Torrance

float GGXDistribution(float3 Normal, float3 HalfVector, float Roughness)
{
	float NoH = dot(Normal, HalfVector);
	float Roughness2 = Roughness * Roughness;
	float NoH2 = NoH * NoH;
	return (NoH * Roughness2) / (PI * (NoH2 * Roughness2 + (1 - NoH2)) * (NoH2 * Roughness2 + (1 - NoH2)));
}

float3 Fresnel(float cosT, float3 F0)
{
	return F0 + (1 - F0) * pow(1 - cosT, 5);
}

float GGXGeometryTerm(float3 ViewVector, float3 Normal, float3 HalfVector, float Roughness)
{
	float GGXGeometry;
	float VoH = saturate(dot(ViewVector, HalfVector));
	float VoH2 = VoH * VoH;
	if (VoH2 / saturate(dot(ViewVector, Normal)) > 0)
	{
		GGXGeometry = (1 * 2) / (1 + sqrt(1 + Roughness * Roughness * (1 - VoH2) / VoH2));
	}
	else 
	{
		GGXGeometry = (0 * 2) / (1 + sqrt(1 + Roughness * Roughness * (1 - VoH2) / VoH2));
	}
	
	return GGXGeometry;
}

float3 CookTorranceSpecular(float3 lightDir1, float3 eyePosition,float3 worldPos, float3 normal1)
{
	float3 lightDir = -lightDir1;
	float3 viewVector = normalize(eyePosition.xyz - worldPos);
	float3 halfVector = normalize(lightDir + viewVector);
	float3 normal = normalize(normal1);
	float lightIntensity = 0.3f;

	float  NoV = saturate(dot(normal, viewVector));
	float geometry = GGXGeometryTerm(viewVector, normal, halfVector, roughnessOrenNayar);
	geometry = geometry * GGXGeometryTerm(viewVector, normal, halfVector, roughnessOrenNayar);
	float denominator = saturate(4 * (NoV * saturate(dot(halfVector, normal)))  );
	float3 fresnel = Fresnel(saturate(dot(halfVector, viewVector)), float3(0.04, 0.04, 0.04));
	float3 specular = (geometry * fresnel * GGXDistribution(normal, halfVector, roughnessOrenNayar)/ denominator); // denominator;
	specular = lightIntensity * max(specular, 0.0f);

	return specular;
}

float4 PS(PS_IN input) : SV_Target
{
	float3 diffuse;
	float3 specular;

	if (DiffuseModels == 0) 
	{
		diffuse = OrenNayarDiffuse(eyePos.xyz, input.worldPos, lightDirection.xyz, input.normal, input.texCoord);
	}
	else 
	{
		diffuse = LambertDiffuse(lightDirection.xyz, input.normal, input.texCoord);
	}


	if (SpecularModels == 0)
	{
		specular = BlinnPhongSpecular(eyePos.xyz, input.worldPos, input.normal, lightDirection.xyz);
	}
	else
	{
		specular = CookTorranceSpecular(lightDirection.xyz, eyePos.xyz, input.worldPos, input.normal);
	}

	float3 ambient = float3(0.15f, 0.0f, 0.0f);
	//float3 ambient = float3(0.0f, 0.0f, 0.0f);
	return float4(pow(ambient + specular + diffuse,0.4545),1); //Gamma correction resposta do monitor para anular o efeito de resposta do monitor

}
