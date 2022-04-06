using UnityEngine;

[CreateAssetMenu(fileName = "LocalParameters", menuName = "Local Parameters")]
public class LocalParameters : ScriptableObject
{
    public enum GameModes { Menu, Singleplayer, Multiplayer }
    public GameModes gameMode;
    public GameEvent setGameModeEvent;
    public enum ControlModes { Side2D, Behind3D }
    public ControlModes controlModes;
    public Dolphin localDolphin, menuDolphin;
    public float cameraPositionSmoothTime, cameraAngleSmoothTime;
    public Vector2 cameraOffset;
    public Vector3 menuDolphinPosition, menuDolphinCamPosition;
    public float nameBarOffset;
    public float niceEntryMaxAngle, multiplierIncreaseNiceEntry;
    public int framesPerFlip, framesPerSideFlip, framesPerTwist, framesPerPoint;
    public int numberOfSplits, secondsPerSplit;
    public float ringForce;

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
}
