using Godot;
using System;

namespace LostWisps.Object
{
    public partial class LightSource : RayCast2D
    {
        [Export] public int DefaultLineLength { get; set; } = 2000;

        private PackedScene raycastScene = GD.Load<PackedScene>("uid://uy1niwobaitt");
        public LightSource IncidentRay { get; set; }
        public bool IsReflection { get; set; }
        public bool HasChildReflection { get; set; }

        public override void _Ready()
        {
            TargetPosition = new Vector2(0, DefaultLineLength);
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateLine();

            if (IsColliding() && !HasChildReflection)
            {
                PhysicsBody2D collider = GetCollider() as PhysicsBody2D;

                if (collider != null && collider.IsInGroup("LightReflectors"))
                {
                    CreateNewRay(collider, true);
                }
            }

            if (IsReflection)
            {
                Reflect();
            }
        }

        private void UpdateLine()
        {
            var line = GetNode<Line2D>("Line2D");

            if (line.GetPointCount() == 0)
            {
                line.AddPoint(Vector2.Zero);
                line.AddPoint(new Vector2(0, DefaultLineLength));
            }

            line.SetPointPosition(1, new Vector2(0, DefaultLineLength));

            if (IsReflection)
            {
                var collisionPoint = IncidentRay.GetCollisionPoint();
                var parentLine = GetParent().GetNode<Line2D>("Line2D");

                if (parentLine.GetPointCount() >= 2)
                {
                    parentLine.SetPointPosition(1, parentLine.ToLocal(collisionPoint));
                }
            }

            if (IsColliding())
            {
                var collisionPoint = GetCollisionPoint();
                line.SetPointPosition(1, ToLocal(collisionPoint));
            }
        }

        private void Reflect()
        {
            if (IsReflection)
            {
                CheckParentCollision();

                var collisionPoint = IncidentRay.GetCollisionPoint();
                Position = collisionPoint;

                var collisionNormal = IncidentRay.GetCollisionNormal();

                var incidentRayOrigin = IncidentRay.GlobalPosition;
                var incidentRayEndpoint = collisionPoint;
                var rayDirection = incidentRayEndpoint - incidentRayOrigin;
                var rayDirectionNormalized = rayDirection.Normalized();

                var reflectionVector = rayDirectionNormalized.Bounce(collisionNormal);
                Rotation = (reflectionVector.Angle() - Mathf.Pi / 2);
            }
        }

        private void CreateNewRay(Node colliderSurface, bool isReflection = false)
        {
            HasChildReflection = true;

            if (raycastScene == null)
            {
                GD.PrintErr("raycastScene не назначен!");
                return;
            }

            var newRaycast = (LightSource)raycastScene.Instantiate();
            newRaycast.TopLevel = true;
            newRaycast.IsReflection = isReflection;

            var collisionPoint = GetCollisionPoint();
            newRaycast.Position = collisionPoint;

            var collisionNormal = GetCollisionNormal();

            var incidentDirection = (collisionPoint - GlobalPosition).Normalized();

            var reflectionDirection = incidentDirection.Bounce(collisionNormal);

            newRaycast.Rotation = reflectionDirection.Angle();

            newRaycast.IncidentRay = this;

            if (colliderSurface is CollisionObject2D collisionObj)
            {
                newRaycast.AddException(collisionObj);
            }

            AddChild(newRaycast);
        }

        private void CheckParentCollision()
        {
            if (!IncidentRay.IsColliding())
            {
                IncidentRay.HasChildReflection = false;
                QueueFree();
                return;
            }

            PhysicsBody2D collider = IncidentRay.GetCollider() as PhysicsBody2D;
            if (collider == null || !collider.IsInGroup("LightReflectors"))
            {
                IncidentRay.HasChildReflection = false;
                QueueFree();
            }
        }
    }
}