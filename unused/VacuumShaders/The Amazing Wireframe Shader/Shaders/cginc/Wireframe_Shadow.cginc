// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef VACUUM_WIREFRAME_SHADOW_CGINC
#define VACUUM_WIREFRAME_SHADOW_CGINC


//Variables//////////////////////////////////

#ifdef V_WIRE_CUTOUT
	half _Cutoff; 

	fixed4 _Color;
	sampler2D _MainTex;
	half4 _MainTex_ST;
	half2 _V_WIRE_MainTex_Scroll;
#endif

#include "UnityCG.cginc"
#include "../cginc/Wireframe_Core.cginc"


////////////////////////////////////////////////////////////////////////////
//																		  //
//Struct    															  //
//																		  //
////////////////////////////////////////////////////////////z////////////////
struct v2f 
{ 
	V2F_SHADOW_CASTER;	
			
	
	#ifdef V_WIRE_CUTOUT
		float4 uv : TEXCOORD1;	
		half3 mass : TEXCOORD2;

		#ifdef V_WIRE_DYNAMIC_MASK_ON
			half3 maskPos : TEXCOORD3;
		#endif
	#endif
};

 
////////////////////////////////////////////////////////////////////////////
//																		  //
//Vertex    															  //
//																		  //
////////////////////////////////////////////////////////////z////////////////
v2f vert( appdata_full v )
{
	v2f o = (v2f)0;
	

//Curved World Compatibility
//V_CW_TransformPointAndNormal(v.vertex, v.normal, v.tangent);


	#ifdef V_WIRE_CUTOUT
		o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
		o.uv.xy += _V_WIRE_MainTex_Scroll.xy * _Time.x;

		#ifdef V_WIRE_TRANSPARENCY_ON
			o.uv.zw = TRANSFORM_TEX(((_V_WIRE_TransparentTex_UVSet == 0) ? v.texcoord.xy : v.texcoord1.xy), _V_WIRE_TransparentTex);
			o.uv.zw += _V_WIRE_TransparentTex_Scroll.xy * _Time.x;
		#endif

		o.mass = ExtructWireframeFromVertexUV(v.texcoord);

		#ifdef V_WIRE_DYNAMIC_MASK_ON		
			o.maskPos = mul(unity_ObjectToWorld, half4(v.vertex.xyz, 1)).xyz;
		#endif
	#endif

	TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
	return o;
}

////////////////////////////////////////////////////////////////////////////
//																		  //
//Fragment    															  //
//																		  //
////////////////////////////////////////////////////////////////////////////
float4 frag( v2f i ) : SV_Target
{
	#ifdef V_WIRE_CUTOUT

		#if defined(V_WIRE_NO_COLOR_BLACK)
			fixed4 retColor = 0;
		#elif defined(V_WIRE_NO_COLOR_WHITE)
			fixed4 retColor = 1;
		#else
			fixed4 retColor = tex2D (_MainTex, i.uv.xy) * _Color;	
		#endif	

							
		//Dynamic Mask
		half dynamicMask = 1;
		#ifdef V_WIRE_DYNAMIC_MASK_ON	
			dynamicMask = V_WIRE_DynamicMask(i.maskPos);
					  
				 
			#ifdef V_WIRE_DYNAMIC_MASK_BASE_TEX_ON
				half maskMainTexA = lerp(1 - 2 * dynamicMask, 2 * dynamicMask - 1, _V_WIRE_DynamicMaskEffectsBaseTexInvert);
						 
				retColor.a = _Cutoff > 0.01 ? retColor.a * maskMainTexA : maskMainTexA;
			#endif  
		#endif  
					 
		half customAlpha = 1;
		#ifdef V_WIRE_TRANSPARENCY_ON
			customAlpha = tex2D(_V_WIRE_TransparentTex, i.uv.zw).a;
					
			customAlpha = lerp(customAlpha, 1 - customAlpha, _V_WIRE_TransparentTex_Invert);

			customAlpha = (customAlpha + _V_WIRE_TransparentTex_Alpha_Offset) < 0.01 ? 0 : 1;
		#endif

		half clipValue = DoWire(retColor, i.mass, dynamicMask, customAlpha, _Cutoff);


		#ifdef V_WIRE_CUTOUT_HALF
			clip(clipValue - 0.5);
		#else
			clip(clipValue); 
		#endif
	#endif

	SHADOW_CASTER_FRAGMENT(i)
}
#endif 
