Shader "Custom/PaletteQuantizationFullScreen"
{
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

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float4 Frag(FSInput input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D_X(
                    _CameraOpaqueTexture,
                    sampler_CameraOpaqueTexture,
                    input.uv
                );

                float3 inputColor = col.rgb;

                float3 palette[4] = {
                    float3(0.6196, 0.1647, 0.1686),
                    float3(0.8745, 0.6235, 0.2431),
                    float3(0.9961, 0.9569, 0.6863),
                    float3(0.2039, 0.3608, 0.4000)
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
