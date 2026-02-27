Shader "Custom/TextureBlendByMask"
{
    Properties
    {
        _Mask ("Mask", 2D) = "white" {}
        _MainTex0 ("Albedo Map 0", 2D) = "white" {}
        _Color0 ("Color 0", Color) = (1,1,1,1)
        _MainTex1 ("Albedo Map 1", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _MainTex2 ("Albedo Map 2", 2D) = "white" {}
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _MainTex3 ("Albedo Map 3", 2D) = "white" {}
        _Color3 ("Color 3", Color) = (1,1,1,1)   
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard addshadow fullforwardshadows 
        //#pragma surface surf Lambert

        #pragma target 3.5


        sampler2D _Mask;
        sampler2D _MainTex0;
        sampler2D _MainTex1;
        sampler2D _MainTex2;
        sampler2D _MainTex3;

        fixed4 _Color0;
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;


        struct Input
        {
            float2 uv_Mask; 
            float2 uv_MainTex0;
            float2 uv_MainTex1;
            float2 uv_MainTex2;
            float2 uv_MainTex3;
            float4 color : COLOR;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 maskColor = tex2D (_Mask, IN.uv_Mask);
    
            fixed4 color0 = tex2D (_MainTex0, IN.uv_MainTex0) * _Color0;
            fixed4 color1 = tex2D (_MainTex1, IN.uv_MainTex1) * _Color1;
            fixed4 color2 = tex2D (_MainTex2, IN.uv_MainTex2) * _Color2;
            fixed4 color3 = tex2D (_MainTex3, IN.uv_MainTex3) * _Color3;
          
            //Дает затемнение при смешивании
            /*
            float color0Lerp = clamp(maskColor.r + maskColor.g + maskColor.b, 0, 1);
        
            color1 = clamp(color1 * maskColor.r, 0, 1);
            color2 = clamp(color2 * maskColor.g, 0, 1);
            color3 = clamp(color3 * maskColor.b, 0, 1);
   

            fixed4 finalColor = clamp(color1 + color2 + color3, 0, 1);

            finalColor = lerp(color0, finalColor, color0Lerp);
            */
            fixed4 finalColor = lerp(lerp(lerp(color0, color1, maskColor.r), color2, maskColor.g), color3, maskColor.b);

            o.Albedo = finalColor;
            o.Metallic = 0;
            o.Smoothness = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
