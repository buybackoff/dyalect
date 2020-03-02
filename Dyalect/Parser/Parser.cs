
using System;
using System.Linq;
using System.Collections.Generic;
using Dyalect.Parser.Model;


namespace Dyalect.Parser
{
    partial class InternalParser
    {
	public const int _EOF = 0;
	public const int _identToken = 1;
	public const int _preprocessor = 2;
	public const int _intToken = 3;
	public const int _floatToken = 4;
	public const int _stringToken = 5;
	public const int _charToken = 6;
	public const int _implicitToken = 7;
	public const int _verbatimStringToken = 8;
	public const int _varToken = 9;
	public const int _constToken = 10;
	public const int _funcToken = 11;
	public const int _returnToken = 12;
	public const int _continueToken = 13;
	public const int _breakToken = 14;
	public const int _yieldToken = 15;
	public const int _ifToken = 16;
	public const int _forToken = 17;
	public const int _whileToken = 18;
	public const int _typeToken = 19;
	public const int _arrowToken = 20;
	public const int _dotToken = 21;
	public const int _commaToken = 22;
	public const int _semicolonToken = 23;
	public const int _colonToken = 24;
	public const int _equalToken = 25;
	public const int _parenLeftToken = 26;
	public const int _parenRightToken = 27;
	public const int _curlyLeftToken = 28;
	public const int _curlyRightToken = 29;
	public const int _squareLeftToken = 30;
	public const int _squareRightToken = 31;
	public const int _minus = 32;
	public const int _plus = 33;
	public const int _not = 34;
	public const int _bitnot = 35;
	public const int maxT = 84;




        private void Get()
        {
            for (;;)
            {
                t = la;
                la = scanner.Scan();

                if (la.kind <= maxT)
                {
                    ++errDist;
                    break;
                }

                la = t;
            }
        }

	void Separator() {
		Expect(23);
	}

	void StandardOperators() {
		switch (la.kind) {
		case 33: {
			Get();
			break;
		}
		case 32: {
			Get();
			break;
		}
		case 36: {
			Get();
			break;
		}
		case 37: {
			Get();
			break;
		}
		case 38: {
			Get();
			break;
		}
		case 39: {
			Get();
			break;
		}
		case 40: {
			Get();
			break;
		}
		case 34: {
			Get();
			break;
		}
		case 41: {
			Get();
			break;
		}
		case 42: {
			Get();
			break;
		}
		case 43: {
			Get();
			break;
		}
		case 44: {
			Get();
			break;
		}
		case 45: {
			Get();
			break;
		}
		case 46: {
			Get();
			break;
		}
		case 47: {
			Get();
			break;
		}
		case 48: {
			Get();
			break;
		}
		case 49: {
			Get();
			break;
		}
		case 35: {
			Get();
			break;
		}
		default: SynErr(85); break;
		}
	}

	void FunctionName() {
		if (la.kind == 1) {
			Get();
		} else if (StartOf(1)) {
			StandardOperators();
		} else if (la.kind == 50) {
			Get();
		} else SynErr(86);
	}

	void Qualident(out string s1, out string s2, out string s3) {
		s1 = null; s2 = null; s3 = null; 
		FunctionName();
		s1 = t.val; 
		if (StartOf(2)) {
			if (la.kind == 21) {
				Get();
			}
			FunctionName();
			s2 = t.val; 
			if (StartOf(2)) {
				if (la.kind == 21) {
					Get();
				}
				FunctionName();
				s3 = t.val; 
			}
		}
	}

	void Import() {
		Expect(51);
		var inc = new DImport(t); Imports.Add(inc); string lastName = null; 
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 5) {
			Get();
		} else SynErr(87);
		lastName = ParseImport(); 
		while (la.kind == 21) {
			Get();
			if (la.kind == 1) {
				Get();
			} else if (la.kind == 5) {
				Get();
			} else SynErr(88);
			if (inc.LocalPath != null)
			   inc.LocalPath = string.Concat(inc.LocalPath, "/", lastName);
			else
			   inc.LocalPath = lastName;
			lastName = ParseImport();
			
		}
		inc.ModuleName = lastName; if (la.AfterEol) return; 
		if (la.kind == 26) {
			Get();
			Expect(5);
			inc.Dll = ParseSimpleString(); 
			Expect(27);
		}
		if (la.kind == 1) {
			if (la.AfterEol) return; 
			Get();
			inc.Alias = t.val;
			
		}
	}

	void Type(out DNode node) {
		DFunctionDeclaration f = null; 
		Expect(19);
		var typ = new DTypeDeclaration(t);
		node = typ;
		
		Expect(1);
		typ.Name = t.val; node = typ; 
		if (la.kind == 26) {
			f = new DFunctionDeclaration(t) { Name = typ.Name, IsStatic = true, IsConstructor = true, TypeName = new Qualident(typ.Name) }; 
			FunctionArguments(f);
			typ.Constructors.Add(f); 
		}
		if (la.kind == 25) {
			Get();
			Expect(1);
			f = new DFunctionDeclaration(t) { Name = t.val, IsStatic = true, IsConstructor = true, TypeName = new Qualident(typ.Name) }; typ.Constructors.Add(f); 
			FunctionArguments(f);
			while (la.kind == 39) {
				Get();
				Expect(1);
				f = new DFunctionDeclaration(t) { Name = t.val, IsStatic = true, IsConstructor = true, TypeName = new Qualident(typ.Name) }; typ.Constructors.Add(f);
				FunctionArguments(f);
			}
		}
	}

	void FunctionArguments(DFunctionDeclaration node) {
		Expect(26);
		if (la.kind == 1) {
			FunctionArgument(out var arg);
			node.Parameters.Add(arg); 
			while (la.kind == 22) {
				Get();
				FunctionArgument(out arg);
				node.Parameters.Add(arg); 
			}
		}
		Expect(27);
	}

	void Statement(out DNode node) {
		node = null; 
		switch (la.kind) {
		case 12: case 13: case 14: case 15: {
			ControlFlow(out node);
			if (la.kind == 52) {
				Guard(node, out node);
			}
			Separator();
			break;
		}
		case 19: {
			Type(out node);
			Separator();
			break;
		}
		case 58: {
			Match(out node);
			if (la.kind == 52) {
				Guard(node, out node);
				Separator();
			}
			break;
		}
		case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 26: case 28: case 30: case 32: case 33: case 34: case 35: case 50: case 63: case 64: case 65: case 68: case 69: case 83: {
			SimpleExpr(out node);
			if (la.kind == 52) {
				Guard(node, out node);
			}
			Separator();
			break;
		}
		case 16: {
			If(out node);
			if (la.kind == 52) {
				Guard(node, out node);
				Separator();
			}
			break;
		}
		case 17: case 18: case 66: {
			Loops(out node);
			if (la.kind == 52) {
				Guard(node, out node);
				Separator();
			}
			break;
		}
		case 11: case 54: case 55: case 56: {
			Function(out node);
			break;
		}
		default: SynErr(89); break;
		}
		node = ProcessImplicits(node); 
	}

	void ControlFlow(out DNode node) {
		node = null; 
		if (la.kind == 14) {
			Break(out node);
		} else if (la.kind == 13) {
			Continue(out node);
		} else if (la.kind == 12) {
			Return(out node);
		} else if (la.kind == 15) {
			Yield(out node);
		} else SynErr(90);
	}

	void Guard(DNode src, out DNode node) {
		node = src; 
		var ot = t; 
		Expect(52);
		SimpleExpr(out var cnode);
		node = new DIf(ot) { Condition = cnode, True = src }; 
		if (la.kind == 53) {
			Get();
			Expr(out cnode);
		}
	}

	void Match(out DNode node) {
		node = null; 
		Expect(58);
		var m = new DMatch(t); 
		Expr(out node);
		m.Expression = node; 
		Expect(28);
		MatchEntry(out var entry);
		m.Entries.Add(entry); 
		while (la.kind == 22) {
			Get();
			MatchEntry(out entry);
			m.Entries.Add(entry); 
		}
		Expect(29);
		node = m; 
	}

	void SimpleExpr(out DNode node) {
		node = null; 
		if (la.kind == 9 || la.kind == 10) {
			Binding(out node);
		} else if (la.kind == 50) {
			Rebinding(out node);
		} else if (IsFunction()) {
			FunctionExpr(out node);
		} else if (IsLabel()) {
			Label(out node);
		} else if (StartOf(3)) {
			Assignment(out node);
		} else if (la.kind == 69) {
			TryCatch(out node);
		} else if (la.kind == 68) {
			Throw(out node);
		} else SynErr(91);
	}

	void If(out DNode node) {
		node = null; 
		Expect(16);
		var @if = new DIf(t); 
		Expr(out node);
		@if.Condition = node; 
		Block(out node);
		@if.True = node; 
		if (la.kind == 53) {
			Get();
			if (la.kind == 28) {
				Block(out node);
				@if.False = node; 
			} else if (la.kind == 16) {
				If(out node);
				@if.False = node; 
			} else SynErr(92);
		}
		node = @if; 
	}

	void Loops(out DNode node) {
		node = null; 
		if (la.kind == 18) {
			While(out node);
		} else if (la.kind == 17) {
			For(out node);
		} else if (la.kind == 66) {
			DoWhile(out node);
			Separator();
		} else SynErr(93);
	}

	void Function(out DNode node) {
		node = null; bool st = false; bool auto = false; bool priv = false; 
		if (la.kind == 54) {
			Get();
			st = true; 
		}
		if (la.kind == 55) {
			Get();
			auto = true; 
		}
		if (la.kind == 56) {
			Get();
			priv = true; 
		}
		Expect(11);
		var f = new DFunctionDeclaration(t) { IsStatic = st, IsPrivate = priv, IsAuto = auto };
		functions.Push(f);
		
		Qualident(out var s1, out var s2, out var s3);
		if (s2 == null && s3 == null)
		   f.Name = s1;
		else if (s3 == null)
		{
		   f.Name = s2;
		   f.TypeName = new Qualident(s1);
		}
		else
		{
		   f.Name = s3;
		   f.TypeName = new Qualident(s2, s1);
		}
		
		FunctionArguments(f);
		Block(out node);
		f.Body = node;
		node = f;
		functions.Pop();
		
	}

	void Expr(out DNode node) {
		node = null; 
		if (la.kind == 16) {
			If(out node);
		} else if (StartOf(4)) {
			SimpleExpr(out node);
		} else if (la.kind == 17 || la.kind == 18 || la.kind == 66) {
			Loops(out node);
		} else if (la.kind == 58) {
			Match(out node);
		} else SynErr(94);
		node = ProcessImplicits(node); 
	}

	void Block(out DNode node) {
		node = null; 
		Expect(28);
		var block = new DBlock(t); 
		if (StartOf(5)) {
			Statement(out node);
			block.Nodes.Add(node); 
			while (StartOf(5)) {
				Statement(out node);
				block.Nodes.Add(node); 
			}
		}
		node = block; 
		Expect(29);
	}

	void FunctionArgument(out DParameter arg) {
		arg = null; 
		Expect(1);
		arg = new DParameter(t) { Name = t.val }; 
		if (la.kind == 25) {
			Get();
			Expr(out var cnode);
			arg.DefaultValue = cnode; 
		}
		if (la.kind == 57) {
			Get();
			arg.IsVarArgs = true; 
		}
	}

	void Binding(out DNode node) {
		if (la.kind == 9) {
			Get();
		} else if (la.kind == 10) {
			Get();
		} else SynErr(95);
		var bin = new DBinding(t) { Constant = t.val == "const" }; 
		OrPattern(out var pat);
		bin.Pattern = pat; 
		if (la.kind == 25) {
			Get();
			Expr(out node);
			bin.Init = node; 
		}
		node = bin; 
	}

	void OrPattern(out DPattern node) {
		node = null; 
		AndPattern(out node);
		while (la.kind == 59) {
			var por = new DOrPattern(t) { Left = node }; 
			Get();
			AndPattern(out node);
			por.Right = node; node = por; 
		}
	}

	void Rebinding(out DNode node) {
		Expect(50);
		var bin = new DRebinding(t); 
		OrPattern(out var pat);
		bin.Pattern = pat; 
		Expect(25);
		Expr(out node);
		bin.Init = node; 
		node = bin; 
	}

	void MatchEntry(out DMatchEntry me) {
		me = new DMatchEntry(t); 
		OrPattern(out var p);
		me.Pattern = p; 
		if (la.kind == 52) {
			Get();
			Expr(out var node);
			me.Guard = node; 
		}
		Expect(20);
		Expr(out var exp);
		me.Expression = exp; 
	}

	void AndPattern(out DPattern node) {
		node = null; 
		RangePattern(out node);
		while (la.kind == 60) {
			var pa = new DAndPattern(t) { Left = node }; 
			Get();
			RangePattern(out node);
			pa.Right = node; node = pa; 
		}
	}

	void RangePattern(out DPattern node) {
		node = null; 
		Pattern(out node);
		if (la.kind == 61) {
			var r = new DRangePattern(t) { From = node }; 
			Get();
			Pattern(out node);
			r.To = node; node = r; 
		}
	}

	void Pattern(out DPattern node) {
		node = null; 
		if (IsConstructor()) {
			CtorPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (IsLabelPattern()) {
			LabelPattern(out node);
		} else if (la.kind == 1) {
			NamePattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 3) {
			IntegerPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 4) {
			FloatPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 6) {
			CharPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 5) {
			StringPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 64 || la.kind == 65) {
			BooleanPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 63) {
			NilPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (IsTuple(allowFields: false)) {
			TuplePattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 26) {
			GroupPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 30) {
			ArrayPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else if (la.kind == 21) {
			MethodCheckPattern(out node);
			if (la.kind == 1) {
				AsPattern(node, out node);
			}
		} else SynErr(96);
	}

	void CtorPattern(out DPattern node) {
		Expect(1);
		var ctor = new DCtorPattern(t) { Constructor = t.val }; 
		Expect(26);
		if (StartOf(6)) {
			OrPattern(out node);
			ctor.Arguments.Add(node); 
		}
		while (la.kind == 22) {
			Get();
			OrPattern(out node);
			ctor.Arguments.Add(node); 
		}
		Expect(27);
		node = ctor; 
	}

	void AsPattern(DPattern target, out DPattern node) {
		node = null; 
		if (la.AfterEol) { node = target; return; }
		var asp = new DAsPattern(t) { Pattern = target };
		
		Expect(1);
		asp.Name = t.val; node = asp; 
	}

	void LabelPattern(out DPattern node) {
		node = null; 
		Expect(1);
		var la = new DLabelPattern(t) { Label = t.val }; 
		Expect(24);
		Pattern(out var pat);
		la.Pattern = pat; node = la; 
	}

	void NamePattern(out DPattern node) {
		node = null; string nm2 = null; string nm1 = null; Token ot = null; 
		Expect(1);
		nm1 = t.val; ot = t; 
		if (la.kind == 21) {
			Get();
			Expect(1);
			nm2 = null; 
		}
		if (nm2 == null) {
		   if (t.val == "_")
		       node = new DWildcardPattern(ot);
		   else
		       node = new DNamePattern(ot) { Name = nm1 };
		} else {
		   var q = new Qualident(nm2, nm1);
		   node = new DTypeTestPattern(ot) { TypeName = q };
		}
		
	}

	void IntegerPattern(out DPattern node) {
		Expect(3);
		node = new DIntegerPattern(t) { Value = ParseInteger() }; 
	}

	void FloatPattern(out DPattern node) {
		Expect(4);
		node = new DFloatPattern(t) { Value = ParseFloat() }; 
	}

	void CharPattern(out DPattern node) {
		Expect(6);
		node = new DCharPattern(t) { Value = ParseChar() }; 
	}

	void StringPattern(out DPattern node) {
		Expect(5);
		node = new DStringPattern(t) { Value = ParseString() }; 
	}

	void BooleanPattern(out DPattern node) {
		if (la.kind == 64) {
			Get();
		} else if (la.kind == 65) {
			Get();
		} else SynErr(97);
		node = new DBooleanPattern(t) { Value = t.val == "true" }; 
	}

	void NilPattern(out DPattern node) {
		Expect(63);
		node = new DNilPattern(t); 
	}

	void TuplePattern(out DPattern node) {
		node = null; 
		Expect(26);
		var tup = new DTuplePattern(t); 
		OrPattern(out node);
		tup.Elements.Add(node); 
		while (la.kind == 22) {
			Get();
			if (StartOf(6)) {
				OrPattern(out node);
				tup.Elements.Add(node); 
			}
		}
		node = tup; 
		Expect(27);
	}

	void GroupPattern(out DPattern node) {
		node = null; 
		Expect(26);
		OrPattern(out node);
		Expect(27);
	}

	void ArrayPattern(out DPattern node) {
		node = null; 
		Expect(30);
		var tup = new DArrayPattern(t); 
		RangePattern(out node);
		tup.Elements.Add(node); 
		while (la.kind == 22) {
			Get();
			RangePattern(out node);
			tup.Elements.Add(node); 
		}
		node = tup; 
		Expect(31);
	}

	void MethodCheckPattern(out DPattern node) {
		node = null; 
		Expect(21);
		Expect(1);
		node = new DMethodCheckPattern(t) { Name = t.val }; 
		Expect(62);
	}

	void While(out DNode node) {
		node = null; 
		Expect(18);
		var @while = new DWhile(t); 
		Expr(out node);
		@while.Condition = node; 
		Block(out node);
		@while.Body = node;
		node = @while;
		
	}

	void For(out DNode node) {
		node = null; 
		Expect(17);
		var @for = new DFor(t); 
		OrPattern(out var pattern);
		@for.Pattern = pattern; 
		Expect(67);
		Expr(out node);
		@for.Target = node; 
		if (la.kind == 52) {
			Get();
			Expr(out node);
			@for.Guard = node; 
		}
		Block(out node);
		@for.Body = node;
		node = @for;
		
	}

	void DoWhile(out DNode node) {
		node = null; 
		var @while = new DWhile(t) { DoWhile = true }; 
		Expect(66);
		Block(out node);
		@while.Body = node; 
		Expect(18);
		Expr(out node);
		@while.Condition = node; node = @while; 
	}

	void Break(out DNode node) {
		Expect(14);
		var br = new DBreak(t); node = br; 
		if (StartOf(7)) {
			if (la.AfterEol) return; 
			Expr(out var exp);
			br.Expression = exp; 
		}
	}

	void Continue(out DNode node) {
		Expect(13);
		node = new DContinue(t); 
	}

	void Return(out DNode node) {
		Expect(12);
		var br = new DReturn(t); node = br;
		if (la.AfterEol) return;
		
		if (StartOf(7)) {
			Expr(out var exp);
			br.Expression = exp; 
		}
	}

	void Yield(out DNode node) {
		Expect(15);
		var yield = new DYield(t);
		node = yield;
		functions.Peek().IsIterator = true;
		
		Expr(out var exp);
		yield.Expression = exp; 
	}

	void FunctionExpr(out DNode node) {
		var f = new DFunctionDeclaration(t);
		node = f;
		
		if (la.kind == 1) {
			FunctionArgument(out var a);
			f.Parameters.Add(a); 
		} else if (la.kind == 26) {
			FunctionArguments(f);
		} else SynErr(98);
		functions.Push(f); 
		Expect(20);
		Expr(out var exp);
		f.Body = exp; 
	}

	void Label(out DNode node) {
		node = null; var name = ""; 
		if (la.kind == 1) {
			Get();
			name = t.val; 
		} else if (la.kind == 5) {
			Get();
			name = ParseSimpleString(); 
		} else SynErr(99);
		Expect(24);
		var ot = t; 
		if (IsFunction()) {
			FunctionExpr(out node);
		} else if (StartOf(3)) {
			Assignment(out node);
		} else SynErr(100);
		node = new DLabelLiteral(ot) { Label = name, Expression = node }; 
	}

	void Assignment(out DNode node) {
		Is(out node);
		if (StartOf(8)) {
			var ass = new DAssignment(t) { Target = node };
			node = ass;
			BinaryOperator? op = null;
			
			switch (la.kind) {
			case 25: {
				Get();
				break;
			}
			case 71: {
				Get();
				op = BinaryOperator.Add; 
				break;
			}
			case 72: {
				Get();
				op = BinaryOperator.Sub; 
				break;
			}
			case 73: {
				Get();
				op = BinaryOperator.Mul; 
				break;
			}
			case 74: {
				Get();
				op = BinaryOperator.Div; 
				break;
			}
			case 75: {
				Get();
				op = BinaryOperator.Rem; 
				break;
			}
			case 76: {
				Get();
				op = BinaryOperator.And; 
				break;
			}
			case 77: {
				Get();
				op = BinaryOperator.Or; 
				break;
			}
			case 78: {
				Get();
				op = BinaryOperator.Xor; 
				break;
			}
			case 79: {
				Get();
				op = BinaryOperator.ShiftLeft; 
				break;
			}
			case 80: {
				Get();
				op = BinaryOperator.ShiftRight; 
				break;
			}
			}
			Expr(out node);
			ass.Value = node;
			ass.AutoAssign = op;
			node = ass;
			
		}
	}

	void TryCatch(out DNode node) {
		node =  null; 
		Expect(69);
		var tc = new DTryCatch(t); 
		Block(out node);
		tc.Expression = node; 
		Expect(70);
		if (la.kind == 28) {
			var m = new DMatch(t); tc.Catch = m; 
			Get();
			MatchEntry(out var entry);
			m.Entries.Add(entry); 
			while (la.kind == 22) {
				Get();
				MatchEntry(out entry);
				m.Entries.Add(entry); 
			}
			Expect(29);
		} else if (la.kind == 1) {
			Get();
			tc.BindVariable = new DName(t) { Value = t.val }; 
			Block(out node);
			tc.Catch = node; 
		} else SynErr(101);
		node = tc; 
	}

	void Throw(out DNode node) {
		node = null; 
		Expect(68);
		var th = new DThrow(t); 
		Expr(out node);
		th.Expression = node; node = th; 
	}

	void Is(out DNode node) {
		Coalesce(out node);
		while (la.kind == 81) {
			Get();
			var ot = t; 
			OrPattern(out var pat);
			node = new DBinaryOperation(node, pat, BinaryOperator.Is, ot); 
		}
	}

	void Coalesce(out DNode node) {
		Or(out node);
		while (la.kind == 82) {
			Get();
			var ot = t; 
			Or(out DNode exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.Coalesce, ot); 
		}
	}

	void Or(out DNode node) {
		And(out node);
		while (la.kind == 59) {
			Get();
			var ot = t; 
			And(out DNode exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.Or, ot); 
		}
	}

	void And(out DNode node) {
		Eq(out node);
		while (la.kind == 60) {
			Get();
			var ot = t; 
			Eq(out DNode exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.And, ot); 
		}
	}

	void Eq(out DNode node) {
		Shift(out node);
		while (StartOf(9)) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			switch (la.kind) {
			case 43: {
				Get();
				ot = t; op = BinaryOperator.Gt; 
				break;
			}
			case 44: {
				Get();
				ot = t; op = BinaryOperator.Lt; 
				break;
			}
			case 45: {
				Get();
				ot = t; op = BinaryOperator.GtEq; 
				break;
			}
			case 46: {
				Get();
				ot = t; op = BinaryOperator.LtEq; 
				break;
			}
			case 41: {
				Get();
				ot = t; op = BinaryOperator.Eq; 
				break;
			}
			case 42: {
				Get();
				ot = t; op = BinaryOperator.NotEq; 
				break;
			}
			}
			Shift(out var exp);
			node = new DBinaryOperation(node, exp, op, ot); 
		}
	}

	void Shift(out DNode node) {
		BitOr(out node);
		while (la.kind == 48 || la.kind == 49) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			if (la.kind == 48) {
				Get();
				ot = t; op = BinaryOperator.ShiftLeft; 
			} else {
				Get();
				ot = t; op = BinaryOperator.ShiftRight; 
			}
			BitOr(out var exp);
			node = new DBinaryOperation(node, exp, op, ot); 
		}
	}

	void BitOr(out DNode node) {
		Xor(out node);
		while (la.kind == 39) {
			Get();
			var ot = t; 
			Xor(out var exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.BitwiseOr, ot); 
		}
	}

	void Xor(out DNode node) {
		BitAnd(out node);
		while (la.kind == 47) {
			DNode exp = null; 
			Get();
			var ot = t; 
			BitAnd(out exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.Xor, ot); 
		}
	}

	void BitAnd(out DNode node) {
		Add(out node);
		while (la.kind == 40) {
			Get();
			var ot = t; 
			Add(out var exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.BitwiseAnd, ot); 
		}
	}

	void Add(out DNode node) {
		Mul(out node);
		while (la.kind == 32 || la.kind == 33) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			if (la.kind == 33) {
				if (la.AfterEol) return; 
				Get();
				ot = t; op = BinaryOperator.Add; 
			} else {
				if (la.AfterEol) return; 
				Get();
				ot = t; op = BinaryOperator.Sub; 
			}
			Mul(out var exp);
			node = new DBinaryOperation(node, exp, op, ot); 
		}
	}

	void Mul(out DNode node) {
		Unary(out node);
		while (la.kind == 36 || la.kind == 37 || la.kind == 38) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			if (la.kind == 36) {
				Get();
				ot = t; op = BinaryOperator.Mul; 
			} else if (la.kind == 37) {
				Get();
				ot = t; op = BinaryOperator.Div; 
			} else {
				Get();
				ot = t; op = BinaryOperator.Rem; 
			}
			Unary(out var exp);
			node = new DBinaryOperation(node, exp, op, ot); 
		}
	}

	void Unary(out DNode node) {
		node = null;
		var op = default(UnaryOperator);
		var ot = default(Token);
		
		if (la.kind == 34) {
			Get();
			ot = t; op = UnaryOperator.Not; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (la.kind == 32) {
			Get();
			ot = t; op = UnaryOperator.Neg; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (la.kind == 33) {
			Get();
			ot = t; op = UnaryOperator.Plus; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (la.kind == 35) {
			Get();
			ot = t; op = UnaryOperator.BitwiseNot; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (StartOf(10)) {
			Range(out node);
		} else SynErr(102);
	}

	void Range(out DNode node) {
		node = null; 
		FieldOrIndex(out node);
		if (la.kind == 61) {
			Get();
			var range = new DRange(t) { From = node }; 
			FieldOrIndex(out node);
			range.To = node; node = range; 
		}
	}

	void FieldOrIndex(out DNode node) {
		Literal(out node);
		while (la.kind == 21 || la.kind == 26 || la.kind == 30) {
			if (la.kind == 21) {
				Get();
				var ot = t; 
				Expect(1);
				var nm = t.val; DMemberCheck chk = null; 
				if (la.kind == 62) {
					Get();
					chk = new DMemberCheck(ot) { Target = node };
					chk.Name = nm;
					node = chk;
					
				}
				if (chk == null)
				{
				   var fld = new DAccess(ot) { Target = node };
				   fld.Name = nm;
				   node = fld;
				}
				
			} else if (la.kind == 30) {
				if (la.AfterEol) return; 
				Get();
				var idx = new DIndexer(t) { Target = node }; 
				Expr(out node);
				idx.Index = node;
				node = idx;
				
				Expect(31);
			} else {
				if (la.AfterEol) return;
				var app = new DApplication(node, t);
				
				Get();
				if (StartOf(7)) {
					ApplicationArguments(app);
				}
				node = app; 
				Expect(27);
			}
		}
	}

	void Literal(out DNode node) {
		node = null; 
		if (la.kind == 1) {
			Name(out node);
		} else if (la.kind == 7) {
			SpecialName(out node);
		} else if (la.kind == 3) {
			Integer(out node);
		} else if (la.kind == 4) {
			Float(out node);
		} else if (la.kind == 5 || la.kind == 8) {
			String(out node);
		} else if (la.kind == 6) {
			Char(out node);
		} else if (la.kind == 64 || la.kind == 65) {
			Bool(out node);
		} else if (la.kind == 63) {
			Nil(out node);
		} else if (IsTuple()) {
			Tuple(out node);
		} else if (la.kind == 30) {
			Array(out node);
		} else if (la.kind == 26) {
			Group(out node);
		} else if (la.kind == 83) {
			Base(out node);
		} else if (IsIterator()) {
			Iterator(out node);
		} else if (la.kind == 28) {
			Block(out node);
		} else if (la.kind == 2) {
			Preprocessor(out node);
		} else SynErr(103);
	}

	void ApplicationArguments(DApplication app) {
		var node = default(DNode); 
		Expr(out node);
		app.Arguments.Add(node); 
		while (la.kind == 22) {
			Get();
			Expr(out node);
			app.Arguments.Add(node); 
		}
	}

	void Name(out DNode node) {
		Expect(1);
		node = new DName(t) { Value = t.val }; 
	}

	void SpecialName(out DNode node) {
		Expect(7);
		var nm = int.Parse(t.val.Substring(1));
		node = new DName(t) { Value = "p" + nm };
		if (implicits == null)
		   implicits = new List<int>();
		implicits.Add(nm);
		
	}

	void Integer(out DNode node) {
		Expect(3);
		node = new DIntegerLiteral(t) { Value = ParseInteger() }; 
	}

	void Float(out DNode node) {
		Expect(4);
		node = new DFloatLiteral(t) { Value = ParseFloat() }; 
	}

	void String(out DNode node) {
		node = null; 
		if (la.kind == 5) {
			Get();
			node = ParseString(); 
		} else if (la.kind == 8) {
			Get();
			node = ParseVerbatimString(); 
		} else SynErr(104);
	}

	void Char(out DNode node) {
		Expect(6);
		node = new DCharLiteral(t) { Value = ParseChar() }; 
	}

	void Bool(out DNode node) {
		if (la.kind == 64) {
			Get();
		} else if (la.kind == 65) {
			Get();
		} else SynErr(105);
		node = new DBooleanLiteral(t) { Value = t.val == "true" }; 
	}

	void Nil(out DNode node) {
		Expect(63);
		node = new DNilLiteral(t); 
	}

	void Tuple(out DNode node) {
		node = null; 
		Expect(26);
		var tup = new DTupleLiteral(t); 
		Expr(out node);
		tup.Elements.Add(node); 
		while (la.kind == 22) {
			Get();
			if (StartOf(7)) {
				Expr(out node);
				tup.Elements.Add(node); 
			}
		}
		node = tup; 
		Expect(27);
	}

	void Array(out DNode node) {
		node = null; 
		Expect(30);
		var arr = new DArrayLiteral(t); 
		if (StartOf(7)) {
			Expr(out node);
			arr.Elements.Add(node); 
			while (la.kind == 22) {
				Get();
				Expr(out node);
				arr.Elements.Add(node); 
			}
		}
		node = arr; 
		Expect(31);
	}

	void Group(out DNode node) {
		node = null; 
		Expect(26);
		Expr(out node);
		Expect(27);
	}

	void Base(out DNode node) {
		Expect(83);
		node = new DBase(t); 
	}

	void Iterator(out DNode node) {
		node = null; 
		Expect(28);
		var it = new DIteratorLiteral(t);
		it.YieldBlock = new DYieldBlock(t);
		
		Expr(out node);
		it.YieldBlock.Elements.Add(node); 
		Expect(22);
		if (StartOf(7)) {
			Expr(out node);
			it.YieldBlock.Elements.Add(node); 
			while (la.kind == 22) {
				Get();
				Expr(out node);
				it.YieldBlock.Elements.Add(node); 
			}
		}
		node = it; 
		Expect(29);
	}

	void Preprocessor(out DNode node) {
		Expect(2);
		var pp = new DPreprocessor(t) { Key = t.val.Substring(1) }; node = pp; 
		Expect(26);
		while (StartOf(11)) {
			switch (la.kind) {
			case 5: {
				Get();
				pp.Attributes.Add(ParseSimpleString()); 
				break;
			}
			case 3: {
				Get();
				pp.Attributes.Add(ParseInteger()); 
				break;
			}
			case 4: {
				Get();
				pp.Attributes.Add(ParseFloat()); 
				break;
			}
			case 6: {
				Get();
				pp.Attributes.Add(ParseChar()); 
				break;
			}
			case 64: {
				Get();
				pp.Attributes.Add(true); 
				break;
			}
			case 65: {
				Get();
				pp.Attributes.Add(false); 
				break;
			}
			case 1: {
				Get();
				pp.Attributes.Add(t.val); 
				break;
			}
			}
		}
		Expect(27);
	}

	void DyalectItem() {
		if (StartOf(5)) {
			Statement(out var node);
			Root.Nodes.Add(node);
			
		} else if (la.kind == 51) {
			Import();
			Separator();
		} else SynErr(106);
	}

	void Dyalect() {
		DyalectItem();
		while (StartOf(12)) {
			DyalectItem();
		}
	}



        public void Parse()
        {
            la = new Token();
            la.val = "";
            Get();
		Dyalect();
		Expect(0);

        }

        static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_T,_T, _T,_x,_T,_x, _x,_x,_x,_T, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_T,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_T,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_T, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_T,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _x,_x,_T,_T, _T,_x,_T,_x, _x,_x,_x,_T, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x}

        };

        private void SynErr(int line, int col, int n)
        {
            string s;

            switch (n)
            {
			case 0: s = "EOF expected"; break;
			case 1: s = "identToken expected"; break;
			case 2: s = "preprocessor expected"; break;
			case 3: s = "intToken expected"; break;
			case 4: s = "floatToken expected"; break;
			case 5: s = "stringToken expected"; break;
			case 6: s = "charToken expected"; break;
			case 7: s = "implicitToken expected"; break;
			case 8: s = "verbatimStringToken expected"; break;
			case 9: s = "varToken expected"; break;
			case 10: s = "constToken expected"; break;
			case 11: s = "funcToken expected"; break;
			case 12: s = "returnToken expected"; break;
			case 13: s = "continueToken expected"; break;
			case 14: s = "breakToken expected"; break;
			case 15: s = "yieldToken expected"; break;
			case 16: s = "ifToken expected"; break;
			case 17: s = "forToken expected"; break;
			case 18: s = "whileToken expected"; break;
			case 19: s = "typeToken expected"; break;
			case 20: s = "arrowToken expected"; break;
			case 21: s = "dotToken expected"; break;
			case 22: s = "commaToken expected"; break;
			case 23: s = "semicolonToken expected"; break;
			case 24: s = "colonToken expected"; break;
			case 25: s = "equalToken expected"; break;
			case 26: s = "parenLeftToken expected"; break;
			case 27: s = "parenRightToken expected"; break;
			case 28: s = "curlyLeftToken expected"; break;
			case 29: s = "curlyRightToken expected"; break;
			case 30: s = "squareLeftToken expected"; break;
			case 31: s = "squareRightToken expected"; break;
			case 32: s = "minus expected"; break;
			case 33: s = "plus expected"; break;
			case 34: s = "not expected"; break;
			case 35: s = "bitnot expected"; break;
			case 36: s = "\"*\" expected"; break;
			case 37: s = "\"/\" expected"; break;
			case 38: s = "\"%\" expected"; break;
			case 39: s = "\"|\" expected"; break;
			case 40: s = "\"&\" expected"; break;
			case 41: s = "\"==\" expected"; break;
			case 42: s = "\"!=\" expected"; break;
			case 43: s = "\">\" expected"; break;
			case 44: s = "\"<\" expected"; break;
			case 45: s = "\">=\" expected"; break;
			case 46: s = "\"<=\" expected"; break;
			case 47: s = "\"^\" expected"; break;
			case 48: s = "\"<<\" expected"; break;
			case 49: s = "\">>\" expected"; break;
			case 50: s = "\"set\" expected"; break;
			case 51: s = "\"import\" expected"; break;
			case 52: s = "\"when\" expected"; break;
			case 53: s = "\"else\" expected"; break;
			case 54: s = "\"static\" expected"; break;
			case 55: s = "\"auto\" expected"; break;
			case 56: s = "\"private\" expected"; break;
			case 57: s = "\"...\" expected"; break;
			case 58: s = "\"match\" expected"; break;
			case 59: s = "\"||\" expected"; break;
			case 60: s = "\"&&\" expected"; break;
			case 61: s = "\"..\" expected"; break;
			case 62: s = "\"?\" expected"; break;
			case 63: s = "\"nil\" expected"; break;
			case 64: s = "\"true\" expected"; break;
			case 65: s = "\"false\" expected"; break;
			case 66: s = "\"do\" expected"; break;
			case 67: s = "\"in\" expected"; break;
			case 68: s = "\"throw\" expected"; break;
			case 69: s = "\"try\" expected"; break;
			case 70: s = "\"catch\" expected"; break;
			case 71: s = "\"+=\" expected"; break;
			case 72: s = "\"-=\" expected"; break;
			case 73: s = "\"*=\" expected"; break;
			case 74: s = "\"/=\" expected"; break;
			case 75: s = "\"%=\" expected"; break;
			case 76: s = "\"&=\" expected"; break;
			case 77: s = "\"|=\" expected"; break;
			case 78: s = "\"^=\" expected"; break;
			case 79: s = "\"<<=\" expected"; break;
			case 80: s = "\">>=\" expected"; break;
			case 81: s = "\"is\" expected"; break;
			case 82: s = "\"??\" expected"; break;
			case 83: s = "\"base\" expected"; break;
			case 84: s = "??? expected"; break;
			case 85: s = "invalid StandardOperators"; break;
			case 86: s = "invalid FunctionName"; break;
			case 87: s = "invalid Import"; break;
			case 88: s = "invalid Import"; break;
			case 89: s = "invalid Statement"; break;
			case 90: s = "invalid ControlFlow"; break;
			case 91: s = "invalid SimpleExpr"; break;
			case 92: s = "invalid If"; break;
			case 93: s = "invalid Loops"; break;
			case 94: s = "invalid Expr"; break;
			case 95: s = "invalid Binding"; break;
			case 96: s = "invalid Pattern"; break;
			case 97: s = "invalid BooleanPattern"; break;
			case 98: s = "invalid FunctionExpr"; break;
			case 99: s = "invalid Label"; break;
			case 100: s = "invalid Label"; break;
			case 101: s = "invalid TryCatch"; break;
			case 102: s = "invalid Unary"; break;
			case 103: s = "invalid Literal"; break;
			case 104: s = "invalid String"; break;
			case 105: s = "invalid Bool"; break;
			case 106: s = "invalid DyalectItem"; break;

                default:
                    s = "unknown " + n;
                    break;
            }

            AddError(s, line, col);
        }
    }
}
