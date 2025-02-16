using UnityEngine;

namespace Code
{
    public class EscapeMenuUI : MonoBehaviour
    {

        [SerializeField] private GameObject escapeMenuCanvas;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (escapeMenuCanvas.activeSelf)
                {
                    CloseEscapeMenu();
                }
                else
                {
                    OpenEscapeMenu();
                }
            }
        }

        public void CloseEscapeMenu()
        {
            escapeMenuCanvas.SetActive(false);
        }

        private void OpenEscapeMenu()
        {
            escapeMenuCanvas.SetActive(true);
        }

    }
}