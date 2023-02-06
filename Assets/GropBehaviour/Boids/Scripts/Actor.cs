using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroupBehaviour {
    public class Actor
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector3 forward = Vector3.forward;
        public Vector3 position = Vector3.zero;
        public int id;
        Vector3 velocity = Vector3.zero;
        bool isValid = false;

        public Actor(int id, GameObject gameObject, Vector3 createPos) {
            forward = Vector3.forward;
            position = createPos;
            velocity = Vector3.zero;
            this.id = id;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            isValid = true;
        }

        public void Update(float deltaTime)
        {
            if (!isValid)
                return;
            Vector3 f_velocity = FollowMouse(deltaTime);
            Vector3 s_velocity = Separation(deltaTime);
            Vector3 a_velocity = Alignment(deltaTime);
            Vector3 c_velocity = Cohesion(deltaTime);

            velocity = (f_velocity + s_velocity + a_velocity + c_velocity) / 4f;

            transform.forward = velocity.normalized;
            if (velocity.magnitude < GroupParams.minMoveSpeed) {
                velocity = velocity.normalized * GroupParams.minMoveSpeed;
            }
            if (velocity.magnitude > GroupParams.maxMoveSpeed)
            {
                velocity = velocity.normalized * GroupParams.maxMoveSpeed;
            }
            position += velocity * deltaTime;
            transform.position = position;
        }

        Vector3 FollowMouse(float deltaTime) {
            if (Input.GetMouseButton(1))
            {
                Vector3 tarPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                Vector3 tarVelocity = new Vector3(tarPos.x, 0, tarPos.z) - transform.position;
                Vector3 dir = Vector3.Lerp(velocity, tarVelocity.normalized, 0.5f);
                return dir;
            }
            return Vector3.zero;
        }

        // 分离
        Vector3 Separation(float deltaTime) {
            List<Actor> actors = ActorManager.GetActorsByPointRaduis(this.position, GroupParams.viewRaduis);
            if (actors == null)
            {
                return velocity;
            }
            int count = 0;
            Vector3 sperationVelocity = Vector3.zero;
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                if (actor.id != this.id) {
                    if (Util.GetDistanceXZSquare(this.position, actor.position) <= GroupParams.separationRadius * GroupParams.separationRadius) {
                        Vector3 vec = this.position - actor.position;
                        sperationVelocity += vec;
                        count++;
                    }
                }
            }
            if (count > 0) {
                Vector3 avgVelocity = sperationVelocity / count;
                return Vector3.Lerp(velocity, avgVelocity, GroupParams.spearationRate);
            }

            return velocity;
        }


        // 趋向一致
        Vector3 Alignment(float deltaTime) {
            List<Actor> actors = ActorManager.GetActorsByPointRaduis(this.position, GroupParams.viewRaduis);
            if (actors == null) {
                return Vector3.zero;
            }
            Vector3 sumVelocity = Vector3.zero;
            int count = 0;
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                sumVelocity += actor.velocity;
                count++;
            }
            if (count > 0) { 
                Vector3 avgVelocity = sumVelocity / count;
                return Vector3.Lerp(velocity, avgVelocity, GroupParams.alignmentRate);
            }
            return velocity;
        }

        // 聚集
        Vector3 Cohesion(float deltaTime) {
            List<Actor> actors = ActorManager.GetActorsByPointRaduis(this.position, GroupParams.viewRaduis);
            if (actors == null)
            {
                return Vector3.zero;
            }
            int count = 0;
            Vector3 sumPos = Vector3.zero;
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                sumPos += actor.position;
            }
            if (count > 0)
            {
                Vector3 centerPos = sumPos / count;
                Vector3 tarVelocity = (centerPos - this.position);
                return Vector3.Lerp(velocity, tarVelocity, GroupParams.cohesionRate);
            }
            return velocity;
        }
    }
}
