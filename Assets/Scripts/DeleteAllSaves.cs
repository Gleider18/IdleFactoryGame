using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteAllSaves : MonoBehaviour
{
    [SerializeField] private Button _deleteButton;

    private void Start()
    {
        _deleteButton.onClick.AddListener(DeleteAllSavesFunc);
    }

    private void DeleteAllSavesFunc()
    {
        File.Delete(Application.persistentDataPath + "/" + "playerSave.json");
        File.Delete(Application.persistentDataPath + "/" + "factories_save_data.json");
        SceneManager.LoadScene(0);
    }
}
