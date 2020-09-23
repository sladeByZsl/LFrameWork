/*
1、外轮廓描边的时候，如果不想最后内部也被描上边，记得描边的时候，请关闭 深度写入，ZWrite off；
2、Unity 的 geometry 类型的渲染顺序是从前往后渲染，而 Transparent 类型是从后往前渲染。天空盒子的渲染顺序位于geometry之后，Transparent 之前。因此我们要把  "Queue" = "Geometry+1000" 或者 "Queue" = "Transparent"；
3、注意这种方式的描边不建议用于棱角分明的物体上，效果会不对，例如正方体等
*/

Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Diffuse("Diffuse",COLOR) = (1,1,1,1)
		 //描边效果
		_OutlineColor("Outline Color",COLOR) = (0,0,0,1)
		_OutlineScale("Outline Scale",Range(0,1))=0.001
    }
    SubShader
    {
		// "Queue" = "Geometry+1000" +1000 防止 在 Game 下，天空盒子渲染覆盖描边，或 改成 "Queue" = "Transparent" 也行
		Tags { "Queue" = "Geometry+1000" "RenderType" = "Opaque" }
		LOD 100

        Pass
        {
			Name "Outline"
			ZWrite off //关闭深度写入，让下一个通道正常写入的时候覆盖之前内部的描边（注释掉即可看到内部也描上边），保证只描轮廓
			Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			float4 _OutlineColor;
			float _OutlineScale;

            struct v2f
            {
				float4 vertex  : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
				v2f o;
				// 描边方法一：物体模型上法线外拓改变顶点的位置
				v.vertex.xyz += v.normal * _OutlineScale;
				o.vertex = UnityObjectToClipPos(v.vertex);

				// 描边方法二：View视野下法线外拓改变顶点的位置
				//float4 pos = mul(UNITY_MATRIX_V,mul(unity_ObjectToWorld,v.vertex));
				//float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV,v.normal));
				//pos += float4(normal,0)* _OutlineScale;
				//o.vertex = mul(UNITY_MATRIX_P,pos);

				// 描边方法三：裁剪空间下下法线外拓改变顶点的位置
				//o.vertex = UnityObjectToClipPos(v.vertex);
				//float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_MV,v.normal));
				//float2 clipNormal = normalize(TransformViewToProjection(viewNormal.xy));
				//o.vertex.xy += clipNormal * _OutlineScale;
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return _OutlineColor;
            }
            ENDCG
        }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			struct v2f
			{

				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Diffuse;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// 环境光
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT;

				// 贴图的本色
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb;

				// 漫反射
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				float halfLambert = dot(worldLightDir,i.worldNormal) * 0.5 + 0.5;

				// 最终漫反射
				fixed3 diffuse = _LightColor0.rgb * albedo  * _Diffuse.rgb * halfLambert;

				return fixed4(ambient + diffuse,1);
			}
			ENDCG
		}
    }
}
