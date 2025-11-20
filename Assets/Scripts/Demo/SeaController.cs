using UnityEngine;

public class SeaController : MonoBehaviour
{
    public Material materialSea;

    const string PROP_SEA_COLOR = "_SeaColor";

    public void ChangeSeaColor(Color color)
    {
        Color startColor = materialSea.GetColor(PROP_SEA_COLOR);
        StartCoroutine(
            ColorUtil.LerpColor(
                startColor, 
                color,
                (c) =>
                {
                    materialSea.SetColor(PROP_SEA_COLOR, c);
                })
            );
        
    }
}
