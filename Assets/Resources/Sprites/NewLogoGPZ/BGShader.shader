Shader "Hidden/BGShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1,0.8,0.2,1)
        _ColorB ("Color B", Color) = (0.1,0.6,1,1)
        _ColorC ("Color C", Color) = (0.9,0.2,0.6,1)
        _TileCount ("Tile Count", Float) = 10
        _WaveSpeed ("Wave Speed", Float) = 1
        _WaveStrength ("Wave Strength", Float) = 0.2
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 _ColorC;
            float _TileCount;
            float _WaveSpeed;
            float _WaveStrength;

            float hexDist(float2 p)
            {
                float2 q = float2(
                    p.x * 2.0 / 3.0,
                    (-p.x / 3.0 + sqrt(3.0)/3.0 * p.y)
                );
                q = abs(q);
                return max(q.x, q.y);
            }

            float gmod(float x, float y)
            {
                return x - y * floor(x / y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 헥사곤 타일 좌표계로 변환
                float2 hexUV = uv * _TileCount;

                // 웨이브 효과
                float wave = sin(hexUV.x * 2.0 + _Time * _WaveSpeed)
                           + cos(hexUV.y * 2.0 + _Time * _WaveSpeed);
                wave *= _WaveStrength;

                // 헥사곤 중심 정렬
                hexUV.x += (gmod(floor(hexUV.y), 2.0) * 0.5);

                // 헥사곤 내 거리 계산
                float2 local = frac(hexUV) - 0.5;
                float d = hexDist(local + wave);

                // 헥사곤 경계선
                float border = smoothstep(0.48, 0.5, d);

                // 컬러 시프트
                float colorShift = 0.5 + 0.5 * sin(_Time + hexUV.x + hexUV.y);

                // 3색 그라데이션
                fixed4 color = lerp(_ColorA, _ColorB, colorShift);
                color = lerp(color, _ColorC, border);

                // 스프라이트 알파 유지
                fixed4 sprite = tex2D(_MainTex, i.uv);
                color.a *= sprite.a;

                return color;
            }
            ENDCG
        }
    }
}
