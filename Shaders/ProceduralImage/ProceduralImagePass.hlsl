#ifndef PROCEDURAL_IMAGE_INCLUDED
#define PROCEDURAL_IMAGE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.firsttry.customutils/Shaders/Shared/Common.hlsl"
#include "Packages/com.firsttry.customutils/Shaders/Shared/SDF.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

CBUFFER_START(UnityPerMaterial)
    float4 _Color;
    float4 _MainTex_ST;
    float4 _TextureSampleAdd;
CBUFFER_END

float4 _ClipRect;
int _UIVertexColorAlwaysGammaSpace;

struct Attributes
{
    float4 positionOS : POSITION;
    float4 color : COLOR;
    float2 uv0 : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    float2 uv2 : TEXCOORD2;
    float2 uv3 : TEXCOORD3;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float4 color : COLOR;
    float4 worldPosition : TEXCOORD0;
    float2 uv : TEXCOORD1;
    float2 size : TEXCOORD2;
    float4 radius : TEXCOORD3;
    float lineWeight : TEXCOORD4;
    float pixelScale : TEXCOORD5;
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings Vertex(Attributes input)
{
    Varyings output;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.worldPosition = input.positionOS;
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = TRANSFORM_TEX(input.uv0, _MainTex);
    output.size = input.uv1;

    float minSide = min(input.uv1.x, input.uv1.y);
    output.radius = float4(Decode2(input.uv2.x), Decode2(input.uv2.y)) * minSide;
    output.lineWeight = input.uv3.x * minSide;
    output.pixelScale = clamp(input.uv3.y, MIN_PIXEL_WORLD_SCALE, MAX_PIXEL_WORLD_SCALE);

    #ifndef UNITY_COLORSPACE_GAMMA
    if (_UIVertexColorAlwaysGammaSpace)
        input.color.rgb = SRGBToLinear(input.color.rgb);
    #endif

    output.color = input.color * _Color;
    return output;
}

float4 Fragment(Varyings input) : SV_Target
{
    half4 color = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) + _TextureSampleAdd) * input.color;

    #ifdef UNITY_UI_CLIP_RECT
    color.a *= UnityGet2DClipping(input.worldPosition.xy, _ClipRect);
    #endif

    #ifdef UNITY_UI_ALPHACLIP
    clip(color.a - 0.001f);
    #endif

    float2 halfSize = input.size * 0.5f;
    float2 centeredPosition = input.uv * input.size - halfSize;
    float sdf = -SdfRoundedRect(centeredPosition, halfSize, input.radius);
    float borderCenter = (input.lineWeight + 1.0f / input.pixelScale) * 0.5f;
    color.a *= saturate((borderCenter - distance(sdf, borderCenter)) * input.pixelScale);

    clip(color.a - 0.001f);

    return color;
}

#endif
