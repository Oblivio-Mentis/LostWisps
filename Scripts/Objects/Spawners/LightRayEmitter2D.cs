using Godot;
using System;

namespace LostWisps.Object
{
    [Tool]
    public partial class LightRayEmitter2D : Node2D
    {
        [ExportGroup("🔗 References")]
        [Export] public DirectionHandle2D DirectionHandle;
        [Export] public Line2D Line;

        [ExportGroup("🛠️ Editor Tools")]
        [Export] public bool EditorInteractive = true;
        [Export(PropertyHint.Range, "16,1000,1")]
        public int EditorTickMs = 100;

        [ExportGroup("⚙️ Collision Flags")]
        [Export] public bool CollideWithBodies = true;
        [Export] public bool CollideWithAreas = true;

        [ExportGroup("🧩 Masks & Modes")]
        [Export(PropertyHint.Layers2DPhysics)] public uint CollisionMask = uint.MaxValue;
        [Export] public NonReflectorHitMode NonReflectorMode = NonReflectorHitMode.Absorb;

        [ExportGroup("🔁 Reflection")]
        [Export(PropertyHint.Range, "0,64,1")] public int MaxReflections = 12;

        [ExportGroup("📏 Length & Precision")]
        [Export(PropertyHint.Range, "1,10000,1")] public float MaxTotalLength = 3000f;
        [Export(PropertyHint.Range, "0.01,10,0.01")] public float MinPointDistance = 0.5f;
        [Export(PropertyHint.Range, "0.01,10,0.01")] public float SurfaceEpsilon = 0.5f;

        [ExportGroup("🎨 Appearance")]
        [Export] public Color LineColor = new Color(1f, 0.95f, 0.6f, 1f);
        [Export(PropertyHint.Range, "1,16,0.5")] public float LineWidth = 3f;
        [Export] public bool AutoHideWhenEmpty = true;

        private LightRayTracer2D _tracer;
        private bool _warnedMissingLine = false;

        private double _editorAccumulatorMs = 0;

        public override void _EnterTree()
        {
            SetProcess(true);
        }

        public override void _Ready()
        {
            EnsureLineNode();
            ApplyLineVisuals();

            _tracer = new LightRayTracer2D(GetWorld2D(), this)
            {
                CollideWithBodies = CollideWithBodies,
                CollideWithAreas = CollideWithAreas,
                CollisionMask = CollisionMask,
                NonReflectorMode = NonReflectorMode,
                MaxReflections = MaxReflections,
                MaxTotalLength = MaxTotalLength,
                MinPointDistance = MinPointDistance,
                SurfaceEpsilon = SurfaceEpsilon
            };
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint())
            {
                if (!EditorInteractive)
                {
                    // В редакторе визуал оставляем как есть, пересчёт выключен.
                    return;
                }

                _editorAccumulatorMs += delta * 1000.0;
                if (_editorAccumulatorMs >= EditorTickMs)
                {
                    _editorAccumulatorMs = 0;
                    UpdateBeam();
                }
                return;
            }

            UpdateBeam();
        }

        private void UpdateBeam()
        {
            if (_tracer == null)
            {
                EnsureLineNode();
                return;
            }

            Vector2 dir = Vector2.Right;
            if (DirectionHandle != null)
                dir = DirectionHandle.GlobalTransform.BasisXform(DirectionHandle.Direction).Normalized();
            if (dir.LengthSquared() <= 0.0f)
                dir = Vector2.Right;

            _tracer.Direction = dir;
            _tracer.Source = GlobalPosition;
            _tracer.CollideWithAreas = CollideWithAreas;
            _tracer.CollideWithBodies = CollideWithBodies;
            _tracer.CollisionMask = CollisionMask;
            _tracer.NonReflectorMode = NonReflectorMode;
            _tracer.MinPointDistance = MinPointDistance;
            _tracer.SurfaceEpsilon = SurfaceEpsilon;
            _tracer.MaxReflections = MaxReflections;
            _tracer.MaxTotalLength = MaxTotalLength;

            var trace = _tracer.Trace();

            if (Line == null)
            {
                if (!_warnedMissingLine)
                {
                    GD.PushWarning("[LightRayEmitter2D] Line2D is not assigned; visualization disabled.");
                    _warnedMissingLine = true;
                }
                return;
            }

            if (trace.Points.Count < 2)
            {
                if (AutoHideWhenEmpty)
                    Line.Visible = false;
                else
                {
                    Line.Visible = true;
                    Line.Points = new Vector2[] { Vector2.Zero, Vector2.Zero };
                }
                return;
            }

            Line.Visible = true;

            var localPoints = new Vector2[trace.Points.Count];
            for (int i = 0; i < trace.Points.Count; i++)
                localPoints[i] = Line.ToLocal(trace.Points[i]);

            Line.Points = localPoints;
        }

        private void ApplyLineVisuals()
        {
            if (Line == null) return;
            Line.DefaultColor = LineColor;
            Line.Width = LineWidth;
            Line.SharpLimit = 16f;
            Line.JointMode = Line2D.LineJointMode.Sharp;
            Line.BeginCapMode = Line2D.LineCapMode.None;
            Line.EndCapMode = Line2D.LineCapMode.None;
            Line.Antialiased = true;
        }

        private void EnsureLineNode()
        {
            if (Line != null) return;
            var created = new Line2D { Name = "AutoLine2D", ZIndex = 1000 };
            AddChild(created);
            Line = created;
            ApplyLineVisuals();
        }
    }
}
