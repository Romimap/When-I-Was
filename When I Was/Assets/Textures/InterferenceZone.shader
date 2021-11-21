Shader "Unlit/InterferenceZone"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Size("Size", Range(1.0, 20.0)) = 10.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                if (uint((i.uv[0] + i.uv[1]) * _Size) % 2 == 0) {
                    col = _Color;
                } else {
                    col = fixed4(0, 0, 0, 0);
                }
                // sample the texture
                return col;
            }
            ENDCG
        }
    }
}
