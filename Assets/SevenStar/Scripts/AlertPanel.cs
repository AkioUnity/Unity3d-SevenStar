using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtil;


public enum AlertType
{
    LoginFail =0,
    LoginDuplication,
    LoginBanned,
    SignUpSucc,
    SignUpFail,
    SignUpNickNameOverlapping,
    SignUpIDOverLapping,
    EnterRoomFail,
    NoRoom,
    FullRoom,
    NoMoney,
    Avata_Same,
    BankDepositSucc,
    BankDepositFail,
    BankWithdrawalSucc,
    BankWithdrawalFail,
    BankGiftSucc,
    BankGiftFail_IDNotExist,
    BankGiftFail_NotEnoughMoney,
    FillTheBlank,
    PasswordConfirmFail,
    ChargeRequestSucc,
    ChargeRequestFail,
    None
}

public class AlertPanel : UtilHalfSingleton<AlertPanel>
{
    public GameObject m_AlertTextPanel;
    public GameObject m_AlertImagePanel;
    public GameObject m_AlertPanel_New;
    public Text m_AlertText;
    public Sprite[] m_AlertSprites;

    private GameObject m_UsePanel;
    public bool m_IsPop = false;

    public void OnClick_AlertImageOk()
    {
        if(m_AlertImagePanel)
            m_AlertImagePanel.SetActive(false);
        if(m_AlertPanel_New)
            m_AlertPanel_New.SetActive(false);
    }

    public void StartAlertOtherObj(GameObject obj, AlertType type)
    {
        if (obj == null)
            return;
        obj.SetActive(true);
        Text text = obj.GetComponent<Text>();
        if (text != null || type != AlertType.None)
            text.text = GetAlertTypeString(type);
    }

    public void StartAlert(string str)
    {
        m_AlertText.text = str;
        m_AlertPanel_New.SetActive(true);
    }

    public void StartAlert(float delay,AlertType type)
    {
        Console.WriteLine("alert"+type);
        if (m_IsPop == false)
            StartCoroutine(AlertRoutine(delay, type));
    }

    private IEnumerator AlertRoutine(float delay, AlertType type)
    {
        m_IsPop = true;
        /*m_AlertText.text = GetAlertTypeString(type);
        Sprite spr = GetAlertTypeSprite(type);
        if (spr == null)
            m_UsePanel = m_AlertTextPanel;
        else
        {
            m_AlertImagePanel.transform.GetChild(0).GetComponent<Image>().sprite = spr;
            m_UsePanel = m_AlertImagePanel;
        }

        m_UsePanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        if(m_UsePanel == m_AlertTextPanel)
            m_UsePanel.SetActive(false);

        m_AlertText.text = "";*/
        m_AlertText.text = GetAlertTypeString(type);
        m_AlertPanel_New.SetActive(true);
        yield return null;
        m_IsPop = false;
    }

    private Sprite GetAlertTypeSprite(AlertType alert)
    {
        Sprite spr = null;
        switch (alert)
        {
            case AlertType.LoginFail:
                spr = m_AlertSprites[0];
                break;
            case AlertType.LoginDuplication:
                spr = m_AlertSprites[1];
                break;
            case AlertType.LoginBanned:
                break;
            case AlertType.SignUpSucc:
                spr = m_AlertSprites[2];
                break;
            case AlertType.SignUpFail:
                spr = m_AlertSprites[3];
                break;
            case AlertType.SignUpNickNameOverlapping:
                spr = m_AlertSprites[4];
                break;
            case AlertType.SignUpIDOverLapping:
                spr = m_AlertSprites[5];
                break;
            case AlertType.EnterRoomFail:
                break;
            case AlertType.NoRoom:
                break;
            case AlertType.FullRoom:
                break;
            case AlertType.NoMoney:
                spr = m_AlertSprites[6];
                break;
            case AlertType.Avata_Same:
                break;
            case AlertType.BankDepositSucc:
                spr = m_AlertSprites[7];
                break;
            case AlertType.BankDepositFail:
                spr = m_AlertSprites[8];
                break;
            case AlertType.BankWithdrawalSucc:
                spr = m_AlertSprites[9];
                break;
            case AlertType.BankWithdrawalFail:
                spr = m_AlertSprites[10];
                break;
            case AlertType.BankGiftSucc:
                spr = m_AlertSprites[11];
                break;
            case AlertType.BankGiftFail_IDNotExist:
                spr = m_AlertSprites[12];
                break;
            case AlertType.BankGiftFail_NotEnoughMoney:
                spr = m_AlertSprites[13];
                break;
        }
        return spr;
    }

    private string GetAlertTypeString(AlertType alert)
    {
        string msg = "";
        switch (alert)
        {
            case AlertType.LoginFail:
                //msg = "Login Fail";
                //msg = "We couldn't find and account matching the user name and password you enterd.\n" +
                //    "Please check your username and password and try again.";
                msg = "Хэрэглэгчийн нэракаунтыг бид хайж олсонгүй\n" +
                    "таны оруулсан нууц үг. Хэрэглэгчийн нэрээ шалгана уу";
                break;
            case AlertType.LoginDuplication:
                //msg = "Login Fail - Duplication Login";
                msg = "Давхардсан нэвтрэх";
                break;
            case AlertType.LoginBanned:
                //msg = "Login Fail - Banned ID";
                msg = "Энэ ID хорисон байна";
                break;
            case AlertType.SignUpSucc:
                msg = "Sign Up Success";
                break;
            case AlertType.SignUpFail:
                msg = "Sign Up Fail";
                break;
            case AlertType.SignUpNickNameOverlapping:
                //msg = "Nickname is overlapping";
                msg = "Хоч нэр давхцаж байна";
                break;
            case AlertType.SignUpIDOverLapping:
                //msg = "ID is overlapping";
                msg = "Хэрэглэгчийн ID давхцаж байна";
                break;
            case AlertType.EnterRoomFail:
                msg = "Өрөөнд орж чадаагүй";
                break;
            case AlertType.NoRoom:
                msg = "Өрөөнд орж чадаагүй - Өрөөн байхгүй";
                break;
            case AlertType.FullRoom:
                msg = "Өрөөнд орж чадаагүй - Өрөөн дүүрэн байна";
                break;
            case AlertType.NoMoney:
                msg = "хангалттай цэг байхгүй байна";
                break;
            case AlertType.Avata_Same:
                msg = "You already have it";
                break;
            case AlertType.BankDepositSucc:
                msg = "Банкны хадгаламжийн амжилт";
                break;
            case AlertType.BankDepositFail:
                msg = "Банкны хадгаламж алдагдлаа";
                break;
            case AlertType.BankWithdrawalSucc:
                msg = "Банк зарлагын гүйлгээ амжилт";
                break;
            case AlertType.BankWithdrawalFail:
                msg = "Мөнгө авах Бүтэлгүйтлээ";
                break;
            case AlertType.BankGiftSucc:
                msg = "Бэлэг дурсгалын амжилт илгээх";
                break;
            case AlertType.BankGiftFail_IDNotExist:
                msg = "Энэ ID байхгүй байна";
                break;
            case AlertType.BankGiftFail_NotEnoughMoney:
                msg = "хангалттай цэг байхгүй байна";
                break;
            case AlertType.FillTheBlank:
                msg = "Тохирох утга оруулна уу";
                break;
            case AlertType.PasswordConfirmFail:
                msg = "Баталгаажуулах нууц үг буруу байна. Дахин оролдоно уу";
                break;
            case AlertType.ChargeRequestSucc:
                msg = "Данс цэнэглэх амжилт";
                break;
            case AlertType.ChargeRequestFail:
                msg = "Данс цэнэглэх Бүтэлгүйтлээ";
                break;
        }
        
        return msg;
    }

}
