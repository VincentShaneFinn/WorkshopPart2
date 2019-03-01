using UnityEngine;
using UnityEngine.UI;

namespace Finisher.UI.Meters
{
    public class UI_Meter : MonoBehaviour
    {
        public Color finishableColor;
        private float newFillAmount;
        protected Image meter;
        private Color originalColor; 

        void Awake()
        {
            meter = GetComponent<Image>();
            originalColor = meter.color;
        }

        // Update is called once per frame
        void Update()
        {
            meter.fillAmount = Mathf.Lerp(meter.fillAmount, newFillAmount, Time.deltaTime * 10);
        }

        public void SetFillAmount(float valueAsPercentage)
        {
            newFillAmount = valueAsPercentage;
        }

        public void SetColor(bool isFinishable)
        {
            if (isFinishable) {
                meter.color = finishableColor;
            }
            else
            {
                meter.color = originalColor;
            }
        }

        public void SetFillAmountInstant(float valueAsPercentage)
        {
            SetFillAmount(valueAsPercentage);
            meter.fillAmount = newFillAmount;
        }
    }
}

