Shader "Unlit/BlendingShader"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _SecondTex ("Texture 2", 2D) = "white" {}
        _ThirdTex ("Texture 3", 2D) = "white" {}
        _FourthTex ("Texture 4", 2D) = "white" {}
        _FifthTex ("Texture 5", 2D) = "white" {}
        _SixthTex ("Texture 6", 2D) = "white" {}

        _LerpValue ("Transition Float", Range(-1,1)) = 0
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
            #pragma multi_compile_fog

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
            sampler2D _SecondTex;
            float4 _SecondTex_ST;
            sampler2D _ThirdTex;
            float4 _ThirdTex_ST;
            sampler2D _FourthTex;
            float4 _FourthTex_ST;
            sampler2D _FifthTex;
            float4 _FifthTex_ST;
            sampler2D _SixthTex;
            float4 _SixthTex_ST;
            float _LerpValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = lerp(tex2D(_MainTex, i.uv), tex2D(_SecondTex, i.uv), (_LerpValue+1)/2);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
