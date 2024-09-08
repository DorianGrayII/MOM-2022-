// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CursorsLibrary
using System.Collections.Generic;
using UnityEngine;

public class CursorsLibrary : MonoBehaviour
{
    public enum Mode
    {
        None = 0,
        Default = 1,
        MoveCamera = 2,
        AttackMelee = 3,
        InvalidMelee = 4,
        AttackRanged = 5,
        InvalidRanged = 6,
        CastSpell = 7,
        InvalidSpell = 8,
        Surveyor = 9,
        RMBInfo = 10,
        EdgescrollingLeft = 11,
        EdgescrollingRight = 12,
        EdgescrollingUp = 13,
        EdgescrollingDown = 14
    }

    private static CursorsLibrary instance;

    public Texture2D defaultCursor;

    public Vector2 defaultCursorHotSpot = Vector2.zero;

    public Texture2D moveCameraCursor;

    public Vector2 moveCameraCursorHotSpot = Vector2.zero;

    public Texture2D attackMeleeCursor;

    public Vector2 attackMeleeCursorHotSpot = Vector2.zero;

    public Texture2D invalidMeleeCursor;

    public Vector2 invalidMeleeCursorHotSpot = Vector2.zero;

    public Texture2D attackRangedCursor;

    public Vector2 attackRangedCursorHotSpot = Vector2.zero;

    public Texture2D invalidRangedCursor;

    public Vector2 invalidRangedCursorHotSpot = Vector2.zero;

    public Texture2D castSpellCursor;

    public Vector2 castSpellCursorHotSpot = Vector2.zero;

    public Texture2D invalidSpellCursor;

    public Vector2 invalidSpellCursorHotSpot = Vector2.zero;

    public Texture2D surveyorCursor;

    public Vector2 surveyorCursorHotSpot = Vector2.zero;

    public Texture2D rmbInfoCursor;

    public Vector2 rmbInfoCursorHotSpot = Vector2.zero;

    public Texture2D edgescrollingLeft;

    public Vector2 edgescrollingLeftHotSpot = Vector2.zero;

    public Texture2D edgescrollingRight;

    public Vector2 edgescrollingRightHotSpot = Vector2.zero;

    public Texture2D edgescrollingUp;

    public Vector2 edgescrollingUpHotSpot = Vector2.zero;

    public Texture2D edgescrollingDown;

    public Vector2 edgescrollingDownHotSpot = Vector2.zero;

    public CursorMode cursorMode;

    public GameObject rangedHitChance;

    private Mode currentMode;

    private bool currentRMB;

    private static HashSet<string> m_RMBAvailiable = new HashSet<string>();

    private void Start()
    {
        CursorsLibrary.instance = this;
    }

    public static void SetMode(Mode m)
    {
        CursorsLibrary.instance.SetModeLocal(m);
    }

    public static void SetRMBAvailiable(bool availiable, string claimed)
    {
        if (availiable)
        {
            if (!CursorsLibrary.m_RMBAvailiable.Contains(claimed))
            {
                CursorsLibrary.m_RMBAvailiable.Add(claimed);
            }
        }
        else
        {
            CursorsLibrary.m_RMBAvailiable.Remove(claimed);
        }
        CursorsLibrary.instance.SetModeLocal(CursorsLibrary.instance.currentMode);
    }

    private void SetModeLocal(Mode m)
    {
        bool flag = CursorsLibrary.m_RMBAvailiable.Count > 0;
        if (m == this.currentMode && this.currentRMB == flag)
        {
            return;
        }
        MouseTooltip.Close();
        this.currentRMB = flag;
        switch (m)
        {
        case Mode.Default:
            if (flag)
            {
                Cursor.SetCursor(this.rmbInfoCursor, this.rmbInfoCursorHotSpot, this.cursorMode);
            }
            else
            {
                Cursor.SetCursor(this.defaultCursor, this.defaultCursorHotSpot, this.cursorMode);
            }
            break;
        case Mode.MoveCamera:
            Cursor.SetCursor(this.moveCameraCursor, this.moveCameraCursorHotSpot, this.cursorMode);
            break;
        case Mode.AttackMelee:
            Cursor.SetCursor(this.attackMeleeCursor, this.attackMeleeCursorHotSpot, this.cursorMode);
            break;
        case Mode.InvalidMelee:
            Cursor.SetCursor(this.invalidMeleeCursor, this.invalidMeleeCursorHotSpot, this.cursorMode);
            break;
        case Mode.AttackRanged:
            Cursor.SetCursor(this.attackRangedCursor, this.attackRangedCursorHotSpot, this.cursorMode);
            break;
        case Mode.InvalidRanged:
            Cursor.SetCursor(this.invalidRangedCursor, this.invalidRangedCursorHotSpot, this.cursorMode);
            break;
        case Mode.CastSpell:
            Cursor.SetCursor(this.castSpellCursor, this.castSpellCursorHotSpot, this.cursorMode);
            break;
        case Mode.InvalidSpell:
            Cursor.SetCursor(this.invalidSpellCursor, this.invalidSpellCursorHotSpot, this.cursorMode);
            break;
        case Mode.Surveyor:
            Cursor.SetCursor(this.surveyorCursor, this.surveyorCursorHotSpot, this.cursorMode);
            break;
        case Mode.RMBInfo:
            Cursor.SetCursor(this.rmbInfoCursor, this.rmbInfoCursorHotSpot, this.cursorMode);
            break;
        case Mode.EdgescrollingLeft:
            Cursor.SetCursor(this.edgescrollingLeft, this.edgescrollingLeftHotSpot, this.cursorMode);
            break;
        case Mode.EdgescrollingRight:
            Cursor.SetCursor(this.edgescrollingRight, this.edgescrollingRightHotSpot, this.cursorMode);
            break;
        case Mode.EdgescrollingUp:
            Cursor.SetCursor(this.edgescrollingUp, this.edgescrollingUpHotSpot, this.cursorMode);
            break;
        case Mode.EdgescrollingDown:
            Cursor.SetCursor(this.edgescrollingDown, this.edgescrollingDownHotSpot, this.cursorMode);
            break;
        }
        this.currentMode = m;
    }
}
