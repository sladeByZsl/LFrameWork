// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|custl-2449-OUT;n:type:ShaderForge.SFN_Tex2d,id:2861,x:31096,y:32242,varname:node_2861,prsc:2,tex:0c7a6bc539dc1ba4c8e9f4c2fb5002be,ntxv:0,isnm:False|UVIN-6856-OUT,TEX-5760-TEX;n:type:ShaderForge.SFN_TexCoord,id:420,x:30054,y:31475,varname:node_420,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:4954,x:30403,y:31478,varname:node_4954,prsc:2,spu:1,spv:0|UVIN-420-UVOUT,DIST-8611-U;n:type:ShaderForge.SFN_Panner,id:5017,x:30403,y:31667,varname:node_5017,prsc:2,spu:0,spv:1|UVIN-420-UVOUT,DIST-8611-V;n:type:ShaderForge.SFN_TexCoord,id:8611,x:30054,y:31646,varname:node_8611,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_ComponentMask,id:5933,x:30619,y:31478,varname:node_5933,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-4954-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:8308,x:30619,y:31667,varname:node_8308,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-5017-UVOUT;n:type:ShaderForge.SFN_Append,id:9695,x:30807,y:31570,varname:node_9695,prsc:2|A-5933-OUT,B-8308-OUT;n:type:ShaderForge.SFN_Multiply,id:775,x:32435,y:31703,varname:node_775,prsc:2|A-6051-RGB,B-2474-OUT;n:type:ShaderForge.SFN_Color,id:6051,x:32100,y:31671,ptovrint:False,ptlb:Main_Color,ptin:_Main_Color,varname:node_6051,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:5760,x:30452,y:32581,ptovrint:False,ptlb:Main_Tex,ptin:_Main_Tex,varname:node_5760,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0c7a6bc539dc1ba4c8e9f4c2fb5002be,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:181,x:32644,y:31629,varname:node_181,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:7724,x:32055,y:33183,varname:node_7724,prsc:2,ntxv:0,isnm:False|UVIN-6979-OUT,TEX-1845-TEX;n:type:ShaderForge.SFN_Slider,id:6934,x:30975,y:32564,ptovrint:False,ptlb:widthe,ptin:_widthe,varname:node_6934,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1070504,max:1;n:type:ShaderForge.SFN_Color,id:4879,x:32259,y:32738,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_4879,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:5711,x:32625,y:32631,varname:node_5711,prsc:2|A-5198-OUT,B-4879-RGB;n:type:ShaderForge.SFN_Add,id:2907,x:32644,y:31870,varname:node_2907,prsc:2|A-775-OUT,B-5711-OUT;n:type:ShaderForge.SFN_Subtract,id:3099,x:32105,y:32403,varname:node_3099,prsc:2|A-5300-OUT,B-2474-OUT;n:type:ShaderForge.SFN_Clamp01,id:5198,x:32298,y:32413,varname:node_5198,prsc:2|IN-3099-OUT;n:type:ShaderForge.SFN_TexCoord,id:8830,x:31074,y:32034,varname:node_8830,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Add,id:8086,x:31299,y:32490,varname:node_8086,prsc:2|A-6403-OUT,B-6934-OUT;n:type:ShaderForge.SFN_Multiply,id:4863,x:32946,y:32564,varname:node_4863,prsc:2|A-5300-OUT,B-181-A,C-1432-OUT,D-2788-OUT;n:type:ShaderForge.SFN_Multiply,id:2449,x:32909,y:31937,varname:node_2449,prsc:2|A-181-RGB,B-2907-OUT,C-4863-OUT,D-7724-RGB;n:type:ShaderForge.SFN_Multiply,id:9569,x:32612,y:32901,varname:node_9569,prsc:2|A-5198-OUT,B-4879-A;n:type:ShaderForge.SFN_Multiply,id:4818,x:32384,y:31863,varname:node_4818,prsc:2|A-6051-A,B-2474-OUT;n:type:ShaderForge.SFN_Add,id:2788,x:32825,y:32738,varname:node_2788,prsc:2|A-4818-OUT,B-9569-OUT;n:type:ShaderForge.SFN_Panner,id:9063,x:29848,y:31882,varname:node_9063,prsc:2,spu:1,spv:0|UVIN-8809-UVOUT,DIST-7481-U;n:type:ShaderForge.SFN_Tex2d,id:6379,x:30472,y:31882,varname:node_6379,prsc:2,tex:0c7a6bc539dc1ba4c8e9f4c2fb5002be,ntxv:0,isnm:False|UVIN-6350-OUT,TEX-5760-TEX;n:type:ShaderForge.SFN_TexCoord,id:8809,x:29588,y:31872,varname:node_8809,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:662,x:30094,y:32298,ptovrint:False,ptlb:niu_qu,ptin:_niu_qu,varname:node_662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:3869,x:30665,y:32114,varname:node_3869,prsc:2|A-6379-R,B-662-OUT;n:type:ShaderForge.SFN_Add,id:6856,x:30841,y:31977,varname:node_6856,prsc:2|A-9695-OUT,B-3869-OUT;n:type:ShaderForge.SFN_TexCoord,id:7481,x:29588,y:32040,varname:node_7481,prsc:2,uv:2,uaff:True;n:type:ShaderForge.SFN_Panner,id:7429,x:29836,y:32040,varname:node_7429,prsc:2,spu:0,spv:1|UVIN-8809-UVOUT,DIST-7481-V;n:type:ShaderForge.SFN_ComponentMask,id:7178,x:30042,y:31882,varname:node_7178,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-9063-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:479,x:30042,y:32040,varname:node_479,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-7429-UVOUT;n:type:ShaderForge.SFN_Append,id:6350,x:30269,y:31882,varname:node_6350,prsc:2|A-7178-OUT,B-479-OUT;n:type:ShaderForge.SFN_TexCoord,id:757,x:31606,y:33157,varname:node_757,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6979,x:31812,y:33313,varname:node_6979,prsc:2|A-757-UVOUT,B-1325-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:1845,x:31881,y:33472,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_1845,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1325,x:31463,y:33289,varname:node_1325,prsc:2|A-3869-OUT,B-5379-OUT;n:type:ShaderForge.SFN_Vector1,id:5379,x:31139,y:33384,varname:node_5379,prsc:2,v1:1;n:type:ShaderForge.SFN_Smoothstep,id:5300,x:31761,y:31885,varname:node_5300,prsc:2|A-2808-OUT,B-3478-OUT,V-3433-OUT;n:type:ShaderForge.SFN_Slider,id:2616,x:31141,y:31943,ptovrint:False,ptlb:smooth,ptin:_smooth,varname:node_2616,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3004161,max:1;n:type:ShaderForge.SFN_Subtract,id:2808,x:31518,y:31776,varname:node_2808,prsc:2|A-3478-OUT,B-2616-OUT;n:type:ShaderForge.SFN_Smoothstep,id:2474,x:31781,y:32301,varname:node_2474,prsc:2|A-5513-OUT,B-8086-OUT,V-3433-OUT;n:type:ShaderForge.SFN_Set,id:7775,x:31245,y:32091,varname:rong_jie,prsc:2|IN-8830-Z;n:type:ShaderForge.SFN_Get,id:3478,x:31209,y:31797,varname:node_3478,prsc:2|IN-7775-OUT;n:type:ShaderForge.SFN_Get,id:6403,x:31028,y:32477,varname:node_6403,prsc:2|IN-7775-OUT;n:type:ShaderForge.SFN_Subtract,id:5513,x:31558,y:32184,varname:node_5513,prsc:2|A-8086-OUT,B-2616-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:3433,x:31277,y:32242,ptovrint:False,ptlb:R/A,ptin:_RA,varname:node_3433,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2861-R,B-2861-A;n:type:ShaderForge.SFN_SwitchProperty,id:1432,x:32382,y:33212,ptovrint:False,ptlb:Mask_R/A,ptin:_Mask_RA,varname:node_1432,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-7724-A,B-7724-R;proporder:6051-5760-3433-6934-4879-662-1845-2616-1432;pass:END;sub:END;*/

Shader "Custom/fx_shader_01_rongJie_add" {
    Properties {
        _Main_Color ("Main_Color", Color) = (1,1,1,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        [MaterialToggle] _RA ("R/A", Float ) = 0
        _widthe ("widthe", Range(0, 1)) = 0.1070504
        _Color2 ("Color2", Color) = (1,1,1,1)
        _niu_qu ("niu_qu", Range(0, 1)) = 0
        _mask ("mask", 2D) = "white" {}
        _smooth ("smooth", Range(0, 1)) = 0.3004161
        [MaterialToggle] _Mask_RA ("Mask_R/A", Float ) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float4 _Main_Color;
            uniform sampler2D _Main_Tex; uniform float4 _Main_Tex_ST;
            uniform float _widthe;
            uniform float4 _Color2;
            uniform float _niu_qu;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float _smooth;
            uniform fixed _RA;
            uniform fixed _Mask_RA;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
                float rong_jie = i.uv1.b;
                float node_8086 = (rong_jie+_widthe);
                float2 node_6350 = float2((i.uv0+i.uv2.r*float2(1,0)).r,(i.uv0+i.uv2.g*float2(0,1)).g);
                float4 node_6379 = tex2D(_Main_Tex,TRANSFORM_TEX(node_6350, _Main_Tex));
                float node_3869 = (node_6379.r*_niu_qu);
                float2 node_6856 = (float2((i.uv0+i.uv1.r*float2(1,0)).r,(i.uv0+i.uv1.g*float2(0,1)).g)+node_3869);
                float4 node_2861 = tex2D(_Main_Tex,TRANSFORM_TEX(node_6856, _Main_Tex));
                float _RA_var = lerp( node_2861.r, node_2861.a, _RA );
                float node_2474 = smoothstep( (node_8086-_smooth), node_8086, _RA_var );
                float node_3478 = rong_jie;
                float node_5300 = smoothstep( (node_3478-_smooth), node_3478, _RA_var );
                float node_5198 = saturate((node_5300-node_2474));
                float2 node_6979 = (i.uv0+(node_3869*1.0));
                float4 node_7724 = tex2D(_mask,TRANSFORM_TEX(node_6979, _mask));
                float3 finalColor = (i.vertexColor.rgb*((_Main_Color.rgb*node_2474)+(node_5198*_Color2.rgb))*(node_5300*i.vertexColor.a*lerp( node_7724.a, node_7724.r, _Mask_RA )*((_Main_Color.a*node_2474)+(node_5198*_Color2.a)))*node_7724.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
