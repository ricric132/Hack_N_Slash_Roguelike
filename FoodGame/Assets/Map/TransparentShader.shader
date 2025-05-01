Shader "Custom/TransparentShader"
{
     Properties{
        [Header(Surface options)]
        [MainTexture] _ColorMap("Color", 2D) = "white" {}
        [MainColor] _ColorTint("Tint", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Float) = 0

        [HideInInspector] _SourceBlend("Source blend", Float) = 0
        [HideInInspector] _DestBlend("Destination blend", Float) = 0
        [HideInInspector] _ZWrite("ZWrite", Float) = 0
        
        [HideInInspector] _SurfaceType("Surface type", Float) = 0
    }
    SubShader {
        Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}

        Pass {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SourceBlend][_DestBlend]
            ZWrite[_ZWrite]
        }

        Pass {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ColorMask 0
        }
    }
    
}
