Shader "Hidden/GlitchSimple"
{
    Properties
    {
        _Intensity ("Intensity", Range(0,1)) = 0.8
        _BlockSize ("Block Size", Range(0.01,0.5)) = 0.1
        _ColorSplit ("Color Split", Range(0,5)) = 2
        _Lines ("Scanlines", Range(0,2)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float _Intensity;
            float _BlockSize;
            float _ColorSplit;
            float _Lines;
            float _TimeSeed;

            float nrand(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                float r = nrand(float2(floor(uv.y / _BlockSize) + _TimeSeed, 0.0));
                float shift = (r - 0.5) * _Intensity * 0.2;
                uv.x += shift;

                float cut = step(1.0 - _Intensity, nrand(float2(_TimeSeed * 1.7, floor(uv.y * 60.0))));
                if (cut > 0.0)
                {
                    uv.x = frac(uv.x + nrand(float2(_TimeSeed, uv.y)) * 0.5);
                }

                float2 uvR = uv + float2(_ColorSplit * _MainTex_TexelSize.x, 0);
                float2 uvB = uv - float2(_ColorSplit * _MainTex_TexelSize.x, 0);
                fixed rcol = tex2D(_MainTex, uvR).r;
                fixed gcol = tex2D(_MainTex, uv).g;
                fixed bcol = tex2D(_MainTex, uvB).b;
                fixed4 col = fixed4(rcol, gcol, bcol, 1);

                float scan = sin(uv.y * _ScreenParams.y * 3.14159 * 0.5 + _TimeSeed * 8.0);
                col.rgb *= 1.0 - _Lines * 0.08 * (scan * 0.5 + 0.5);

                fixed3 baseCol = tex2D(_MainTex, i.uv).rgb;
                col.rgb = lerp(baseCol, col.rgb, _Intensity);

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}

