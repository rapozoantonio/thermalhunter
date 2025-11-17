Shader "ThermalHunt/ThermalVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeatGradient ("Heat Gradient", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0, 0.1)) = 0.02
        _ScanLineTime ("Scan Line Time", Float) = 0
        _BatteryLevel ("Battery Level", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            sampler2D _HeatGradient;
            float4 _MainTex_ST;
            float _NoiseIntensity;
            float _ScanLineTime;
            float _BatteryLevel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Simple noise function
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Convert to grayscale (heat intensity)
                float heat = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Apply heat gradient
                fixed4 thermalColor = tex2D(_HeatGradient, float2(heat, 0.5));

                // Add thermal noise
                float noise = rand(i.uv + _Time.y) * _NoiseIntensity;
                thermalColor.rgb += noise;

                // Scan lines effect
                float scanLine = sin(i.uv.y * 800 + _ScanLineTime * 10) * 0.05;
                thermalColor.rgb += scanLine;

                // Vignette effect
                float2 center = i.uv - 0.5;
                float vignette = 1 - dot(center, center) * 0.8;
                thermalColor.rgb *= vignette;

                // Battery low effect (flickering)
                if (_BatteryLevel < 0.2)
                {
                    float flicker = sin(_Time.y * 20) * 0.2 + 0.8;
                    thermalColor.rgb *= flicker;
                }

                return thermalColor;
            }
            ENDCG
        }
    }
}
