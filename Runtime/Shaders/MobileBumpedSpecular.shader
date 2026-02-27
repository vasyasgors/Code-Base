// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped Specular shader. Differences from regular Bumped Specular one:
// - no Main Color nor Specular Color
// - specular lighting directions are approximated per vertex
// - writes zero to alpha channel
// - Normalmap uses Tiling/Offset of the Base texture
// - no Deferred Lighting support
// - no Lightmap support
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile Bumped Specular" 
{
    Properties 
    {
        [PowerSlider(5.0)] _Specular ("Specular", Range (0.001, 1)) = 0.078125
        [PowerSlider(5.0)] _Gloss ("Gloss", Range (0.001, 1)) = 0.078125
        _MainTex ("Base (RGB)", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normalmap", 2D) = "bump" {}
        _NormalStrength ("NormalStrength", Range (0.0, 1)) = 0.5

    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 250

        CGPROGRAM
            #pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

            inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
            {
                fixed diff = max (0, dot (s.Normal, lightDir));
                fixed nh = max (0, dot (s.Normal, halfDir));
                fixed spec = pow (nh, s.Specular*128) * s.Gloss;

                fixed4 c;
                c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
                UNITY_OPAQUE_ALPHA(c.a);
                return c;
            }

            sampler2D _MainTex;
            sampler2D _NormalMap;
            half _Specular;
            half _Gloss;
            half _NormalStrength;

            struct Input {
                float2 uv_MainTex;
            };

            void surf (Input IN, inout SurfaceOutput o) 
            {
                fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = tex.rgb;
                o.Gloss = _Gloss;
                o.Alpha = tex.a;
                o.Specular = _Specular;
                o.Normal = UnpackScaleNormal (tex2D(_NormalMap, IN.uv_MainTex), _NormalStrength);
            }

        ENDCG
    }

    FallBack "Mobile/VertexLit"
}