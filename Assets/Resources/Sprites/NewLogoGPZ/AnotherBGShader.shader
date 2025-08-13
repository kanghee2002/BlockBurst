Shader "Hidden/AnotherBGShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Palette ("Color Palette", 2D) = "white" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Palette;
            float4 _Tint;

            // Create a moving elegant checkered pattern
            float2 checkeredPattern(float2 uv, float time)
            {
                float2 p = uv * 10.0 + time * 0.5;
                float2 grid = frac(p);
                float2 dist = abs(grid - 0.5);
                float pattern = step(0.5, dist.x) * step(0.5, dist.y);
                return float2(pattern, pattern);

            }  

            /*
            
            float2(0.125, 0.125);
            float2(0.375, 0.375);
            float2(0.25, 0.75);
            float2(0.75, 0.75);
            
            */

            float2 patternusingpalette(float2 uv, float time)
            {

            

                uv = float2(fmod(uv.x * 2 + time, 1), fmod(uv.y * 2 + time, 1));

                float quantise = (sin(time * 10) + 2) / 10 + 0.2;

                if((fmod(uv.x, quantise) + fmod(uv.y, quantise)) <= 0.125 || (fmod(uv.x, quantise) + fmod(uv.y, quantise)) >= 0.375)
                {
                    return float2(0.75, 0.75);
                }
                else if((fmod(uv.x, quantise * 2) + fmod(uv.y, quantise * 2)) <= 0.25 || (fmod(uv.x, quantise * 2) + fmod(uv.y, quantise * 2)) >= 0.75)
                {
                    return float2(0.375, 0.375);
                }
                else if((fmod(uv.x, quantise * 4) + fmod(uv.y, quantise * 4)) <= 0.5 || (fmod(uv.x, quantise * 4) + fmod(uv.y, quantise * 4)) >= 1.5)
                {
                    return float2(0.25, 0.75);
                }
                else
                {
                    return float2(0.125, 0.125);
                }
                
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_Palette, patternusingpalette(i.uv, _Time));
                // just invert the colors
                col *= _Tint;

                return col * i.color;
            }
            ENDCG
        }
    }
}
