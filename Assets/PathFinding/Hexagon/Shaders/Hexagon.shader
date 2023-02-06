Shader "GameAI/Hexagon/Hexagon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InsideColor ("InSideColor", color) = (1, 1, 1, 1)
        _OutsideColor("OutsideColor", color) = (1, 1, 1, 1)
        _EdgeWidth ("EdgeWidth", float) = 0.1
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
            // make fog work
            //#pragma multi_compile_fog

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _InsideColor;
            float4 _OutsideColor;
            float _EdgeWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                float4 col = lerp(smoothstep(i.uv.x, 0.95 - _EdgeWidth , 0.95 + _EdgeWidth), _InsideColor, _OutsideColor);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                //col *= _Color;
                return col;
            }
            ENDCG
        }
    }
}
