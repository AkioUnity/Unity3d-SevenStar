using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtil;


public enum CardShapeType
{
    Back =0,
    Clover,
    Diamond,
    Heart,
    Spade
}

public class CardMgr : UtilHalfSingleton<CardMgr>
{
    public int m_NowIdx;
    public Sprite m_BackSprite;
    public Sprite[] m_CloverCardSprites;
    public Sprite[] m_DiamondCardSprites;
    public Sprite[] m_HeartCardSprites;
    public Sprite[] m_SpadeCardSprites;

    public Sprite GetSpriteByShapeType(CardShapeType type, int idx)
    {
        if (idx > 14)
            return null;
        idx--;
        Sprite spr = null;
        switch (type)
        {
            case CardShapeType.Back:
                spr = m_BackSprite;
                break;
            case CardShapeType.Clover:
                spr = m_CloverCardSprites[idx];
                break;
            case CardShapeType.Diamond:
                spr = m_DiamondCardSprites[idx];
                break;
            case CardShapeType.Heart:
                spr = m_HeartCardSprites[idx];
                break;
            case CardShapeType.Spade:
                spr = m_SpadeCardSprites[idx];
                break;
        }
        return spr;
    }
}
		
