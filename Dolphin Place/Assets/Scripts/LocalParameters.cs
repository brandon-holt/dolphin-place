using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "LocalParameters", menuName = "Local Parameters")]
public class LocalParameters : ScriptableObject
{
    public enum GameModes { Menu, Singleplayer, Multiplayer }
    public GameModes gameMode;
    public GameEvent setGameModeEvent, setLocalDolphinEvent;
    public enum ControlModes { Side2D, Behind3D }
    public ControlModes controlModes;
    public Dolphin localDolphin, menuDolphin;
    public float cameraPositionSmoothTime, cameraAngleSmoothTime;
    public Vector2 cameraOffset;
    public Vector3 trickCameraOffset, menuDolphinPosition, menuDolphinCamPosition;
    public float nameBarOffset;
    public float niceEntryMaxAngle, multiplierIncreaseNiceEntry;
    public int framesPerFlip, framesPerSideFlip, framesPerTwist, framesPerPoint;
    public int numberOfSplits, secondsPerSplit;
    public float ringForce;
    public float swimMultiplierFlyingThreshold;

    public void SetViewMode(float value)
    {
        if (value == -1) controlModes = ControlModes.Side2D;
        else if (value == 1) controlModes = ControlModes.Behind3D;
    }

    public void SetGameMode(GameModes gameMode)
    {
        this.gameMode = gameMode;

        setGameModeEvent.Raise();
    }

    public void SetLocalDolphin(Dolphin dolphin)
    {
        localDolphin = dolphin;

        setLocalDolphinEvent.Raise();
    }

    public void SetCameraSpeed(System.Single value)
    {
        cameraPositionSmoothTime = Mathf.Max(.05f, Mathf.Lerp(0f, .2f, 1f - value));
    }

    public void SetCameraDistance(System.Single value)
    {
        cameraOffset.x = Mathf.Lerp(10f, 40f, value);
    }

    public void SetCameraHeight(System.Single value)
    {
        cameraOffset.y = Mathf.Lerp(5f, 20f, value);
    }
}
