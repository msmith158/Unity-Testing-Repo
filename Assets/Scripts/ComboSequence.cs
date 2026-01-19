using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mitchel.UI
{
    public class ComboSequence : MonoBehaviour
    {
        [Header("Combo List")]
        [SerializeField] private List<ComboList> combos;

        [Header("Object References")]
        [SerializeField] private TextMeshProUGUI comboTextObject;
        [SerializeField] private TextMeshProUGUI plusSpacerObject;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    [System.Serializable]
    public class ComboList
    {
        public KeyCode[] keys;
    }
}