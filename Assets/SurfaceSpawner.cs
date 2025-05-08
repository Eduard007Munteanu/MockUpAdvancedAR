// using Meta.XR.MRUtilityKit;
// using UnityEngine;
// using System.Collections.Generic;

// public class SurfaceSpawner : MonoBehaviour
// {
//     public GameObject prefabToSpawn;

//     void OnEnable()
//     {
//         MRUK.Instance.SceneLoadedEvent.AddListener(() =>
//         {
//             OnSceneLoaded(MRUK.Instance.GetAllSceneObjects());
//         });
//     }

//     void OnDisable()
//     {
//         MRUK.Instance.SceneLoadedEvent.RemoveAllListeners(); // safer than RemoveListener with lambda
//     }

//     void OnSceneLoaded(List<MRUKSceneObject> sceneObjects)
//     {
//         foreach (var obj in sceneObjects)
//         {
//             foreach (var label in obj.GetLabels())
//             {
//                 Debug.Log("Label: " + label);

//                 if (label.ToUpper().Contains("FLOOR")) // or "TABLE"
//                 {
//                     GameObject instance = Instantiate(prefabToSpawn);
//                     instance.transform.position = obj.transform.position + Vector3.up * 0.05f;
//                     return;
//                 }
//             }
//         }

//         Debug.LogWarning("No FLOOR label found in scene.");
//     }
// }
