sampler uImage0 : register(s0);//u0就是tml里面的gd.Texture[0]
sampler uImage1 : register(s1);//u1就是tml里面的gd.Texture[1]

float Length;//定一个float变量，用于控制扭曲幅度
float Rot;//整一个float变量，用于控制扭曲的方向
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0,coords);
    float4 c2 = tex2D(uImage1, coords); //用u1的纹理上的颜色参数去扭曲u0,r代表方向,g代表大小
    if (!any(c2))//u1对应的像素上没有颜色就返回本来的图像
        return c;
    else
    {
        float2 vec = float2(0, 0);
        float rotation = (c2.r * 6.283 + Rot) % 6.283; //将这个像素上的r值设为方向(0~2pi)
        vec = float2(sin(rotation), cos(rotation)) * c2.g * Length;
        return tex2D(uImage0, coords + vec);

    }
}

technique Technique1
{
	pass Distort
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
   
}