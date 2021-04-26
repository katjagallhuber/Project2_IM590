Shader "Unlit/RenderParticles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Particle
            {
                float2 position;
                float2 direction;
                float speed;
            };

            struct v2g
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float2 position : TEXCOORD1;
                float2 direction : TEXCOORD2;
                float speed : TEXCOORD3;
            };

            struct g2f
            {
                float4 position : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            StructuredBuffer<Particle> particlesBuffer;


            v2g vert (uint id : SV_VertexID)
            {
                v2g o = (v2g)0;
                o.position = particlesBuffer[id].position;
                o.direction = particlesBuffer[id].direction;
                o.speed = particlesBuffer[id].speed;
                return o;
            }

            [maxvertexcount(2)]
            void geom(point v2g In[1], inout LineStream<g2f> linestream)
            {
                g2f o = (g2f)0;

                float distance = 1.0;
                float directionLength = 0.05;

                float2 position = In[0].position;
                float2 direction = In[0].direction;
                float speed = In[0].speed;

                float4 color = float4(0.0, 0.1, 1.0, 1.0);

                o.position = UnityObjectToClipPos(float4(position, 0.0, 1.0));
                o.color = color;
                linestream.Append(o);
                o.position = UnityObjectToClipPos(float4(position + (direction * directionLength), 0.0, 1.0));
                o.color = color;
                linestream.Append(o);
                linestream.RestartStrip();
            }

            fixed4 frag(g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
