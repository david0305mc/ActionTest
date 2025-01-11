Shader "Seajin/Custom/VersatileLightingURP" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FrontColor ("Front Color", Color) = (1,0.73,0.117,1)
        _TopColor ("Top Color", Color) = (0.05,0.275,0.275,1)
        _RightColor ("Right Color", Color) = (0,0,0,1)
        _SideLightColor ("Side Light Color", Color) = (0.2,0.2,0.2,1)
        _SideLightIntensity ("Side Light Intensity", Range(0, 3)) = 0.5
        _RimColor ("Rim Color", Color) = (0,0,0,1)
        _RimPower ("Rim Power", Float) = 2.0
        _ColorBoost ("Color Boost", Range(-3,5)) = 1.0

        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowSoftness ("Shadow Softness", Range(0.0, 1.0)) = 0.5

        _FogColor ("Fog Color", Color) = (1,1,1,1)
        _FogYStartPos ("Fog Y-start Pos", Float) = 0.0
        _FogHeight ("Fog Height", Float) = 5.0
    }

    SubShader {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        LOD 200

        Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            // URP 라이브러리 포함
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // 속성 선언
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float3 _FrontColor;
            float3 _TopColor;
            float3 _RightColor;
            float3 _SideLightColor;
            float _SideLightIntensity;
            float3 _RimColor;
            float _RimPower;
            float _ColorBoost;

            float4 _ShadowColor;
            float _ShadowSoftness;

            float4 _FogColor;
            float _FogYStartPos;
            float _FogHeight;

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                // 텍스처 샘플링
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // 기본 Lambert 조명 계산
                Light mainLight = GetMainLight();
                half3 normal = normalize(IN.normalWS);
                half NdotL = saturate(dot(normal, mainLight.direction));

                // 그림자 계산
                half shadow = MainLightRealtimeShadow(IN.shadowCoord);
                shadow = lerp(shadow, 1.0, _ShadowSoftness);
                half3 shadowColor = lerp(_ShadowColor.rgb, 1.0, shadow);

                half3 frontLighting = _FrontColor * NdotL * mainLight.color * shadowColor;

                // Top Color 조명 계산 (Y축 방향)
                half NdotT = saturate(dot(normal, float3(0, 1, 0)));
                half3 topLighting = _TopColor * NdotT;

                // Right Color 조명 계산 (X축 방향)
                half NdotR = saturate(dot(normal, float3(1, 0, 0)));
                half3 rightLighting = _RightColor * NdotR;

                // 옆면 강조 조명 계산
                half sideLightMask = saturate(1.0 - NdotL);
                half3 sideLighting = _SideLightColor * sideLightMask * _SideLightIntensity;

                // Rim Lighting 계산
                half rim = 1.0 - saturate(dot(normal, normalize(_WorldSpaceCameraPos - IN.positionWS)));
                half3 rimLighting = _RimColor * pow(rim, _RimPower);

                // 최종 조명 합산
                half3 lighting = frontLighting + topLighting + rightLighting + sideLighting + rimLighting;
                half3 finalColor = texColor.rgb * lighting * _ColorBoost;

                // 포그 적용
                float fogFactor = saturate((IN.positionWS.y - _FogYStartPos) / _FogHeight);
                finalColor = lerp(_FogColor.rgb, finalColor, fogFactor);

                return half4(finalColor, texColor.a);
            }

            ENDHLSL
        }

        // ShadowCaster 패스 추가
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
    FallBack "Hidden/InternalErrorShader"
}