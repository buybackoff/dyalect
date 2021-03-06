﻿using Dyalect.Compiler;
using Dyalect.Debug;
using System;
using System.Collections.Generic;

namespace Dyalect.Runtime.Types
{
    public abstract class DyTypeInfo : DyObject
    {
        [Flags]
        public enum SupportedOperations
        {
            None = 0xFF,
            Add = 0x01,
            Sub = 0x02,
            Mul = 0x04,
            Div = 0x08,
            Rem = 0x10,
            Shl = 0x20,
            Shr = 0x40,
            And = 0x80,
            Or  = 0x100,
            Xor = 0x200,
            Eq  = 0x400,
            Neq = 0x800,
            Gt  = 0x1000,
            Lt  = 0x2000,
            Gte = 0x4000,
            Lte = 0x8000,
            Neg = 0x10000,
            BitNot = 0x20000,
            Bit =  0x40000,
            Plus = 0x80000,
            Not =  0x100000,
            Get =  0x200000,
            Set =  0x400000,
            Len =  0x800000
        }

        protected abstract SupportedOperations GetSupportedOperations();

        private bool Support(SupportedOperations op)
        {
            return (GetSupportedOperations() & op) == op;
        }

        private readonly Dictionary<int, DyFunction> members = new Dictionary<int, DyFunction>();
        private readonly Dictionary<int, DyFunction> staticMembers = new Dictionary<int, DyFunction>();

        public override object ToObject() => this;

        public override string ToString() => TypeName.PutInBrackets();

        public abstract string TypeName { get; }

        public int TypeCode { get; internal set; }

        internal DyTypeInfo(int typeCode) : base(DyType.TypeInfo)
        {
            TypeCode = typeCode;
        }

        #region Binary Operations
        //x + y
        private DyFunction add;
        protected virtual DyObject AddOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (right.TypeId == DyType.String && TypeCode != DyType.String)
                return ctx.Types[DyType.String].Add(ctx, left, right);
            return ctx.OperationNotSupported(Builtins.Add, left);
        }
        internal DyObject Add(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (add != null)
                return add.Clone(ctx, left).Call1(right, ctx);

            return AddOp(left, right, ctx);
        }

        //x - y
        private DyFunction sub;
        protected virtual DyObject SubOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Sub, left);
        internal DyObject Sub(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (sub != null)
                return sub.Clone(ctx, left).Call1(right, ctx);
            return SubOp(left, right, ctx);
        }

        //x * y
        private DyFunction mul;
        protected virtual DyObject MulOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Mul, left);
        internal DyObject Mul(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (mul != null)
                return mul.Clone(ctx, left).Call1(right, ctx);
            return MulOp(left, right, ctx);
        }

        //x / y
        private DyFunction div;
        protected virtual DyObject DivOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Div, left);
        internal DyObject Div(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (div != null)
                return div.Clone(ctx, left).Call1(right, ctx);
            return DivOp(left, right, ctx);
        }

        //x % y
        private DyFunction rem;
        protected virtual DyObject RemOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Rem, left);
        internal DyObject Rem(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (rem != null)
                return rem.Clone(ctx, left).Call1(right, ctx);
            return RemOp(left, right, ctx);
        }

        //x << y
        private DyFunction shl;
        protected virtual DyObject ShiftLeftOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Shl, left);
        internal DyObject ShiftLeft(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (shl != null)
                return shl.Clone(ctx, left).Call1(right, ctx);
            return ShiftLeftOp(left, right, ctx);
        }

        //x >> y
        private DyFunction shr;
        protected virtual DyObject ShiftRightOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Shr, left);
        internal DyObject ShiftRight(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (shr != null)
                return shr.Clone(ctx, left).Call1(right, ctx);
            return ShiftRightOp(left, right, ctx);
        }

        //x & y
        private DyFunction and;
        protected virtual DyObject AndOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.And, left);
        internal DyObject And(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (and != null)
                return and.Clone(ctx, left).Call1(right, ctx);
            return AndOp(left, right, ctx);
        }

        //x | y
        private DyFunction or;
        protected virtual DyObject OrOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Or, left);
        internal DyObject Or(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (or != null)
                return or.Clone(ctx, left).Call1(right, ctx);
            return OrOp(left, right, ctx);
        }

        //x ^ y
        private DyFunction xor;
        protected virtual DyObject XorOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Xor, left);
        internal DyObject Xor(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (xor != null)
                return xor.Clone(ctx, left).Call1(right, ctx);
            return XorOp(left, right, ctx);
        }

        //x == y
        private DyFunction eq;
        protected virtual DyObject EqOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ReferenceEquals(left, right) ? DyBool.True : DyBool.False;
        internal DyObject Eq(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (eq != null)
                return eq.Clone(ctx, left).Call1(right, ctx);
            if (right.TypeId == DyType.Bool)
                return left.GetBool() == right.GetBool() ? DyBool.True : DyBool.False;
            return EqOp(left, right, ctx);
        }

        //x != y
        private DyFunction neq;
        protected virtual DyObject NeqOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Eq(ctx, left, right) == DyBool.True ? DyBool.False : DyBool.True;
        internal DyObject Neq(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (neq != null)
                return eq.Clone(ctx, left).Call1(right, ctx);
            return NeqOp(left, right, ctx);
        }

        //x > y
        private DyFunction gt;
        protected virtual DyObject GtOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Gt, left);
        internal DyObject Gt(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (gt != null)
                return gt.Clone(ctx, left).Call1(right, ctx);
            return GtOp(left, right, ctx);
        }

        //x < y
        private DyFunction lt;
        protected virtual DyObject LtOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Lt, left);
        internal DyObject Lt(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (lt != null)
                return lt.Clone(ctx, left).Call1(right, ctx);
            return LtOp(left, right, ctx);
        }

        //x >= y
        private DyFunction gte;
        protected virtual DyObject GteOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            var ret = Gt(ctx, left, right) == DyBool.True || Eq(ctx, left, right) == DyBool.True;
            return ret ? DyBool.True : DyBool.False;
        }
        internal DyObject Gte(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (gte != null)
                return gte.Clone(ctx, left).Call1(right, ctx);
            return GteOp(left, right, ctx);
        }

        //x <= y
        private DyFunction lte;
        protected virtual DyObject LteOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            var ret = Lt(ctx, left, right) == DyBool.True || Eq(ctx, left, right) == DyBool.True;
            return ret ? DyBool.True : DyBool.False;
        }
        internal DyObject Lte(ExecutionContext ctx, DyObject left, DyObject right)
        {
            if (lte != null)
                return lte.Clone(ctx, left).Call1(right, ctx);
            return LteOp(left, right, ctx);
        }
        #endregion

        #region Unary Operations
        //-x
        private DyFunction neg;
        protected virtual DyObject NegOp(DyObject arg, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Neg, arg);
        internal DyObject Neg(ExecutionContext ctx, DyObject arg)
        {
            if (neg != null)
                return neg.Clone(ctx, arg).Call0(ctx);
            return NegOp(arg, ctx);
        }

        //+x
        private DyFunction plus;
        protected virtual DyObject PlusOp(DyObject arg, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Plus, arg);
        internal DyObject Plus(ExecutionContext ctx, DyObject arg)
        {
            if (plus != null)
                return plus.Clone(ctx, arg).Call0(ctx);
            return PlusOp(arg, ctx);
        }

        //!x
        private DyFunction not;
        protected virtual DyObject NotOp(DyObject arg, ExecutionContext ctx) =>
            arg.GetBool() ? DyBool.False : DyBool.True;
        internal DyObject Not(ExecutionContext ctx, DyObject arg)
        {
            if (not != null)
                return not.Clone(ctx, arg).Call0(ctx);
            return NotOp(arg, ctx);
        }

        //~x
        private DyFunction bitnot;
        protected virtual DyObject BitwiseNotOp(DyObject arg, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.BitNot, arg);
        internal DyObject BitwiseNot(ExecutionContext ctx, DyObject arg)
        {
            if (bitnot != null)
                return bitnot.Clone(ctx, arg).Call0(ctx);
            return BitwiseNotOp(arg, ctx);
        }

        //x.len
        private DyFunction len;
        protected virtual DyObject LengthOp(DyObject arg, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Len, arg);
        internal DyObject Length(ExecutionContext ctx, DyObject arg)
        {
            if (len != null)
                return len.Clone(ctx, arg).Call0(ctx);
            return LengthOp(arg, ctx);
        }

        //x.toString
        private DyFunction tos;
        protected virtual DyObject ToStringOp(DyObject arg, ExecutionContext ctx) => new DyString(arg.ToString());
        internal DyObject ToString(ExecutionContext ctx, DyObject arg)
        {
            if (tos != null)
            {
                var retval = tos.Clone(ctx, arg).Call0(ctx);
                return retval.TypeId == DyType.String ? (DyString)retval : DyString.Empty;
            }

            return ToStringOp(arg, ctx);
        }
        #endregion

        #region Other Operations
        //x[y]
        private DyFunction get;
        protected virtual DyObject GetOp(DyObject self, DyObject index, ExecutionContext ctx) =>
            index.TypeId == DyType.String ? GetOp(self, index.GetString(), ctx) : ctx.OperationNotSupported(Builtins.Get, self);
        protected virtual DyObject GetOp(DyObject self, string index, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Get, self);
        protected virtual DyObject GetOp(DyObject self, int index, ExecutionContext ctx) =>
            GetOp(self, DyInteger.Get(index), ctx);
        internal DyObject Get(ExecutionContext ctx, DyObject self, DyObject index)
        {
            if (get != null)
                return get.Clone(ctx, self).Call1(index, ctx);

            return GetOp(self, index, ctx);
        }
        internal DyObject Get(ExecutionContext ctx, DyObject self, int index)
        {
            if (get != null)
                return get.Clone(ctx, self).Call1(DyInteger.Get(index), ctx);

            return GetOp(self, index, ctx);
        }

        //x[y] = z
        private DyFunction set;
        protected virtual DyObject SetOp(DyObject self, DyObject index, DyObject value, ExecutionContext ctx) =>
            ctx.OperationNotSupported(Builtins.Set, self);
        protected virtual DyObject SetOp(DyObject self, int index, DyObject value, ExecutionContext ctx) =>
            SetOp(self, DyInteger.Get(index), value, ctx);
        internal DyObject Set(ExecutionContext ctx, DyObject self, DyObject index, DyObject value)
        {
            if (set != null)
                return set.Clone(ctx, self).Call2(index, value, ctx);

            return SetOp(self, index, value, ctx);
        }
        internal DyObject Set(ExecutionContext ctx, DyObject self, int index, DyObject value)
        {
            if (set != null)
                return set.Clone(ctx, self).Call2(DyInteger.Get(index), value, ctx);

            return SetOp(self, index, value, ctx);
        }
        #endregion

        #region Service code
        internal bool CheckStaticMember(int nameId, Unit unit, ExecutionContext ctx)
        {
            nameId = unit.MemberIds[nameId];

            if (!staticMembers.ContainsKey(nameId))
            {
                var name = ctx.Composition.Members[nameId];
                return InternalGetStaticMember(name, ctx) != null;
            }

            return true;
        }

        internal bool CheckStaticMember(string name, ExecutionContext ctx)
        {
            return InternalGetStaticMember(name, ctx) != null;
        }

        internal DyObject GetStaticMember(int nameId, Unit unit, ExecutionContext ctx)
        {
            nameId = unit.MemberIds[nameId];

            if (!staticMembers.TryGetValue(nameId, out var value))
            {
                var name = ctx.Composition.Members[nameId];
                value = InternalGetStaticMember(name, ctx);

                if (value != null)
                    staticMembers.Add(nameId, value);
            }

            if (value != null)
                return value;

            return ctx.StaticOperationNotSupported(ctx.Composition.Members[nameId], TypeName);
        }

        internal void SetStaticMember(int nameId, DyObject value, Unit unit, ExecutionContext ctx)
        {
            var func = value as DyFunction;
            nameId = unit.MemberIds[nameId];
            staticMembers.Remove(nameId);

            if (func != null)
                staticMembers.Add(nameId, func);
        }

        internal DyObject HasMember(DyObject self, int nameId, Unit unit, ExecutionContext ctx)
        {
            nameId = unit.MemberIds[nameId];
            var name = ctx.Composition.Members[nameId];
            return (DyBool)HasMemberDirect(self, name, nameId, ctx);
        }

        internal DyObject HasStaticMember(int nameId, Unit unit, ExecutionContext ctx)
        {
            return (DyBool)CheckStaticMember(nameId, unit, ctx);
        }

        internal DyObject HasStaticMember(string name, ExecutionContext ctx)
        {
            return (DyBool)CheckStaticMember(name, ctx);
        }

        protected virtual bool HasMemberDirect(DyObject self, string name, int nameId, ExecutionContext ctx)
        {
            switch (name)
            {
                case Builtins.Add: return Support(SupportedOperations.Add);
                case Builtins.Sub: return Support(SupportedOperations.Sub);
                case Builtins.Mul: return Support(SupportedOperations.Mul);
                case Builtins.Div: return Support(SupportedOperations.Div);
                case Builtins.Rem: return Support(SupportedOperations.Rem);
                case Builtins.Shl: return Support(SupportedOperations.Shl);
                case Builtins.Shr: return Support(SupportedOperations.Shr);
                case Builtins.And: return Support(SupportedOperations.And);
                case Builtins.Or: return Support(SupportedOperations.Or);
                case Builtins.Xor: return Support(SupportedOperations.Xor);
                case Builtins.Eq: return Support(SupportedOperations.Eq);
                case Builtins.Neq: return Support(SupportedOperations.Neq);
                case Builtins.Gt: return Support(SupportedOperations.Gt);
                case Builtins.Lt: return Support(SupportedOperations.Lt);
                case Builtins.Gte: return Support(SupportedOperations.Gte);
                case Builtins.Lte: return Support(SupportedOperations.Lte);
                case Builtins.Neg: return Support(SupportedOperations.Neg);
                case Builtins.BitNot: return Support(SupportedOperations.BitNot);
                case Builtins.Plus: return Support(SupportedOperations.Plus);
                case Builtins.Get: return Support(SupportedOperations.Get);
                case Builtins.Set: return Support(SupportedOperations.Set);
                case Builtins.Len: return Support(SupportedOperations.Len);
                case Builtins.Not:
                case Builtins.ToStr:
                case Builtins.Clone:
                case Builtins.Has:
                    return true;
                default:
                    return nameId == -1
                        ? CheckHasMemberDirect(self, name, ctx)
                        : CheckHasMemberDirect(self, nameId, ctx);
            }
        }

        internal DyObject GetMember(DyObject self, int nameId, Unit unit, ExecutionContext ctx)
        {
            nameId = unit.MemberIds[nameId];
            var value = GetMemberDirect(self, nameId, ctx);

            if (value != null)
                return value;

            return ctx.OperationNotSupported(ctx.Composition.Members[nameId], self);
        }

        internal DyObject GetMemberDirect(DyObject self, int nameId, ExecutionContext ctx)
        {
            if (!members.TryGetValue(nameId, out var value))
            {
                var name = ctx.Composition.Members[nameId];
                value = InternalGetMember(self, name, ctx);

                if (value != null)
                    members.Add(nameId, value);
            }

            if (value != null)
                return value.Clone(ctx, self);

            return value;
        }

        internal bool CheckHasMemberDirect(DyObject self, int nameId, ExecutionContext ctx)
        {
            if (!members.TryGetValue(nameId, out var value))
            {
                var name = ctx.Composition.Members[nameId];
                value = InternalGetMember(self, name, ctx);

                if (value != null)
                {
                    members.Add(nameId, value);
                    return true;
                }
                else
                    return false;
            }

            return true;
        }

        internal bool CheckHasMemberDirect(DyObject self, string name, ExecutionContext ctx) => InternalGetMember(self, name, ctx) != null;

        internal void SetMember(int nameId, DyObject value, Unit unit, ExecutionContext ctx)
        {
            var func = value as DyFunction;
            nameId = unit.MemberIds[nameId];
            var name = ctx.Composition.Members[nameId];
            SetBuiltin(name, func);
            members.Remove(nameId);

            if (func != null)
                members.Add(nameId, func);
        }

        private void SetBuiltin(string name, DyFunction func)
        {
            switch (name)
            {
                case Builtins.Add: add = func; break;
                case Builtins.Sub: sub = func; break;
                case Builtins.Mul: mul = func; break;
                case Builtins.Div: div = func; break;
                case Builtins.Rem: rem = func; break;
                case Builtins.Shl: shl = func; break;
                case Builtins.Shr: shr = func; break;
                case Builtins.And: and = func; break;
                case Builtins.Or: or = func; break;
                case Builtins.Xor: xor = func; break;
                case Builtins.Eq: eq = func; break;
                case Builtins.Neq: neq = func; break;
                case Builtins.Gt: gt = func; break;
                case Builtins.Lt: lt = func; break;
                case Builtins.Gte: gte = func; break;
                case Builtins.Lte: lte = func; break;
                case Builtins.Neg: neg = func; break;
                case Builtins.Not: not = func; break;
                case Builtins.BitNot: bitnot = func; break;
                case Builtins.Len: len = func; break;
                case Builtins.ToStr: tos = func; break;
                case Builtins.Plus: plus = func; break;
                case Builtins.Set: set = func; break;
                case Builtins.Get: get = func; break;
            }
        }

        private DyObject Clone(ExecutionContext ctx, DyObject obj) => obj.Clone();

        private DyObject Has(ExecutionContext ctx, DyObject self, DyObject member)
        {
            if (member.TypeId != DyType.String)
                return ctx.InvalidType(member);
            var name = member.GetString();

            if (self == null) //We're calling against type itself
                return HasStaticMember(name, ctx);
            else if (ctx.Composition.MembersMap.TryGetValue(name, out var nameId))
                return (DyBool)HasMemberDirect(self, name, nameId, ctx);
            else
                return (DyBool)HasMemberDirect(self, name, -1, ctx);
        }

        private DyFunction InternalGetMember(DyObject self, string name, ExecutionContext ctx) =>
            name switch
            {
                Builtins.Add => Support(SupportedOperations.Add) ? DyForeignFunction.Member(name, Add, -1, new Par("other")) : null,
                Builtins.Sub => Support(SupportedOperations.Sub) ? DyForeignFunction.Member(name, Sub, -1, new Par("other")) : null,
                Builtins.Mul => Support(SupportedOperations.Mul) ? DyForeignFunction.Member(name, Mul, -1, new Par("other")) : null,
                Builtins.Div => Support(SupportedOperations.Div) ? DyForeignFunction.Member(name, Div, -1, new Par("other")) : null,
                Builtins.Rem => Support(SupportedOperations.Rem) ? DyForeignFunction.Member(name, Rem, -1, new Par("other")) : null,
                Builtins.Shl => Support(SupportedOperations.Shl) ? DyForeignFunction.Member(name, ShiftLeft, -1, new Par("other")) : null,
                Builtins.Shr => Support(SupportedOperations.Shr) ? DyForeignFunction.Member(name, ShiftRight, -1, new Par("other")) : null,
                Builtins.And => Support(SupportedOperations.And) ? DyForeignFunction.Member(name, And, -1, new Par("other")) : null,
                Builtins.Or => Support(SupportedOperations.Or) ? DyForeignFunction.Member(name, Or, -1, new Par("other")) : null,
                Builtins.Xor => Support(SupportedOperations.Xor) ? DyForeignFunction.Member(name, Xor, -1, new Par("other")) : null,
                Builtins.Eq => DyForeignFunction.Member(name, Eq, -1, new Par("other")),
                Builtins.Neq => DyForeignFunction.Member(name, Neq, -1, new Par("other")),
                Builtins.Gt => Support(SupportedOperations.Gt) ? DyForeignFunction.Member(name, Gt, -1, new Par("other")) : null,
                Builtins.Lt => Support(SupportedOperations.Lt) ? DyForeignFunction.Member(name, Lt, -1, new Par("other")) : null,
                Builtins.Gte => Support(SupportedOperations.Gte) ? DyForeignFunction.Member(name, Gte, -1, new Par("other")) : null,
                Builtins.Lte => Support(SupportedOperations.Lte) ? DyForeignFunction.Member(name, Lte, -1, new Par("other")) : null,
                Builtins.Neg => Support(SupportedOperations.Neg) ? DyForeignFunction.Member(name, Neg) : null,
                Builtins.Not => DyForeignFunction.Member(name, Not),
                Builtins.BitNot => Support(SupportedOperations.BitNot) ? DyForeignFunction.Member(name, BitwiseNot) : null,
                Builtins.Plus => Support(SupportedOperations.Plus) ? DyForeignFunction.Member(name, Plus) : null,
                Builtins.Get => Support(SupportedOperations.Get) ? DyForeignFunction.Member(name, Get, -1, new Par("index")) : null,
                Builtins.Set => Support(SupportedOperations.Set) ? DyForeignFunction.Member(name, Set, -1, new Par("index"), new Par("value")) : null,
                Builtins.Len => Support(SupportedOperations.Len) ? DyForeignFunction.Member(name, Length) : null,
                Builtins.ToStr => DyForeignFunction.Member(name, ToString),
                Builtins.Iterator => self is IEnumerable<DyObject> ? DyForeignFunction.Member(name, GetIterator) : null,
                Builtins.Clone => DyForeignFunction.Member(name, Clone),
                Builtins.Has => DyForeignFunction.Member(name, Has, -1, new Par("member")),
                Builtins.Type => DyForeignFunction.Member(name, (context, o) => context.Types[self.TypeId]),
                _ => GetMember(name, ctx)
            };

        private DyObject GetIterator(ExecutionContext ctx, DyObject self)
        {
            if (self is IEnumerable<DyObject> en)
                return new DyIterator(en);
            else
                return ctx.OperationNotSupported(Builtins.Iterator, self);
        }

        protected virtual DyFunction GetMember(string name, ExecutionContext ctx) => null;

        private DyFunction InternalGetStaticMember(string name, ExecutionContext ctx) =>
            name switch
            {
                "TypeInfo" => DyForeignFunction.Static(name, (c, obj) => c.Types[obj.TypeId], -1, new Par("value")),
                "__deleteMember" => DyForeignFunction.Static(name,
                    (context, strObj) =>
                    {
                        var nm = strObj.GetString();
                        if (context.Composition.MembersMap.TryGetValue(nm, out var nameId))
                        {
                            SetBuiltin(nm, null);
                            members.Remove(nameId);
                            staticMembers.Remove(nameId);
                        }
                        return DyNil.Instance;
                    }, -1, new Par("name")),
                "has" => DyForeignFunction.Member(name, Has, -1, new Par("member")),
                _ => GetStaticMember(name, ctx),
            };

        protected virtual DyFunction GetStaticMember(string name, ExecutionContext ctx) => null;

        public override int GetHashCode() => TypeCode.GetHashCode();
        #endregion
    }

    internal sealed class DyTypeTypeInfo : DyTypeInfo
    {
        public DyTypeTypeInfo() : base(DyType.TypeInfo)
        {

        }

        protected override SupportedOperations GetSupportedOperations() =>
            SupportedOperations.Eq | SupportedOperations.Neq | SupportedOperations.Not | SupportedOperations.Get;

        public override string TypeName => DyTypeNames.TypeInfo;

        protected override DyObject GetOp(DyObject self, string index, ExecutionContext ctx) =>
            index switch
            {
                "id" => DyInteger.Get(((DyTypeInfo)self).TypeCode),
                "name" => new DyString(((DyTypeInfo)self).TypeName),
                _ => ctx.IndexOutOfRange(index)
            };

        protected override DyObject ToStringOp(DyObject arg, ExecutionContext ctx) =>
            new DyString(("typeInfo " + ((DyTypeInfo)arg).TypeName).PutInBrackets());
    }
}