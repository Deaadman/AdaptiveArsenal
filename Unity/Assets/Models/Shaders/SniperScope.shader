Shader "Custom/SniperScope"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
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
            sampler2D _MaskTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Get the texture color
                fixed4 col = tex2D(_MainTex, i.uv);

                // Get the mask texture
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // Apply the circular mask
                float2 coords = (i.uv - 0.5) * 1.3;
                float maskValue = smoothstep(0.45, 0.5, 1 - length(coords));
                col *= maskValue * mask;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}