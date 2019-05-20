
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
	public const int _intToken = 2;
	public const int _floatToken = 3;
	public const int _stringToken = 4;
	public const int _charToken = 5;
	public const int _implicitToken = 6;
	public const int _varToken = 7;
	public const int _constToken = 8;
	public const int _funcToken = 9;
	public const int _returnToken = 10;
	public const int _continueToken = 11;
	public const int _breakToken = 12;
	public const int _yieldToken = 13;
	public const int _ifToken = 14;
	public const int _forToken = 15;
	public const int _whileToken = 16;
	public const int _typeToken = 17;
	public const int _arrowToken = 18;
	public const int _dotToken = 19;
	public const int _commaToken = 20;
	public const int _semicolonToken = 21;
	public const int _colonToken = 22;
	public const int _equalToken = 23;
	public const int _parenLeftToken = 24;
	public const int _parenRightToken = 25;
	public const int _curlyLeftToken = 26;
	public const int _curlyRightToken = 27;
	public const int _squareLeftToken = 28;
	public const int _squareRightToken = 29;
	public const int _minus = 30;
	public const int _plus = 31;
	public const int _not = 32;
	public const int _bitnot = 33;
	public const int maxT = 73;




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
		Expect(21);
	}

	void StandardOperators() {
		switch (la.kind) {
		case 31: {
			Get();
			break;
		}
		case 30: {
			Get();
			break;
		}
		case 34: {
			Get();
			break;
		}
		case 35: {
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
		case 32: {
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
		case 33: {
			Get();
			break;
		}
		default: SynErr(74); break;
		}
	}

	void FunctionName() {
		if (la.kind == 1) {
			Get();
		} else if (StartOf(1)) {
			StandardOperators();
		} else SynErr(75);
	}

	void Qualident(out string s1, out string s2, out string s3) {
		s1 = null; s2 = null; s3 = null; 
		FunctionName();
		s1 = t.val; 
		if (StartOf(2)) {
			if (la.kind == 19) {
				Get();
			}
			FunctionName();
			s2 = t.val; 
			if (StartOf(2)) {
				if (la.kind == 19) {
					Get();
				}
				FunctionName();
				s3 = t.val; 
			}
		}
	}

	void Import() {
		Expect(48);
		var inc = new DImport(t); 
		Expect(1);
		inc.ModuleName = t.val; 
		if (la.kind == 23) {
			Get();
			Expect(1);
			inc.Alias = inc.ModuleName;
			inc.ModuleName = t.val;
			
		}
		if (la.kind == 24) {
			Get();
			Expect(4);
			inc.Dll = ParseSimpleString(); 
			Expect(25);
		}
		Imports.Add(inc); 
	}

	void Type(out DNode node) {
		Expect(17);
		var typ = new DTypeDeclaration(t);
		node = typ;
		
		Expect(1);
		typ.Name = t.val; 
		if (la.kind == 26) {
			Get();
			if (la.kind == 1) {
				TypeField(out var fld);
				typ.Fields.Add(fld); 
				while (la.kind == 20) {
					Get();
					Expect(1);
					typ.Fields.Add(new DFieldDeclaration(t) { Name = t.val }); 
				}
			}
			Expect(27);
		}
	}

	void TypeField(out DFieldDeclaration node) {
		Expect(1);
		node = new DFieldDeclaration(t) { Name = t.val }; 
	}

	void Statement(out DNode node) {
		node = null; 
		switch (la.kind) {
		case 7: case 8: {
			Binding(out node);
			Separator();
			break;
		}
		case 1: case 2: case 3: case 4: case 5: case 6: case 24: case 28: case 30: case 31: case 32: case 33: case 53: case 54: case 71: case 72: {
			SimpleExpr(out node);
			Separator();
			break;
		}
		case 10: case 11: case 12: case 13: {
			ControlFlow(out node);
			Separator();
			break;
		}
		case 17: {
			Type(out node);
			Separator();
			break;
		}
		case 26: {
			Block(out node);
			break;
		}
		case 14: {
			If(out node);
			break;
		}
		case 15: case 16: {
			Loops(out node);
			break;
		}
		case 9: case 49: {
			Function(out node);
			break;
		}
		default: SynErr(76); break;
		}
	}

	void Binding(out DNode node) {
		if (la.kind == 7) {
			Get();
		} else if (la.kind == 8) {
			Get();
		} else SynErr(77);
		var bin = new DBinding(t) { Constant = t.val == "const" }; 
		Expect(1);
		bin.Name = t.val; 
		if (la.kind == 23) {
			Get();
			Expr(out node);
			bin.Init = node; 
		}
		node = bin; 
	}

	void SimpleExpr(out DNode node) {
		node = null; 
		if (IsFunction()) {
			FunctionExpr(out node);
		} else if (IsLabel()) {
			Label(out node);
		} else if (StartOf(3)) {
			Assignment(out node);
		} else SynErr(78);
		if (implicits != null)
		{
		   var func = new DFunctionDeclaration(node.Location)
		   {
		       Body = node
		   };
		   implicits.Sort();
		   func.Parameters.AddRange(implicits.Distinct().Select(i => new DParameter(t) { Name = "p" + i }));
		   node = func;
		   implicits = null;
		}
		
	}

	void ControlFlow(out DNode node) {
		node = null; 
		if (la.kind == 12) {
			Break(out node);
		} else if (la.kind == 11) {
			Continue(out node);
		} else if (la.kind == 10) {
			Return(out node);
		} else if (la.kind == 13) {
			Yield(out node);
		} else SynErr(79);
	}

	void Block(out DNode node) {
		node = null; 
		Expect(26);
		var block = new DBlock(t); 
		if (StartOf(4)) {
			Statement(out node);
			block.Nodes.Add(node); 
			while (StartOf(4)) {
				Statement(out node);
				block.Nodes.Add(node); 
			}
		}
		node = block; 
		Expect(27);
	}

	void If(out DNode node) {
		node = null; 
		Expect(14);
		var @if = new DIf(t); 
		Expr(out node);
		@if.Condition = node; 
		Block(out node);
		@if.True = node; 
		if (la.kind == 55) {
			Get();
			if (la.kind == 26) {
				Block(out node);
				@if.False = node; 
			} else if (la.kind == 14) {
				If(out node);
				@if.False = node; 
			} else SynErr(80);
		}
		node = @if; 
	}

	void Loops(out DNode node) {
		node = null; 
		if (la.kind == 16) {
			While(out node);
		} else if (la.kind == 15) {
			For(out node);
		} else SynErr(81);
	}

	void Function(out DNode node) {
		node = null; bool st = false; 
		if (la.kind == 49) {
			Get();
			st = true; 
		}
		Expect(9);
		var f = new DFunctionDeclaration(t) { IsStatic = st };
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
		   f.TypeName = new Qualident(s1, s2);
		}
		
		FunctionArguments(f);
		Expr(out node);
		Separator();
		f.Body = node;
		node = f;
		functions.Pop();
		
	}

	void FunctionArguments(DFunctionDeclaration node) {
		Expect(24);
		if (la.kind == 1) {
			FunctionArgument(out var arg);
			node.Parameters.Add(arg); 
			while (la.kind == 20) {
				Get();
				FunctionArgument(out arg);
				node.Parameters.Add(arg); 
			}
		}
		Expect(25);
	}

	void Expr(out DNode node) {
		node = null; 
		if (StartOf(3)) {
			SimpleExpr(out node);
		} else if (la.kind == 14) {
			If(out node);
		} else if (la.kind == 26) {
			Block(out node);
		} else if (la.kind == 15 || la.kind == 16) {
			Loops(out node);
		} else if (la.kind == 51) {
			Match(out node);
		} else SynErr(82);
	}

	void FunctionArgument(out DParameter arg) {
		arg = null; 
		Expect(1);
		arg = new DParameter(t) { Name = t.val }; 
		if (la.kind == 23) {
			Get();
			Expr(out var cnode);
			arg.DefaultValue = cnode; 
		}
		if (la.kind == 50) {
			Get();
			arg.IsVarArgs = true; 
		}
	}

	void Match(out DNode node) {
		node = null; 
		Expect(51);
		var m = new DMatch(t); 
		Expr(out node);
		m.Expression = node; 
		Expect(26);
		MatchEntry(out var entry);
		m.Entries.Add(entry); 
		while (la.kind == 20) {
			Get();
			MatchEntry(out entry);
			m.Entries.Add(entry); 
		}
		Expect(27);
		node = m; 
	}

	void MatchEntry(out DMatchEntry me) {
		me = new DMatchEntry(t); 
		Pattern(out var p);
		me.Pattern = p; 
		if (la.kind == 52) {
			Get();
			Expr(out var node);
			me.Guard = node; 
		}
		Expect(18);
		Expr(out var exp);
		me.Expression = exp; 
	}

	void Pattern(out DPattern node) {
		node = null; 
		switch (la.kind) {
		case 1: {
			NamePattern(out node);
			break;
		}
		case 2: {
			IntegerPattern(out node);
			break;
		}
		case 3: {
			FloatPattern(out node);
			break;
		}
		case 5: {
			CharPattern(out node);
			break;
		}
		case 4: {
			StringPattern(out node);
			break;
		}
		case 53: case 54: {
			BooleanPattern(out node);
			break;
		}
		case 24: {
			TuplePattern(out node);
			break;
		}
		default: SynErr(83); break;
		}
	}

	void NamePattern(out DPattern node) {
		Expect(1);
		node = new DNamePattern(t) { Name = t.val }; 
	}

	void IntegerPattern(out DPattern node) {
		Expect(2);
		node = new DIntegerPattern(t) { Value = ParseInteger() }; 
	}

	void FloatPattern(out DPattern node) {
		Expect(3);
		node = new DFloatPattern(t) { Value = ParseFloat() }; 
	}

	void CharPattern(out DPattern node) {
		Expect(5);
		node = new DCharPattern(t) { Value = ParseChar() }; 
	}

	void StringPattern(out DPattern node) {
		Expect(4);
		node = new DStringPattern(t) { Value = ParseString() }; 
	}

	void BooleanPattern(out DPattern node) {
		if (la.kind == 53) {
			Get();
		} else if (la.kind == 54) {
			Get();
		} else SynErr(84);
		node = new DBooleanPattern(t) { Value = t.val == "true" }; 
	}

	void TuplePattern(out DPattern node) {
		node = null; 
		Expect(24);
		var tup = new DTuplePattern(t); 
		TuplePatternElement(out node);
		tup.Elements.Add(node); 
		while (la.kind == 20) {
			Get();
			TuplePatternElement(out node);
			tup.Elements.Add(node); 
		}
		node = tup; 
		Expect(25);
	}

	void TuplePatternElement(out DPattern node) {
		node = null; 
		if (IsLabel()) {
			FieldPattern(out node);
		} else if (StartOf(5)) {
			Pattern(out node);
		} else SynErr(85);
	}

	void FieldPattern(out DPattern node) {
		node = null; 
		Expect(1);
		var la = new DLabelPattern(t) { Label = t.val }; 
		Expect(22);
		Pattern(out node);
		la.Pattern = node; node = la; 
	}

	void While(out DNode node) {
		node = null; 
		Expect(16);
		var @while = new DWhile(t); 
		Expr(out node);
		@while.Condition = node; 
		Block(out node);
		@while.Body = node;
		node = @while;
		
	}

	void For(out DNode node) {
		node = null; 
		Expect(15);
		var @for = new DFor(t); 
		Name(out node);
		@for.Variable = (DName)node; 
		Expect(56);
		Expr(out node);
		@for.Target = node; 
		Block(out node);
		@for.Body = node;
		node = @for;
		
	}

	void Name(out DNode node) {
		Expect(1);
		node = new DName(t) { Value = t.val }; 
	}

	void Break(out DNode node) {
		Expect(12);
		var br = new DBreak(t); node = br; 
		if (StartOf(6)) {
			if (la.AfterEol) return; 
			Expr(out var exp);
			br.Expression = exp; 
		}
	}

	void Continue(out DNode node) {
		Expect(11);
		node = new DContinue(t); 
	}

	void Return(out DNode node) {
		Expect(10);
		var br = new DReturn(t); node = br; 
		if (StartOf(6)) {
			Expr(out var exp);
			br.Expression = exp; 
		}
	}

	void Yield(out DNode node) {
		Expect(13);
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
		} else if (la.kind == 24) {
			FunctionArguments(f);
		} else SynErr(86);
		Expect(18);
		Expr(out var exp);
		f.Body = exp; 
	}

	void Label(out DNode node) {
		node = null; var name = ""; 
		Expect(1);
		name = t.val; 
		Expect(22);
		var ot = t; 
		Or(out node);
		node = new DLabelLiteral(ot) { Label = name, Expression = node }; 
	}

	void Assignment(out DNode node) {
		Or(out node);
		if (StartOf(7)) {
			var ass = new DAssignment(t) { Target = node };
			node = ass;
			BinaryOperator? op = null;
			
			switch (la.kind) {
			case 23: {
				Get();
				break;
			}
			case 57: {
				Get();
				op = BinaryOperator.Add; 
				break;
			}
			case 58: {
				Get();
				op = BinaryOperator.Sub; 
				break;
			}
			case 59: {
				Get();
				op = BinaryOperator.Mul; 
				break;
			}
			case 60: {
				Get();
				op = BinaryOperator.Div; 
				break;
			}
			case 61: {
				Get();
				op = BinaryOperator.Rem; 
				break;
			}
			case 62: {
				Get();
				op = BinaryOperator.And; 
				break;
			}
			case 63: {
				Get();
				op = BinaryOperator.Or; 
				break;
			}
			case 64: {
				Get();
				op = BinaryOperator.Xor; 
				break;
			}
			case 65: {
				Get();
				op = BinaryOperator.ShiftLeft; 
				break;
			}
			case 66: {
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

	void Or(out DNode node) {
		And(out node);
		while (la.kind == 67) {
			Get();
			var ot = t; 
			And(out DNode exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.Or, ot); 
		}
	}

	void And(out DNode node) {
		Eq(out node);
		while (la.kind == 68) {
			Get();
			var ot = t; 
			Eq(out DNode exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.And, ot); 
		}
	}

	void Eq(out DNode node) {
		Shift(out node);
		while (StartOf(8)) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			switch (la.kind) {
			case 41: {
				Get();
				ot = t; op = BinaryOperator.Gt; 
				break;
			}
			case 42: {
				Get();
				ot = t; op = BinaryOperator.Lt; 
				break;
			}
			case 43: {
				Get();
				ot = t; op = BinaryOperator.GtEq; 
				break;
			}
			case 44: {
				Get();
				ot = t; op = BinaryOperator.LtEq; 
				break;
			}
			case 39: {
				Get();
				ot = t; op = BinaryOperator.Eq; 
				break;
			}
			case 40: {
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
		while (la.kind == 46 || la.kind == 47) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			if (la.kind == 46) {
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
		while (la.kind == 37) {
			Get();
			var ot = t; 
			Xor(out var exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.BitwiseOr, ot); 
		}
	}

	void Xor(out DNode node) {
		BitAnd(out node);
		while (la.kind == 45) {
			DNode exp = null; 
			Get();
			var ot = t; 
			BitAnd(out exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.Xor, ot); 
		}
	}

	void BitAnd(out DNode node) {
		Add(out node);
		while (la.kind == 38) {
			Get();
			var ot = t; 
			Add(out var exp);
			node = new DBinaryOperation(node, exp, BinaryOperator.BitwiseAnd, ot); 
		}
	}

	void Add(out DNode node) {
		Mul(out node);
		while (la.kind == 30 || la.kind == 31) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			if (la.kind == 31) {
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
		while (la.kind == 34 || la.kind == 35 || la.kind == 36) {
			var op = default(BinaryOperator);
			var ot = default(Token);
			
			if (la.kind == 34) {
				Get();
				ot = t; op = BinaryOperator.Mul; 
			} else if (la.kind == 35) {
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
		
		if (la.kind == 32) {
			Get();
			ot = t; op = UnaryOperator.Not; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (la.kind == 30) {
			Get();
			ot = t; op = UnaryOperator.Neg; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (la.kind == 31) {
			Get();
			ot = t; op = UnaryOperator.Plus; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (la.kind == 33) {
			Get();
			ot = t; op = UnaryOperator.BitwiseNot; 
			Range(out node);
			node = new DUnaryOperation(node, op, ot); 
		} else if (StartOf(9)) {
			Range(out node);
		} else SynErr(87);
	}

	void Range(out DNode node) {
		node = null; 
		FieldOrIndex(out node);
		if (la.kind == 69) {
			Get();
			var range = new DRange(t) { From = node }; 
			FieldOrIndex(out node);
			range.To = node; node = range; 
		}
	}

	void FieldOrIndex(out DNode node) {
		Literal(out node);
		while (la.kind == 19 || la.kind == 24 || la.kind == 28) {
			if (la.kind == 19) {
				Get();
				var ot = t; 
				Expect(1);
				var nm = t.val; DMemberCheck chk = null; 
				if (la.kind == 70) {
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
				
			} else if (la.kind == 28) {
				if (la.AfterEol) return; 
				Get();
				var idx = new DIndexer(t) { Target = node }; 
				Expr(out node);
				idx.Index = node;
				node = idx;
				
				Expect(29);
			} else {
				if (la.AfterEol) return;
				var app = new DApplication(node, t); 
				
				Get();
				if (StartOf(6)) {
					ApplicationArguments(app);
				}
				node = app; 
				Expect(25);
			}
		}
	}

	void Literal(out DNode node) {
		node = null; 
		if (la.kind == 1) {
			Name(out node);
		} else if (la.kind == 6) {
			SpecialName(out node);
		} else if (la.kind == 2) {
			Integer(out node);
		} else if (la.kind == 3) {
			Float(out node);
		} else if (la.kind == 4) {
			String(out node);
		} else if (la.kind == 5) {
			Char(out node);
		} else if (la.kind == 53 || la.kind == 54) {
			Bool(out node);
		} else if (la.kind == 72) {
			Nil(out node);
		} else if (IsTuple()) {
			Tuple(out node);
		} else if (la.kind == 28) {
			Array(out node);
		} else if (la.kind == 24) {
			Group(out node);
		} else if (la.kind == 71) {
			Base(out node);
		} else SynErr(88);
	}

	void ApplicationArguments(DApplication app) {
		var node = default(DNode); 
		Expr(out node);
		app.Arguments.Add(node); 
		while (la.kind == 20) {
			Get();
			Expr(out node);
			app.Arguments.Add(node); 
		}
	}

	void SpecialName(out DNode node) {
		Expect(6);
		var nm = int.Parse(t.val.Substring(1));
		node = new DName(t) { Value = "p" + nm };
		if (implicits == null)
		   implicits = new List<int>();
		implicits.Add(nm);
		
	}

	void Integer(out DNode node) {
		Expect(2);
		node = new DIntegerLiteral(t) { Value = ParseInteger() }; 
	}

	void Float(out DNode node) {
		Expect(3);
		node = new DFloatLiteral(t) { Value = ParseFloat() }; 
	}

	void String(out DNode node) {
		Expect(4);
		node = ParseString(); 
	}

	void Char(out DNode node) {
		Expect(5);
		node = new DCharLiteral(t) { Value = ParseChar() }; 
	}

	void Bool(out DNode node) {
		if (la.kind == 53) {
			Get();
		} else if (la.kind == 54) {
			Get();
		} else SynErr(89);
		node = new DBooleanLiteral(t) { Value = t.val == "true" }; 
	}

	void Nil(out DNode node) {
		Expect(72);
		node = new DNilLiteral(t); 
	}

	void Tuple(out DNode node) {
		node = null; 
		Expect(24);
		var tup = new DTupleLiteral(t); 
		TupleElement(out node);
		tup.Elements.Add(node); 
		while (la.kind == 20) {
			Get();
			TupleElement(out node);
			tup.Elements.Add(node); 
		}
		node = tup; 
		Expect(25);
	}

	void Array(out DNode node) {
		node = null; 
		Expect(28);
		var arr = new DArrayLiteral(t); 
		if (StartOf(6)) {
			Expr(out node);
			arr.Elements.Add(node); 
			while (la.kind == 20) {
				Get();
				Expr(out node);
				arr.Elements.Add(node); 
			}
		}
		node = arr; 
		Expect(29);
	}

	void Group(out DNode node) {
		node = null; 
		Expect(24);
		Expr(out node);
		Expect(25);
	}

	void Base(out DNode node) {
		Expect(71);
		node = new DBase(t); 
	}

	void TupleElement(out DNode node) {
		node = null; 
		Expr(out node);
	}

	void DyalectItem() {
		if (StartOf(4)) {
			Statement(out var node);
			Root.Nodes.Add(node); 
		} else if (la.kind == 48) {
			Import();
			Separator();
		} else SynErr(90);
	}

	void Dyalect() {
		DyalectItem();
		while (StartOf(10)) {
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _T,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _T,_x,_T,_x, _T,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x},
		{_x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _T,_x,_T,_x, _T,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _T,_x,_T,_x, _T,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x}

        };

        private void SynErr(int line, int col, int n)
        {
            string s;

            switch (n)
            {
			case 0: s = "EOF expected"; break;
			case 1: s = "identToken expected"; break;
			case 2: s = "intToken expected"; break;
			case 3: s = "floatToken expected"; break;
			case 4: s = "stringToken expected"; break;
			case 5: s = "charToken expected"; break;
			case 6: s = "implicitToken expected"; break;
			case 7: s = "varToken expected"; break;
			case 8: s = "constToken expected"; break;
			case 9: s = "funcToken expected"; break;
			case 10: s = "returnToken expected"; break;
			case 11: s = "continueToken expected"; break;
			case 12: s = "breakToken expected"; break;
			case 13: s = "yieldToken expected"; break;
			case 14: s = "ifToken expected"; break;
			case 15: s = "forToken expected"; break;
			case 16: s = "whileToken expected"; break;
			case 17: s = "typeToken expected"; break;
			case 18: s = "arrowToken expected"; break;
			case 19: s = "dotToken expected"; break;
			case 20: s = "commaToken expected"; break;
			case 21: s = "semicolonToken expected"; break;
			case 22: s = "colonToken expected"; break;
			case 23: s = "equalToken expected"; break;
			case 24: s = "parenLeftToken expected"; break;
			case 25: s = "parenRightToken expected"; break;
			case 26: s = "curlyLeftToken expected"; break;
			case 27: s = "curlyRightToken expected"; break;
			case 28: s = "squareLeftToken expected"; break;
			case 29: s = "squareRightToken expected"; break;
			case 30: s = "minus expected"; break;
			case 31: s = "plus expected"; break;
			case 32: s = "not expected"; break;
			case 33: s = "bitnot expected"; break;
			case 34: s = "\"*\" expected"; break;
			case 35: s = "\"/\" expected"; break;
			case 36: s = "\"%\" expected"; break;
			case 37: s = "\"|\" expected"; break;
			case 38: s = "\"&\" expected"; break;
			case 39: s = "\"==\" expected"; break;
			case 40: s = "\"!=\" expected"; break;
			case 41: s = "\">\" expected"; break;
			case 42: s = "\"<\" expected"; break;
			case 43: s = "\">=\" expected"; break;
			case 44: s = "\"<=\" expected"; break;
			case 45: s = "\"^\" expected"; break;
			case 46: s = "\"<<\" expected"; break;
			case 47: s = "\">>\" expected"; break;
			case 48: s = "\"import\" expected"; break;
			case 49: s = "\"static\" expected"; break;
			case 50: s = "\"...\" expected"; break;
			case 51: s = "\"match\" expected"; break;
			case 52: s = "\"when\" expected"; break;
			case 53: s = "\"true\" expected"; break;
			case 54: s = "\"false\" expected"; break;
			case 55: s = "\"else\" expected"; break;
			case 56: s = "\"in\" expected"; break;
			case 57: s = "\"+=\" expected"; break;
			case 58: s = "\"-=\" expected"; break;
			case 59: s = "\"*=\" expected"; break;
			case 60: s = "\"/=\" expected"; break;
			case 61: s = "\"%=\" expected"; break;
			case 62: s = "\"&=\" expected"; break;
			case 63: s = "\"|=\" expected"; break;
			case 64: s = "\"^=\" expected"; break;
			case 65: s = "\"<<=\" expected"; break;
			case 66: s = "\">>=\" expected"; break;
			case 67: s = "\"||\" expected"; break;
			case 68: s = "\"&&\" expected"; break;
			case 69: s = "\"..\" expected"; break;
			case 70: s = "\"?\" expected"; break;
			case 71: s = "\"base\" expected"; break;
			case 72: s = "\"nil\" expected"; break;
			case 73: s = "??? expected"; break;
			case 74: s = "invalid StandardOperators"; break;
			case 75: s = "invalid FunctionName"; break;
			case 76: s = "invalid Statement"; break;
			case 77: s = "invalid Binding"; break;
			case 78: s = "invalid SimpleExpr"; break;
			case 79: s = "invalid ControlFlow"; break;
			case 80: s = "invalid If"; break;
			case 81: s = "invalid Loops"; break;
			case 82: s = "invalid Expr"; break;
			case 83: s = "invalid Pattern"; break;
			case 84: s = "invalid BooleanPattern"; break;
			case 85: s = "invalid TuplePatternElement"; break;
			case 86: s = "invalid FunctionExpr"; break;
			case 87: s = "invalid Unary"; break;
			case 88: s = "invalid Literal"; break;
			case 89: s = "invalid Bool"; break;
			case 90: s = "invalid DyalectItem"; break;

                default:
                    s = "unknown " + n;
                    break;
            }

            AddError(s, line, col);
        }
    }
}
