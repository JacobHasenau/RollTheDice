using UnityEngine;

public interface IDieSide
{
    short Side { get; }
    bool FacingCamera();
    void Activate();
}
