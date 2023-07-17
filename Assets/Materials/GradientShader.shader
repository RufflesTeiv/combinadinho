Shader "Unlit/GradientShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color ) = (1,1,1,1)
        _ColorB ("Color B", Color ) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0,1) ) = 0
        _ColorEnd ("Color End", Range(0,1) ) = 1
        [Toggle] _Vertical("Vertical", Float) = 0
        [Toggle] _Invert("Invert", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite off
        Cull off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ColorStart;
            float _ColorEnd;
            float4 _ColorA;
            float4 _ColorB;
            float _Vertical;
            float _Invert;
            

            struct MeshData {
                float4 vertex : POSITION;
                float4 uv0: TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = v.uv0;
                o.uv = TRANSFORM_TEX(v.uv0, _MainTex);
                return o;
            }

            
            float InverseLerp (float a, float b, float v) {
                return (v-a)/(b-a);
            }
            
            float4 frag (Interpolators i) : SV_Target {
                // Black to white:
                // return float4(i.uv.xxx, 1); // Horizontal
                // return float4(i.uv.yyy, 1); // Vertical

                // Between two colors:
                // float4 outColor = lerp( _ColorA, _ColorB, i.uv.x);

                // Changing start and end: (saturate clamps the value between 0 and 1, which is a shitty name)
                float uvDirection = (_Vertical > 0.0 ? i.uv.y : i.uv.x);
                uvDirection = _Invert > 0.0 ? 1-uvDirection : uvDirection;
                float t = saturate(InverseLerp( _ColorStart, _ColorEnd, uvDirection));
                // return t; // Debug
                
                float4 gradientColor = lerp( _ColorA, _ColorB, t);
                float4 outColor = gradientColor * tex2D(_MainTex, i.uv);
                return outColor;
            }
            ENDCG
        }
    }
}
