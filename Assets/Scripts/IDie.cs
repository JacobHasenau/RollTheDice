using System.Collections;
using System.Collections.Generic;

public interface IDie
{
    IReadOnlyCollection<IDieSide> Sides { get; }
    IEnumerator RollDie();
    void ForceDieRoll(IDieSide dieSide);
}

public class Events
{
    private static ScoreGain _onDiceRolled;

    public delegate void ScoreGain(long score);
    public static void CallOnDiceRolled(long score) => _onDiceRolled?.Invoke(score);
    public static void SubscriebeToOnDiceRolled(ScoreGain deleg) => _onDiceRolled += deleg;
    public static void UnSubscriebeToOnDiceRolled(ScoreGain deleg) => _onDiceRolled -= deleg;
}
