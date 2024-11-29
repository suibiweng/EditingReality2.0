Shader "Custom/AmbientOcclusionEnhance"
{
    Properties
    {
        _MainTex("RGB Texture", 2D) = "white" {}
        _DepthMap("Depth Map", 2D) = "white" {}
        _AOIntensity("AO Intensity", Float) = 1.0
        _AORadius("AO Radius", Float) = 0.005
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DepthMap;
            float _AOIntensity;
            float _AORadius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float computeAO(float2 uv, sampler2D depthMap, float radius)
            {
                float depthCenter = tex2D(depthMap, uv).r;

                // AO kernel sampling offsets
                float2 offsets[4] = {
                    float2(radius, 0),
                    float2(-radius, 0),
                    float2(0, radius),
                    float2(0, -radius)
                };

                float ao = 0.0;
                for (int i = 0; i < 4; i++)
                {
                    float depthSample = tex2D(depthMap, uv + offsets[i]).r;
                    float diff = saturate(depthCenter - depthSample);
                    ao += diff;
                }

                // Normalize AO and invert for brightening the RGB
                ao = 1.0 - saturate(ao / 4.0);
                return ao;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Sample textures
                float3 rgb = tex2D(_MainTex, uv).rgb;
                float ao = computeAO(uv, _DepthMap, _AORadius);

                // Enhance RGB with AO
                float3 enhancedRGB = rgb * lerp(1.0, ao, _AOIntensity);

                return fixed4(enhancedRGB, 1.0);
            }
            ENDCG
        }
    }
}
