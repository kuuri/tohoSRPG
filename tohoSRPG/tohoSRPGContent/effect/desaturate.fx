// 色の彩度を動的に変更するカスタム エフェクト。

sampler TextureSampler : register(s0);


float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // テクスチャーの色を取得します。
    float4 tex = tex2D(TextureSampler, texCoord);
    
    // グレースケールに変換します。定数 0.3、0.59、および 0.11 は、
    // 人間の目が青より緑のライトの方に敏感であるためです。
    float greyscale = dot(tex.rgb, float3(0.3, 0.59, 0.11));
    
    // 彩度レベルを計算し、出力される COLOR0 を保存します。
    tex.rgb = lerp(greyscale, tex.rgb, color.a * 4);
    
    return tex;
}


technique Desaturate
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
