Shader "Hidden/UnderwaterFogBlit"
{
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "UnderwaterFog"
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float4 _FogColor;     // rgb = 雾颜色
            float  _FogDensity;   // 指数雾强度
            float  _LinearStart;  // 线性雾起点
            float  _LinearEnd;    // 线性雾终点
            int    _UseLinear;    // 0: Exponential, 1: Linear
            int    _ShowDepth;    // 调试：显示深度灰阶

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            // 指数雾：更像水下
            inline float FogFactorExp(float eyeDepth)
            {
                // 使用 exp2 更稳定：exp(-d^2 * z) = 2^(-d^2 * z / ln2)
                return exp2(-_FogDensity * _FogDensity * eyeDepth);
            }

            // 线性雾
            inline float FogFactorLinear(float eyeDepth)
            {
                return saturate( (_LinearEnd - eyeDepth) / max(1e-5, (_LinearEnd - _LinearStart)) );
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // 采样硬件深度并线性化为视空间 Z（单位米）
                float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float eyeDepth = LinearEyeDepth(rawDepth);   // 依赖 _ZBufferParams

                // 计算雾系数（越小越接近雾色）
                float f = (_UseLinear != 0) ? FogFactorLinear(eyeDepth) : FogFactorExp(eyeDepth);

                // 调试：显示深度
                if (_ShowDepth != 0)
                {
                    float d = saturate(eyeDepth / 100.0); // 把 0..100m 映射到 0..1
                    return float4(d, d, d, 1);
                }

                float3 fogged = lerp(_FogColor.rgb, col.rgb, f);
                return float4(fogged, col.a);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
