Shader "Hidden/PaletteMap4"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // FULLSCREEN PASS — no transforms needed
            Varyings Vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = IN.positionOS;   // pass-through
                OUT.uv = IN.uv;
                return OUT;
            }

            float3 Frag (Varyings IN) : SV_Target
            {
                float3 col = tex2D(_MainTex, IN.uv).rgb;

                // Your exact palette (hardcoded)
                float3 palette[4] = {
                    float3(0.619f, 0.165f, 0.169f), // 9e2a2b
                    float3(0.875f, 0.624f, 0.243f), // df9f3e
                    float3(0.996f, 0.957f, 0.686f), // fef4af
                    float3(0.204f, 0.361f, 0.400f)  // 345c66
                };

                float bestDist = 999999.0;
                float3 bestColor = col;

                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    float d = distance(col, palette[i]);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestColor = palette[i];
                    }
                }

                return bestColor;
            }

            ENDHLSL
        }
    }
}
