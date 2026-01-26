Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainColor ("Fondo Color", Color) = (1,1,1,1) // Fondo blanco
        _OutlineColor ("Borde Color", Color) = (1,0,0,1) // Color del borde
        _OutlineWidth ("Grosor del Borde", Range(0.01, 0.1)) = 0.03 // Grosor del borde
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                float3 normal = normalize(v.normal) * _OutlineWidth;
                v.vertex.xyz += normal; // Expande el modelo para crear el borde
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor; // Color del borde
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _MainColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _MainColor; // Color del fondo
            }
            ENDCG
        }
    }
}
