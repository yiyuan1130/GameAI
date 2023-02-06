using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroupBehaviour { 
    public static class ActorManager
    {
        static Dictionary<int, Actor> actorDict;
        static List<int> delActorList;
        static int actorId = 0;
        public static void Init() {
            actorDict = new Dictionary<int, Actor>();
            delActorList = new List<int>();
        }

        public static void Update(float deltaTime) {
            for (int i = delActorList.Count - 1; i >= 0; i--)
            {
                int removeId = delActorList[i];
                delActorList.RemoveAt(i);
                if (actorDict.ContainsKey(removeId)) {
                    actorDict.Remove(removeId);
                }
            }

            foreach (var item in actorDict)
            {
                int id = item.Key;
                Actor actor = item.Value;
                actor.Update(deltaTime);
            }
        }

        public static Actor CreateActor(GameObject actorGO, Vector3 createPos) {
            actorGO.SetActive(true);
            int id = GetActorInstanceId();
            actorGO.name = "Actor_" + id;
            Actor actor = new Actor(id, actorGO, createPos);
            actorDict.Add(id, actor);
            return actor;
        }

        public static void RemoveActor(Actor actor) {
            delActorList.Add(actor.id);
        }

        static int GetActorInstanceId() {
            actorId++;
            return actorId;
        }

        public static Dictionary<int, Actor> GetActorDictionary() {
            return actorDict;
        }

        public static List<Actor> GetActorsByPointRaduis(Vector3 point, float radius) {
            List<Actor> result = new List<Actor>();
            foreach (var item in actorDict)
            {
                Actor actor = item.Value;
                if (Util.GetDistanceXZSquare(point, actor.position) <= radius * radius) {
                    result.Add(actor);
                }
            }
            return result;
        }
    }
}
