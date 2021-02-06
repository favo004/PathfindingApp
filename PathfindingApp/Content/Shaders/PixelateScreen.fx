#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler inputTexture;
int pixelation;
float2 gameSize;

float4 MainPS(float2 originalUV: TEXCOORD0) : COLOR0
{
	originalUV *= gameSize;
	
	float2 newUV;
	newUV.x = round(originalUV.x / pixelation) * pixelation;
	newUV.y = round(originalUV.y / pixelation) * pixelation;

	newUV /= gameSize;

	return tex2D(inputTexture, newUV);

}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 MainPS();
	}
};