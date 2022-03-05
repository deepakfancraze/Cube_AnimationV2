using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchController : MonoBehaviour
{
    [SerializeField] Material glitchMaterial;
    [SerializeField] Texture2D commonLogo;
    [SerializeField] Texture2D rareLogo;
    [SerializeField] Texture2D epicLogo;
    [SerializeField] Texture2D legendaryLogo;
    [SerializeField] Texture2D genisisLogo;
    [SerializeField] Texture2D platinumLogo;

    public void SetGlitchlogo(CubeRarityType cubeRarityType)
    {
        switch(cubeRarityType)
        {
            case CubeRarityType.common:
                SetImageInMat(commonLogo);
                break;
            case CubeRarityType.rare:
                SetImageInMat(rareLogo);
                break;
            case CubeRarityType.epic:
                SetImageInMat(epicLogo);
                break;
            case CubeRarityType.legendary:
                SetImageInMat(legendaryLogo);
                break;
            case CubeRarityType.genesis:
                SetImageInMat(genisisLogo);
                break;
            case CubeRarityType.platinum:
                SetImageInMat(platinumLogo);
                break;

        }
    }

    void SetImageInMat(Texture2D glitchLogo)
    {
        glitchMaterial.SetTexture("logo", glitchLogo);
    }
}
