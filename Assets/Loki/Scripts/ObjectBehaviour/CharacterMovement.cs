using System;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization;
using Grandora.Behaviour;
using Grandora.GameInput;
using Grandora.Manager;
using UniRx;
using UnityEngine;
using YGG;

namespace Grandora.Character
{
    public class CharacterMovement : ObjectBehaviour
    {
        [SerializeField] PlayerInputSystem _input;
        Vector3 direction;

        [SerializeField]
        Transform playerInputSpace = default;
        [SerializeField]
        bool steepJumping = false;

        [SerializeField, Range(0f, 100f)]
        public float WalkSpeed = 2f, RunSpeed = 10f;

        [SerializeField, Range(0f, 100f)]
        public float maxSpeed = 10f, maxClimbSpeed = 4f, maxSwimSpeed = 5f;
        float currentJumpSpeed;
        public float currentSpeed;

        [SerializeField, Range(0f, 100f)]
        float
            maxAcceleration = 10f,
            maxAirAcceleration = 1f,
            maxClimbAcceleration = 40f,
            maxSwimAcceleration = 5f;

        [SerializeField, Range(0f, 10f)]
        float jumpHeight = 2f;

        [SerializeField, Range(0, 5)]
        int maxAirJumps = 0;

        [SerializeField, Range(0, 270)]
        float maxGroundAngle = 25f, maxStairsAngle = 50f;

        [SerializeField, Range(90, 170)]
        float maxClimbAngle = 140f;

        [SerializeField, Range(0f, 100f)]
        float maxSnapSpeed = 100f;

        [SerializeField, Min(0f)]
        float probeDistance = 1f;

        [SerializeField]
        float submergenceOffset = 0.5f;

        [SerializeField, Min(0.1f)]
        float submergenceRange = 1f;

        [SerializeField, Min(0f)]
        float buoyancy = 1f;

        [SerializeField, Range(0f, 10f)]
        float waterDrag = 1f;

        [SerializeField, Range(0.01f, 1f)]
        float swimThreshold = 0.5f;

        [SerializeField]
        LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0;

        [SerializeField]
        Material
            normalMaterial = default,
            climbingMaterial = default,
            swimmingMaterial = default;

        [SerializeField, Min(0.1f)]
        float radius = 0.5f;

        [SerializeField, Min(0f)]
        float alignSpeed = 180f;

        [SerializeField, Min(0f)]
        float
            airRotation = 0.5f,
            swimRotation = 2f;
        [SerializeField, Min(0f)]
        float reviveDuration = 1f;

        Rigidbody body, connectedBody, previousConnectedBody;

        Vector3 playerInput;

        [SerializeField]Vector3 velocity, connectionVelocity;

        Vector3 connectionWorldPosition, connectionLocalPosition;

        [SerializeField] Vector3 upAxis, rightAxis, forwardAxis;

        bool desiredJump, desiresClimbing;

        Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

        Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

        public int groundContactCount, steepContactCount, climbContactCount;

        bool OnGround => groundContactCount > 0;

        bool OnSteep => steepContactCount > 0;

        bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

        bool InWater => submergence > 0f;

        bool Swimming => submergence >= swimThreshold;

        float submergence;

        int jumpPhase;

        float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

        int stepsSinceLastGrounded, stepsSinceLastJump;
        //extend
        bool canMove = true;
        private bool isStunned = false;
        private bool wasStunned = false;
        private bool isKnockDown = false;
        private bool wasKnockDown = false;
        private bool isRevive = false;
        private bool slide = false;
        Vector3 gravity;

        [SerializeField] MeshRenderer meshRenderer;



        public void PreventSnapToGround()
        {
            stepsSinceLastJump = -1;
        }
        public void SetRigidBodyInterPolation(RigidbodyInterpolation interpolation)
        {
            body.interpolation = interpolation;
        }
        void OnValidate()
        {
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
            minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
        }
        public void SetupInput(PlayerInputSystem _inputSystem)
        {
            _input = _inputSystem;
            // _input.shift.ObserveEveryValueChanged(m => m.Value).Subscribe(_ =>
            // {
            //     if (jumpPhase <= 0)
            //         maxSpeed = _ ? RunSpeed : WalkSpeed;
            // }).AddTo(this);
            // this.ObserveEveryValueChanged(i => i.jumpPhase).Subscribe(_ =>
            // {
            //     if (_ <= 0)
            //         maxSpeed = _input.shift.Value ? RunSpeed : WalkSpeed;
            // }).AddTo(this);
            _input.movement.ObserveEveryValueChanged(m => m.Value).Subscribe(_ =>
            {
                //_actionStatus.Value.isMove = _.normalized.magnitude > 0;
                MovementHandle(_);
            }).AddTo(this);
        }
        public void SetupCameraTransform(Transform cameraTransform)
        {
            playerInputSpace = cameraTransform;
        }
        // public void SetupActionStatus(ReactiveProperty<CharacterActionStatus> actionStatus)
        // {
        //     _actionStatus = actionStatus;
        // }
        // public void SetStatusEffect(ReactiveDictionary<string, StatusEffectData> statusEffect)
        // {
        //     _statusEffects = statusEffect;
        // }
        // public void StartLocalNetworkController(NetworkBehaviour clientNetwork)
        // {
        //     print("startLocalNetworkController");
        //     networkBehaviour = clientNetwork;
        //     //_input.EnableInput(true);
        //     AddListener();
        //     RegisterObject();
        // }
        public override void AddListener()
        {
            base.AddListener();
            // this.ObserveEveryValueChanged(c => c.isStunned).Subscribe(wasStuned =>{
            // 	_actionStatus.Value.isStunned = wasStuned;
            // }).AddTo(this);
            // this.ObserveEveryValueChanged(c =>c.isKnockDown).Subscribe(wasKnockDown =>{
            // 	_actionStatus.Value.isKnockDown = wasKnockDown;
            // }).AddTo(this);
        }
        public void MovementHandle(Vector2 inputMovement)
        {
            if (isStunned || isKnockDown || isRevive) return;
            direction = new Vector3(inputMovement.x, 0, inputMovement.y);
        }

        public override void OnCreate()
        {
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            OnValidate();
            base.OnCreate();
        }
        public void StopMovement()
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
            velocity = Vector3.zero;
        }
        public void StartMovement()
        {
            GetComponent<CapsuleCollider>().isTrigger = false;
            //RegisterObject();
        }
        public void FreezeCollider()
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
            body.useGravity = false;
            body.isKinematic = true;
            canMove = false;
            velocity = Vector3.zero;
        }
        public void UnFreezeCollider()
        {
            GetComponent<CapsuleCollider>().isTrigger = false;
            body.useGravity = true;
            body.isKinematic = false;
            canMove = true;
            //RegisterObject();
        }
        public override void OnInActive()
        {
            base.OnInActive();
        }
        public override void OnActive()
        {
            base.OnActive();
            GetComponent<CapsuleCollider>().isTrigger = false;
            body.useGravity = true;
        }
        public override void OnUpdate()
        {
            if (_input == null || !canMove) return;
            playerInput.x = _input.movement.Value.x;
            playerInput.z = _input.movement.Value.y;
            playerInput.y = 0;
            playerInput = Vector3.ClampMagnitude(playerInput, 1f);
           // _actionStatus.Value.isMove = playerInput.magnitude > 0;
            if (playerInputSpace)
            {
                rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
                forwardAxis =
                    ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
            }
            else
            {
                rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
                forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
            }

            if (Swimming)
            {
                desiresClimbing = false;
            }
            else
            {
                if (isStunned || isKnockDown || isRevive) return;
                //desiredJump |= _input.jump.Value;
                //desiresClimbing = _input.climb.Value;
            }
            UpdateRotation();
        }
        void UpdateRotation()
        {
            Vector3 targetDir = Vector3.zero;
            if (playerInput == Vector3.zero)
            {
                targetDir = transform.forward;
            }
            else
            {
                targetDir = (playerInputSpace.forward * playerInput.z) + (playerInputSpace.right * playerInput.x);
                targetDir.Normalize();
                targetDir.y = 0;
            }
            Quaternion rotation = Quaternion.LookRotation(targetDir);
            Quaternion tergetRotation = Quaternion.Slerp(transform.rotation, rotation, 5 * Time.deltaTime);
            transform.rotation = tergetRotation;
        }

        public override void OnFixedUpdate()
        {
            gravity = CustomGravity.GetGravity(transform.position, out upAxis);
            UpdateState();

            if (InWater)
            {
                velocity *= 1f - waterDrag * submergence * Time.deltaTime;
            }
            if (isStunned || isKnockDown || isRevive) return;
            AdjustVelocity();
            if (desiredJump)
            {
                desiredJump = false;
               // _input.jump.Value = false;
                Jump(gravity);
            }

            if (Climbing)
            {
                velocity -=
                    contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
            }
            else if (InWater)
            {
                velocity +=
                    gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
            }
            else if (OnGround && velocity.sqrMagnitude < 0.01f)
            {
                velocity +=
                    contactNormal *
                    (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
            }
            else if (desiresClimbing && OnGround)
            {
                velocity +=
                    (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) *
                    Time.deltaTime;
            }
            else
            {
                velocity += gravity * Time.deltaTime;
            }
            body.velocity = velocity;
            ClearState();
        }

        void ClearState()
        {
            lastContactNormal = contactNormal;
            lastSteepNormal = steepNormal;
            lastConnectionVelocity = connectionVelocity;
            groundContactCount = steepContactCount = climbContactCount = 0;
            contactNormal = steepNormal = climbNormal = Vector3.zero;
            connectionVelocity = Vector3.zero;
            previousConnectedBody = connectedBody;
            connectedBody = null;
            submergence = 0f;
        }

        void UpdateState()
        {
            stepsSinceLastGrounded += 1;
            stepsSinceLastJump += 1;
            velocity = body.velocity;
            if (
                CheckClimbing() || CheckSwimming() ||
                OnGround || SnapToGround() || CheckSteepContacts()
            )
            {
                stepsSinceLastGrounded = 0;
                if (stepsSinceLastJump > 1)
                {
                    jumpPhase = 0;
                }
                if (groundContactCount > 1)
                {
                    contactNormal.Normalize();
                }
            }
            else
            {
                contactNormal = upAxis;
            }

            if (connectedBody)
            {
                if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
                {
                    UpdateConnectionState();
                }
            }
            // _actionStatus.Value.inWater = Swimming;
            // _actionStatus.Value.isJump = !OnGround && !_actionStatus.Value.inWater;
            // _actionStatus.Value.isAir = !OnGround && !_actionStatus.Value.inWater;
            // _actionStatus.Value.jumpCount = jumpPhase;
        }

        void UpdateConnectionState()
        {
            if (connectedBody == previousConnectedBody)
            {
                Vector3 connectionMovement =
                    connectedBody.transform.TransformPoint(connectionLocalPosition) -
                    connectionWorldPosition;
                connectionVelocity = connectionMovement / Time.deltaTime;
            }
            connectionWorldPosition = body.position;
            connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
                connectionWorldPosition
            );
        }

        bool CheckClimbing()
        {
            if (Climbing)
            {
                if (climbContactCount > 1)
                {
                    climbNormal.Normalize();
                    float upDot = Vector3.Dot(upAxis, climbNormal);
                    if (upDot >= minGroundDotProduct)
                    {
                        climbNormal = lastClimbNormal;
                    }
                }
                groundContactCount = 1;
                contactNormal = climbNormal;
                return true;
            }
            return false;
        }

        bool CheckSwimming()
        {
            if (Swimming)
            {
                groundContactCount = 0;
                contactNormal = upAxis;
                return true;
            }
            return false;
        }

        bool SnapToGround()
        {
            if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater)
            {
                return false;
            }
            float speed = velocity.magnitude;
            if (speed > maxSnapSpeed)
            {
                return false;
            }
            if (!Physics.Raycast(
                body.position, -upAxis, out RaycastHit hit,
                probeDistance, probeMask, QueryTriggerInteraction.Ignore
            ))
            {
                return false;
            }

            float upDot = Vector3.Dot(upAxis, hit.normal);
            if (upDot < GetMinDot(hit.collider.gameObject.layer))
            {
                return false;
            }

            groundContactCount = 1;
            contactNormal = hit.normal;
            float dot = Vector3.Dot(velocity, hit.normal);
            if (dot > 0f)
            {
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectedBody = hit.rigidbody;
            return true;
        }

        bool CheckSteepContacts()
        {
            if (steepContactCount > 1)
            {
                steepNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, steepNormal);
                if (upDot >= minGroundDotProduct)
                {
                    steepContactCount = 0;
                    groundContactCount = 1;
                    contactNormal = steepNormal;
                    return true;
                }
            }
            return false;
        }

        void AdjustVelocity()
        {
            float acceleration, speed;
            Vector3 xAxis, zAxis;
            if (Climbing)
            {
                acceleration = maxClimbAcceleration;
                speed = maxClimbSpeed;
                xAxis = Vector3.Cross(contactNormal, upAxis);
                zAxis = upAxis;
            }
            else if (InWater)
            {
                float swimFactor = Mathf.Min(1f, submergence / swimThreshold);
                acceleration = Mathf.LerpUnclamped(
                    OnGround ? maxAcceleration : maxAirAcceleration,
                    maxSwimAcceleration, swimFactor
                );
                speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor);
                xAxis = rightAxis;
                zAxis = forwardAxis;
            }
            else
            {
                acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
                speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed;
                xAxis = rightAxis;
                zAxis = forwardAxis;
            }
            xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
            zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;

            Vector3 adjustment;
            adjustment.x =
                playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
            adjustment.z =
                playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
            adjustment.y = Swimming ?
                playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

            adjustment =
                Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);
            velocity += xAxis * adjustment.x + zAxis * adjustment.z;
            if (Swimming)
            {
                velocity += upAxis * adjustment.y;
            }
        }

        void Jump(Vector3 gravity)
        {
            Vector3 jumpDirection;
            if (OnGround)
            {
                jumpDirection = contactNormal;
            }
            else if (OnSteep && steepJumping)
            {
                jumpDirection = steepNormal;
                jumpPhase = 0;
            }
            else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)
            {
                if (jumpPhase == 0)
                {
                    jumpPhase = 1;
                }
                jumpDirection = contactNormal;
            }
            else
            {
                return;
            }

            stepsSinceLastJump = 0;
            jumpPhase += 1;
            // if (jumpPhase > 1)
            // {
            //     _actionStatus.Value.isDoubleJump = false;
            // }
            float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
            if (InWater)
            {
                jumpSpeed *= Mathf.Max(0f, 1f - submergence / swimThreshold);
            }
            jumpDirection = (jumpDirection + upAxis).normalized;
            float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            if (velocity.y <= 0)
                velocity.y = 0;
            velocity += jumpDirection * jumpSpeed;
        }

        void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void EvaluateCollision(Collision collision)
        {
            if (Swimming)
            {
                return;
            }
            int layer = collision.gameObject.layer;

            float minDot = GetMinDot(layer);
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(upAxis, normal);
                if (upDot >= minDot)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                    connectedBody = collision.rigidbody;
                }
                else
                {
                    if (upDot > -0.01f)
                    {
                        steepContactCount += 1;
                        steepNormal += normal;
                        if (groundContactCount == 0)
                        {
                            connectedBody = collision.rigidbody;
                        }
                    }
                    if (
                        desiresClimbing && upDot >= minClimbDotProduct &&
                        (climbMask & (1 << layer)) != 0
                    )
                    {
                        climbContactCount += 1;
                        climbNormal += normal;
                        lastClimbNormal = normal;
                        connectedBody = collision.rigidbody;
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if ((waterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergence(other);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if ((waterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergence(other);
            }
        }

        void EvaluateSubmergence(Collider collider)
        {
            if (Physics.Raycast(
                body.position + upAxis * submergenceOffset,
                -upAxis, out RaycastHit hit, submergenceRange + 1f,
                waterMask, QueryTriggerInteraction.Collide
            ))
            {
                submergence = 1f - hit.distance / submergenceRange;
            }
            else
            {
                submergence = 1f;
            }
            if (Swimming)
            {
                connectedBody = collider.attachedRigidbody;
            }
        }

        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        float GetMinDot(int layer)
        {
            return (stairsMask & (1 << layer)) == 0 ?
                minGroundDotProduct : minStairsDotProduct;
        }
        //extend
        private float pushForce;
        // public void HitPlayer(Vector3 velocityF)
        // {
        //     var findStatus = GameUtils.FindStatusEffectDataByEffectType(_statusEffects, StatusEffectType.SuperArmor);
        //     if (findStatus.IsNotNull()) return;
        //     body.velocity = Vector3.zero;
        //     Decrease(velocityF);
        // }
        // void Decrease(Vector3 velocityF)
        // {
        //     body.AddForce(velocityF, ForceMode.Impulse);
        // }

       // public override OnobjectBehaviour
       public override void OnObjectBehaviourAdded(ObjectBehaviour obj)
       {
            switch(obj)
            {
                case PlayerInputSystem input:
                    SetupInput(input);
                break;
                case CharacterLocalOB characterLocalOB:
                    SetupCameraTransform(characterLocalOB.CameraRoot);  
                break;
            }
       }
    }
}
