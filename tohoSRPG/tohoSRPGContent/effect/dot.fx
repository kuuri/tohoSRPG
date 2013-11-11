texture Texture;
sampler Sampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
};

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(Sampler, texCoord);
	tex *= color;

    return tex;
}

technique DrawDot
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}