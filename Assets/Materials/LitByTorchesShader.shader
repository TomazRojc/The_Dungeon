Shader "Custom/LitByTorches"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaxRange ("Max Range", Float) = 10.0   // Torch max radius
        _MinRange ("Min Range", Float) = 8.0   // Torch min radius
        _RootBase ("Root Base", Float) = 1.0   // base of the root used in the opacity calculation
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _TorchPositions[10];
            float _TorchCount;
            float _MaxRange;
            float _MinRange;
            float _RootBase;
            float _SmoothDist;
            

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            float rootBase(float x, float b) {
                return pow(x, 1.0 / b);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float alpha = 0.0;
                _SmoothDist = _MaxRange - _MinRange;
                
                for (int j = 0; j < _TorchCount; j++)
                {
                    float dist = distance(i.worldPos, _TorchPositions[j].xy);
                    
                    if (dist < _MaxRange)
                    {
                        float intensity = 1.0;
                        if (dist > _MinRange)
                        {
                            intensity = 1.0 - rootBase((dist - _MinRange) / _SmoothDist, _RootBase);
                        }
                        alpha = max(alpha, intensity); // Use max to blend multiple torches
                    }
                }

                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= saturate(alpha); // Ensure final alpha stays between 0 and 1
                return col;
            }
            ENDCG
        }
    }
}
