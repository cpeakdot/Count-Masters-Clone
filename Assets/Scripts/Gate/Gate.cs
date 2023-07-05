using UnityEngine;
using TMPro;

namespace CMC
{
    public class Gate : MonoBehaviour
    {
        private GateType gateType;
        [SerializeField] private TMP_Text gateText;
        private int value;
        [SerializeField] private int minValue, maxValue;

        public int GetValue => value;
        public GateType GetGateType => gateType;

        private void Awake() 
        {
            InitGate();
        }

        private void InitGate()
        {
            SetGateType();

            SetGateValue();

            SetGateText();
        }
        private void SetGateType()
        {
            int randomInt = UnityEngine.Random.Range(0, 2);

            gateType = (randomInt == 0) ? GateType.Multiply : GateType.Sum;
        }

        private void SetGateValue()
        {
            value = UnityEngine.Random.Range(minValue, maxValue);
        }

        private void SetGateText()
        {
            string operatorStr = (gateType == GateType.Multiply) ? "x" : "+";

            gateText.text = operatorStr + value;
        }

    }

    public enum GateType
    {
        Multiply,
        Sum
    }
}

