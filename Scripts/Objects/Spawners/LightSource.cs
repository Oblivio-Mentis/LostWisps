#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public partial class LightSource : RayCast2D
    {
        [Export] public float DefaultLineLength { get; set; } = 2000f;
        [Export] public int   MaxReflections { get; set; } = 2;
        [Export] public float HitOffset { get; set; } = 0f;
        [Export] public StringName ReflectorGroup { get; set; } = "LightReflectors";
        [Export] public bool IsReflection { get; set; } = false;

        public LightSource? IncidentRay { get; set; }

        private bool _hasChildRay = false;
        private int  _reflectionIndex = 0;

        private Line2D? _line;
        private static readonly float EPSILON = 0.0001f;
        private static PackedScene? _lightScene;

        public override void _Ready()
        {
            _line = GetNodeOrNull<Line2D>("Line2D");

            TargetPosition = new Vector2(0, DefaultLineLength);

            if (_line != null)
            {
                _line.ClearPoints();
                _line.AddPoint(Vector2.Zero);
                _line.AddPoint(new Vector2(0, DefaultLineLength));
            }

            _lightScene ??= GD.Load<PackedScene>("uid://uy1niwobaitt");
        }

        public override void _ExitTree()
        {
            if (IncidentRay != null)
                IncidentRay._hasChildRay = false;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (IsReflection)
            {
                if (!ValidateParentCollision())
                {
                    QueueFree();
                    return;
                }

                if (TryComputeReflection(out var hitPoint, out var reflectDir))
                {
                    GlobalPosition = hitPoint + reflectDir * EPSILON;
                    Rotation = DirToRayRotation(reflectDir);
                }
            }

            UpdateLine();

            if (!_hasChildRay && TryGetReflectorCollider(out var reflector) && _reflectionIndex < MaxReflections)
            {
                CreateChildRay(reflector);
            }
        }

        private void UpdateLine()
        {
            if (_line == null)
                return;

            if (IsColliding())
            {
                var hit = GetCollisionPoint();
                _line.SetPointPosition(1, ToLocal(hit));
            }
            else
            {
                _line.SetPointPosition(1, new Vector2(0, DefaultLineLength));
            }
        }

        private static float DirToRayRotation(Vector2 dir) => dir.Angle() - Mathf.Pi / 2f;

        private bool ValidateParentCollision()
        {
            if (IncidentRay == null)
                return false;

            if (!IncidentRay.IsColliding())
                return false;

            if (!IncidentRay.TryGetReflectorCollider(out _))
                return false;

            return true;
        }

        private bool TryGetReflectorCollider(out CollisionObject2D collider)
        {
            collider = default!;
            if (!IsColliding())
                return false;

            var obj = GetCollider();
            if (obj is CollisionObject2D co && co.IsInGroup(ReflectorGroup))
            {
                collider = co;
                return true;
            }

            return false;
        }

        private bool TryComputeReflection(out Vector2 hitPoint, out Vector2 reflectDir)
        {
            hitPoint = default;
            reflectDir = default;

            if (!IsColliding())
                return false;

            var normal = GetCollisionNormal();
            if (normal.IsZeroApprox())
                return false;

            normal = normal.Normalized();

            hitPoint = GetCollisionPoint();

            var incDir = (hitPoint - GlobalPosition);
            if (incDir.IsZeroApprox())
                return false;

            incDir = incDir.Normalized();
            reflectDir = incDir.Bounce(normal).Normalized();
            return true;
        }

        private void CreateChildRay(CollisionObject2D reflector)
        {
            if (_lightScene == null)
                return;

            if (!TryComputeReflection(out var hitPoint, out var reflectDir))
                return;

            var child = _lightScene.Instantiate<LightSource>();
            child.TopLevel = true;

            child.IsReflection = true;
            child.IncidentRay = this;
            child._reflectionIndex = _reflectionIndex + 1;

            child.TargetPosition = new Vector2(0, DefaultLineLength);

            // Старт новой «палки» ровно из конца предыдущей
            child.GlobalPosition = hitPoint + reflectDir * (HitOffset <= 0f ? EPSILON : HitOffset);
            child.Rotation = DirToRayRotation(reflectDir);

            child.AddException(reflector);

            _hasChildRay = true;
            AddChild(child);
        }
    }
}
