#ifndef COMMON_INCLUDED
#define COMMON_INCLUDED

#define DEGREES_MAX 360.0f
#define MAX_16_BIT_VALUE 65535.0f
#define MAX_PIXEL_WORLD_SCALE 2048.0f
#define MIN_PIXEL_WORLD_SCALE 1.0f / MAX_PIXEL_WORLD_SCALE

float ProjectUVOnAxis(float2 centerPoint, float angleDegrees)
{
    float radians = DegToRad(angleDegrees);
    float2 direction = float2(cos(radians), sin(radians));
    return dot(centerPoint, direction);
}

float2 Decode2(float value)
{
    float2 encodeMul = float2(1.0f, MAX_16_BIT_VALUE);
    float encodeBit = 1.0f / MAX_16_BIT_VALUE;
    float2 enc = encodeMul * value;
    enc = frac(enc);
    enc.x -= enc.y * encodeBit;
    return enc;
}

float SdfRoundedRect(float2 position, float2 halfSize, float4 radii)
{
    float cornerRadius = position.x > 0.0f
                            ? position.y > 0.0f ? radii.y : radii.z
                            : position.y > 0.0f ? radii.x : radii.w;

    float2 distanceToCornerCenter = abs(position) - halfSize + cornerRadius;
    float distanceToRoundedCorner = length(max(distanceToCornerCenter, 0.0f));
    float distanceToStraightEdge = min(max(distanceToCornerCenter.x, distanceToCornerCenter.y), 0.0f);
    return cornerRadius - distanceToRoundedCorner - distanceToStraightEdge;
}

#endif
