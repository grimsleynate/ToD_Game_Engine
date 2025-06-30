//vertex.hlsl
struct VSIn
{
    float2 Position : POSITION;
    float3 Color : COLOR;
};

struct PSIn
{
    float4 Position : SV_POSITION;
    float3 Color : COLOR;
};

PSIn VSMain(VSIn input)
{
    PSIn output;
    output.Position = float4(input.Position, 0.0, 1.0);
    output.Color = input.Color;
    return output;
}

float4 PSMain(PSIn input) : SV_TARGET
{
    return float4(input.Color, 1.0);
}
