Shader "Custom/AverageColour"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
		Cull Off
		Blend One OneMinusSRCAlpha

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
			

			half4 AverageColorFromTexture (Texture2D tex)
			{
				half4[] texColors = tex.GetPixels32();

				int total = texColors.Length;

				float r = 0;
				float g = 0;
				float b = 0;

				for (int i = 0; i < total; i++)
				{
					r += texColors[i].r;
					g += texColors[i].g;
					b += texColors[i].b;
				}

				return new Color32((byte)(r/total), (byte)(g/total), (byte)(b/total), 0);
			}

            Color32 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				Color32 avgCol = AverageColorFromTexture(col);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, avgCol);
                return avgCol;
            }

            ENDCG
        }
    }
}
