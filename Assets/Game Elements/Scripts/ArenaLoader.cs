using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ArenaLoader : MonoBehaviour
{
    [SerializeField]
    public List<SceneReference> sceneList;

    public int currentSceneIndex;

    // Méthode pour charger une scène par son index
    public void LoadSceneByIndex(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}