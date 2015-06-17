// Generated from Test.ecs by LeMP custom tool. LLLPG version: 1.3.0.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --no-out-header       Suppress this message
// --verbose             Allow verbose messages (shown by VS as 'warnings')
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// Use #importMacros to use macros in a given namespace, e.g. #importMacros(Loyc.LLPG);
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Loyc;
using Loyc.Syntax;
using Loyc.Syntax.Lexing;
struct StringToken : ISimpleToken<string>
{
	public string Type
	{
		get;
		set;
	}
	public object Value
	{
		get {
			return Type;
		}
	}
	public int StartIndex
	{
		get;
		set;
	}
}
class ExprParser : BaseParserForList<StringToken,string>
{
	public ExprParser(string input) : this(input.Split(' ').Select(word => new StringToken { 
			Type = word
		}).ToList())
	{
	}
	public ExprParser(IList<StringToken> tokens, ISourceFile file = null) : base(tokens, default(StringToken), file ?? EmptySourceFile.Unknown)
	{
		F = new LNodeFactory(SourceFile);
	}
	protected override string ToString(string tokenType)
	{
		return tokenType;
	}
	LNodeFactory F = new LNodeFactory(EmptySourceFile.Unknown);
	LNode Op(LNode lhs, StringToken op, LNode rhs)
	{
		return F.Call((Symbol) op.Type, lhs, rhs, lhs.Range.StartIndex, rhs.Range.EndIndex);
	}
	public LNode Expr(int prec = 0)
	{
		string la0, la1;
		LNode got_Expr = default(LNode);
		StringToken lit_dash = default(StringToken);
		StringToken litx3D = default(StringToken);
		LNode result = default(LNode);
		LNode rhs = default(LNode);
		// Line 40: ("-" Expr / Atom)
		la0 = (string) LA0;
		if (la0 == "-") {
			la1 = (string) LA(1);
			if (la1 != EOF) {
				lit_dash = MatchAny();
				var r = Expr(50);
				// line 40
				result = F.Call((Symbol) "-", r, lit_dash.StartIndex, r.Range.EndIndex);
			} else
				result = Atom();
		} else
			result = Atom();
		// Line 45: greedy( &{prec <= 10} @"=" Expr | &{prec < 20} (@"&&"|@"||") Expr | &{prec < 30} (@"!="|@"<"|@"<="|@"=="|@">"|@">=") Expr | &{prec < 40} (@"-"|@"+") Expr | &{prec < 50} (@"*"|@"/"|@"<<"|@">>") Expr | @"(" Expr @")" | @"." Atom )*
		for (;;) {
			switch ((string) LA0) {
			case @"=":
				{
					if (prec <= 10) {
						la1 = (string) LA(1);
						if (la1 != EOF) {
							litx3D = MatchAny();
							var r = Expr(10);
							// line 47
							result = Op(result, litx3D, r);
						} else
							goto stop;
					} else
						goto stop;
				}
				break;
			case @"&&":
			case @"||":
				{
					if (prec < 20) {
						la1 = (string) LA(1);
						if (la1 != EOF) {
							var op = MatchAny();
							var r = Expr(20);
							// line 50
							result = Op(result, op, r);
						} else
							goto stop;
					} else
						goto stop;
				}
				break;
			case @"!=":
			case @"<":
			case @"<=":
			case @"==":
			case @">":
			case @">=":
				{
					if (prec < 30) {
						la1 = (string) LA(1);
						if (la1 != EOF) {
							var op = MatchAny();
							var r = Expr(30);
							// line 53
							result = Op(result, op, r);
						} else
							goto stop;
					} else
						goto stop;
				}
				break;
			case @"-":
			case @"+":
				{
					if (prec < 40) {
						la1 = (string) LA(1);
						if (la1 != EOF) {
							var op = MatchAny();
							var r = Expr(40);
							// line 56
							result = Op(result, op, r);
						} else
							goto stop;
					} else
						goto stop;
				}
				break;
			case @"*":
			case @"/":
			case @"<<":
			case @">>":
				{
					if (prec < 50) {
						la1 = (string) LA(1);
						if (la1 != EOF) {
							var op = MatchAny();
							var r = Expr(50);
							// line 59
							result = Op(result, op, r);
						} else
							goto stop;
					} else
						goto stop;
				}
				break;
			case @"(":
				{
					la1 = (string) LA(1);
					if (la1 != EOF) {
						Skip();
						got_Expr = Expr();
						Match(@")");
						// line 61
						result = F.Call(result, got_Expr, result.Range.StartIndex);
					} else
						goto stop;
				}
				break;
			case @".":
				{
					la1 = (string) LA(1);
					if (la1 != EOF) {
						Skip();
						rhs = Atom();
						// line 63
						result = F.Dot(result, rhs, result.Range.StartIndex);
					} else
						goto stop;
				}
				break;
			default:
				goto stop;
			}
		}
	stop:;
		return result;
	}
	LNode PrefixExpr()
	{
		string la0, la1;
		StringToken lit_dash = default(StringToken);
		LNode result = default(LNode);
		// Line 67: (@"-" PrefixExpr / PrimaryExpr)
		la0 = (string) LA0;
		if (la0 == @"-") {
			la1 = (string) LA(1);
			if (la1 != EOF) {
				lit_dash = MatchAny();
				var r = PrefixExpr();
				// line 67
				result = F.Call((Symbol) "-", r, lit_dash.StartIndex, r.Range.EndIndex);
			} else
				result = PrimaryExpr();
		} else
			result = PrimaryExpr();
		return result;
	}
	LNode PrimaryExpr()
	{
		string la0;
		LNode got_Expr = default(LNode);
		LNode result = default(LNode);
		LNode rhs = default(LNode);
		result = Atom();
		// Line 73: (@"(" Expr @")" | @"." Atom)*
		for (;;) {
			la0 = (string) LA0;
			if (la0 == @"(") {
				Skip();
				got_Expr = Expr();
				Match(@")");
				// line 73
				result = F.Call(result, got_Expr, result.Range.StartIndex);
			} else if (la0 == @".") {
				Skip();
				rhs = Atom();
				// line 74
				result = F.Dot(result, rhs, result.Range.StartIndex);
			} else
				break;
		}
		return result;
	}
	LNode Atom()
	{
		string la0, la1;
		LNode result = default(LNode);
		StringToken tok__ = default(StringToken);
		// Line 78: (@"(" Expr @")" / ~(EOF))
		do {
			la0 = (string) LA0;
			if (la0 == @"(") {
				la1 = (string) LA(1);
				if (la1 != EOF) {
					Skip();
					result = Expr();
					Match(@")");
					// line 78
					result = F.InParens(result);
				} else
					goto match2;
			} else
				goto match2;
			break;
		match2:
			{
				tok__ = MatchExcept();
				// line 80
				double n;
				result = double.TryParse(tok__.Type, out n) ? F.Literal(n) : F.Id(tok__.Type);
			}
		} while (false);
		return result;
	}
}
