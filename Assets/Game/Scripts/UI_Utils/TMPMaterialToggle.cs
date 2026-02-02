using TMPro;
using UnityEngine;

namespace Game.Scripts.UI_Utils
{
    [DisallowMultipleComponent]
    public sealed class TMPMaterialToggle : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        [Header("Materials")]
        [SerializeField] private Material falseMaterial;
        [SerializeField] private Material trueMaterial;

        public void SetState(bool value)
        {
            var target = value ? trueMaterial : falseMaterial;

            text.fontMaterial = target;
        }
    }
}