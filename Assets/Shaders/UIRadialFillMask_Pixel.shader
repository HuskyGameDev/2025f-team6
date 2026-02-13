Shader "UI/RadialFillMask_ArcStyledPixel"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Tint", Color) = (1,1,1,1)

        _Fill ("Fill (0-1)", Range(0,1)) = 0
        _StartAngle ("Start Angle (deg)", Range(-180,180)) = -135
        _SweepAngle ("Sweep Angle (deg)", Range(0,360)) = 270
        [Toggle] _FlipDirection ("Flip Direction", Float) = 0
        _Center ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)

        [Toggle] _SnapUVToTexel ("Snap UV To Texel", Float) = 1

        // Arc gradient
        _LowColor  ("Low Color",  Color) = (0.15, 1.00, 0.20, 1)
        _MidColor  ("Mid Color",  Color) = (1.00, 0.90, 0.20, 1)
        _HighColor ("High Color", Color) = (1.00, 0.25, 0.20, 1)
        _MidPoint ("Mid Point (0-1)", Range(0,1)) = 0.65

      
        [Toggle] _QuantizeArc ("Quantize Arc", Float) = 1
        _ArcSteps ("Arc Steps", Range(4,64)) = 16

        
        [Toggle] _UseHead ("Use Head", Float) = 1
        _HeadColor ("Head Color", Color) = (1,1,1,1)
        _HeadWidth ("Head Width (arc fraction)", Range(0.001,0.2)) = 0.05
        _HeadStrength ("Head Strength", Range(0,2)) = 0.9

        // trail glow behind the head
        [Toggle] _UseTrail ("Use Trail", Float) = 1
        _TrailWidth ("Trail Width (arc fraction)", Range(0.001,0.5)) = 0.15
        _TrailStrength ("Trail Strength", Range(0,2)) = 0.35

        // simple arc flicker near head 
        [Toggle] _UseFlicker ("Use Flicker", Float) = 1
        _FlickerStrength ("Flicker Strength", Range(0,1)) = 0.15
        _FlickerSpeed ("Flicker Speed", Range(0,20)) = 8

        // UI mask/stencil support
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ UNITY_UI_CLIP_RECT
            #pragma multi_compile _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            float _Fill, _StartAngle, _SweepAngle, _FlipDirection;
            float4 _Center;
            float _SnapUVToTexel;

            fixed4 _LowColor, _MidColor, _HighColor;
            float _MidPoint;

            float _QuantizeArc, _ArcSteps;

            float _UseHead;
            fixed4 _HeadColor;
            float _HeadWidth, _HeadStrength;

            float _UseTrail;
            float _TrailWidth, _TrailStrength;

            float _UseFlicker, _FlickerStrength, _FlickerSpeed;

            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            float WrapAngle(float a)
            {
                const float TWO_PI = 6.28318530718;
                a = fmod(a, TWO_PI);
                if (a < 0) a += TWO_PI;
                return a;
            }

            fixed4 Lerp3(fixed4 a, fixed4 b, fixed4 c, float t, float midPoint)
            {
                float t1 = saturate(t / max(midPoint, 1e-5));
                float t2 = saturate((t - midPoint) / max(1.0 - midPoint, 1e-5));
                fixed4 ab = lerp(a, b, t1);
                return lerp(ab, c, t2);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // texel snap for pixel stability
                if (_SnapUVToTexel > 0.5)
                {
                    float2 ts = _MainTex_TexelSize.xy;
                    uv = (floor(uv / ts) + 0.5) * ts;
                }

                fixed4 tex = tex2D(_MainTex, uv) * i.color;

                // angle math
                float2 p = uv - _Center.xy;
                float ang = WrapAngle(atan2(p.y, p.x));
                float start = WrapAngle(radians(_StartAngle));
                float sweep = clamp(radians(_SweepAngle), 0.0, 6.2831853);

                float tFill = saturate(_Fill);
                float fillAng = sweep * tFill;

                float deltaCCW = WrapAngle(ang - start);
                float deltaCW  = WrapAngle(start - ang);
                float delta = (_FlipDirection > 0.5) ? deltaCW : deltaCCW;

                // hard fill mask
                float inside = step(delta, fillAng);

                // arc progress (0..1 along the sweep)
                float u = (sweep > 1e-5) ? saturate(delta / sweep) : 0.0;
                if (_QuantizeArc > 0.5)
                {
                    float steps = max(_ArcSteps, 1.0);
                    u = floor(u * steps) / steps;
                }

    
                fixed4 ramp = Lerp3(_LowColor, _MidColor, _HighColor, u, _MidPoint);


                float head = 0.0;
                float trail = 0.0;

                if (_UseHead > 0.5 || _UseTrail > 0.5)
                {
                    float fillU = (sweep > 1e-5) ? saturate(fillAng / sweep) : 0.0;

                    //how far behind the head this pixel is (0 at head, positive behind)
                    float behind = fillU - u;

                    if (_UseHead > 0.5)
                    {
                        // hard-ish band at the front: behind within HeadWidth
                        head = step(0.0, behind) * step(behind, _HeadWidth);
                    }

                    if (_UseTrail > 0.5)
                    {
                        float tw = max(_TrailWidth, 1e-5);
                        float x = saturate(1.0 - (behind / tw));
                        // keep it relatively “steppy” by squaring
                        trail = x * x;
                        // only behind the head
                        trail *= step(0.0, behind);
                    }
                }

                //flicker
                float flicker = 1.0;
                if (_UseFlicker > 0.5)
                {
                   // simple deterministic flicker using u + time
                    float s = sin((_Time.y * _FlickerSpeed) + (u * 20.0));
                    s = (s * 0.5 + 0.5); // 0..1
                    flicker = 1.0 + (s * _FlickerStrength);
                }

                fixed3 col = tex.rgb * ramp.rgb;

                col = col * (1.0 + trail * _TrailStrength);
                col = col + (head * _HeadStrength) * _HeadColor.rgb;

                col *= flicker;

                tex.rgb = col;

                tex.a *= inside;

                #ifdef UNITY_UI_CLIP_RECT
                tex.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(tex.a - 0.001);
                #endif

                return tex;
            }
            ENDCG
        }
    }
}
