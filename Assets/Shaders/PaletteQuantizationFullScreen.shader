Shader "Custom/PaletteQuantizationFullScreen"
{
    Properties
    {
        _Color0 ("Palette Color 0", Color) = (0.6196, 0.1647, 0.1686, 1)
        _Color1 ("Palette Color 1", Color) = (0.8745, 0.6235, 0.2431, 1)
        _Color2 ("Palette Color 2", Color) = (0.9961, 0.9569, 0.6863, 1)
        _Color3 ("Palette Color 3", Color) = (0.2039, 0.3608, 0.4000, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "PalettePass"

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct FSInput
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            FSInput VertDefault(uint id : SV_VertexID)
            {
                FSInput o;
                o.positionCS = GetFullScreenTriangleVertexPosition(id);
                o.uv         = GetFullScreenTriangleTexCoord(id);
                return o;
            }

            // Camera texture
            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            // Editable palette colors
            float4 _Color0;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;

            float4 Frag(FSInput input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D_X(
                    _CameraOpaqueTexture,
                    sampler_CameraOpaqueTexture,
                    input.uv
                );

                float3 inputColor = col.rgb;

                float3 palette[4] = {
                    _Color0.rgb,
                    _Color1.rgb,
                    _Color2.rgb,
                    _Color3.rgb
                };

                float3 bestColor = palette[0];
                float minDist = dot(inputColor - palette[0], inputColor - palette[0]);

                [unroll]
                for (int i = 1; i < 4; i++)
                {
                    float dist = dot(inputColor - palette[i], inputColor - palette[i]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestColor = palette[i];
                    }
                }

                return float4(bestColor, col.a);
            }

            ENDHLSL
        }
    }
}
