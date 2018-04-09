Shader "Unlit/IndicatorShader"
{
	Properties {
        _Color ("Main Color", Color) = (1,0,0,1)
        _Emission ("Emmisive Color", Color) = (0.5,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    SubShader {
        Pass {
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Emission [_Emission]
            }
            Lighting On
            SeparateSpecular On
            SetTexture [_MainTex] {
                constantColor [_Color]
                Combine primary DOUBLE, texture * constant
            }
        }
    }
}
