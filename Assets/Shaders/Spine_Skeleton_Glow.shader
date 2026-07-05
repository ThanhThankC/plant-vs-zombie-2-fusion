Shader "Custom/Spine_Skeleton_Glow" {
    Properties {
        _Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
        [NoScaleOffset] _MainTex("Main Texture", 2D) = "black" {}
        [Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
        _GlowColor("Glow Color", Color) = (0,0.5,1,1)
        _GlowIntensity("Glow Intensity", Range(0, 10)) = 1
        _GlowContrast("Glow Contrast", Range(1, 5)) = 2
    }

    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Fog { Mode Off }
        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Lighting Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _GlowColor;
            half _GlowIntensity;
            half _GlowContrast;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            VertexOutput vert(VertexInput v) {
                VertexOutput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.vertexColor = v.vertexColor;
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target {
                float4 texColor = tex2D(_MainTex, i.uv);
                #if defined(_STRAIGHT_ALPHA_INPUT)
                texColor.rgb *= texColor.a;
                #endif

                float luminance = dot(texColor.rgb, float3(0.299, 0.587, 0.114));
                float glowMask = pow(luminance, _GlowContrast);
                texColor.rgb += _GlowColor.rgb * _GlowIntensity * texColor.a * glowMask;

                float4 result;
                result.rgb = texColor.rgb * i.vertexColor.rgb * i.vertexColor.a;
                result.a   = texColor.a   * i.vertexColor.a;
                return result;
            }
            ENDCG
        }
    }
}