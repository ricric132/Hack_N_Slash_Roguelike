Shader "CustomDisableZWrite"
{
    SubShader{
        Tags{
            "RenderType" = "Opaque"
        }

        pass{
            ZWrite Off
        }
    }
}