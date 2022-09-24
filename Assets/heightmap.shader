Shader "Unlit/heightmap"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HeightMap ("HeightMap", 2D) = "white" {}
        _scaleFactor("Scale factor", range(1, 100)) = 10
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
            float4 _MainTex_ST;
            sampler2D _HeightMap;
            float4 _HeightMap_ST;
            float _scaleFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _HeightMap);
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex -= float4(0, tex2Dlod(_HeightMap, float4(o.uv, 0, 0)).x, 0, 0) * _scaleFactor;
                o.vertex -= float4(0, tex2Dlod(_HeightMap, 
                    float4(o.uv, 0, 0)).x, 0, 
                    0)*_scaleFactor;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
