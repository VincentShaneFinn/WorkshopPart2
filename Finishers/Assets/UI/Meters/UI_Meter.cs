using UnityEngine;
using UnityEngine.UI;

namespace Finisher.UI.Meters
{
    public class UI_Meter : MonoBehaviour
    {
        private float newFillAmount;
        protected Image meter;

        void Start()
        {
            meter = GetComponent<Image>();
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

        public void SetFillAmountInstant(float valueAsPercentage)
        {
            SetFillAmount(valueAsPercentage);
            meter.fillAmount = newFillAmount;
        }
    }
}

