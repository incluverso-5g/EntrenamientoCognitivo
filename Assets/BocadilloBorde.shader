Shader "Custom/Bocadil1oBorde"
{
    Properties
    {
        _MainColor ("Fondo Color", Color) = (1,1,1,1) // Color del fondo
        _BorderColor ("Borde Color", Color) = (1,0,0,1) // Color del borde
        _BorderThickness ("Borde Grosor", Range(0.01, 0.1)) = 0.05 // Grosor del borde
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

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainColor; // Fondo
            float4 _BorderColor; // Borde
            float _BorderThickness; // Grosor del borde

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.uv - 0.5); // Distancia al centro
                if (dist > 0.5 - _BorderThickness) // Si está en el borde
                {
                    return _BorderColor; // Aplica color del borde
                }
                return _MainColor; // Aplica color del fondo
            }
            ENDCG
        }
    }
}
