Shader "Unlit/Hologram"
{
   Properties
    {

        _MainTex ("Albedo Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1, 1, 1, 1)

         _TransparencyTex ("Transparency Texture", 2D) = "white" {}
         _Transparency("Transparency", Range(0.0, 2.0)) = 0.25
         _TransparencySpeed("Transparency Speed", float) = 0.25


         _CutoutTex("Cutout Texture", 2D) = "white"{}
        _CutoutStrength("Cutout Strength", Range(0.0, 1.0)) = 0.2

        _Distance("Distance", Float) = 1
        _Frequency("Frequency", Float) = 1
        _Speed ("Speed", Float) = 1
        _Amount("Amount", Range(0.0,1.0)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

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
                float2 uv_Alpha : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            // Main
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;

            sampler2D _TransparencyTex;
            float4 _TransparencyTex_ST;
            float _Transparency;
            float _TransparencySpeed;

             // Cutout
             sampler2D _CutoutTex;
             float _CutoutStrength;

             // Wave
             float _Distance;
             float _Frequency;
             float _Speed;
             float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.z += sin(_Time.y * _Speed + v.vertex.y * _Frequency) * (_Distance * 0.1) * _Amount;
                v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Frequency) * (_Distance * 0.1) * _Amount;

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_Alpha = TRANSFORM_TEX(v.uv, _TransparencyTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;


                i.uv_Alpha += _Time.x * _TransparencySpeed;
                fixed4 alpha = tex2D(_TransparencyTex, i.uv_Alpha);
                col.a = alpha * _Transparency;



                fixed4 cutoutColor = tex2D(_CutoutTex, i.uv);

                clip(cutoutColor - _CutoutStrength);
     
                return col;
            }
            ENDCG
        }
    }
}
