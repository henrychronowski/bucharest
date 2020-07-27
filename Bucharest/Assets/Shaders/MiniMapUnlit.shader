Shader "Custom/MiniMapUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rows("Pixel Rows", Float) = 64
        _Columns("Pixel Columns", Float) = 64
        _MapTint("Map Tint", COLOR) = (1, 1, 1, 1)

        _ScanColor("Scan Color", COLOR) = (1, 1, 1, 1)
        _ScanDensity("Scan density", Range(0.0, 1.0)) = 1.0
        _ScanThickness("Scan thickness", Range(0.0, 0.1)) = 0.05
        _ScanPoint("Scan point", Range(0, 1)) = 0.5
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

            float _Rows;
            float _Columns;
            float4 _MapTint;
            float4 _ScanColor;
            float _ScanDensity;
            float _ScanThickness;
            float _ScanPoint;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float2 uv = i.uv;
                uv.x *= _Columns;
                uv.y *= _Rows;

                uv.x = floor(uv.x);
                uv.y = floor(uv.y);

                uv.x /= _Columns;
                uv.y /= _Rows;

                //sample the texture
                fixed4 col = tex2D(_MainTex, uv);

                if (i.uv.y >= _ScanPoint && i.uv.y < _ScanPoint + _ScanThickness)
                    col *= _ScanColor * _ScanDensity;

                return col * _MapTint;
            }
            ENDCG
        }
    }
}
