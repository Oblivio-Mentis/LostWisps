using Godot;
using LostWisps.Debug;
using System;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public interface IRaycastHitListener
    {
        void OnLightRayHit(Vector2 point, Vector2 direction);
    }

    public enum NonReflectorHitMode
    {
        Absorb,
        PassThrough
    }

    public sealed class LightRayTracer2D
    {
        public Vector2 Source { get; set; }
        public Vector2 Direction { get; set; } = Vector2.Right;
        public uint CollisionMask { get; set; } = uint.MaxValue;
        public bool CollideWithBodies { get; set; } = true;
        public bool CollideWithAreas { get; set; } = true;
        public NonReflectorHitMode NonReflectorMode { get; set; } = NonReflectorHitMode.Absorb;

        public int MaxReflections { get; set; } = 8;
        public float MaxTotalLength { get; set; } = 2000f;
        public float MinPointDistance { get; set; } = 0.5f;
        public float SurfaceEpsilon { get; set; } = 0.5f;

        public Func<Node, bool> IsReflectorPredicate { get; set; }

        private readonly World2D _world;
        private readonly Node _ownerForLogging;

        public LightRayTracer2D(World2D world, Node ownerForLogging = null)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _ownerForLogging = ownerForLogging;
            IsReflectorPredicate = DefaultReflectorCheck;
        }

        private static bool DefaultReflectorCheck(Node node)
        {
            return node != null && node.IsInGroup("LightReflectors");
        }

        public struct Segment { public Vector2 From; public Vector2 To; }
        public struct TraceResult { public List<Segment> Segments; public List<Vector2> Points; }

        public TraceResult Trace(ICollection<Rid> excludeRidsPerIteration = null)
        {
            var points = new List<Vector2>(16);
            var segments = new List<Segment>(16);

            var dss = _world.DirectSpaceState;
            if (dss == null)
            {
                Logger.Warn(LogCategory.Raycast, "DirectSpaceState is null; cannot trace.", _ownerForLogging);
                return new TraceResult { Segments = segments, Points = points };
            }

            var origin = Source;
            var dir = Direction;
            if (dir.LengthSquared() <= 0.0f)
            {
                Logger.Warn(LogCategory.Raycast, "Direction is zero; no trace performed.", null);
                return new TraceResult { Segments = segments, Points = points };
            }
            dir = dir.Normalized();

            float remaining = MaxTotalLength;
            int reflections = 0;

            points.Add(origin);

            var exclude = new HashSet<Rid>();
            if (excludeRidsPerIteration != null)
                foreach (var rid in excludeRidsPerIteration) exclude.Add(rid);

            while (remaining > MinPointDistance && reflections <= MaxReflections)
            {
                var from = origin;
                var to = origin + dir * remaining;

                var query = PhysicsRayQueryParameters2D.Create(from, to, CollisionMask);
                query.CollideWithAreas = CollideWithAreas;
                query.CollideWithBodies = CollideWithBodies;
                if (exclude.Count > 0)
                    query.Exclude = new Godot.Collections.Array<Rid>(exclude);

                var hit = dss.IntersectRay(query);

                if (hit.Count == 0)
                {
                    var final = to;
                    if ((final - points[^1]).Length() >= MinPointDistance)
                    {
                        segments.Add(new Segment { From = points[^1], To = final });
                        points.Add(final);
                    }
                    break;
                }

                var hitPoint = (Vector2)hit["position"];
                var hitNormal = (Vector2)hit["normal"];
                var hitRid = (Rid)hit["rid"];

                Node colliderNode = null;
                if (hit.TryGetValue("collider", out var colliderVar) && colliderVar.VariantType != Variant.Type.Nil)
                    colliderNode = colliderVar.As<Node>();

                if (hitNormal == Vector2.Zero)
                    hitNormal = -dir;

                if ((hitPoint - points[^1]).Length() >= MinPointDistance)
                {
                    segments.Add(new Segment { From = points[^1], To = hitPoint });
                    points.Add(hitPoint);
                }

                remaining -= (hitPoint - origin).Length();
                if (remaining <= MinPointDistance)
                    break;

                if (colliderNode != null)
                {
                    try
                    {
                        if (colliderNode is LostWisps.Object.IRaycastHitListener listener)
                            listener.OnLightRayHit(hitPoint, dir);
                        else if (colliderNode.HasMethod("OnLightRayHit"))
                            colliderNode.CallDeferred("OnLightRayHit", hitPoint, dir);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(LogCategory.Raycast, $"Hit notification threw: {e.Message}", null);
                    }
                }

                bool isReflector = colliderNode != null && (IsReflectorPredicate?.Invoke(colliderNode) ?? false);

                if (isReflector)
                {
                    dir = dir.Bounce(hitNormal).Normalized();
                    origin = hitPoint + hitNormal * SurfaceEpsilon;
                    reflections += 1;
                    exclude.Clear(); exclude.Add(hitRid);
                }
                else
                {
                    if (NonReflectorMode == NonReflectorHitMode.Absorb)
                        break;
                    origin = hitPoint + dir * SurfaceEpsilon;
                    exclude.Clear(); exclude.Add(hitRid);
                    continue;
                }
            }

            return new TraceResult { Segments = segments, Points = points };
        }
    }
}
