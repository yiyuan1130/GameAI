using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroupBehaviour { 
    public class GameMain : MonoBehaviour
    {
        public GameObject actorPrefab;
        private void Awake()
        {
            ActorManager.Init();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                for (int i = 0; i < 100; i++)
                {
                    Vector3 createPos = new Vector3(worldPos.x, 0, worldPos.z);
                    createPos += new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-30f, 30f));
                    CreateActor(createPos);
                }
            }
            ActorManager.Update(Time.deltaTime);
        }

        void CreateActor(Vector3 createPos) {
            GameObject actorGO = Instantiate(actorPrefab);
            actorGO.transform.position = createPos;
            ActorManager.CreateActor(actorGO, createPos);
        }
    }
}
