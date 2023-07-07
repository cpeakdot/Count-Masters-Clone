using UnityEngine;
using TMPro;

namespace CMC
{
    public class Gate : MonoBehaviour
    {
        private GateType gateType;
        [SerializeField] private Collider coll;
        [SerializeField] private TMP_Text gateText;
        private int value;
        [SerializeField] private int minValue, maxValue;
        [SerializeField] private Vector3 nearbyGateDetectionSize;
        [SerializeField] private LayerMask gateLayerMask;
        public int GetValue => value;
        public GateType GetGateType => gateType;
        private Gate nearbyGate;

        private void Awake() 
        {
            InitGate();
        }

        private void InitGate()
        {
            SetGateType();

            SetGateValue();

            SetGateText();

            TryGetNearbyGate();
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

        private void TryGetNearbyGate()
        {
            Collider[] gatesArray = Physics.OverlapBox(
                transform.position, 
                nearbyGateDetectionSize / 2, 
                this.transform.rotation, 
                gateLayerMask, 
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < gatesArray.Length; i++)
            {
                if (!gatesArray[i].TryGetComponent(out Gate gate) || gate == this) { continue; }
                
                nearbyGate = gate;
                break;
            }
        }

        public void HandleOnTriggered(bool triggerNearbyGate = true)
        {
            coll.enabled = false;

            if (nearbyGate == null || !triggerNearbyGate) { return; }

            nearbyGate.HandleOnTriggered(false);
        }

        private void OnDrawGizmosSelected() 
        {
            Gizmos.DrawWireCube(transform.position, nearbyGateDetectionSize);
        }

    }

    public enum GateType
    {
        Multiply,
        Sum
    }
}

