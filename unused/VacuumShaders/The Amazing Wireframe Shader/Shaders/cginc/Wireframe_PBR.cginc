#ifndef VACUUM_WIREFRAME_PBR_CGINC
#define VACUUM_WIREFRAME_PBR_CGINC

  
#include "../cginc/Wireframe_Core.cginc"

#if defined(V_WIRE_TRANSPARENCY_ON) && defined(V_WIRE_VERTEX_COLOR_ON)
	#undef V_WIRE_VERTEX_COLOR_ON
#endif

//Variables//////////////////////////////////
fixed4 _Color;
sampler2D _MainTex;
half4 _MainTex_ST;
half2 _V_WIRE_MainTex_Scroll;

half _Glossiness;
half _Metallic;

#ifdef _NORMALMAP
	sampler2D _V_WIRE_NormalMap;
#endif


#ifdef V_WIRE_CUTOUT 
	half _Cutoff;
#endif



//Struct/////////////////////////////////////////////////////////
struct Input 
{
	float4 texcoord; // xy - uv, zw - worldNormal.xy (worldNormal.z inside 'mass.w')
	
	
	#if defined(V_WIRE_DYNAMIC_MASK_ON) || defined(V_WIRE_FRESNEL_ON)
		half3 worldPos;
	#endif

	#ifdef V_WIRE_VERTEX_COLOR_ON 
		half4 color : COLOR;
	#endif

	#ifdef V_WIRE_TRANSPARENCY_ON
		float2 texcoord1;
	#endif

	half4 mass; //xyz - mass, w - worldNormal.z
};


//Vertex Shader///////////////////////////////////////////
void vert (inout appdata_full v, out Input o) 
{
	UNITY_INITIALIZE_OUTPUT(Input,o);	


//Curved World Compatibility
//V_CW_TransformPointAndNormal(v.vertex, v.normal, v.tangent);


	o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);		
	o.texcoord.xy += _V_WIRE_MainTex_Scroll.xy * _Time.x;

	#ifdef V_WIRE_TRANSPARENCY_ON
		o.texcoord1 = TRANSFORM_TEX(((_V_WIRE_TransparentTex_UVSet == 0) ? v.texcoord.xy : v.texcoord1.xy), _V_WIRE_TransparentTex);
		o.texcoord1 += _V_WIRE_TransparentTex_Scroll.xy * _Time.x;
	#endif

	#if defined(V_WIRE_FRESNEL_ON) && !defined(_NORMALMAP)
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		o.texcoord.zw = worldNormal.xy;
		o.mass.w = worldNormal.z;
	#endif

	#ifndef V_WIRE_NO
		o.mass.xyz = ExtructWireframeFromVertexUV(v.texcoord);
	#else
		o.mass.xyz = 0;
	#endif
}


//Pixel Shader///////////////////////////////////////////
void surf (Input IN, inout SurfaceOutputStandard o) 
{
	half4 mainTex = tex2D (_MainTex, IN.texcoord.xy);

	//Color
	#if defined(V_WIRE_NO_COLOR_BLACK)
		fixed4 retColor = 0;
	#elif defined(V_WIRE_NO_COLOR_WHITE)
		fixed4 retColor = 1;
	#else
		fixed4 retColor = mainTex * _Color;
	#endif	

	#ifdef V_WIRE_VERTEX_COLOR_ON
		retColor.rgb *= IN.color.rgb;
	#endif
	
	#ifdef _NORMALMAP
		o.Normal = UnpackNormal(tex2D(_V_WIRE_NormalMap, IN.texcoord.xy));
	#endif

	
	// Metallic and smoothness come from slider variables
	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;
	 
	 
	
	#ifdef V_WIRE_FRESNEL_ON
		
		fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(IN.worldPos));

		#ifdef _NORMALMAP
			half fresnel = saturate(dot (worldViewDir, fixed3(0, 0, 1))); //o.Normal = 0
		#else
			half fresnel = saturate(dot (worldViewDir, fixed3(IN.texcoord.zw, IN.mass.w)));
		#endif

		half wFresnel = saturate(fresnel + _V_WIRE_FresnelBias);
		wFresnel = lerp(wFresnel, 1 - wFresnel, _V_WIRE_FresnelInvert);
		
		wFresnel = pow(wFresnel, _V_WIRE_FresnelPow);

		_V_WIRE_Color.a *= wFresnel;
	#endif 

	 
	 
	#ifdef V_WIRE_NO

		//Mask
		#ifdef V_WIRE_DYNAMIC_MASK_ON
			half dynamicMask = V_WIRE_DynamicMask(IN.worldPos);

			#ifdef V_WIRE_DYNAMIC_MASK_BASE_TEX_ON
				half maskMainTexA = lerp(1 - 2 * dynamicMask, 2 * dynamicMask - 1, _V_WIRE_DynamicMaskEffectsBaseTexInvert);

				#ifdef V_WIRE_CUTOUT
					retColor.a = _Cutoff > 0.01 ? retColor.a * maskMainTexA : maskMainTexA;
				#else
					retColor.a *= maskMainTexA;
				#endif
			#endif
		#endif 

	#else 
		#if defined(V_WIRE_LIGHT_ON)
			#ifdef V_WIRE_CUTOUT

				half customAlpha = 1;
				#ifdef V_WIRE_TRANSPARENCY_ON
					customAlpha = tex2D(_V_WIRE_TransparentTex, IN.texcoord1).a;

					customAlpha = lerp(customAlpha, 1 - customAlpha, _V_WIRE_TransparentTex_Invert);

					customAlpha = (customAlpha + _V_WIRE_TransparentTex_Alpha_Offset) < 0.01 ? 0 : 1;
				#endif
				 
				half dynamicMask = 1;
				#ifdef V_WIRE_DYNAMIC_MASK_ON
					dynamicMask = V_WIRE_DynamicMask(IN.worldPos);

					#ifdef V_WIRE_DYNAMIC_MASK_BASE_TEX_ON
						half maskMainTexA = lerp(1 - 2 * dynamicMask, 2 * dynamicMask - 1, _V_WIRE_DynamicMaskEffectsBaseTexInvert);

						retColor.a = _Cutoff > 0.01 ? retColor.a * maskMainTexA : maskMainTexA;
					#endif
				#endif 

				half clipValue = DoWire(retColor, IN.mass, dynamicMask, customAlpha, _Cutoff);


				#ifdef V_WIRE_CUTOUT_HALF
					clip(clipValue - 0.5);
				#else
					clip(clipValue);
				#endif

			#else //V_WIRE_CUTOUT

				#ifdef V_WIRE_TRANSPARENCY_ON
					half customAlpha = tex2D(_V_WIRE_TransparentTex, IN.texcoord1).a;

					customAlpha = lerp(customAlpha, 1 - customAlpha, _V_WIRE_TransparentTex_Invert);

					customAlpha = saturate(customAlpha + _V_WIRE_TransparentTex_Alpha_Offset);


					_V_WIRE_Color.a *= customAlpha;
				#endif
			
				//Mask
				half dynamicMask = 1;
				#ifdef V_WIRE_DYNAMIC_MASK_ON
					dynamicMask = V_WIRE_DynamicMask(IN.worldPos);

					#ifdef V_WIRE_DYNAMIC_MASK_BASE_TEX_ON
						half maskMainTexA = lerp(1 - 2 * dynamicMask, 2 * dynamicMask - 1, _V_WIRE_DynamicMaskEffectsBaseTexInvert);

						retColor.a *= maskMainTexA;
					#endif
				#endif 


				half value = DoWire(retColor, IN.mass, dynamicMask);


				#ifdef V_WIRE_DYNAMIC_MASK_ON
					#ifdef _NORMALMAP
						half3 newNormal = half3(o.Normal.x * value, o.Normal.y * value, o.Normal.z);
						o.Normal = lerp(o.Normal, normalize(newNormal), _V_WIRE_Color.a);
					#endif

					#ifdef V_WIRE_REFLECTION_ON
						o.Emission = lerp(o.Emission, o.Emission * value, _V_WIRE_Color.a);
					#endif
			 
					#ifdef V_WIRE_GLOSS
						o.Gloss *= lerp(o.Gloss, o.Gloss * value, _V_WIRE_Color.a);
					#endif			
				#endif

			#endif //V_WIRE_CUTOUT
		#endif //V_WIRE_LIGHT_ON
	#endif


	o.Albedo = retColor.rgb;
	o.Alpha = retColor.a;  
}

void WireFinalColor (Input IN, SurfaceOutputStandard o, inout fixed4 color)
{	
	#if !defined(V_WIRE_LIGHT_ON) && !defined(V_WIRE_NO)
		#ifdef V_WIRE_CUTOUT
			
			half customAlpha = 1;
			#ifdef V_WIRE_TRANSPARENCY_ON
				customAlpha = tex2D(_V_WIRE_TransparentTex, IN.texcoord1).a;

				customAlpha = lerp(customAlpha, 1 - customAlpha, _V_WIRE_TransparentTex_Invert);
			#endif
				 
			half dynamicMask = 1;
			#ifdef V_WIRE_DYNAMIC_MASK_ON
				dynamicMask = V_WIRE_DynamicMask(IN.worldPos);

				#ifdef V_WIRE_DYNAMIC_MASK_BASE_TEX_ON
					half maskMainTexA = lerp(1 - 2 * dynamicMask, 2 * dynamicMask - 1, _V_WIRE_DynamicMaskEffectsBaseTexInvert);

					color.a = _Cutoff > 0.01 ? color.a * maskMainTexA : maskMainTexA;
				#endif
			#endif 
						
			half clipValue = DoWire(color, IN.mass, dynamicMask, customAlpha, _Cutoff);

			#ifdef V_WIRE_CUTOUT_HALF
				clip(clipValue - 0.5);
			#else
				clip(clipValue);
			#endif

		#else

			#ifdef V_WIRE_TRANSPARENCY_ON
				half customAlpha = tex2D(_V_WIRE_TransparentTex, IN.texcoord1).a;

				_V_WIRE_Color.a *= lerp(customAlpha, 1 - customAlpha, _V_WIRE_TransparentTex_Invert);
			#endif
			
			half dynamicMask = 1;
			#ifdef V_WIRE_DYNAMIC_MASK_ON
				dynamicMask = V_WIRE_DynamicMask(IN.worldPos);

				#ifdef V_WIRE_DYNAMIC_MASK_BASE_TEX_ON
					half maskMainTexA = lerp(1 - 2 * dynamicMask, 2 * dynamicMask - 1, _V_WIRE_DynamicMaskEffectsBaseTexInvert);

					#ifdef V_WIRE_CUTOUT
						color.a = _Cutoff > 0.01 ? retColor.a * maskMainTexA : maskMainTexA;
					#else
						color.a *= maskMainTexA;
					#endif
				#endif
			#endif

			DoWire(color, IN.mass, dynamicMask);
		#endif
	#endif
} 



#endif
