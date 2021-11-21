Shader "Unlit/PastPresentShader"
{
    Properties
    {
        _PastTex ("Past Texture", 2D) = "white" {}
        _PresentTex ("Present Texture", 2D) = "white" {}

        _Color_SlopeA("Slope Color Past", Color) = (1,1,1,1)
        _SlopeA ("Slope Past", Range(0, 2)) = 1.0
        _Color_OffsetA("Offset Color Past", Color) = (1,1,1,1)
        _OffsetA ("Offset Past", Range(-1, 1)) = 0.0
        _Color_PowerA("Power Color Past", Color) = (1,1,1,1)
        _PowerA ("Power Past", Range(0, 2)) = 1.0
        _Color_SlopeB("Slope Color Present", Color) = (1,1,1,1)
        _SlopeB ("Slope Present", Range(0, 2)) = 1.0
        _Color_OffsetB("Offset Color Present", Color) = (1,1,1,1)
        _OffsetB ("Offset Present", Range(-1, 1)) = 0.0
        _Color_PowerB("Power Color Present", Color) = (1,1,1,1)
        _PowerB ("Power Present", Range(0, 2)) = 1.0

        _Blend ("Blend", Range(0, 1)) = 0.0
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

            sampler2D _PastTex;
            float4 _PastTex_ST;

            sampler2D _PresentTex;
            float4 _PresentTex_ST;

            float _Blend;

            float4 _Color_SlopeA;
            float4 _Color_OffsetA;
            float4 _Color_PowerA;
            float _SlopeA;
            float _OffsetA;
            float _PowerA;

            float4 _Color_SlopeB;
            float4 _Color_OffsetB;
            float4 _Color_PowerB;
            float _SlopeB;
            float _OffsetB;
            float _PowerB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float max (float a, float b) {
                if (a < b) return b;
                return a;
            }

            float remap(float value, float istart, float istop, float ostart, float ostop) {
	return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
}

            fixed4 frag (v2f i) : SV_Target {
                float blend = remap(_Blend, 0, 1, -1, 2) + i.uv[0];
                blend = clamp(blend, 0, 1);

                float2 centeredUvScreen = i.uv - float2(0.5, 0.5);
                float distanceToCenter = (centeredUvScreen[0] * centeredUvScreen[0]) + (centeredUvScreen[1] * centeredUvScreen[1]);
                float2 normalized = centeredUvScreen / distanceToCenter;
                float2 deviation = centeredUvScreen * distanceToCenter - (centeredUvScreen);

                deviation *= blend * 0.1 + 0.05;
                float light = remap(sin((i.uv[1] + deviation[1]) * 800), -1, 1, 0.7, 1);

                // sample the texture
                fixed4 cola = tex2D(_PastTex, i.uv + deviation);
                fixed4 colb = tex2D(_PresentTex, i.uv + deviation);

                // color correction ASC-CDL
                cola[0] = pow((max(0, cola[0] * _SlopeA * _Color_SlopeA[0]) + _OffsetA * _Color_OffsetA[0]), _PowerA * _Color_PowerA[0]);
                cola[1] = pow((max(0, cola[1] * _SlopeA * _Color_SlopeA[1]) + _OffsetA * _Color_OffsetA[1]), _PowerA * _Color_PowerA[1]);
                cola[2] = pow((max(0, cola[2] * _SlopeA * _Color_SlopeA[2]) + _OffsetA * _Color_OffsetA[2]), _PowerA * _Color_PowerA[2]);
                cola *= light;
                cola[3] = 1.0;

                colb[0] = pow((max(0, colb[0] * _SlopeB * _Color_SlopeB[0]) + _OffsetB * _Color_OffsetB[0]), _PowerB * _Color_PowerB[0]);
                colb[1] = pow((max(0, colb[1] * _SlopeB * _Color_SlopeB[1]) + _OffsetB * _Color_OffsetB[1]), _PowerB * _Color_PowerB[1]);
                colb[2] = pow((max(0, colb[2] * _SlopeB * _Color_SlopeB[2]) + _OffsetB * _Color_OffsetB[2]), _PowerB * _Color_PowerB[2]);
                colb *= light;
                colb[3] = 1.0;

                // blend
                return cola * blend + colb * (1 - blend);
            }
            ENDCG
        }
    }
}
