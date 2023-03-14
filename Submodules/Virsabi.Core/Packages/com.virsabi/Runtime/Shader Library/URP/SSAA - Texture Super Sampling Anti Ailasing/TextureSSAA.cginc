/*

    Original SSAA code supplied by bgolus, via Unity Forums, March 2019

*/

#ifndef TEXTURE_SSAA_INCLUDED
#define TEXTURE_SSAA_INCLUDED

half4 col;
// static const float DEFAULT_BIAS = -0.75;
static const float2 UV_OFFSETS = float2(0.125, 0.375);

// fixed4 Tex2DSS(sampler2D tex, float2 uv, float bias)
fixed4 Tex2DSS(sampler2D tex, float2 uv)
{
    // get uv derivatives
    float2 dx = ddx(uv);
    float2 dy = ddy(uv);

    col = 0;

    // MSAA style "four rooks" rotated grid super sampling
    // samples the texture 4 times
    // col += tex2Dbias(tex, float4(uv + UV_OFFSETS.x * dx + UV_OFFSETS.y * dy, 0, bias));
    // col += tex2Dbias(tex, float4(uv - UV_OFFSETS.x * dx - UV_OFFSETS.y * dy, 0, bias));
    // col += tex2Dbias(tex, float4(uv + UV_OFFSETS.y * dx - UV_OFFSETS.x * dy, 0, bias));
    // col += tex2Dbias(tex, float4(uv - UV_OFFSETS.y * dx + UV_OFFSETS.x * dy, 0, bias));

    // No longer using mip maps, so no bias needed
    col += tex2D(tex, float2(uv + UV_OFFSETS.x * dx + UV_OFFSETS.y * dy));
    col += tex2D(tex, float2(uv - UV_OFFSETS.x * dx - UV_OFFSETS.y * dy));
    col += tex2D(tex, float2(uv + UV_OFFSETS.y * dx - UV_OFFSETS.x * dy));
    col += tex2D(tex, float2(uv - UV_OFFSETS.y * dx + UV_OFFSETS.x * dy));

    col *= 0.25;

    return col;
}

#endif // TEXTURE_SSAA_INCLUDED