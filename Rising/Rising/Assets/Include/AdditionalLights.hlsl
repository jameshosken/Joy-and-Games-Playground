Light GetAdditionalLightCustom(int i, float3 positionWS)
{
	int perObjectLightIndex = GetPerObjectLightIndex(i);

	// The following code will turn into a branching madhouse on platforms that don't support
	// dynamic indexing. Ideally we need to configure light data at a cluster of
	// objects granularity level. We will only be able to do that when scriptable culling kicks in.
	// TODO: Use StructuredBuffer on PC/Console and profile access speed on mobile that support it.
	// Abstraction over Light input constants
	float3 lightPositionWS = _AdditionalLightsPosition[perObjectLightIndex].xyz;
	half4 distanceAndSpotAttenuation = _AdditionalLightsAttenuation[perObjectLightIndex];
	half4 spotDirection = _AdditionalLightsSpotDir[perObjectLightIndex];

	float3 lightVector = lightPositionWS - positionWS;
	float distanceSqr = max(dot(lightVector, lightVector), HALF_MIN);
	//float distanceSqr = 1;

	half3 lightDirection = half3(lightVector * rsqrt(distanceSqr));
	half attenuation = DistanceAttenuation(1, distanceAndSpotAttenuation.xy) * AngleAttenuation(spotDirection.xyz, lightDirection, distanceAndSpotAttenuation.zw);

	Light light;
	light.direction = lightDirection;
	light.distanceAttenuation = attenuation;
	light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, positionWS);
	light.color = _AdditionalLightsColor[perObjectLightIndex].rgb;

	// In case we're using light probes, we can sample the attenuation from the `unity_ProbesOcclusion`
#if defined(LIGHTMAP_ON)
	// First find the probe channel from the light.
	// Then sample `unity_ProbesOcclusion` for the baked occlusion.
	// If the light is not baked, the channel is -1, and we need to apply no occlusion.
	half4 lightOcclusionProbeInfo = _AdditionalLightsOcclusionProbes[perObjectLightIndex];

	// probeChannel is the index in 'unity_ProbesOcclusion' that holds the proper occlusion value.
	int probeChannel = lightOcclusionProbeInfo.x;

	// lightProbeContribution is set to 0 if we are indeed using a probe, otherwise set to 1.
	half lightProbeContribution = lightOcclusionProbeInfo.y;

	half probeOcclusionValue = unity_ProbesOcclusion[probeChannel];
	light.distanceAttenuation *= max(probeOcclusionValue, lightProbeContribution);
#endif

	return light;

}


void AdditionalLights_half(half3 SpecColor, half Smoothness, half3 WorldPosition, half3 WorldNormal, half3 WorldView, out half3 Diffuse, out half3 Specular)
{
   half3 diffuseColor = 0;
   half3 specularColor = 0;

#ifndef SHADERGRAPH_PREVIEW
   Smoothness = exp2(10 * Smoothness + 1);
   WorldNormal = normalize(WorldNormal);
   WorldView = SafeNormalize(WorldView);
   int pixelLightCount = GetAdditionalLightsCount();
   for (int i = 0; i < pixelLightCount; ++i)
   {
       Light light = GetAdditionalLightCustom(i, WorldPosition);
       //half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);

	   half3 attenuatedLightColor = clamp(light.color * light.distanceAttenuation* light.shadowAttenuation, 0., 1.);

       diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
       specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, half4(SpecColor, 0), Smoothness);

	   
   }
#endif

   Diffuse = diffuseColor;
   Specular = specularColor;
}