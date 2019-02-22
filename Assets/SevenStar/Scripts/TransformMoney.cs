using System;
using System.Collections;
using System.Collections.Generic;

public static class TransformMoney
{
    public static string GetDollarMoney(UInt64 Money)
    {
        string tMoney = "0";
        UInt64 Money_d = Money / 100;
        UInt64 Money_c = Money % 100;
        if (Money_d != 0)
            tMoney = Money_d.ToString();
        if (Money_c != 0)
        {
            if (Money_d == 0)
                tMoney += "0." + Money_c.ToString("D2");
            else
                tMoney += "." + Money_c.ToString("D2");
        }
        return tMoney;
    }

    public static string MoneyTransform(UInt64 pMoney, bool isDecPoint)
    {
        string tMoney = "";
        UInt64 d = pMoney / 100;
        UInt64 c = pMoney % 100;

        if (d != 0)
            tMoney = d.ToString();
        if (c != 0)
        {
            if(d==0)
                tMoney += "0." + c.ToString();
            else
                tMoney += "." + c.ToString();
        }

        return tMoney + " $";

        /*
        string tMoney = "";
        // B
        UInt64 b = pMoney / 1000000000;
        if (b != 0)
        {
            if (isDecPoint)
                return GetMoneyDecPoint_B(pMoney);
            else
                tMoney += b.ToString() + "B";
        }
        // M
        UInt64 m = (pMoney % 1000000000) / 1000000;
        if (m != 0)
        {
            if (isDecPoint)
                return GetMoneyDecPoint_M(pMoney);
            else
                tMoney += " " + m.ToString() + "M";
        }
        // K
        UInt64 k = (pMoney % 1000000) / 1000;
        if (k != 0)
        {
            if (isDecPoint)
                return GetMoneyDecPoint_K(pMoney);
            else
                tMoney += " " + k.ToString() + "K";
        }
        UInt64 n = pMoney % 1000;
        if(b!=0 || m!=0 || k!=0)
        {
            if (n == 0)
                tMoney += "";
            else
                tMoney += " " + n.ToString();
        }
        else
            tMoney += n.ToString();
        return tMoney;*/
    }

    public static string MoneyTransform_BMK(UInt64 pMoney, bool isDecPoint)
    {

        string tMoney = "";
        // B
        UInt64 b = pMoney / 1000000000;
        if (b != 0)
        {
            if (isDecPoint)
                return GetMoneyDecPoint_B(pMoney);
            else
                tMoney += b.ToString() + "B";
        }
        // M
        UInt64 m = (pMoney % 1000000000) / 1000000;
        if (m != 0)
        {
            if (isDecPoint)
                return GetMoneyDecPoint_M(pMoney);
            else
                tMoney += " " + m.ToString() + "M";
        }
        // K
        UInt64 k = (pMoney % 1000000) / 1000;
        if (k != 0)
        {
            if (isDecPoint)
                return GetMoneyDecPoint_K(pMoney);
            else
                tMoney += " " + k.ToString() + "K";
        }
        UInt64 n = pMoney % 1000;
        if(b!=0 || m!=0 || k!=0)
        {
            if (n == 0)
                tMoney += "";
            else
                tMoney += " " + n.ToString();
        }
        else
            tMoney += n.ToString();
        return tMoney;
    }

    public static string GetMoneyDecPoint_B(UInt64 money)
    {
        string tMoney = "";
        UInt64 b = money / 1000000000;
        tMoney += b.ToString();
        UInt64 m = (money % 1000000000);
        if (m.ToString().Length >= 8)
        {
            if (m.ToString().Length == 8)
                tMoney += ".0" + m.ToString().Substring(0, 1);
            else
                tMoney += "." + m.ToString().Substring(0, 2);
        }
        return tMoney+"B";
    }

    public static string GetMoneyDecPoint_M(UInt64 money)
    {
        string tMoney = "";
        UInt64 m = (money % 1000000000) / 1000000;
        tMoney += m.ToString();
        UInt64 k = (money % 1000000);
        if (k.ToString().Length >= 5)
        {
            if (k.ToString().Length == 5)
                tMoney += ".0" + k.ToString().Substring(0, 1);
            else
                tMoney += "." + k.ToString().Substring(0, 2);
        }
        return tMoney+"M";
    }

    public static string GetMoneyDecPoint_K(UInt64 money)
    {
        string tMoney = "";
        UInt64 k = (money % 1000000) / 1000;
        tMoney += k.ToString();
        UInt64 n = (money % 1000);
        if (n.ToString().Length >= 2)
        {
            if (n.ToString().Length == 2)
                tMoney += ".0" + n.ToString().Substring(0, 1);
            else
                tMoney += "."+n.ToString().Substring(0, 2);
        }
        return tMoney+"K";
    }

    public static string MoneyTransform(string money)
    {
        UInt64 pMoney = UInt64.Parse(money);
        string tMoney = "";
        // B
        UInt64 b = pMoney / 1000000000;
        if (b != 0)
            tMoney += b.ToString() + "B";
        // M
        UInt64 m = (pMoney % 1000000000) / 1000000;
        if (m != 0)
            tMoney += " " + m.ToString() + "M";
        // K
        UInt64 k = (pMoney % 1000000) / 1000;
        if (k != 0)
            tMoney += " " + k.ToString() + "K";
        UInt64 n = pMoney % 1000;
        if (b != 0 || m != 0 || k != 0)
        {
            if (n == 0)
                tMoney = "";
            else
                tMoney += " " + n.ToString();
        }
        else
            tMoney += " " + n.ToString();
        return tMoney;
    }

    public static string MoneyTransform_WholeNumber(UInt64 money)
    {
        return string.Format("{0:N0}", money);
    }
}
