﻿Shader "Hidden/Blur"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_kernelAWidth("Kernel Width", Float) = 1
		_kernelBWidth("Kernel Width", Float) = 1
		_stepThreshold("Step Threshold", Float) = 0.5
		_cellDifference("Cell Difference", Float) = 0.98
		_cellSharpness("Cell Sharpness", Float) = 0.75
		_edgeIntensity("Edge Intensity", Range(0,1)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define PI 3.141592653589793238462643383279502884197169399375105820974944592307816406286
			#define SIGMA 0.4

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform int _kernelAWidth;
			uniform int _kernelBWidth;
			uniform float _stepThreshold;
			uniform float _cellDifference;
			uniform float _cellSharpness;
			uniform float _edgeIntensity;

			sampler2D _CameraDepthNormalsTexture;
			sampler2D_float _CameraDepthTexture;

			float Gauss(float x, float propagationRate)
			{
				float a = 1 / (propagationRate * sqrt(2 * PI));
				float b = 0;
				float c = propagationRate;

				return a * exp(-pow(x - b, 2) / 2 * pow(c, 2));
			}

			float remap(float value, float l1, float h1, float l2, float h2) {
				return l2 + (value - l1) * (h2 - l2) / (h1 - l1);
			}

			float DoG(float a, float b, float t) {
				return a - t * b;
			}

			float DoGSharpen(float DoG, float t) {
				return DoG / (t - 1);
			}

			float DoGThreshold(float SharpenedDoG, float sT, float cS) {
				if (SharpenedDoG >= sT) return 1;
				else return 1 + tanh(cS*(SharpenedDoG - sT));
			}

			float4 frag(v2f_img i) : COLOR
			{
				float4 finalColor, blurA, blurB = float4(0,0,0,0);

				int kernelASize = _kernelAWidth + (1 - fmod(_kernelAWidth, 2));
				int kernelBSize = _kernelBWidth + (1 - fmod(_kernelBWidth, 2));

				for (int y = -(kernelASize-1)/2; y <= (kernelASize - 1) / 2; y++)
				{
					for (int x = -(kernelASize - 1) / 2; x <= (kernelASize - 1) / 2; x++) {

						float2 texCoords = float2(i.uv.x + x * _MainTex_TexelSize.x, i.uv.y + y * _MainTex_TexelSize.y);
						blurA += Gauss(length(texCoords - i.uv), SIGMA) * tex2D(_MainTex, texCoords);
						//blurA += 3 * SIGMA / Gauss(length(texCoords - i.uv), SIGMA) * Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv + texCoords));
					}
				}

				for (int y2 = -(kernelBSize - 1); y2 <= (kernelBSize - 1); y2++)
				{
					for (int x2 = -(kernelBSize - 1); x2 <= (kernelBSize - 1); x2++) {

						float2 texCoords = float2(i.uv.x + x2 * _MainTex_TexelSize.x, i.uv.y + y2  * _MainTex_TexelSize.y);
						blurB += Gauss(length(texCoords - i.uv), SIGMA) * tex2D(_MainTex, texCoords);
					}
				}

				blurA = float4(remap(blurA.x, 0, pow(kernelASize, 2) * Gauss(0, SIGMA), 0, 1), remap(blurA.y, 0, pow(kernelASize, 2) *  Gauss(0, SIGMA), 0, 1), remap(blurA.z, 0, pow(kernelASize, 2) *  Gauss(0, SIGMA), 0, 1), 1);
				blurB = float4(remap(blurB.x, 0, pow(kernelBSize * 2, 2) *  Gauss(0, SIGMA), 0, 1), remap(blurB.y, 0, pow(kernelBSize * 2, 2) *  Gauss(0, SIGMA), 0, 1), remap(blurB.z, 0, pow(kernelBSize * 2, 2) * Gauss(0, SIGMA), 0, 1), 1);
				
				//float4 DoG = 1 - step(min(min(blurA.r, blurA.g), blurA.b) - min(min(blurB.r, blurB.g), blurB.b), _stepThreshold);

				//finalblur = min(min(blurA.r, blurA.g),blurA.b) - min(min(blurB.r, blurB.g), blurB.b);

				//e = theshold value
				float lumA = 0.3*blurA.r + 0.59*blurA.g + 0.11*blurA.b;
				float lumB = 0.3*blurB.r + 0.59*blurB.g + 0.11*blurB.b;

				float d = DoG(lumA, lumB, _cellDifference);
				float ds = DoGSharpen(d, _cellDifference);

				finalColor = DoGThreshold(ds, _stepThreshold, _cellSharpness);

				return lerp(float4(1, 1, 1, 1), /*blurB*/ float4(0, 0, 0, 1), lerp(0, finalColor, _edgeIntensity)) *tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
