#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler inputTexture;

float4 MainPS(float2 textureCoordinates: TEXCOORD0): COLOR0
{
	float4 color = tex2D(inputTexture, textureCoordinates);
	color.rgba = float4(1.0f, 1.0f, 1.0f, 1.0f);
	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 MainPS();
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
	}
};