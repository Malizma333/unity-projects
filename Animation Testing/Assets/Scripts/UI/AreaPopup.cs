using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AreaPopup : MonoBehaviour
{
    public static AreaPopup Instance;

    [SerializeField] private Image popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;

    private Color textC, panelC;
    private float originalAlpha;

    private void Start()
    {
        Instance = this;
    }

    public IEnumerator Display()
    {
        popupPanel.gameObject.SetActive(true);
        popupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        panelC = popupPanel.color;
        textC = popupText.color;

        originalAlpha = popupPanel.color.a;

        for (float i = 1; i >= 0; i -= 0.01f)
        {
            panelC.a = originalAlpha * i;
            textC.a = i;

            popupPanel.color = panelC;
            popupText.color = textC;

            yield return new WaitForSeconds(0.01f);
        }

        popupPanel.gameObject.SetActive(false);
        popupText.gameObject.SetActive(false);

        panelC.a = originalAlpha;
        textC.a = 1;

        popupPanel.color = panelC;
        popupText.color = textC;
    }
}
