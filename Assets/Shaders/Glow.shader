Shader "Hidden/IHateShaders"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Glow_Intensity ("Glow Intensity", float) = 0.3

    }
    SubShader
    {
        // No culling or depth
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

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
                float4 color: COLOR;
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
            float _Glow_Intensity;

            float3 HueToRGB(float hue)
            {
                float3 rgb = abs(hue * 6 - float3(3, 2, 4)) - float3(1, 1, 1);
                return clamp(rgb, 0.0, 1.0);
            }


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                float time = _Time.y * 0.3;
                float hue = frac(i.uv.x * 0.1 + time * 0.4);
                float hue2 = frac(i.uv.y * 0.1 - time + 0.5);
                float3 lerpdata = HueToRGB(hue);
                float3 lerpdata2 = HueToRGB(hue2);
                
                return fixed4(col.rgb + lerpdata * _Glow_Intensity + lerpdata2 * _Glow_Intensity, col.a * i.color.a);
            }
            ENDCG
        }
    }
}
