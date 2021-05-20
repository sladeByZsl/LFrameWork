// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PostEffect/Edge Detection" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EdgeOnly ("Edge Only", Float) = 1.0
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
		_BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Pass {  
			ZTest Always Cull Off ZWrite Off
			
			CGPROGRAM
			
			#include "UnityCG.cginc"
			
			#pragma vertex vert  
			#pragma fragment fragSobel
			
			sampler2D _MainTex;  
			uniform half4 _MainTex_TexelSize;//纹素大小xxx__TexelSize
			fixed _EdgeOnly;
			fixed4 _EdgeColor;
			fixed4 _BackgroundColor;
			
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv[9] : TEXCOORD0;   //数组声明
			};
			  
			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				//移动到顶点着色器计算可以提升性能
				//从顶点着色器到片元着色器的插值是线性的,所以这样的转移不会影响纹理坐标的计算结果
				half2 uv = v.texcoord;
				
				o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
				o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, 1);
				o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, 1);
				o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
				o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
				o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
				o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
				o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, -1);
				o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, -1);
						 
				return o;
			}
			
			fixed luminance(fixed4 color) {
				return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 
			}
			
			/*
			这里的Sobel算子是基于坐标轴以屏幕左上为原点，右下分别为+x,+y方向的，而不是类似于uv坐标轴的以屏幕左下为原点，右上分别为+x,+y方向的
			*/
			half Sobel(v2f i) {
				const half Gx[9] = {-1,  0,  1,
									-2,  0,  2,
									-1,  0,  1};
				const half Gy[9] = {-1, -2, -1,
									0,  0,  0,
									1,  2,  1};		
				
				half texColor;
				half edgeX = 0;
				half edgeY = 0;
				for (int it = 0; it < 9; it++) {
					texColor = luminance(tex2D(_MainTex, i.uv[it]));
					edgeX += texColor * Gx[it];
					edgeY += texColor * Gy[it];
				}
				
				/*没什么特别意义，只是因为混合因子edge在lerp里面的位置：也就是说，如果是纯边界的地方，edge是0。而梯度可能为负也可能为正，绝对值表示了它跟邻域之间值的差距大小，所以绝对值越大表示差异越大，越是边界，那么1-abs就是我们需要的edge值。
				*/
				//half edge = 1 - abs(edgeX) - abs(edgeY);
				
				//绝对值相加近似模拟最终梯度值
				half edge = abs(edgeX) + abs(edgeY);
				return edge;
			}
			
			fixed4 fragSobel(v2f i) : SV_Target {
				half edge = Sobel(i);
				fixed4 col=tex2D(_MainTex, i.uv[4]);
				//利用得到的梯度值进行插值操作，其中梯度值越大，越接近边缘的颜色
				fixed4 withEdgeColor = lerp(col,_EdgeColor, edge);

				fixed4 onlyEdgeColor = lerp(_BackgroundColor,_EdgeColor , edge);
				//_EdgeOnly0原图加个边缘   1背景色加边缘
				return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
 			}
			
			ENDCG
		} 
	}
	FallBack Off
}