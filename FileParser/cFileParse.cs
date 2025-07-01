using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Electroimpact.FileParser
{
  public class cFileParse : IDisposable
  {
    public Electroimpact.StringCalc.cStringCalc _StringCalc = new Electroimpact.StringCalc.cStringCalc();

    #region Publics
    public cFileParse()
    {
    }
    ~cFileParse()
    {
      Dispose(false);
    }

    Dictionary<string, Regex> regs = new Dictionary<string, Regex>();
    bool GetArgumentLite(string line, string varName, out double val)
    {
      Regex r;
      if (!regs.ContainsKey(varName))
      {
        //regs.Add(varName, new Regex(varName + @"-?[0-9]+(\.)?[0-9]{0,3}"));
        regs.Add(varName, new Regex(varName + @"((-?[0-9]*(\.)?[0-9]{0,3}))"));
      }
      val = 0;
      r = regs[varName];
      var m = r.Match(line);
      if (m.Success)
      {
        if (double.TryParse(m.Value.Substring(varName.Length), out val))
        {
          return true;
        }
        return false;
        //val = double.Parse();
      }
      return false;

    }

    public bool TryGetArgument(string szLine, string varname, ref double dPos)
    {
      string sArg;
      int StartArg;
      int Length;
      double odPos;
      bool rv = this.GetArgument(szLine, varname, out odPos, out sArg, out StartArg, out Length);
      if (rv) //only update if we were successful
        dPos = odPos;
      return rv;
    }
    public bool TryGetArgument(string szLine, string varname, ref int iPos)
    {
      string sArg;
      int StartArg;
      int Length;
      double dPos;
      bool rv = this.GetArgument(szLine, varname, out dPos, out sArg, out StartArg, out Length);
      if (rv) //only update if we were successful
        iPos = (int)dPos;
      
      return rv;
    }
    /// <summary>
    /// Searches for an argument in a given string and returns the value found.  
    /// </summary>
    /// <param name="szLine"></param>
    /// <param name="varname"></param>
    /// <returns>Value found on success or double.NaN on failure</returns>
    public double GetArgument2(string szLine, string varname)
    {
      string sArg;
      int StartArg;
      int Length;
      double dPos;
      bool found = this.GetArgument(szLine, varname, out dPos, out sArg, out StartArg, out Length);
      if (!found)
        dPos = double.NaN;
      return dPos;
     }
   [Obsolete("Replaced by TryGetArgument which uses \"ref\" instead of \"out\".  GetArgument2 (also static) will return a value rather than a boolean.")]
    //public bool GetArgument(string szLine, string varname, out double dPos)
    //{
    //  string sArg;
    //  int StartArg;
    //  int Length;
    //  return this.GetArgument(szLine, varname, out dPos, out sArg, out StartArg, out Length);
     
    //}
    public bool GetArgument(string szLine, string varname, out double dPos, bool lite = false)
    {
      string sArg;
      int StartArg;
      int Length;
      if (lite)
      {
        return GetArgumentLite(szLine, varname, out dPos);
      }
      else
        return this.GetArgument(szLine, varname, out dPos, out sArg, out StartArg, out Length);
    }

    public bool GetArgument(string szLine, string varname, out double dPos, out string sArg, out int StartArg, out int Length)
    {
      Electroimpact.csString iString = new Electroimpact.csString();
      bool bTestLeft = false;
      bool bTestRight = false;
      int nStartArg = 0;
      int nEndArg = 0;
      dPos = 0;
      sArg = "0";
      StartArg = -1;
      Length = -1;
      bool bgotadot = false;
      while (bTestLeft == false || bTestRight == false)
      {
        nStartArg = szLine.IndexOf(varname, nStartArg, szLine.Length - nStartArg);
        if (szLine.Length <= nStartArg + varname.Length) nStartArg = -1;
        if (nStartArg == -1)
          break;
        if (nStartArg == varname.Length - 1)
          bTestLeft = true;
        else
          bTestLeft = this.CheckLeftSide(szLine.Substring(nStartArg - 1, 1));

        int offset = szLine.Substring(nStartArg + varname.Length, 1) == "=" ? 1 : 0; //robot part programs use the "=" as an assignement.  We need to check for that. 

        bTestRight = this.CheckRightSide(szLine.Substring(nStartArg + varname.Length + offset, 1));

        nStartArg += varname.Length + offset;
      }

      if (bTestRight && bTestLeft)
      {

        StartArg = nStartArg;
        iString.String = szLine.Substring(nStartArg, szLine.Length - nStartArg);
        nEndArg = nStartArg = 0;
        string ch = iString.GetLeft(1);
        bool gotEq = false;
        if (ch == "")
          return false;
        if (this.CheckForOpenParen(ch))
        {
          int LP = 1;
          while ((ch = iString.GetLeft(1)) != "")
          {
            nEndArg++;
            switch (ch)
            {
              case "{":
              case "(":
              case "[":
                LP++;
                break;
              case "}":
              case ")":
              case "]":
                LP--;
                break;
              default:
                break;
            }
            gotEq = LP == 0;
            if (gotEq)
              break;
          }
          if (gotEq)
          {
            string eq = iString.String.Substring(nStartArg, nEndArg + 1);
            dPos = this._StringCalc.SimpleCalc(eq);
            return dPos != double.NaN;
          }
        }
        else
        {
          iString.Rewind();
          string rest = iString.String;
          string szCompare = "0123456789+-."; //The '+' is questionable.

          for (int ii = 0; ii < rest.Length; ii++)
          {
            bool bitsadot = rest.Substring(ii, 1) == ".";
            if (bitsadot && bgotadot)
              break;
            bgotadot = bgotadot || bitsadot;

            if (!iString.InString(rest.Substring(ii, 1), szCompare))
              break;
            nEndArg++;
          }
          string szResult = rest.Substring(0, nEndArg);
          dPos = iString.ToDouble(szResult);
          Length = nEndArg;
          sArg = szResult;
          return true;
        }
      }
      return false;
    }
    /// <summary>
    /// Replaces whatever argument held in varname with newval.  Returns true if it found the argument and changed it. 
    /// </summary>
    /// <param name="szLine"></param>
    /// <param name="varname"></param>
    /// <param name="newval"></param>
    /// <param name="NewLine"></param>
    /// <param name="FMT"></param>
    /// <returns></returns>
    ///

    public bool ReplaceArgument(string szLine, string varname, double newval, out string NewLine, string FMT = "F3")
    {
      double dhold;
      string sz;
      int Start, Length;
      NewLine = szLine;
      if (GetArgument(szLine, varname, out dhold, out sz, out Start, out Length))
      {
        string left = szLine.Substring(0, Start);
        string right = szLine.Substring(Start + Length);
        NewLine = left + newval.ToString(FMT) + right;
        return true;
      }
      return false;
    }

    public bool ReplaceArgument(string szLine, string varname, string newval, out string NewLine)
    {
      double dhold;
      string sz;
      int Start, Length;
      NewLine = szLine;
      if (GetArgument(szLine, varname, out dhold, out sz, out Start, out Length))
      {
        string left = szLine.Substring(0, Start);
        string right = szLine.Substring(Start + Length);
        NewLine = left + newval + right;
        return true;
      }
      return false;
    }
    #endregion

    #region Privates
    private bool CheckLeftSide(string szIn)
    {
      //Watch out...I added a weird case "("
      string[] szOK = { "(", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", "]", "}", ")", "-", "+" };

      for (int ii = 0; ii < szOK.Length; ii++)
      {
        if (szIn == szOK[ii])
        {
          return true;
        }
      }
      return false;
    }

    private bool CheckRightSide(string szIn)
    {
      string[] szOK = { ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", "[", "{", "(", "-", "+" };


      for (int ii = 0; ii < szOK.Length; ii++)
      {
        if (szIn == szOK[ii])
        {
          return true;
        }
      }
      return false;
    }

    private bool CheckForOpenParen(string szIn)
    {
      string[] szOK = { "(", "{", "[" };

      for (int ii = 0; ii < szOK.Length; ii++)
      {
        if (szIn == szOK[ii])
        {
          return true;
        }
      }
      return false;
    }

    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
      Dispose(true);
      System.GC.SuppressFinalize(this);
    }

    void Dispose(bool explicitCall)
    {
      if (explicitCall)
      {
        if (_StringCalc != null)
          _StringCalc = null;
      }
    }

    #endregion
  }
}
