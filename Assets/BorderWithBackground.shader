Shader "Custom/BocadilloShader"
{
    Properties
    {
        _MainColor ("Fondo Color", Color) = (1,1,1,1) // Fondo Blanco
        _BorderColor ("Borde Color", Color) = (1,0,0,1) // Borde Rojo
        _BorderThickness ("Grosor del Borde", Range(0.01, 0.1)) = 0.03 // Grosor del Borde
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Primera pasada: Generar el borde
        Pass
        {
            Name "OUTLINE"
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

            float _BorderThickness;
            float4 _BorderColor;

            v2f vert (appdata v)
            {
                v2f o;
                float3 normalOffset = normalize(v.normal) * _BorderThickness;
                v.vertex.xyz += normalOffset; // Expande el modelo para crear el borde
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _BorderColor; // Aplica el color del borde
            }
            ENDCG
        }

        // Segunda pasada: Renderizar el fondo
        Pass
        {
            Name "BASE"
            Cull Back
            ZWrite On
            ZTest LEqual

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

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _MainColor; // Aplica el color del fondo
            }
            ENDCG
        }
    }
}