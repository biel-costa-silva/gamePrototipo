Shader "Custom/SpriteDestaqueDourado"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite", 2D) = "white" {}
        _CorDestaque ("Cor dourada", Color) = (1, 0.82, 0.25, 1)
        _Destaque ("Destaque (0 a 1)", Range(0, 1)) = 0

        [Header(Contorno)]
        _Espessura ("Espessura do contorno (px)", Float) = 1.5
        _VelocidadePulso ("Velocidade do pulso", Float) = 4

        [Header(Pelicula)]
        _Pelicula ("Opacidade da pelicula", Range(0, 1)) = 0.25
        _Trama ("Trama (0 = pelicula lisa)", Range(0, 1)) = 0
        _TramaTamanho ("Tamanho da trama (px)", Float) = 6

        [Header(Orbes de luz)]
        _TamanhoCelula ("Espacamento dos orbes (px)", Float) = 20
        _DensidadeOrbes ("Quantidade de orbes", Range(0, 1)) = 0.4
        _TamanhoOrbe ("Tamanho do orbe", Range(0.02, 0.4)) = 0.12
        _VelocidadeOrbes ("Velocidade de subida", Float) = 0.15
        _IntensidadeOrbes ("Brilho dos orbes", Float) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize; // x,y = tamanho de 1 texel | z,w = largura,altura em texels

            CBUFFER_START(UnityPerMaterial)
                half4 _CorDestaque;
                half _Destaque;
                half _Espessura;
                half _VelocidadePulso;
                half _Pelicula;
                half _Trama;
                half _TramaTamanho;
                half _TamanhoCelula;
                half _DensidadeOrbes;
                half _TamanhoOrbe;
                half _VelocidadeOrbes;
                half _IntensidadeOrbes;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half AlphaEm(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            // numero "aleatorio" estavel (0..1) para cada celula
            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 c = tex * i.color;
                float2 pixel = i.uv * _MainTex_TexelSize.zw; // posicao em texels

                // ---------- contorno: pixel transparente com vizinho opaco ----------
                float2 px = _MainTex_TexelSize.xy * _Espessura;
                half vizinho = max(
                    max(AlphaEm(i.uv + float2(px.x, 0)), AlphaEm(i.uv - float2(px.x, 0))),
                    max(AlphaEm(i.uv + float2(0, px.y)), AlphaEm(i.uv - float2(0, px.y))));
                half borda = saturate(vizinho - tex.a) * _Destaque;
                half pulso = 0.65h + 0.35h * sin(_Time.y * _VelocidadePulso);

                // ---------- pelicula dourada fixa (com trama opcional) ----------
                float2 gp = frac(pixel / max(_TramaTamanho, 1.0));
                half linha = max(step(0.85, gp.x), step(0.85, gp.y));
                half pelicula = _Pelicula * (1.0h + _Trama * 2.0h * linha) * _Destaque * tex.a;

                half3 rgb = lerp(c.rgb, _CorDestaque.rgb, saturate(pelicula));

                // ---------- orbes de luz subindo devagar ----------
                // o sprite e dividido em celulas quadradas; cada celula pode ter um orbe
                float2 celula = pixel / max(_TamanhoCelula, 1.0);
                float2 id = floor(celula);
                float2 f = frac(celula);

                float h1 = Hash(id);
                float h2 = Hash(id + 17.3);
                float h3 = Hash(id + 41.7);

                half temOrbe = step(h3, _DensidadeOrbes);
                half vida = frac(h2 + _Time.y * _VelocidadeOrbes * (0.6 + 0.8 * h1));  // 0 -> 1: nasce embaixo, some em cima
                float2 centro = float2(0.2 + 0.6 * h1 + 0.08 * sin(_Time.y * 1.5 + h2 * 6.2831), 0.1 + 0.8 * vida);
                half d = distance(f, centro);
                half orbe = smoothstep(_TamanhoOrbe, 0.0, d) * sin(vida * 3.14159) * temOrbe;

                rgb += _CorDestaque.rgb * orbe * _IntensidadeOrbes * _Destaque * tex.a;

                // ---------- contorno por cima ----------
                rgb = lerp(rgb, _CorDestaque.rgb * pulso, borda);
                half alpha = max(c.a, borda);

                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }
}
