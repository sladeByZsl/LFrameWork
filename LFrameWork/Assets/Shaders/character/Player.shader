// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PlanarShadow/Player"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowColor("ShadowColor",Color) = (1,1,1,1)
		_ShadowFalloff ("ShadowFalloff", float) = 1.35
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+10" }
		LOD 100
		
		Pass
		{
			CGPROGRAM

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
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			
			ENDCG
		}

		Pass
		{		
			Blend SrcAlpha  OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			ColorMask RGB
			//深度稍微偏移防止阴影与地面穿插
			Offset -1 , 0
			/*
			上面的意思就是当像素通过了深度测试，背面裁剪，可以渲染的情况下，如果像素的模版值等于0，则写入模版值，这里写入的模版值是用的Invert状态，也就是255, 当然也可以用IncrSat状态，这样写入的模版值就为1
			可以想象一下，当一个像素所在的位置已经模版值不为0的情况下，另外一个像素再想写入这个像素的位置，它是通过不了模版测试的。
			通过模版测试的状态设置，就可以得到正确的阴影效果
			这个是王者平面阴影的亮点所在了。一般的平面阴影在ps输出颜色的时候，只会单纯的输出纯色，或者没有Alpha混合状态，王者为了阴影有淡化效果，使用了模版
			*/
			Stencil
			{
				Ref 0			
				Comp Equal			
				Pass Invert
				Fail Keep
				ZFail Keep
			}

			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _LightDir;
			float4 _ShadowColor;
			float _ShadowFalloff;

			float3 ShadowProjectPos(float4 vertPos)
			{
				float3 shadowPos;

				//得到顶点的世界空间坐标
				float3 worldPos = mul(unity_ObjectToWorld, vertPos).xyz;

				//灯光方向
				float3 lightDir = normalize(_LightDir.xyz);

				//计算阴影的世界空间坐标(如果顶点低于地面，则阴影点实际就是顶点在世界空间的位置，不做改变)
				shadowPos.y = min(worldPos.y, _LightDir.w);
				shadowPos.xz = worldPos.xz - lightDir.xz * max(0, worldPos.y - _LightDir.w) / lightDir.y;

				return shadowPos;
			}

			v2f vert(appdata v)
			{
				v2f o;

				//得到阴影的世界空间坐标
				float3 shadowPos = ShadowProjectPos(v.vertex);

				//转换到裁切空间
				o.vertex = UnityWorldToClipPos(shadowPos);


				/*
				Unity Shader中获取模型中心点世界坐标的几种写法
				float3 center = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
				float3 center = float3(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23);
				float3 center = mul(unity_ObjectToWorld , float(0,0,0,1)).xyz;
				float3 center = unity_ObjectToWorld._14_24_34;
				*/
				//得到中心点世界坐标
				float3 center = float3(unity_ObjectToWorld[0].w, _LightDir.w, unity_ObjectToWorld[2].w);
				//计算阴影衰减
				float falloff = 1 - saturate(distance(shadowPos, center) * _ShadowFalloff);

				//阴影颜色
				o.color = _ShadowColor;
				o.color.a *= falloff;

				return o;
			}
			
			float4 frag(v2f i) : SV_Target
			{
				//return float4(i.color.r,i.color.g,i.color.b,i.color.a);
				return i.color;
			}
			
			ENDCG
		}
	}
}
