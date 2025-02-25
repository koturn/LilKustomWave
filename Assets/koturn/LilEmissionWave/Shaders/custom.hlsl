//----------------------------------------------------------------------------------------------------------------------
// Macro
#include "lil_current_version.hlsl"

// Custom variables
//#define LIL_CUSTOM_PROPERTIES \
//    float _CustomVariable;
#define LIL_CUSTOM_PROPERTIES \
    float _DisplayTime; \
    float _CrossFadeTime; \
    float _NumColors; \
    float4 _Color2; \
    float4 _Color3; \
    float4 _Color4; \
    float4 _EmissionWaveColor1; \
    float4 _EmissionWaveColor2; \
    float4 _EmissionWaveColor3; \
    float4 _EmissionWaveColor4; \
    float _EmissionWaveNoiseAmp; \
    float _EmissionWaveSpeed; \
    float _EmissionWaveInitPhase; \
    float2 _EmissionWaveParam; \
    float _EmissionPosMin; \
    float _EmissionPosMax; \
    float3 _WaveAxisAngles; \
    int _WavePosSpace; \
    int _WaveAxis;

// Custom textures
#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_EmissionWaveMask);

// Add vertex shader input
//#define LIL_REQUIRE_APP_POSITION
//#define LIL_REQUIRE_APP_TEXCOORD0
//#define LIL_REQUIRE_APP_TEXCOORD1
//#define LIL_REQUIRE_APP_TEXCOORD2
//#define LIL_REQUIRE_APP_TEXCOORD3
//#define LIL_REQUIRE_APP_TEXCOORD4
//#define LIL_REQUIRE_APP_TEXCOORD5
//#define LIL_REQUIRE_APP_TEXCOORD6
//#define LIL_REQUIRE_APP_TEXCOORD7
//#define LIL_REQUIRE_APP_COLOR
//#define LIL_REQUIRE_APP_NORMAL
//#define LIL_REQUIRE_APP_TANGENT
#define LIL_REQUIRE_APP_VERTEXID

// Add vertex shader output
//#define LIL_V2F_FORCE_TEXCOORD0
//#define LIL_V2F_FORCE_TEXCOORD1
//#define LIL_V2F_FORCE_POSITION_OS
//#define LIL_V2F_FORCE_POSITION_WS
//#define LIL_V2F_FORCE_POSITION_SS
//#define LIL_V2F_FORCE_NORMAL
//#define LIL_V2F_FORCE_TANGENT
//#define LIL_V2F_FORCE_BITANGENT
#if LIL_CURRENT_VERSION_VALUE == 34 && defined(UNITY_PASS_SHADOWCASTER)
// Work around for the following bug in lilxyzw/lilToon ver.1.4.0:
//   https://github.com/lilxyzw/lilToon/issues/98
// Fixed in lilxyzw/lilToon ver.1.4.1:
//   https://github.com/lilxyzw/lilToon/commit/a8548792c56537575bb2933d65233c8c9bdca4de
#    define LIL_CUSTOM_V2G_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
        float emissionWavePos : TEXCOORD ## id1;
#else
#    define LIL_CUSTOM_V2G_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
        float emissionWavePos : TEXCOORD ## id0;
#endif
#define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
    float emissionWavePos : TEXCOORD ## id0;

// Add vertex copy
#define LIL_CUSTOM_VERT_COPY \
    output.emissionWavePos = pickupPosition(getEmissionPos(input.positionOS)) \
        + (2.0 * rand((float)input.vertexID, LIL_TIME) - 1.0) * _EmissionWaveNoiseAmp;

// Inserting a process into the vertex shader
//#define LIL_CUSTOM_VERTEX_OS
//#define LIL_CUSTOM_VERTEX_WS

// Inserting a process into pixel shader
//#define BEFORE_xx
//#define OVERRIDE_xx

#define BEFORE_UNPACK_V2F \
    const float baseTime = LIL_TIME * _EmissionWaveSpeed + _EmissionWaveInitPhase - remap01(input.emissionWavePos, _EmissionPosMin, _EmissionPosMax); \
    const float crossFadeTime = max(1.0e-5, _CrossFadeTime); \
    const float oneCycleTime = _DisplayTime + crossFadeTime; \
    const float colorShiftTmpIdx1 = floor(fmodglsl(baseTime, oneCycleTime * _NumColors) / oneCycleTime); \
    const float colorShiftTmpIdx2 = round(fmodglsl(colorShiftTmpIdx1 + 1.0, _NumColors)); \
    const float colorShiftIdx1 = _EmissionWaveSpeed >= 0.0 ? colorShiftTmpIdx1 : colorShiftTmpIdx2; \
    const float colorShiftIdx2 = _EmissionWaveSpeed >= 0.0 ? colorShiftTmpIdx2 : colorShiftTmpIdx1; \
    const float crossFadeRate = fmodglsl(baseTime, oneCycleTime) / crossFadeTime; \
    float colorShiftBlend = saturate(crossFadeRate); \
    colorShiftBlend = (_EmissionWaveSpeed >= 0.0 ? colorShiftBlend : 1.0 - colorShiftBlend);

#define OVERRIDE_MAIN \
    LIL_GET_MAIN_TEX \
    LIL_APPLY_MAIN_TONECORRECTION \
    fd.col *= colorShiftGetTint(colorShiftIdx1, colorShiftIdx2, colorShiftBlend);

#define BEFORE_BLEND_EMISSION \
    const float sDiff = 2.0 * colorShiftBlend - 1.0; \
    const float eFact = pow(0.5 * cos(clamp(sDiff * _EmissionWaveParam.x, -1.0, 1.0) * UNITY_PI) + 0.5, _EmissionWaveParam.y); \
    fd.col.rgb += calcEmissionColor(getEmissionWaveColor(colorShiftIdx2) * LIL_SAMPLE_2D(_EmissionWaveMask, sampler_MainTex, fd.uvMain) * eFact, fd.col.a);


//----------------------------------------------------------------------------------------------------------------------
// Information about variables
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
// Vertex shader inputs (appdata structure)
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   input.positionOS        POSITION
// float2   input.uv0               TEXCOORD0
// float2   input.uv1               TEXCOORD1
// float2   input.uv2               TEXCOORD2
// float2   input.uv3               TEXCOORD3
// float2   input.uv4               TEXCOORD4
// float2   input.uv5               TEXCOORD5
// float2   input.uv6               TEXCOORD6
// float2   input.uv7               TEXCOORD7
// float4   input.color             COLOR
// float3   input.normalOS          NORMAL
// float4   input.tangentOS         TANGENT
// uint     vertexID                SV_VertexID

//----------------------------------------------------------------------------------------------------------------------
// Vertex shader outputs or pixel shader inputs (v2f structure)
//
// The structure depends on the pass.
// Please check lil_pass_xx.hlsl for details.
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   output.positionCS       SV_POSITION
// float2   output.uv01             TEXCOORD0 TEXCOORD1
// float2   output.uv23             TEXCOORD2 TEXCOORD3
// float3   output.positionOS       object space position
// float3   output.positionWS       world space position
// float3   output.normalWS         world space normal
// float4   output.tangentWS        world space tangent

//----------------------------------------------------------------------------------------------------------------------
// Variables commonly used in the forward pass
//
// These are members of `lilFragData fd`
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   col                     lit color
// float3   albedo                  unlit color
// float3   emissionColor           color of emission
// -------- ----------------------- --------------------------------------------------------------------
// float3   lightColor              color of light
// float3   indLightColor           color of indirectional light
// float3   addLightColor           color of additional light
// float    attenuation             attenuation of light
// float3   invLighting             saturate((1.0 - lightColor) * sqrt(lightColor));
// -------- ----------------------- --------------------------------------------------------------------
// float2   uv0                     TEXCOORD0
// float2   uv1                     TEXCOORD1
// float2   uv2                     TEXCOORD2
// float2   uv3                     TEXCOORD3
// float2   uvMain                  Main UV
// float2   uvMat                   MatCap UV
// float2   uvRim                   Rim Light UV
// float2   uvPanorama              Panorama UV
// float2   uvScn                   Screen UV
// bool     isRightHand             input.tangentWS.w > 0.0;
// -------- ----------------------- --------------------------------------------------------------------
// float3   positionOS              object space position
// float3   positionWS              world space position
// float4   positionCS              clip space position
// float4   positionSS              screen space position
// float    depth                   distance from camera
// -------- ----------------------- --------------------------------------------------------------------
// float3x3 TBN                     tangent / bitangent / normal matrix
// float3   T                       tangent direction
// float3   B                       bitangent direction
// float3   N                       normal direction
// float3   V                       view direction
// float3   L                       light direction
// float3   origN                   normal direction without normal map
// float3   origL                   light direction without sh light
// float3   headV                   middle view direction of 2 cameras
// float3   reflectionN             normal direction for reflection
// float3   matcapN                 normal direction for reflection for MatCap
// float3   matcap2ndN              normal direction for reflection for MatCap 2nd
// float    facing                  VFACE
// -------- ----------------------- --------------------------------------------------------------------
// float    vl                      dot(viewDirection, lightDirection);
// float    hl                      dot(headDirection, lightDirection);
// float    ln                      dot(lightDirection, normalDirection);
// float    nv                      saturate(dot(normalDirection, viewDirection));
// float    nvabs                   abs(dot(normalDirection, viewDirection));
// -------- ----------------------- --------------------------------------------------------------------
// float4   triMask                 TriMask (for lite version)
// float3   parallaxViewDirection   mul(tbnWS, viewDirection);
// float2   parallaxOffset          parallaxViewDirection.xy / (parallaxViewDirection.z+0.5);
// float    anisotropy              strength of anisotropy
// float    smoothness              smoothness
// float    roughness               roughness
// float    perceptualRoughness     perceptual roughness
// float    shadowmix               this variable is 0 in the shadow area
// float    audioLinkValue          volume acquired by AudioLink
// -------- ----------------------- --------------------------------------------------------------------
// uint     renderingLayers         light layer of object (for URP / HDRP)
// uint     featureFlags            feature flags (for HDRP)
// uint2    tileIndex               tile index (for HDRP)


/*!
 * @brief Returns a value between 0.0 and 1.0 based on the linear interpolation
 * of the value of input x between a and b.
 *
 * @param [in] x  Input value.
 * @param [in] a  Minimum value for input interpolation.
 * @param [in] b  Maximum value for input interpolation.
 * @return Linear interpolated value of x.
 */
float remap01(float x, float a, float b)
{
    return (x - a) / (b - a);
}


/*!
 * @brief Returns a random value between 0.0 and 1.0.
 * @param [in] x  First seed value vector used for generation.
 * @param [in] y  Second seed value vector used for generation.
 * @return Pseudo-random number value between 0.0 and 1.0.
 */
float rand(float x, float y)
{
    return frac(sin(x * 12.9898 + y * 78.233) * 43758.5453);
}


/*!
 * @brief Returns the remainder of x divided by y with the same sign as y.
 * @param [in] x  Vector or scalar numerator.
 * @param [in] y  Vector or scalar denominator.
 * @return Remainder of x / y with the same sign as y.
 */
float fmodglsl(float x, float y)
{
    return x - y * floor(x / y);
}
