void Main_Light_half(float3 WorldPos, out half3 Direction, out half3 Color, out half Attenuation)
{
#if SHADERGRAPH_PREVIEW
	Direction = half3(0.5, 0.5, -0.5);
	Color = 1;
	Attenuation = 1;
#else
#if SHADOWS_SCREEN
	half4 clipPos = TransformWorldToHClip(WorldPos);
	half4 shadowCoord = ComputeScreenPos(clipPos);
#else
	half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	Attenuation = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
#endif
}

void Main_Light_float(float3 WorldPos, out float3 Direction, out float3 Color, out float Attenuation)
{
#if SHADERGRAPH_PREVIEW
	Direction = float3(0.5, 0.5, -0.5);
	Color = 1;
	Attenuation = 1;
#else
#if SHADOWS_SCREEN
	float4 clipPos = TransformWorldToHClip(WorldPos);
	float4 shadowCoord = ComputeScreenPos(clipPos);
#else
	float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	Attenuation = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
#endif
}