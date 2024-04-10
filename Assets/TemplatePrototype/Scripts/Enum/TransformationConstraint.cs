using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransformationConstraint
{
    None = 0,
    Position = 2,
    Rotation = 4,
    Scale = 8,
    PositionRotation = 6,
    PositionScale = 10,
    RotationScale = 12,
    All = 14
}
