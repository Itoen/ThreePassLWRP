Shader "ThirdPassLWRP/Sample"
{
    Properties
    {
        _FirstPassColor ("1st Pass Color", Color) = (1,0,0,1)
        _SecondPassColor ("2nd Pass Color", Color) = (0,1,0,1)
        _ThirdPassColor ("3rd Pass Color", Color) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="LightweightPipeline" }

        Pass
        {
            Tags{"LightMode"="LightweightForward"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
         
            #include "LWRP/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _FirstPassColor;
            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }
         
            float4 frag (v2f i) : SV_Target
            {
                return _FirstPassColor;
            }
            ENDHLSL
        }

        Pass
        {
            Tags{"LightMode"="LightweightForward2nd"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
         
            #include "LWRP/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _SecondPassColor;
            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }
         
            float4 frag (v2f i) : SV_Target
            {
                return _SecondPassColor;
            }
            ENDHLSL
        }

        Pass
        {
            Tags{"LightMode"="LightweightForward3rd"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
         
            #include "LWRP/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _ThirdPassColor;
            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }
         
            float4 frag (v2f i) : SV_Target
            {
                return _ThirdPassColor;
            }
            ENDHLSL
        }
    }
}
