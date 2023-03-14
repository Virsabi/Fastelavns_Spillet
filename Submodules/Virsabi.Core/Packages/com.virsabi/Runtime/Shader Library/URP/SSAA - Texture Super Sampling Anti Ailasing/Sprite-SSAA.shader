Shader "Custom/SSAA Sprite"
{
    Properties{
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {} // Used to have [NoScaleOffset] set
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
    }
        SubShader{
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
            }

            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
            Cull Off
            Lighting Off
            ZWrite Off
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "TextureSSAA.cginc"

                fixed2 _Flip;
                sampler2D _MainTex;
                inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip) // From UnitySprites.cginc
                {
                    return float4(pos.xy * flip, pos.z, 1.0);
                }

                struct v2f {
                    float4 pos : SV_Position;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };
                void vert(appdata_t v, out v2f o)
                {
                    o.pos = UnityFlipSprite(v.vertex, _Flip);
                    o.pos = UnityObjectToClipPos(o.pos);
                    o.uv = v.texcoord;
                    o.color = v.color;
                }
                half4 frag(v2f i) : SV_Target
                {
                    return Tex2DSS(_MainTex, i.uv) * i.color;
                }
                ENDCG
            }
        }

            Fallback "Sprites/Default"
}