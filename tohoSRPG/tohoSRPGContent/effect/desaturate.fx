// �F�̍ʓx�𓮓I�ɕύX����J�X�^�� �G�t�F�N�g�B

sampler TextureSampler : register(s0);


float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // �e�N�X�`���[�̐F���擾���܂��B
    float4 tex = tex2D(TextureSampler, texCoord);
    
    // �O���[�X�P�[���ɕϊ����܂��B�萔 0.3�A0.59�A����� 0.11 �́A
    // �l�Ԃ̖ڂ����΂̃��C�g�̕��ɕq���ł��邽�߂ł��B
    float greyscale = dot(tex.rgb, float3(0.3, 0.59, 0.11));
    
    // �ʓx���x�����v�Z���A�o�͂���� COLOR0 ��ۑ����܂��B
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
