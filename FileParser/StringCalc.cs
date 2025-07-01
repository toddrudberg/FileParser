using System;
using System.Collections;
using System.Windows.Forms;

namespace Electroimpact
{

	public class csString
	{

		#region MEMBERS
		public delegate void ErrorThrower(string szError);
		public event ErrorThrower StringError;

		private string mString;
		private string[] mszarParsed;
		private int mnCount;
		private int mnCurrentIndex;
		private int mnCurrentPosition;
		#endregion

		System.Collections.Hashtable Constants = new System.Collections.Hashtable();
		System.Collections.ICollection ConstantKeys;
		double ConstantLast;

		#region CONSTRUCTORS
		public csString()
		{
			this.mString = "";
			mnCount = 0;
			mnCurrentIndex = 0;
			mnCurrentPosition = 0;
			SetupConstants();
		}

		public csString(string StringIn)
		{
			this.mString = StringIn;
			mnCount = 0;
			mnCurrentIndex = 0;
			mnCurrentPosition = 0;
			SetupConstants();
		}

		private void SetupConstants()
		{
			this.Constants.Add("pi", System.Math.PI);
			this.Constants.Add("E", System.Math.E);
			this.ConstantKeys = this.Constants.Keys;
		}

		#endregion

		#region Events
		private void ErrorHandler(string szError)
		{
			if (StringError != null)
			{
				this.StringError(szError);
			}
			else
			{
				//      System.Windows.Forms.MessageBox.Show( szError );
			}
		}

		#endregion

		#region Properties
		/// <summary>
		/// Sets or gets internal string.  Setting rewinds the string pointer.
		/// </summary>
		public string String
		{
			get
			{
				return this.mString;
			}
			set
			{
				this.mString = value;
				this.Rewind();
			}
		}
		/// <summary>
		/// Returns length of internal string.
		/// </summary>
		public int Length
		{
			get { return this.mString.Length; }
		}
		/// <summary>
		/// Returns the constants defined as numeric values.  Currently e and pi are it.  e doesn't work in the calculator and I don't care.
		/// </summary>
		public string constants
		{
			get
			{
				string rtn = "";
				foreach (string key in this.ConstantKeys)
				{
					rtn += key + " ";
				}
				rtn = rtn.Substring(0, rtn.Length - 1);
				return rtn;
			}
		}

		#endregion

		#region METHODS
		/// <summary>
		/// Returns array of strings broken up based on the size passed.
		/// </summary>
		/// <param name="str">string to break up</param>
		/// <param name="size">max length of string</param>
		/// <returns>array of System.String[] array</returns>
		public string[] BreakUp(string str, int size)
		{
			int items = str.Length / size + 1;
			string[] ret = new string[items];
			int ii;
			for (ii = 0; ii < items - 1; ii++)
			{
				ret[ii] = str.Substring(ii * size, size);
			}
			ret[ii] = str.Substring(ii * size);
			return ret;
		}
		/// <summary>
		/// Finds integer portion of the member string in string class beginning at start.
		/// </summary>
		/// <param name="start">integer position to start looking for integer in string</param>
		/// <param name="Value">out int the integer this function finds</param>
		/// <returns>the end of the integer portion of the string.</returns>
		public int FindInteger(int start, out int Value)
		{
			Value = -1; //Initial Value.
			if (start >= this.mString.Length)
				return -1;

			bool breakout = false;
			string integer = "";
			int ii;
			for (ii = start; ii < this.mString.Length; ii++)
			{
				string ch = this.mString.Substring(ii, 1);
				switch (ch)
				{
					case "0":
					case "1":
					case "2":
					case "3":
					case "4":
					case "5":
					case "6":
					case "7":
					case "8":
					case "9":
						integer += ch;
						break;
					case "-":
						if (ii == start)
							integer += ch;
						else
							breakout = true;
						break;
					default:
						if (ii == start)
							return -1;

						breakout = true;
						break;
				}
				if (breakout)
					break;
			}
			Value = this.ToInt(integer);
			return ii - 1;
		}
		/// <summary>
		/// Wrapper for system.string.IndexOf(string in).  Returns boolean if CompareString contains c.
		/// </summary>
		/// <param name="c">string:  Substring you are looking for</param>
		/// <param name="CompareString">string: String that may have c in it.</param>
		/// <returns>bool</returns>
		/// <seealso cref="InString(string c, string CompareString)"/>
		public bool InString(string c, string CompareString)
		{
			if (CompareString.IndexOf(c) > -1)
				return true;
			else
				return false;
		}
		/// <summary>
		/// compares member string to Substring
		/// </summary>
		/// <param name="SubString">System.string</param>
		/// <returns>bool</returns>
		/// <seealso cref="public bool InString(string c, string CompareString)"/>
		public bool InThis(string SubString)
		{
			if ((this.mString.IndexOf(SubString) > -1) && (SubString != ""))
				return true;
			else
				return false;
		}
		public bool IsInt(string szLine)
		{
			return CheckInt(szLine);
		}
		public bool IsInt()
		{
			return CheckInt(this.mString);
		}

		public bool IsNumeric()
		{
			return this.CheckNumeric(this.mString);
		}

		public bool IsNumeric(string szIn)
		{
			return this.CheckNumeric(szIn);
		}

		public int ToInt()
		{
			if (this.IsNumeric())
			{
				double d = double.Parse(this.mString);
				if ((d < System.Math.Pow(2, 32)) && (d > -System.Math.Pow(2, 32)))
				{
					int ii = (int)(double.Parse(this.mString));
					return ii;
				}
				else
				{
					this.ErrorHandler("Int32 Overflow!");
					return 0;
				}
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public int ToInt(string inString)
		{
			//this.mString = inString;
			if (this.IsNumeric(inString))
			{
				double d = double.Parse(inString);
				if ((d < System.Math.Pow(2, 32)) && (d > -System.Math.Pow(2, 32)))
				{
					int ii = (int)(double.Parse(inString));
					return ii;
				}
				else
				{
					this.ErrorHandler("Int32 Overflow!");
					return 0;
				}
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public double ToDouble()
		{
      //double ret;

      //if (double.TryParse(this.mString, out ret))
      //  return ret;
      //else
      //{
      //  this.ErrorHandler("String is not Numeric!");
      //  return 0;
      //}

      if (this.IsNumeric())
      {
        if (!double.IsNaN(this.ConstantLast))
          return this.ConstantLast;
        double d = double.Parse(this.mString);
        return d;
      }
      else
      {
        this.ErrorHandler("String is not Numeric!");
        return 0;
      }
		}
		public void KillChar(ref string szIn, char cKill)
		{
			string[] strings = szIn.Split(cKill);
			szIn = "";
			foreach (string ass in strings)
				szIn += ass;
		}
		public double ToDouble(char KillChar)
		{
			string s = "";
			char[] arc = this.mString.ToCharArray();
			foreach (char c in arc)
			{
				if (c != KillChar)
					s += c.ToString();
			}
			this.mString = s;

			if (this.IsNumeric())
			{
				if (!double.IsNaN(this.ConstantLast))
					return this.ConstantLast;
				double d = double.Parse(this.mString);
				return d;
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public double ToDouble(string StringIn)
		{

      double ret;
      if (double.TryParse(StringIn, out ret))
        return ret;
      else

      //if (this.IsNumeric(StringIn))
      //{
      //  if (!double.IsNaN(this.ConstantLast))
      //    return this.ConstantLast;
      //  double d = double.Parse(StringIn);
      //  return d;
      //}
      //else
      {
        this.ErrorHandler("String is not Numeric!");
        return 0;
      }
		}
		public int Parse(char cDelim)
		{
			this.mszarParsed = this.mString.Split(cDelim);
			this.mnCount = this.mszarParsed.Length;
			this.mnCurrentIndex = 0;
			return this.mnCount;
		}
		/// <summary>
		/// Returns a string of one character.  This is the next character to the string pointer.  This function advances the string pointer
		/// </summary>
		/// <returns>string of on character</returns>
		public string GetItem()
		{
			if (this.mnCurrentIndex < this.mnCount)
			{
				this.mnCurrentIndex++;
				return this.mszarParsed[this.mnCurrentIndex - 1]; //Have to increment first due to the Return Statement.
			}
			else
				return "";
		}
		public void Rewind()
		{
			this.mnCurrentIndex = 0;
			this.mnCurrentPosition = 0;
		}
		public void Rewind(int amount)
		{
			if (this.mnCurrentPosition >= amount)
				this.mnCurrentPosition -= amount;
			else
				this.mnCurrentPosition = 0;
		}
		/// <summary>
		/// Returns a substring from the current position and advancess the string pointer by ii.
		/// </summary>
		/// <param name="ii">number of characters to return and advance the string pointer by.</param>
		/// <returns>string</returns>
		/// <seealso cref="GetLeft(int ii)"/>
		public string GetLeft(int ii)
		{
			if (this.mnCurrentPosition + ii <= this.mString.Length)
			{
				string szReturn = this.mString.Substring(this.mnCurrentPosition, ii);
				this.mnCurrentPosition += ii;
				return szReturn;
			}
			else
				return "";
		}
		/// <summary>
		/// Returns a substring from the current position without affecting the string pointer.
		/// </summary>
		/// <param name="ii">integer number of characters to return</param>
		/// <returns>string</returns>
		/// <seealso cref="GetLeft(int ii)"/>
		public string GetLeftNoAdv(int ii)
		{
			if (this.mnCurrentPosition + ii <= this.mString.Length)
			{
				string szReturn = this.mString.Substring(this.mnCurrentPosition, ii);
				return szReturn;
			}
			else
				return "";
		}
		public string GetRemaining
		{
			get { return this.mString.Substring(this.mnCurrentPosition, this.mString.Length - this.mnCurrentPosition); }
		}
		public bool Peek(ref string nextChar)
		{
			int ii = 0;
			nextChar = " ";
			while (nextChar == " ")
			{
				if (this.mnCurrentPosition + ii < this.mString.Length)
					nextChar = this.mString.Substring(this.mnCurrentPosition + ii, 1);
				else
					return false;
			}
			return true;
		}
		public void NukeWhiteSpace()
		{
			string newstring = "";
			string ch = "";
			for (int ii = 0; ii < this.mString.Length; ii++)
			{
				ch = this.mString.Substring(ii, 1);
				if (!char.IsWhiteSpace(ch, 0))
					newstring += ch;
			}
			this.mString = newstring;
		}
		public void NukeWhiteSpace(ref string sz)
		{
			string newstring = "";
			string ch = "";
			for (int ii = 0; ii < sz.Length; ii++)
			{
				ch = sz.Substring(ii, 1);
				if (!char.IsWhiteSpace(ch, 0))
					newstring += ch;
			}
			sz = newstring;
		}
		#endregion

		#region PRIVATE METHODS
		private bool CheckInt(string szLine)
		{
			bool bInt = true;
			if (szLine.Length == 0)
				bInt = false;
			int nCount = 0;
			for (int ii = 0; ii < szLine.Length; ii++)
			{
				switch (szLine.Substring(ii, 1))
				{
					case "-":
						if (nCount > 0) //sign only valid in first character.
							bInt = false;
						break;
					case "+":
						if (nCount > 0) //sign only valid in first character.
							bInt = false;
						break;
					case "0":
						break;
					case "1":
						break;
					case "2":
						break;
					case "3":
						break;
					case "4":
						break;
					case "5":
						break;
					case "6":
						break;
					case "7":
						break;
					case "8":
						break;
					case "9":
						break;
					default:
						bInt = false;
						break;
				}
				if (!bInt)
					break;
				nCount++;
			}
			return bInt;
		}

		private bool InList(string szIn)
		{
			string ch = szIn.Substring(0, 1);
			bool minus = false;
			if (ch == "-")
			{
				ch = szIn.Substring(1, szIn.Length - 1);
				minus = true;
			}
			else
				ch = szIn;

			foreach (string key in this.ConstantKeys)
			{
				if (ch == key)
				{
					this.ConstantLast = minus ? -(double)this.Constants[ch] : (double)this.Constants[ch];
					return true;
				}
			}
			this.ConstantLast = double.NaN;
			return false;
		}

		private bool CheckNumeric(string szLine)
		{
			if (szLine == "")
				return false;

			if (this.InList(szLine))
				return true;

			try
			{
				double dog;
				return double.TryParse(szLine, out dog);
			}
			catch
			{
				return false;
			}
		}
		#endregion

	}
	
	namespace StringCalc
	{
		/// <summary>
		/// 
		/// To utilize string calc, simply call SimpleCalc(string myString)
		/// where myString is an equation.  
		/// 
		/// To get more fancy assign variables and use them
		/// 
		/// To break the steps down, you can call SetOrderOfOperations2(string myString)
		/// to see how the string will be sent to the parser.
		/// 
		/// Then call parse(string returnfromSetOrderOfOperations2)
		/// 
		/// To add operators, first you will need to modify code in:
		/// 
		/// SetOrderOfOperation2
		/// parse
		/// ReturnThisLevel
		/// WhoIsBigger
		/// 
		/// Written for a veritety of purposes for Electroimpact by Todd W. Rudberg.
		/// </summary>
		/// 
    public interface IVariables
    {
      void _AssignVariable(string Tag, double Value);
      void _RemoveVariable(string Tag);
      double _GetVariable(string Tag);
      void KillScope();
      IList _GetAllVariables();
      IList _GetAllVariableNames();
      bool _ContainsVariable(string Tag);
    }
		public class cStringCalc : IVariables
		{
			#region MEMBERS
			private csString Operators = new csString("+ * / ^ = > < & |");
			private csString fns = new csString("sin asin cos acos tan atan abs exp ln log rnd sqrt int ! fact sign");
			private cVariables _vars = new cVariables();
			private cMacroVars _MacroVars;

			private enum eWhoIsBigger
			{
				Tied = 0,
				Op1,
				Op2
			}

			private bool _Degrees = true;
			#endregion

			#region CONSTRUCTOR
			public cStringCalc()
			{
				//this._MacroVars = new cMacroVars();
			}
			public cStringCalc(FANUC.OpenCNC CNC)
			{
				this._MacroVars = new cMacroVars(CNC);
			}
			#endregion

			#region PROPERTIES
			public bool Degrees
			{
				get { return this._Degrees; }
				set { this._Degrees = value; }
			}
			public double Pi
			{
				get { return System.Math.PI; }
			}
			public string Functions
			{
				get { return this.fns.String; }
			}

			#endregion

			#region PUBLIC METHODS
			public double SimpleCalc(string Expr)
			{
				string dog = SetOrderOfOperation2(Expr);
				return parse(dog);
			}
			public string SetOrderOfOperation2(csString Expr)
			{
				return this.SetOrderOfOperation2(Expr, 0, false);
			}
			public string SetOrderOfOperation2(string Expr)
			{
				csString cs = new csString(Expr);
				return this.SetOrderOfOperation2(cs);
			}
			private string SetOrderOfOperation2(csString Expr, int RecursionLevel, bool InaBracket)
			{
				string ch;
				string Op1 = "";
				string Op2 = "";
				string Arg1 = "";
				string Arg2 = "";
				string hold = "";
				string sz = "";
				bool first = true;

				Expr.NukeWhiteSpace(); //Whitespace is evil.

				//Catch Evil Leading minus signs.
				ch = Expr.GetLeftNoAdv(1);
				if (ch == "-")
				{
					ch = Expr.GetLeft(1);
					string fixit = Expr.GetRemaining;
					fixit = "-1*" + fixit;
					Expr = new csString(fixit);
				}

				while ((ch = Expr.GetLeft(1)) != "")
				{
					switch (ch)
					{
						case " ":
							break;
						case "{":
						case "(":
						case "[":
							{
								int LP = 1;
								csString substr = new csString(Expr.GetRemaining);
								string newstring = "";
								string ss;
								string lp = ch;
								int ii = 0;
								while (LP > 0)
								{
									ss = substr.GetLeft(1);
									if (ss == "")
										break;
									switch (ss)
									{
										case "{":
										case "(":
										case "[":
											{
												LP++;
												break;
											}
										case "}":
										case ")":
										case "]":
											LP--;
											break;
									}
									newstring += ss;
									ii++;
								}
								string rp = newstring.Substring(newstring.Length - 1, 1);
								newstring = newstring.Substring(0, newstring.Length - 1);
								csString newexpr = new csString(newstring);
								newstring = lp + this.SetOrderOfOperation2(newexpr, 0, false) + rp;
								hold += newstring;
								Expr.Rewind(-ii);
								break;
							}
						case "}":
						case ")":
						case "]":
							break;

						case "+":
						case "*":
						case "/":
						case "^":
						case "=":
						case ">":
						case "<":
						case "&":
						case "|":
						case "-":
							if (ch == "-")
							{
								if (((Op1 != "") || first) && (Arg1 == "") && (hold == ""))
								{
									hold += ch;
									//Just put this here...it gets wiped out later, but indicates we have part of Arg1 for future serches.
									break;
								}
								else if ((Op1 != "") && (hold == ""))
								{
									hold += ch;
									break;
								}
							}
							if (Op1 == "")
							{
								Arg1 = hold;
								Op1 = ch;
								hold = "";
							}
							else
							{
								if (this.IsE(hold))
								{
									hold += ch;
									break;
								}
								Arg2 = hold;
								hold = "";
								Op2 = ch;
								eWhoIsBigger ii = this.WhoIsBigger(Op1, Op2);
								if (ii == eWhoIsBigger.Tied)
								{
									sz += Arg1 + Op1 + Arg2;
									int rwd = Op2.Length;
									Expr.Rewind(rwd);
									Arg1 = Arg2 = Op1 = Op2 = "";
								}
								else if (ii == eWhoIsBigger.Op1)
								{
									int rwd = Op2.Length;
									Expr.Rewind(rwd);
									sz += Arg1 + Op1 + Arg2;
									Arg1 = Arg2 = Op1 = Op2 = "";
								}
								else //Op2 is bigger.
								{
									int amt = Arg2.Length + Op2.Length;
									Expr.Rewind(amt);
									csString expr = this.ReturnThisLevel(Expr, Op1);
									string szRtn = SetOrderOfOperation2(expr, RecursionLevel + 1, false);
									hold = "(" + szRtn;
									Op2 = Arg2 = "";
								}
							}
							break;
						default:
							hold += ch;
							break;
					}
					first = false;
				}

				if (RecursionLevel > 0)
					return sz + Arg1 + Op1 + hold + ")";
				else
					return sz + Arg1 + Op1 + hold;
			}
			public double parse(string expr)			
			{
				csString cs = new csString(expr);
				return this.parse(cs);
			}
			public double parse(csString expr)
			{
				csString aString = new csString();
				string ch;
				string Operand = "";
				string Operator = "";
				double Arg1 = double.NaN;
				bool first = true;
				while ((ch = expr.GetLeft(1)) != "")
				{
					switch (ch)
					{
						case " ":
							break;
						case "{":
						case "(":
						case "[":
							{
								if (Operator != "")
								{
									double Arg2;
									string minus = "";

									if (Operand != "")
									{
										minus = Operand.Substring(0, 1);
										if (minus == "-")
											Operand = Operand.Substring(1, Operand.Length - 1);
									}
									if (fns.InThis(Operand)) //Mono Variable Functions
									{
										double Arg = this.parse(expr);
										Arg2 = this.CalculateMonoVar(Operand, Arg);
									}
									else //Regular Functions
									{
										Arg2 = parse(expr);
									}
									if (minus == "-")
										Arg2 = -Arg2;
									Arg1 = this.Calculate(Operator, Arg1, Arg2);
									Operand = "";
									Operator = "";
								}
								else
								{
									string minus = "";
									if (Operand != "")
									{
										minus = Operand.Substring(0, 1);
										if (minus == "-")
											Operand = Operand.Substring(1, Operand.Length - 1);
									}
									if (fns.InThis(Operand)) //Mono Variable Functions
									{
										Arg1 = this.parse(expr);
										Arg1 = this.CalculateMonoVar(Operand, Arg1);
										Operand = "";
									}
									else //Regular Functions
										Arg1 = parse(expr);
									if (minus == "-")
										Arg1 = -1;
								}
								break;
							}
						case "}":
						case ")":
						case "]":
							{
								if (Operator != "")
								{
									if (this.IsANumber(Operand))
										return this.Calculate(Operator, Arg1, this.ToDouble(Operand));
									else
										return Arg1; //Should have already calculated at this point.  (1+(1)).
								}
								else if (this.IsANumber(Operand))
									return this.ToDouble(Operand);  //1+(1)
								else
									return Arg1;  //Give Back what we recieved. ((1+1))
							}
						case "+":
						case "*":
						case "/":
						case "^":
						case "=":
						case ">":
						case "<":
						case "&":
						case "|":
							{
								if (Operator != "")
								{
									if (this.IsANumber(Operand))
										Arg1 = this.Calculate(Operator, Arg1, this.ToDouble(Operand));
								}
								else
								{
									if (this.IsANumber(Operand))
										Arg1 = this.ToDouble(Operand);
								}
								Operand = "";
								Operator = ch;
								break;
							}
						case "-":
							{
								if (Operator != "")
								{
									if (this.IsANumber(Operand))
									{
										if (this.IsE(Operand))
										{
											Operand += ch;
											break;
										}
										Arg1 = this.Calculate(Operator, Arg1, this.ToDouble(Operand));
										Operand = "";
										Operator = ch;
									}
									else
									{
										Operand += ch;
										break;
									}
								}
								else
								{
									if (this.IsANumber(Operand))
									{
										if (this.IsE(Operand))
										{
											Operand += ch;
											break;
										}
										Arg1 = this.ToDouble(Operand);
										Operator = ch;
										Operand = "";
									}
									else if (first)
										Operand += ch;
									else
									{
										Operator = ch;
										Operand = "";
									}
								}
								break;
							}
						default:
							{
								Operand += ch;
								break;
							}
					}
					first = false;
				}
				if (this.IsANumber(Operand))
				{
					if ((Operator != "") && (Operand != ""))
						return this.Calculate(Operator, Arg1, this.ToDouble(Operand));
					else if (this.IsANumber(Operand))
						return this.ToDouble(Operand);
					return double.NaN;
				}
				else if (Operand == "")
					return Arg1;
				else
					return double.NaN;
			}

			public void KillAllVars()
			{
				this._vars.KillAllVars();
				//			this._MacroVars.ClearMacroVars();
			}
			#endregion

			#region PRIVATE METHODS
			private bool IsANumber(string operand)
			{
				csString css = new csString(operand);
				return css.IsNumeric() || this._vars._ContainsVar(operand) || (this._MacroVars != null && this._MacroVars.IsMacroVar(operand));
			}
			private double ToDouble(string operand)
			{
				csString css = new csString(operand);
				if (this._vars._ContainsVar(operand))
					return this._vars._GetVariable(operand);
				else if( this._MacroVars != null && this._MacroVars.IsMacroVar(operand) )
						return this._MacroVars.ToDouble(operand);
				else
					return css.ToDouble();
			}
			/// <summary>
			/// Checks last character in a string for e.  However, if the 
			/// characters prior to e are not numeric, this returns false.
			/// The purpose of this function is to determin if the "e"
			/// stands for exp or a power of ten function.
			/// </summary>
			/// <param name="Operand">string to be evaluated</param>
			/// <returns>bool</returns>
			private bool IsE(string Operand)
			{
				csString cs = new csString(Operand);
				if( !cs.IsNumeric(cs.GetLeft(cs.Length-1)) )
					return false;
				string LastChar = Operand.Substring(Operand.Length - 1, 1);
				return LastChar == "e";
			}
			private csString ReturnThisLevel(csString Expr, string LastOp)
			{
				string ch = "";
				string hold = "";
				string Op1 = "";
				string Op2 = "";
				string arg = "";
				int LP = 0;

				csString rtn;
				while ((ch = Expr.GetLeft(1)) != "")
				{
					switch (ch)
					{
						case " ":
							break;
						case "{":
						case "(":
						case "[":
							{
								hold += ch;
								LP++;
								break;
							}
						case "}":
						case ")":
						case "]":
							{
								hold += ch;
								LP--;
								break;
							}
						case "+":
						case "*":
						case "/":
						case "^":
						case "=":
						case ">":
						case "<":
						case "&":
						case "|":
						case "-":
							if (ch == "-")
							{
								string Test = "";
								Expr.Peek(ref Test);

								if (arg == "")
								{
									hold += ch;
									break;
								}
							}
							if (Op1 == "")
							{
								Op1 = ch;
								hold += Op1;
								arg = "";
							}
							else
							{
								Op2 = ch;
								arg = "";
								if (LP == 0)
								{
									if (this.WhoIsBigger(Op1, Op2) == eWhoIsBigger.Op1)
									{
										eWhoIsBigger who = this.WhoIsBigger(Op2, LastOp);
										if (who == eWhoIsBigger.Tied || who == eWhoIsBigger.Op2)
										{
											rtn = new csString(hold);
											Expr.Rewind(Op2.Length);
											return rtn;
										}
									}
								}
								hold += Op2;
								Op1 = Op2;
								Op2 = "";
							}
							break;
						default:
							arg += ch;
							hold += ch;
							break;
					}
				}
				rtn = new csString(hold);
				return rtn;
			}
			private eWhoIsBigger WhoIsBigger(string Op1, string Op2)
			{
				System.Collections.Hashtable myOpHash = new System.Collections.Hashtable();
				myOpHash.Add("<", 1);
				myOpHash.Add(">", 1);
				myOpHash.Add("=", 1);
				myOpHash.Add("&", 1);
				myOpHash.Add("|", 1);
				myOpHash.Add("-", 2);
				myOpHash.Add("+", 2);
				myOpHash.Add("/", 3);
				myOpHash.Add("*", 3);
				//			myOpHash.Add( "*", 4 ); //Changed 10 Nov 2004
				myOpHash.Add("^", 5);

				if ((int)myOpHash[Op1] == (int)myOpHash[Op2])
					return eWhoIsBigger.Tied;
				if ((int)myOpHash[Op1] > (int)myOpHash[Op2])
					return eWhoIsBigger.Op1;
				else
					return eWhoIsBigger.Op2;
			}

			private double Calculate(string Fn, double Arg1, double Arg2)
			{
				switch (Fn)
				{
					case "+":
						return Arg1 + Arg2;
					case "-":
						return Arg1 - Arg2;
					case "*":
						return Arg1 * Arg2;
					case "/":
						{
							if (Arg2 != 0)
							{
								return Arg1 / Arg2;
							}
							else
							{
								System.Windows.Forms.MessageBox.Show("Divide by Zero", "Calculate Error");
								return -1;
							}
						}
					case "^":
						return Math.Pow(Arg1, Arg2);
					case "=":
						return Arg1 == Arg2 ? 1 : 0;
					case ">":
						return Arg1 > Arg2 ? 1 : 0;
					case "<":
						return Arg1 < Arg2 ? 1 : 0;
					case "&":
						{
							bool bArg1 = Arg1 == 1;
							bool bArg2 = Arg2 == 1;
							return bArg1 && bArg2 ? 1 : 0;
						}
					case "|":
						{
							bool bArg1 = Arg1 == 1;
							bool bArg2 = Arg2 == 1;
							return bArg1 || bArg2 ? 1 : 0;
						}
					default:
						break;
				}
				return -1;
			}
			private double CalculateMonoVar(string Fn, double Arg1)
			{
				double ret;
				switch (Fn)
        {
          case "sign":
            try
            {
              return Math.Sign(Arg1);
            }
            catch (Exception ex)
            {
              System.Windows.Forms.MessageBox.Show(ex.Message, "Error in sign function");
              break;
            }
          case "sin":
            try
            {
              Arg1 = this.Degrees ? Arg1 * Pi / 180.0 : Arg1;
              return Math.Sin(Arg1);
            }
            catch (Exception ex)
            {
              System.Windows.Forms.MessageBox.Show(ex.Message, "Error in sin function");
              break;
            }

					case "asin":
						try
						{
							ret = Math.Asin(Arg1);
							return this.Degrees ? ret * 180 / Pi : ret;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in asin function");
							break;
						}

					case "cos":
						try
						{
							Arg1 = this.Degrees ? Arg1 * Pi / 180.0 : Arg1;
							return Math.Cos(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}

					case "acos":
						try
						{
							ret = Math.Acos(Arg1);
							return this.Degrees ? ret * 180 / Pi : ret;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "tan":
						try
						{
							Arg1 = this.Degrees ? Arg1 * Pi / 180.0 : Arg1;
							return Math.Tan(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "atan":
						try
						{
							ret = Math.Atan(Arg1);
							return this.Degrees ? ret * 180 / Pi : ret;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "abs":
						return Math.Abs(Arg1);
					case "exp":
						try
						{
							return Math.Exp(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "ln":
						try
						{
							Arg1 = Math.Log(Arg1, Math.E);
							return Arg1;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "log":
						try
						{
							return Math.Log10(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "rnd":
						return Math.Round(Arg1, 0);
					case "sqrt":
						try
						{
							return Math.Sqrt(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "int":
						return (long)Arg1;
					case "!":
						Arg1 = Arg1 == 1 ? 0 : 1;
						return Arg1;
					case "fact":
						int Number = (int)Arg1;
						ret = Number;
						if (Number > 0)
						{
							while (Number > 1)
							{
								ret *= --Number;
							}
							return ret;
						}
						else
						{
							System.Windows.Forms.MessageBox.Show("Cannot have negative number for Factorials");
							break;
						}
				}
				return 0;
			}
			#endregion

			#region Variable classes

			public class cVariable
			{
				string _tag;
				double _value;
        string _note;

				public cVariable()
				{
					this._tag = "";
					this._value = 0;
				}

				public cVariable(string Tag, double Value)
				{
					this._tag = Tag;
					this._value = Value;
				}


        public cVariable(string Tag, double Value, string note)
        {
          this._tag = Tag;
          this._value = Value;
          this._note = note;
        }

				public string Tag
				{
					get { return this._tag; }
					set { this._tag = value; }
				}

				public double Value
				{
					get { return this._value; }
					set { this._value = value; }
				}

        public string Note
        {
          get { return this._note; }
          set { this._note = value; }
        }

				public override string ToString()
				{
					return this._tag + " " + this._value.ToString();
				}
			}

			private class cVariables : System.Collections.SortedList
			{
				private uint _cntscope;
				private uint _cntsubscope;

				private class cScope
				{
					public uint scope;
					public uint subscope;

					public cScope(uint scope, uint subscope)
					{
						this.scope = scope;
						this.subscope = subscope;
					}
				}
				System.Collections.ArrayList scopes = new ArrayList();


				//HashTable Format will be "scope.subscope.tag", this must be unique.
				public cVariables()
				{
					this._cntscope = 0;
					this._cntsubscope = 0;
				}

				public void _AssignVariable(cVariable var)
				{
					string key = this._createkey(var.Tag);
					if (this.ContainsKey(key))
					{
						cVariable avar = (cVariable)this[key];
						avar.Value = var.Value;
					}
					else
						this.Add(key, var);
				}

				public void _AssignVariable(string Tag, double Value)
				{
					cVariable newvar = new cVariable(Tag, Value);
					this._AssignVariable(newvar);
				}

        public void _AssignVariableNote(string Tag, string note)
        {
          string key = this._createkey(Tag);
          if (this.ContainsKey(key))
          {
            ((cVariable)this[key]).Note = note;
          }
        }

				public void _RemoveVariable(string Tag)
				{
					string key = this._createkey(Tag);
					try
					{
						this.Remove(key);
					}
					catch
					{
						System.Windows.Forms.MessageBox.Show(Tag + " Doesn't exist...dumbass!.");
					}
				}

        /// <summary>
        /// Returns the double variable value
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
				public double _GetVariable(string Tag)
				{
					string Key = this._createkey(Tag);
					if (this.ContainsKey(Key))
					{
						return ((cVariable)this[Key]).Value;
					}
					return 0.0;
				}

        /// <summary>
        /// Returns the double variable value
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
        public string _GetVariableNote(string Tag)
        {
          string Key = this._createkey(Tag);
          if (this.ContainsKey(Key))
          {
            return ((cVariable)this[Key]).Note;
          }
          return null;
        }



				public bool _VariableExists(string Tag)
				{
					string Key = this._createkey(Tag);
					if (this.ContainsKey(Key))
					{
						return true;
					}
					else
					{
						return false;
					}
				}


				public uint _Scope
				{
					get { return this._cntscope; }
					set
					{
						if (value > this._cntscope)
						{
							cScope pastscope = new cScope(this._cntscope, this._cntsubscope);
							this.scopes.Add(pastscope);
							this._cntsubscope = 0;
							this._cntscope = value;
						}
						else
						{
							if (value >= 0)
							{
								System.Collections.IList keys = this.GetKeyList();
								for (int ii = (keys.Count - 1); ii >= 0; ii--)
								{
									string[] pcs = ((string)keys[ii]).Split('.');
									int scope = int.Parse(pcs[0]);
									if (scope == this._cntscope)
										this.Remove(keys[ii]);
								}
								cScope thisscope = (cScope)this.scopes[this.scopes.Count - 1];
								if ((this._cntscope - 1) != thisscope.scope)
									System.Windows.Forms.MessageBox.Show("ScopeError");
								this._cntsubscope = thisscope.subscope;
								this.scopes.RemoveAt(this.scopes.Count - 1);
								this._cntscope = value;
							}
						}
					}
				}
				public uint _SubScope
				{
					get { return this._cntsubscope; }
					set
					{
						if (this._cntsubscope > value)
						{
							System.Collections.IList list = this.GetKeyList();
							for (int ii = (list.Count - 1); ii >= 0; ii--)
							{
								string[] pcs = ((string)list[ii]).Split('.');
								int scope = int.Parse(pcs[0]);
								int subscope = int.Parse(pcs[1]);
								if ((scope == this._Scope && subscope >= this._SubScope) || scope > this._Scope)
									this.Remove(list[ii]);
							}
						}
						this._cntsubscope = value;
					}
				}

				public bool _ContainsVar(string Tag)
				{
					string key = this._createkey(Tag);
					return this.ContainsKey(key);
				}

				private string _createkey(string Tag)
				{
					return this._cntscope.ToString() + "." + this._cntsubscope.ToString() + "." + Tag;
				}
				public void KillAllVars()
				{
					System.Collections.IList keys = this.GetKeyList();
					for (int ii = keys.Count - 1; ii >= 0; ii--)
					{
						this.Remove(keys[ii]);
					}
				}
				public void KillScope()
				{
					System.Collections.IList keys = this.GetKeyList();
					for (int ii = keys.Count - 1; ii >= 0; ii--)
					{
						string scopePrefix = keys[ii].ToString();
						int index = scopePrefix.IndexOf(".");
						index = scopePrefix.IndexOf(".", index + 1);
						scopePrefix = scopePrefix.Substring(0, index);
						string compareTo = this._cntscope.ToString() + "." + this._cntsubscope.ToString();
						if (scopePrefix == compareTo)
						{
							this.Remove(keys[ii]);
						}
					}
				}

			}

			private class cMacroVars
			{
				Electroimpact.FANUC.OpenCNC _CNC;

				public cMacroVars()
				{
					FANUC.Err_Code myErr;
					this._CNC = new FANUC.OpenCNC(0, out myErr);
				}

				public cMacroVars(FANUC.OpenCNC CNC)
				{
					this._CNC = CNC;
				}
				public bool IsMacroVar(string operand)
				{
					csString Operand = new csString(operand);
					if (Operand.GetLeft(1) == "#")
					{
						int num;
						Operand.FindInteger(1, out num);
						if (num > 0 && num < 9999)
							return true;
					}
					return false;
				}
				public double ToDouble(string operand)
				{
					csString cs = new csString(operand);
					int macnum;
					cs.FindInteger(1, out macnum);
					if (macnum > 0 && macnum < 9999)
					{
						return this._CNC.ReadMacroVariable((short)macnum);
					}
					return double.NaN;
				}
				/// <summary>
				/// Clears macro variables #100-#199. This will
				/// be used when a new program is loaded or when
				/// M2, M30, or M31 is run. Closer to the way
				/// the A380 control program runs.
				/// </summary>
				public void ClearMacroVars()
				{
					for (short ii = 100; ii < 200; ii++)
					{
						this._CNC.WriteMacroVariable(ii, 0);
					}
				}
			}

			#endregion

			#region IVariables Members

			public void _AssignVariable(string Tag, double Value)
			{
				this._vars._AssignVariable(Tag, Value);
			}

      public void _AssignVariableNote(string Tag, string note)
      {
        this._vars._AssignVariableNote(Tag, note);
      }

			public void _RemoveVariable(string Tag)
			{
				this._vars._RemoveVariable(Tag);
			}

			public double _GetVariable(string Tag)
			{
				return this._vars._GetVariable(Tag);
			}

      public string _GetVariableNote(string Tag)
      {
        return this._vars._GetVariableNote(Tag);
      }

			public IList _GetAllVariables()
			{
				return this._vars.GetValueList();
			}

			public IList _GetAllVariableNames()
			{
				return this._vars.GetKeyList();
			}

			public void KillScope()
			{
				this._vars.KillScope();
			}

      public bool _ContainsVariable(string Tag)
      {
        return this._vars._VariableExists(Tag);
      }

			#endregion

    }
	}
}