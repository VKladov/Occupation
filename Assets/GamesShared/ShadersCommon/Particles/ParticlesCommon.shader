Shader "FXGL/Particles/ParticlesCommon" 
{
	Properties 
	{
		[HDR] _TintColor ("Tint Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Particle Texture", 2D) = "white" {}
		
		// Will set "USE_COLOR_MUL_TO_ALPHA" shader keyword when set
		[Toggle] USE_COLOR_MUL_TO_ALPHA ("Use color mul to alpha (Smooth ADD)", Float) = 0
		
		// Will set "USE_ALBEDO_ALPHA_FROM_GRAYSCALE" shader keyword when set
		// NOTE: для рендеринга в RTT с альфа каналом текстуры без альфы, A канал заливается сплошным полигоном
		// для таких ситуаций мы корректируем прозрачность в пиксельном шейдере, 
		// взяв за значение альфы "градиент серого"	базовой текстуры
		[Toggle] USE_ALBEDO_ALPHA_FROM_GRAYSCALE ("Alpha from grayscale", Float) = 0
		
		[HideInInspector] _ZTestMode ("ZTestMode", Int) = 4
		[HideInInspector] _CullMode ("CullMode", Int) = 2
		[HideInInspector] _SrcBlend ("SrcBlend", Int) = 5
		[HideInInspector] _DstBlend ("DstBlend", Int) = 10
		[HideInInspector] _ColorMask ("Color Mask", Int) = 14
		
		
		
	}

	Category 
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"RenderType"="Transparent"
			"IgnoreProjector"="True" 
			"ForceNoShadowCasting"="True"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="False"
		}
		
		
		Cull [_CullMode]
		Lighting Off
		ZWrite Off
		ZTest [_ZTestMode]
		Blend [_SrcBlend] [_DstBlend] , One One

		// для частиц в стандартных шейдерах от юнити  это позиция жёстко устанавливается в RGB
		// но для рендеринга в текстуру с прозрачностью,  нам нужно писать ещё в A канал (RGBA)
		// поэтому этот параметр мы будем настраивать индивидуально из приложения
		ColorMask [_ColorMask]
		
		Fog { Color (0,0,0,0) }
		

		BindChannels 
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader 
		{

			Pass 
			{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_particles

				#pragma multi_compile __ USE_COLOR_MUL_TO_ALPHA_ON
				#pragma multi_compile __ USE_ALBEDO_ALPHA_FROM_GRAYSCALE_ON
				
				#include "UnityCG.cginc"

				

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					return o;
				}


				sampler2D _MainTex;
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 albedo = tex2D(_MainTex, i.texcoord);
					
					#ifdef USE_ALBEDO_ALPHA_FROM_GRAYSCALE_ON
					
					// суть нижеприведённого кода 
					// из текстуры без альфа канала создать текстуру с альфой
					// RGB будут кодировать цвет, а сила света - прозрачность
					//
					// Это приведение будет верно только для адитивного блендинга
					
					half sum = (albedo.r+albedo.g+albedo.b);
					
					half maxChanel = max(max(albedo.r,albedo.g), albedo.b);
					albedo.rgb = albedo.rgb/maxChanel;
					
					// домножаем сумму на 1/3 - получив среднее значение светимости
					albedo.a = sum*0.5;
					#endif
					
					
					half4 result = i.color * _TintColor * albedo;
					
					#ifdef USE_COLOR_MUL_TO_ALPHA_ON
					result = half4(result.rgb*result.a, result.a);
					#endif
					
					return result;
				}
				ENDCG 
			}
		}  
	}
	
	CustomEditor "CustomRenderStateForTransparencyMaterialInspector"
}