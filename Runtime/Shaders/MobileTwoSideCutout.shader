Shader "Custom/MobileTwoSideCutout"
{ 
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 150
        Cull off   
        CGPROGRAM

        #pragma surface surf Lambert noforwardadd

        sampler2D _MainTex;
            float _Cutoff;

        struct Input 
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            clip(col.a - _Cutoff);

            o.Albedo = col.rgb;
            o.Alpha = col.a; 
          
        }

        ENDCG
    }

    Fallback "Mobile/VertexLit"
}

